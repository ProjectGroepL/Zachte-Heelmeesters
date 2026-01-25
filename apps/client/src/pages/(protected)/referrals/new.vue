<script setup lang="ts">
import { ref, computed } from 'vue'
import { useDoctorPatients } from '@/composables/useDoctorPatients'
import { useCreateReferral, useTreatments } from '@/composables/useReferral'
import { useRouter } from 'vue-router'

const router = useRouter()

// Queries
const { data: patients, loading: loadingPatients, error: patientsError } = useDoctorPatients()
const { data: treatments, loading: loadingTreatments, error: treatmentsError } = useTreatments()

// Mutation
const createReferral = useCreateReferral()

// Form state
const patientId = ref<number | null>(null)
const treatmentId = ref<number | null>(null)
const notes = ref('')

// Derived
const hasPatients = computed(() => (patients.value?.length ?? 0) > 0)
const hasTreatments = computed(() => (treatments.value?.length ?? 0) > 0)

const canSubmit = computed(() =>
  patientId.value !== null &&
  treatmentId.value !== null &&
  !createReferral.loading.value
)

// Submit
const submitReferral = async () => {
  if (!canSubmit.value) return

  await createReferral.mutate({
    patientId: patientId.value!,
    treatmentId: treatmentId.value!,
    notes: notes.value.trim() || undefined
  })

  if (!createReferral.error.value) {
    router.push('/referrals')
  }
}
</script>

<template>
  <main class="p-6 max-w-xl mx-auto space-y-4" aria-labelledby="title">
    <h1 id="title" class="text-xl font-semibold">
      Nieuwe doorverwijzing
    </h1>

    <!-- Non-blocking load / error messages -->
    <p v-if="loadingPatients || loadingTreatments" class="text-sm text-gray-500">
      Gegevens laden…
    </p>

    <p v-if="patientsError || treatmentsError" role="alert" class="text-sm text-red-600">
      Sommige gegevens konden niet worden geladen.
    </p>

    <form class="space-y-4" @submit.prevent="submitReferral">
      <!-- Patient -->
      <div>
        <label for="patient" class="block font-medium">
          Patiënt
        </label>

        <select id="patient" v-model="patientId" class="border p-2 rounded w-full" aria-describedby="patient-help"
          required>
          <option :value="null" disabled>
            {{ hasPatients ? 'Selecteer patiënt' : 'Geen patiënten beschikbaar' }}
          </option>

          <option v-for="p in patients ?? []" :key="p.patientId" :value="p.patientId">
            {{ p.fullName }}
          </option>
        </select>

        <p id="patient-help" v-if="!hasPatients" class="text-sm text-gray-600 mt-1">
          Er zijn momenteel geen patiënten beschikbaar om te selecteren.
        </p>
      </div>

      <!-- Treatment -->
      <div>
        <label for="treatment" class="block font-medium">
          Behandeling
        </label>

        <select id="treatment" v-model="treatmentId" class="border p-2 rounded w-full" aria-describedby="treatment-help"
          required>
          <option :value="null" disabled>
            {{ hasTreatments ? 'Selecteer behandeling' : 'Geen behandelingen beschikbaar' }}
          </option>

          <option v-for="t in treatments ?? []" :key="t.id" :value="t.id">
            {{ t.name }}
          </option>
        </select>

        <p id="treatment-help" v-if="!hasTreatments" class="text-sm text-gray-600 mt-1">
          Er zijn momenteel geen behandelingen beschikbaar.
        </p>
      </div>

      <!-- Notes -->
      <div>
        <label for="notes" class="block font-medium">
          Opmerkingen (optioneel)
        </label>

        <textarea id="notes" v-model="notes" class="border p-2 rounded w-full h-24"
          placeholder="Eventuele aanvullende informatie" />
      </div>

      <!-- Submit -->
      <button type="submit" :disabled="!canSubmit"
        class="bg-blue-600 text-white px-4 py-2 rounded w-full disabled:opacity-50">
        {{ createReferral.loading.value ? 'Verzenden…' : 'Verzenden' }}
      </button>

      <!-- Submit error -->
      <p v-if="createReferral.error.value" role="alert" class="text-sm text-red-600">
        {{ createReferral.error.value.response?.data || 'Er is een fout opgetreden bij het aanmaken van de doorverwijzing.' }}
      </p>
    </form>
  </main>
</template>
