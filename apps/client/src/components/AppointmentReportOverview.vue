<script setup lang="ts">
import { computed } from 'vue'
import { usePatientAppointmentReport } from '@/composables/usePatientAppointmentReport'

const props = defineProps<{
  appointmentId: number
}>()

const { data: report, loading, error } =
  usePatientAppointmentReport(props.appointmentId)

const formattedDate = computed(() =>
  report.value
    ? new Date(report.value.createdAt).toLocaleDateString('nl-NL', {
        day: '2-digit',
        month: 'long',
        year: 'numeric'
      })
    : ''
)
</script>

<template>
  <div class="max-w-4xl mx-auto p-6 space-y-6">
    <h1 class="text-2xl font-bold">Afspraakrapport</h1>

    <!-- Loading -->
    <div v-if="loading" class="text-muted-foreground italic">
      Rapport wordt geladen...
    </div>

    <!-- Error -->
    <div v-else-if="error" class="text-red-600">
      Het rapport kon niet worden geladen.
    </div>

    <!-- Geen rapport -->
    <div v-else-if="!report" class="text-muted-foreground italic">
      Voor deze afspraak is nog geen rapport beschikbaar.
    </div>

    <!-- Report -->
    <div v-else class="space-y-6">
      <!-- Samenvatting -->
      <section class="bg-card p-6 border rounded-xl shadow-sm space-y-2">
        <p class="text-sm text-muted-foreground">
          Opgesteld op {{ formattedDate }}
        </p>

        <h2 class="text-lg font-semibold">Medische samenvatting</h2>
        <p class="whitespace-pre-line">
          {{ report.summary }}
        </p>
      </section>

      <!-- Kosten -->
      <section class="bg-card p-6 border rounded-xl shadow-sm">
        <h2 class="text-lg font-semibold mb-4">Medische verrichtingen</h2>

        <ul class="list-disc pl-5 space-y-1">
          <li v-for="(item, i) in report.items" :key="i">
            {{ item }}
          </li>
        </ul>
      </section>
    </div>
  </div>
</template>
