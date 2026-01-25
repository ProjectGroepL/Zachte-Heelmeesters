<script setup lang="ts">
import { ref, computed } from 'vue'
import AppointmentReportOverview from '@/components/AppointmentReportOverview.vue'
import { usePatientAppointments } from '@/composables/usePatientAppointments'
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card'
import { Label } from '@/components/ui/label'
import { Combobox } from '@/components/ui/combobox'
import type { ComboboxOption } from '@/components/ui/combobox/Combobox.vue'
import { FileText, Calendar } from 'lucide-vue-next'

const { data, loading } = usePatientAppointments()

const selectedAppointmentId = ref<string>('')

const appointments = computed(() => data.value?.appointments ?? [])

const completedAppointments = computed(() => {
  return appointments.value.filter(a => {
    console.log("Status van afspraak:", a.id, a.status); 
    
    return a.status === 'Completed' || 
           Number(a.status) === 2 || 
           a.status?.toString().toLowerCase() === 'completed';
  });
});

const appointmentOptions = computed<ComboboxOption[]>(() => {
  return completedAppointments.value.map(a => ({
    value: String(a.id),
    label: `${new Date(a.date).toLocaleDateString('nl-NL')}${a.specialistName ? ` – ${a.specialistName}` : ''}`
  }))
})
</script>

<template>
  <div class="container mx-auto p-6 space-y-6">
    <h1 class="text-3xl font-bold">Mijn rapporten</h1>

    <!-- Appointment Selection -->
    <Card>
      <CardHeader>
        <CardTitle class="flex items-center gap-2">
          <Calendar class="h-5 w-5" />
          Selecteer een afspraak
        </CardTitle>
      </CardHeader>
      <CardContent class="space-y-4">
        <div class="space-y-2">
          <Label>Kies een afgeronde afspraak</Label>
          <Combobox
            v-model="selectedAppointmentId"
            :options="appointmentOptions"
            placeholder="Kies een afgeronde afspraak…"
            search-placeholder="Zoek afspraak..."
            empty-message="Geen afspraken gevonden"
          />
        </div>

        <p
          v-if="!loading && completedAppointments.length === 0"
          class="text-sm text-muted-foreground italic"
        >
          Er zijn nog geen afgeronde afspraken met een rapport.
        </p>
      </CardContent>
    </Card>

    <!-- Report Display -->
    <div v-if="selectedAppointmentId" class="animate-in fade-in slide-in-from-top-4">
      <Card>
        <CardHeader>
          <CardTitle class="flex items-center gap-2">
            <FileText class="h-5 w-5" />
            Rapport Details
          </CardTitle>
        </CardHeader>
        <CardContent>
          <AppointmentReportOverview :appointment-id="Number(selectedAppointmentId)" />
        </CardContent>
      </Card>
    </div>
  </div>
</template>
