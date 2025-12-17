<script setup lang="ts">
import { ref } from 'vue'
import { useRequestAccess } from '@/composables/useSpecialistAccessRequests'

const patientId = ref<number | null>(null)
const reason = ref('')

const requestMutation = useRequestAccess()
const showSuccess = ref(false)
const submit = async () => {
  if (!patientId.value || !reason.value) return

  await requestMutation.mutate({
    patientId: patientId.value,
    reason: reason.value
  })

  if (!requestMutation.error.value) {
    showSuccess.value = true
    patientId.value = null
    reason.value = ''

    setTimeout(() => {
      showSuccess.value = false
    }, 10000)
  }


}
</script>

<template>
  <main class="max-w-xl mx-auto p-6" aria-labelledby="request-title">
    <h1 id="request-title" class="text-2xl font-bold mb-4">
      Toegang aanvragen
    </h1>

    <div 
      v-if="showSuccess" 
      class="mb-4 p-4 bg-green-100 border border-green-400 text-green-700 rounded-lg flex items-center shadow-sm"
      role="alert"
    >
      <span class="mr-2">✅</span>
      De aanvraag is succesvol verzonden naar de patiënt.
    </div>

    <form @submit.prevent="submit" class="space-y-4">
      <div>
        <label for="patient" class="block font-medium">
          Patiënt ID
        </label>
        <input
          id="patient"
          type="number"
          v-model="patientId"
          class="border p-2 rounded w-full"
          required
        />
      </div>

      <div>
        <label for="reason" class="block font-medium">
          Reden van aanvraag
        </label>
        <textarea
          id="reason"
          v-model="reason"
          class="border p-2 rounded w-full h-24"
          required
        />
      </div>

      <button
        type="submit"
        class="w-full bg-blue-600 text-white py-2 rounded"
        :disabled="requestMutation.loading.value"
      >
        {{ requestMutation.loading.value ? 'Versturen…' : 'Aanvraag versturen' }}
      </button>

      <p
        v-if="requestMutation.error.value"
        role="alert"
        class="text-red-600 text-sm"
      >
        Aanvraag mislukt.
      </p>
    </form>
  </main>
</template>
