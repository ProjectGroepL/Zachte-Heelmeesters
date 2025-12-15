<script setup lang="ts">
import { ref, watch } from 'vue'
import { useDoctorPatients } from '@/composables/useDoctorPatients'
import { useCreateReferral } from '@/composables/useReferral'
import { useRouter } from 'vue-router'

const router = useRouter()

// 🔹 QUERY: patiënten van dokter
const { data: patients, loading, error } = useDoctorPatients()

// 🔹 MUTATION: referral aanmaken
const createReferral = useCreateReferral()

// 🔹 form state
const patientId = ref<number | null>(null)
const treatmentId = ref<number | null>(1)
const notes = ref('')

// 🔹 zodra patiënten geladen zijn → selecteer eerste
watch(patients, (list) => {
  if (!list || list.length === 0) return

  if (patientId.value === null) {
    patientId.value = list[0]!.patientId
  }
})

// 🔹 submit
const submitReferral = async () => {
  if (!patientId.value || !treatmentId.value) return

  const result = await createReferral.mutate({
    patientId: patientId.value,
    treatmentId: treatmentId.value,
    notes: notes.value || undefined
  })

  if (result) {
    router.push('/referrals')
  }
}
</script>


<template>
  <div class="p-6 space-y-4">
    <h2 class="text-xl font-semibold">Nieuwe doorverwijzing</h2>

    <div v-if="loading">Patiënten laden...</div>
    <div v-else-if="error">Fout bij laden patiënten</div>

    <template v-else>
      <select v-model="patientId" class="border p-2 rounded w-full">
        <option
          v-for="p in patients"
          :key="p.patientId"
          :value="p.patientId"
        >
          {{ p.patientId }} — {{ p.fullName }}
        </option>
      </select>

      <input
        v-model="treatmentId"
        type="number"
        placeholder="Behandeling ID"
        class="border p-2 rounded w-full"
      />

      <textarea
        v-model="notes"
        placeholder="Opmerkingen (optioneel)"
        class="border p-2 rounded w-full h-24"
      />

      <button
        :disabled="createReferral.loading.value"
        @click="submitReferral"
        class="bg-blue-600 text-white px-4 py-2 rounded w-full"
      >
        {{ createReferral.loading.value ? 'Verzenden...' : 'Verzenden' }}
      </button>

      <div v-if="createReferral.error.value" class="text-red-600 text-sm">
        Verwijzing aanmaken mislukt
      </div>
    </template>
  </div>
</template>

