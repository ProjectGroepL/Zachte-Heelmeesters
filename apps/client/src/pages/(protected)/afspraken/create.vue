<script setup lang="ts">
import { ref, computed } from 'vue'
import { usePatientReferrals } from '@/composables/useReferral'
import { useSpecialists } from '@/composables/useSpecialists.ts'
import { useCreateAppointment } from '@/composables/useCreateAppointment'

const { data: referrals, loading: loadingReferrals, error: referralsError } = usePatientReferrals()
const { data: specialists, loading: loadingSpecialists, error: specialistsError } = useSpecialists()
const createAppointment = useCreateAppointment()

const referralId = ref<number | null>(null)
const specialistId = ref<number | null>(null)
const date = ref('')
const time = ref('')
const success = ref(false)

const canSubmit = computed(() =>
  referralId.value &&
  specialistId.value &&
  date.value &&
  time.value &&
  !createAppointment.loading.value
)

const submit = async () => {
  if (!canSubmit.value) return

  success.value = false

  try {
    await createAppointment.mutate({
      referralId: referralId.value!,
      specialistId: specialistId.value!,
      date: date.value,
      time: time.value
    })

    // ✅ succes
    success.value = true

    // (optioneel) formulier resetten
    referralId.value = null
    specialistId.value = null
    date.value = ''
    time.value = ''
  } catch (e) {
    // error wordt al door composable gezet
  }
}

</script>

<template>
    <p
    v-if="success"
    class="text-sm text-green-600 bg-green-50 border border-green-200 p-2 rounded"
  >
    ✅ Afspraak is succesvol aangemaakt.
  </p>
  <main class="p-6 max-w-xl mx-auto space-y-4">
    <h1 class="text-xl font-semibold">Nieuwe afspraak</h1>

    <p v-if="loadingReferrals || loadingSpecialists" class="text-sm text-gray-500">
      Gegevens laden…
    </p>

    <p v-if="referralsError || specialistsError" class="text-sm text-red-600">
      Sommige gegevens konden niet worden geladen.
    </p>

    <form class="space-y-4" @submit.prevent="submit">

      <!-- Referral -->
      <div>
        <label class="block font-medium">Doorverwijzing</label>
        <select v-model="referralId" class="border p-2 rounded w-full" required>
          <option :value="null" disabled>Selecteer doorverwijzing</option>
          <option v-for="r in referrals ?? []" :key="r.id" :value="r.id">
            {{ r.treatmentName }}
          </option>
        </select>
      </div>

      <!-- Specialist -->
      <div>
        <label class="block font-medium">Specialist</label>
        <select v-model="specialistId" class="border p-2 rounded w-full" required>
          <option :value="null" disabled>Selecteer specialist</option>
          <option v-for="s in specialists ?? []" :key="s.id" :value="s.id">
            {{ s.fullName }}
          </option>
        </select>
      </div>

      <!-- Date & time -->
      <div>
        <label class="block font-medium">Datum</label>
        <input type="date" v-model="date" class="border p-2 rounded w-full" required />
      </div>

      <div>
        <label class="block font-medium">Tijd</label>
        <input type="time" v-model="time" class="border p-2 rounded w-full" required />
      </div>

      <button
        type="submit"
        :disabled="!canSubmit"
        class="bg-blue-600 text-white px-4 py-2 rounded w-full disabled:opacity-50"
      >
        {{ createAppointment.loading.value ? 'Opslaan…' : 'Afspraak maken' }}
      </button>

      <p v-if="createAppointment.error.value" class="text-sm text-red-600">
        Afspraak aanmaken mislukt.
      </p>
    </form>
  </main>
</template>

