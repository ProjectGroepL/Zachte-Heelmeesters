<script setup lang="ts">
import { ref } from 'vue'
import { useRequestReportAccess } from '@/composables/useAccessRequests'

const props = defineProps<{ appointmentId: number }>()
const emit = defineEmits(['close'])

const reason = ref('')
const { mutate, loading } = useRequestReportAccess()

async function submit() {
  await mutate({
    appointmentId: props.appointmentId,
    reason: reason.value
  })

  emit('close')
}
</script>

<template>
  <div class="modal">
    <h2 class="text-lg font-bold">Toegang aanvragen</h2>

    <label class="block mt-4">
      Reden
      <textarea
        v-model="reason"
        class="w-full border rounded-md p-2 mt-1"
      />
    </label>

    <div class="flex gap-2 mt-4">
      <button class="btn-primary" @click="submit" :disabled="loading">
        Verzenden
      </button>
      <button @click="$emit('close')">Annuleren</button>
    </div>
  </div>
</template>
