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
    <div class="p-6 max-w-3xl mx-auto" role="main" aria-label="Afspraak details pagina">

        <h1 class="text-2xl font-bold text-blue-600 mb-4" id="appointment-title">
            Afspraak details
        </h1>

        <!-- Loading -->
        <div v-if="loading" class="text-gray-500" role="status" aria-live="polite">
            Afspraak wordt geladen…
        </div>

        <!-- Error -->
        <div v-else-if="error" class="text-red-600" role="alert" aria-live="assertive">
            {{ error }}
        </div>

        <!-- Appointment info -->
        <div v-else-if="appointment" class="bg-white rounded-2xl shadow p-6 space-y-3" role="region"
            :aria-labelledby="'appointment-title'">

            <p>
                <strong>Behandeling:</strong>
                <span>{{ appointment.treatmentDescription }}</span>
            </p>

            <p>
                <strong>Datum:</strong>
                <span>{{ new Date(appointment.date).toLocaleString('nl-NL') }}</span>
            </p>

            <p>
                <strong>Status:</strong>
                <span>{{ appointment.status }}</span>
            </p>

            <p>
                <strong>Patiënt:</strong>
                <span>{{ appointment.patientName }}</span>
            </p>

            <p v-if="appointment.notes">
                <strong>Notities:</strong>
                <span>{{ appointment.notes }}</span>
            </p>

            <div v-if="appointment.treatmentInstructions" class="pt-3 border-t" role="region"
                aria-label="Instructies van de afspraak">
                <p class="font-semibold text-blue-500">Instructies</p>
                <p>{{ appointment.treatmentInstructions }}</p>
            </div>
        </div>

        <!-- Not found -->
        <div v-else class="text-gray-500" role="alert" aria-live="polite">
            Afspraak niet gevonden of ID ongeldig.
        </div>

        <!-- Back link -->
        <router-link to="/" class="inline-block mt-6 text-blue-500 hover:text-blue-700"
            aria-label="Terug naar overzicht">
            ← Terug naar overzicht
        </router-link>

    </div>
</template>
