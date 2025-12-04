<script setup lang="ts">
import { ref } from 'vue'
import { saveSpecialistIcal } from '@/lib/api'
import {Button} from "@/components/ui/button";

type Appointment = {
  id: number
  patientName: string
  time: string
  reason: string
}

// Mocked appointments for now
const appointments = ref<Appointment[]>([
  { id: 1, patientName: 'Jan Jansen', time: '09:00 - 09:30', reason: 'Controle afspraak' },
  { id: 2, patientName: 'Piet Pieters', time: '10:00 - 10:30', reason: 'Eerste consult' },
  { id: 3, patientName: 'Anna de Vries', time: '11:00 - 11:30', reason: 'Follow-up' }
])

const showIcalModal = ref(false)
const icalUrl = ref<string | null>(null)
const icalInput = ref('')
const isSaving = ref(false)
const errorMessage = ref<string | null>(null)

function openIcalModal() {
  errorMessage.value = null
  icalInput.value = icalUrl.value ?? ''
  showIcalModal.value = true
}

async function saveIcal() {
  const trimmed = icalInput.value.trim()
  if (!trimmed) {
    errorMessage.value = 'Voer een geldige iCal-link in.'
    return
  }

  isSaving.value = true
  errorMessage.value = null

  try {
    const result = await saveSpecialistIcal(trimmed)
    icalUrl.value = result.url
    showIcalModal.value = false
  } catch (err) {
    errorMessage.value = 'Opslaan van de iCal-link is mislukt. Probeer het later opnieuw.'
    // optionally log err to console
    console.error(err)
  } finally {
    isSaving.value = false
  }
}
</script>

<template>
  <div class="p-6 space-y-6">
    <!-- Header with button in top-right -->
    <div class="flex items-center justify-between">
      <h1 class="text-2xl font-semibold">Specialist Dashboard</h1>

      <Button @click="openIcalModal">
        {{ icalUrl ? 'Bewerk iCal-link' : 'Voeg iCal-link toe' }}
      </Button>
    </div>

    <!-- Agenda: all appointments -->
    <section class="space-y-3">
      <div class="flex items-center justify-between">
        <h2 class="text-lg font-medium">Agenda</h2>
        <p class="text-sm text-muted-foreground">
          Overzicht van al je afspraken.
        </p>
      </div>

      <div class="rounded-md border divide-y">
        <div
          v-if="appointments.length === 0"
          class="p-4 text-sm text-muted-foreground"
        >
          Nog geen afspraken gepland.
        </div>
        <div
          v-else
          v-for="appointment in appointments"
          :key="appointment.id"
          class="flex items-start justify-between gap-4 p-4"
        >
          <div>
            <p class="text-sm font-medium">
              {{ appointment.patientName }}
            </p>
            <p class="text-xs text-muted-foreground">
              {{ appointment.reason }}
            </p>
          </div>
          <div class="text-right">
            <p class="text-sm font-semibold">
              {{ appointment.time }}
            </p>
          </div>
        </div>
      </div>
    </section>

    <!-- iCal status (simple for now) -->
    <section class="space-y-2">
      <h2 class="text-lg font-medium">iCal Status</h2>
      <p class="text-sm text-muted-foreground" v-if="!icalUrl">
        Nog geen iCal-link ingesteld. Gebruik de knop rechtsboven om je agenda te koppelen.
      </p>
      <div v-else class="rounded-md border p-4 space-y-1 text-sm">
        <p class="font-medium">Gekoppelde iCal-link:</p>
        <p class="break-all text-muted-foreground">{{ icalUrl }}</p>
        <p class="text-xs text-muted-foreground">
          In de toekomst kan hier de laatste synchronisatie en eventuele fouten getoond worden.
        </p>
      </div>
    </section>

    <!-- Simple modal for adding/editing iCal link -->
    <div
        v-if="showIcalModal"
        class="fixed inset-0 z-50 flex items-center justify-center bg-black/40"
    >
      <div class="w-full max-w-md rounded-lg bg-white p-6 shadow-lg">
        <h2 class="mb-4 text-lg font-semibold">
          iCal-link {{ icalUrl ? 'bewerken' : 'toevoegen' }}
        </h2>

        <label class="block text-sm font-medium text-gray-700 mb-1">
          iCal URL
        </label>
        <input
            v-model="icalInput"
            type="url"
            placeholder="https://..."
            class="w-full rounded-md border px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
        />

        <p class="mt-2 text-xs text-muted-foreground">
          Plak hier de iCal-link van de agenda van de specialist (bijvoorbeeld uit Outlook of Google Calendar).
        </p>

        <p v-if="errorMessage" class="mt-2 text-xs text-red-600">
          {{ errorMessage }}
        </p>

        <div class="mt-4 flex justify-end gap-2">
          <Button
              class="bg-red-700 hover:bg-red-600"
              @click="showIcalModal = false"
              :disabled="isSaving"
          >
            Annuleren
          </Button>
          <Button
              @click="saveIcal"
              :disabled="isSaving"
          >
            {{ isSaving ? 'Opslaan...' : 'Opslaan' }}
          </Button>
        </div>
      </div>
    </div>
  </div>
</template>
