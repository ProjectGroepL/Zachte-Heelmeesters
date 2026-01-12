<script setup lang="ts">
import { computed, ref } from 'vue'
import { useSpecialistAppointments } from '@/composables/useSpecialistAppointments'
import AppointmentReportForm from '@/components/AppointmentReportForm.vue'

const { data: appointments, loading } = useSpecialistAppointments()
const selectedAppointmentId = ref<number | null>(null)

// We filteren op afspraken die nog de status 'Scheduled' hebben
// want de service zet ze na het rapport op 'Completed'
const openAppointments = computed(() => 
  (appointments.value ?? []).filter(a => a.status === 'Scheduled')
)

</script>

<template>
  <div class="max-w-4xl mx-auto p-6 space-y-8">
    <h1 class="text-2xl font-bold">Rapportage Opstellen</h1>

    <section class="bg-card p-6 border rounded-xl shadow-sm">
      <label class="block text-sm font-medium mb-2">Selecteer de afspraak waarvoor u rapporteert:</label>
      <select 
        v-model="selectedAppointmentId" 
        class="w-full border p-3 rounded-lg bg-background"
      >
        <option :value="null" disabled>Kies een afspraak uit de lijst...</option>
        <option v-for="a in openAppointments" :key="a.id" :value="a.id">
          {{ new Date(a.date).toLocaleDateString('nl-NL') }} - {{ a.patientName }} ({{ a.treatmentDescription }})
        </option>
      </select>
      
      <p v-if="openAppointments.length === 0 && !loading" class="text-sm text-muted-foreground mt-2 italic">
        Er zijn momenteel geen openstaande afspraken die een rapport vereisen.
      </p>
    </section>

    <section v-if="selectedAppointmentId" class="bg-card p-6 border rounded-xl shadow-sm animate-in fade-in slide-in-from-top-4">
      <AppointmentReportForm :appointment-id="selectedAppointmentId" @success="selectedAppointmentId = null" />
    </section>
  </div>
</template>