<script setup lang="ts">
import { ref, onMounted } from 'vue'
import api from '@/lib/api'
import { useReferral } from "@/composables/userReferral";
import { useRouter } from 'vue-router'

interface DoctorPatient {
  patientId: number
  fullName: string
}

const { createReferral } = useReferral()
const router = useRouter()
const patients = ref<DoctorPatient[]>([])
const patientId = ref<number | null>(null)

const treatmentId = ref<number | null>(1)
const notes = ref<string>('')
const submitting = ref(false)

onMounted(async () => {
  try {
    const res = await api.get('/DoctorPatients') // controller: DoctorPatientsController.GetMyPatients
    const raw = res.data || []
    // Debug: log raw payload to help identify casing/shape issues
    console.debug('DoctorPatients raw response:', raw)

    // Normalize possible server shapes (camelCase / PascalCase / different keys)
    patients.value = (raw as any[])
      .map(p => ({
        patientId: p.patientId ?? p.PatientId ?? p.PatientID ?? p.patientID ?? null,
        fullName:
          p.fullName ?? p.FullName ?? p.full_name ?? p.Full_Name ??
          ((p.firstName ?? p.FirstName ?? '') + ' ' + (p.lastName ?? p.LastName ?? '')).trim() ?? '(onbekend)'
      }))
      .filter(p => p.patientId != null)

    if (patients.value.length) {
      const first = patients.value[0]
      patientId.value = first?.patientId ?? null
    }
  } catch (err) {
    console.error('Failed to load patients', err)
  }
})

const submitReferral = async () => {
  if (!patientId.value) { alert('Selecteer een patiënt'); return }
  try {
    submitting.value = true
    await createReferral({
      patientId: patientId.value,
      treatmentId: treatmentId!.value ?? 1,
      notes: notes.value || undefined
    })
    router.push('/referrals')
  } catch (err) {
    alert("Verwijzing aanmaken mislukt")
  } finally {
    submitting.value = false
  }
}
</script>

<template>
  <div class="p-6 space-y-4">
    <h2 class="text-xl font-semibold">Nieuwe doorverwijzing</h2>

    <select v-model="patientId" class="border p-2 rounded w-full">
  <option v-for="p in patients" :key="p.patientId" :value="p.patientId">
    {{ p.patientId }} — {{ p.fullName }}
  </option>
</select>

<!-- If you have a treatments list, replace with a select; fallback to numeric input otherwise -->
<input v-model="treatmentId" type="number" placeholder="Behandeling ID" class="border p-2 rounded w-full" />

<textarea v-model="notes" placeholder="Opmerkingen (optioneel)" class="border p-2 rounded w-full h-24"/>

    <button
      :disabled="submitting"
      @click="submitReferral"
      class="bg-blue-600 text-white px-4 py-2 rounded w-full"
    >
      {{ submitting ? 'Verzenden...' : 'Verzenden' }}
    </button>
  </div>
</template>
