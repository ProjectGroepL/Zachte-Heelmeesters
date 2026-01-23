<script setup lang="ts">
import { ref, computed } from 'vue'
import AppointmentReportOverview from '@/components/AppointmentReportOverview.vue'
import { usePatientAppointments } from '@/composables/usePatientAppointments'

const { data, loading } = usePatientAppointments()

const selectedAppointmentId = ref<number | null>(null)

const appointments = computed(() => data.value?.appointments ?? [])

const completedAppointments = computed(() => {
  return appointments.value.filter(a => {
    // Console log om te zien wat 'a.status' precies is tijdens het laden
    console.log("Status van afspraak:", a.id, a.status); 
    
    return a.status === 'Completed' || 
           Number(a.status) === 2 || 
           a.status?.toString().toLowerCase() === 'completed';
  });
});
</script>

<template>
  <div class="max-w-4xl mx-auto p-6 space-y-6">
    <h1 class="text-2xl font-bold">Mijn rapporten</h1>

    <!-- Afspraak selecteren -->
    <section class="bg-card p-6 border rounded-xl shadow-sm space-y-4">
      <label class="text-sm font-semibold">
        Selecteer een afspraak
      </label>

      <select
        v-model="selectedAppointmentId"
        class="w-full border p-3 rounded-lg bg-background"
      >
        <option :value="null" disabled>
          Kies een afgeronde afspraak…
        </option>

        <option
          v-for="a in completedAppointments"
          :key="a.id"
          :value="a.id"
        >
          {{ new Date(a.date).toLocaleDateString('nl-NL') }}
          <span v-if="a.specialistName">
            – {{ a.specialistName }}
          </span>
        </option>
      </select>

      <p
        v-if="!loading && completedAppointments.length === 0"
        class="text-sm text-muted-foreground italic"
      >
        Er zijn nog geen afgeronde afspraken met een rapport.
      </p>
    </section>

    <!-- Rapport tonen -->
    <section
      v-if="selectedAppointmentId"
      class="animate-in fade-in slide-in-from-top-4"
    >
      <AppointmentReportOverview
        :appointment-id="selectedAppointmentId"
      />
    </section>
  </div>
</template>
