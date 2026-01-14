<script setup lang="ts">
import { computed, watch } from 'vue'
import { useRoute } from 'vue-router'
import { useAppointment } from '@/composables/useAppointments'
import type { AppointmentDto } from '@/types/appointment'

const route = useRoute()
const { appointments, loading, error, fetchAppointments } = useAppointment()

// Reactive appointment ID from route
const appointmentId = computed<number>(() => {
  const id = (route.params as { id?: string }).id
  return id ? Number(id) : NaN
})

// Compute the appointment matching the route ID
const appointment = computed<AppointmentDto | null>(() => {
  if (Number.isNaN(appointmentId.value)) return null
  return appointments.value.find(a => a.id === appointmentId.value) ?? null
})

// Fetch all appointments when page loads
fetchAppointments()

// Refetch if route ID changes (optional)
watch(appointmentId, () => fetchAppointments(), { immediate: false })
</script>

<template>
  <div class="p-6 max-w-3xl mx-auto">
    <h1 class="text-2xl font-bold text-blue-600 mb-4">Afspraak details</h1>

    <div v-if="loading" class="text-gray-500">Afspraak wordt geladen…</div>
    <div v-else-if="error" class="text-red-600">{{ error }}</div>

    <div v-else-if="appointment" class="bg-white rounded-2xl shadow p-6 space-y-3">
      <p><strong>Behandeling:</strong> {{ appointment.treatmentDescription }}</p>
      <p><strong>Datum:</strong> {{ new Date(appointment.date).toLocaleString('nl-NL') }}</p>
      <p><strong>Status:</strong> {{ appointment.status }}</p>
      <p><strong>Patiënt:</strong> {{ appointment.patientName }}</p>
      <p v-if="appointment.notes"><strong>Notities:</strong> {{ appointment.notes }}</p>
      <div v-if="appointment.treatmentInstructions" class="pt-3 border-t">
        <p class="font-semibold text-blue-500">Instructies</p>
        <p>{{ appointment.treatmentInstructions }}</p>
      </div>
    </div>

    <div v-else class="text-gray-500">Afspraak niet gevonden of ID ongeldig.</div>

    <router-link to="/" class="inline-block mt-6 text-blue-500 hover:text-blue-700">
      ← Terug naar overzicht
    </router-link>
  </div>
</template>
