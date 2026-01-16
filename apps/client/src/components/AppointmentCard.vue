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
  (e: 'cancel-request'): void
}>()

// 🔥 LOKALE UI STATUS (DIT IS DE SLEUTEL)
const uiStatus = ref<'Pending' | 'Sent' | 'Approved' | 'Denied'>('Pending')

// sync met backend als die verandert
watch(
  () => props.appointment.requestStatus,
  (v) => {
    if (v === 'Approved') uiStatus.value = 'Approved'
    else if (v === 'Denied') uiStatus.value = 'Denied'
    else if (v === null) uiStatus.value = 'Sent'
    else uiStatus.value = 'Pending'
  },
  { immediate: true }
)

function requestAccess() {
  uiStatus.value = 'Sent'          // ✅ SWITCH METEEN
  emit('request-access', props.appointment.id)
}

// wordt aangeroepen bij ANNULEREN
function resetStatus() {
  uiStatus.value = 'Pending'       // ✅ TERUG NAAR BEGIN
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

    <!-- NOG NIET AANGEVRAAGD -->
<!-- VRAAG TOEGANG -->
<button
  v-if="uiStatus === 'Pending'"
  class="btn-primary"
  @click="requestAccess"
>
  Vraag toegang aan
</button>

<!-- AANVRAAG VERSTUURD -->
<span
  v-else-if="uiStatus === 'Sent'"
  class="text-yellow-600 font-medium"
>
  Aanvraag verstuurd
</span>

<!-- GOEDGEKEURD -->
<span
  v-else-if="uiStatus === 'Approved'"
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

</template>
