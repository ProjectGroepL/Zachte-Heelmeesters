<script setup lang="ts">
import { ref, watch } from 'vue'

const props = defineProps<{
  appointment: {
    id: number
    date: string
    patientName: string
    requestStatus: 'Pending' | 'Approved' | 'Denied' | null
  }
}>()

const emit = defineEmits<{
  (e: 'request-access', appointmentId: number): void
}>()

const localStatus = ref(props.appointment.requestStatus ?? null)

watch(
  () => props.appointment.requestStatus,
  (v) => (localStatus.value = v ?? null)
)

function requestAccess() {
  emit('request-access', props.appointment.id)
}
</script>

<template>
  <div class="border rounded-xl p-4 flex justify-between items-center bg-card">
    <div>
      <p class="font-semibold">{{ appointment.patientName }}</p>
      <p class="text-sm text-muted-foreground">
        {{ new Date(appointment.date).toLocaleDateString() }}
      </p>
    </div>

    <div>
      <!-- GEEN AANVRAAG -->
      <button
        v-if="localStatus === null"
        class="btn-primary"
        @click="requestAccess"
      >
        Vraag toegang aan
      </button>

      <!-- AANVRAAG VERSTUURD -->
      <span
        v-else-if="localStatus === 'Pending'"
        class="text-yellow-600 font-medium"
      >
        Aanvraag verstuurd
      </span>

      <!-- GOEDGEKEURD -->
      <span
        v-else-if="localStatus === 'Approved'"
        class="text-green-600 font-medium"
      >
        Toegang verleend
      </span>

      <!-- AFGEWEZEN -->
      <span
        v-else
        class="text-gray-500 text-sm italic"
      >
        Aanvraag afgewezen
      </span>
    </div>
  </div>
</template>
