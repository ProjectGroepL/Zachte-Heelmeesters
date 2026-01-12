<script setup lang="ts">
  import { computed } from 'vue'
import { useSpecialistAppointments } from '@/composables/useSpecialistAppointments'
import AppointmentDocuments from '@/components/AppointmentDocuments.vue'

const { data: appointments, loading } = useSpecialistAppointments()
const visibleAppointments = computed(() =>
  (appointments.value ?? []).filter(a =>
    a.status === 'Scheduled' || a.status === 'Completed'
  )
)

</script>

<template>
  <main class="max-w-6xl mx-auto p-6 space-y-6">
    <h1 class="text-2xl font-bold">
      Medisch dossier
    </h1>

    <p v-if="loading">Afspraken laden…</p>

    <p
      v-else-if="visibleAppointments.length === 0"
      class="text-gray-400 italic"
    >
      Geen afspraken met goedgekeurde toegang.
    </p>

    <section
      v-for="appointment in visibleAppointments"
      :key="appointment.id"
      class="border rounded-2xl p-5 space-y-4"
    >
      <header>
        <h2 class="text-lg font-semibold">
          Afspraak op
          {{ new Date(appointment.date).toLocaleDateString('nl-NL') }}
        </h2>

        <p class="text-sm text-gray-500">
          {{ appointment.treatmentDescription }}
        </p>
      </header>

      <!-- Alleen hier worden documenten opgehaald -->
      <AppointmentDocuments :appointment-id="appointment.id" />
    </section>
  </main>
</template>

