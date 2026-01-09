<script setup lang="ts">
import { ref } from 'vue'
import { useSpecialistAppointments } from '@/composables/useSpecialistAppointments'
import { useRequestAccess } from '@/composables/useSpecialistAccessRequests'

const { data: appointments, loading, error } = useSpecialistAppointments()

const selectedAppointmentId = ref<number | null>(null)
const reason = ref('')
const showSuccess = ref(false)

const requestMutation = useRequestAccess()

const submit = async () => {
  if (!selectedAppointmentId.value || !reason.value) return

  await requestMutation.mutate({
    appointmentId: selectedAppointmentId.value,
    reason: reason.value
  })

  if (!requestMutation.error.value) {
    showSuccess.value = true
    selectedAppointmentId.value = null
    reason.value = ''
    setTimeout(() => (showSuccess.value = false), 8000)
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
      class="mb-4 p-4 bg-green-100 border border-green-400 text-green-700 rounded"
      role="alert"
    >
      ✅ De aanvraag is succesvol verzonden.
    </div>

    <p v-if="loading">Afspraken laden…</p>
    <p v-else-if="error" class="text-red-600" role="alert">
      Kon afspraken niet laden.
    </p>

    <form v-else @submit.prevent="submit" class="space-y-4">
      <div>
        <label for="appointment" class="block font-medium">
          Afspraak
        </label>
        <select
          id="appointment"
          v-model="selectedAppointmentId"
          class="border p-2 rounded w-full"
          required
        >
          <option disabled :value="null">Selecteer afspraak</option>
          <option
            v-for="a in appointments"
            :key="a.id"
            :value="a.id"
            :disabled="a.status !== 'PendingAccess'"
          >
            {{ a.patientName }} – {{ new Date(a.date).toLocaleString('nl-NL') }}
            ({{ a.status }})
          </option>
        </select>
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

      <p v-if="requestMutation.error.value" class="text-red-600" role="alert">
        {{ requestMutation.error.value }}
      </p>
    </form>
  </main>
</template>
