<script setup lang="ts">
import { ref, computed, onMounted } from "vue";
import { usePatientReferrals } from "@/composables/useReferral";
import { useCreateAppointment } from '@/composables/useCreateAppointment';
import type { AppointmentDto } from '@/types/appointment'
import { watch } from 'vue'
const {
  data: referrals,
  loading,
  error,
  execute: fetchReferrals
} = usePatientReferrals();
watch(referrals, (newVal) => {
  console.log('REFERRALS BINNEN:', newVal)
})
const {
  mutate: createAppointment,
  loading: creating,
  error: createError
} = useCreateAppointment();
const appointments = ref<AppointmentDto[]>([])
const selectedReferralId = ref<number | null>(null);
const appointmentDate = ref("");
const appointmentTime = ref("");
const successMessage = ref<string | null>(null);

/**
 * Null-safe referrals
 */
const safeReferrals = computed(() => referrals.value ?? []);

/**
 * Geselecteerde referral als object
 */
const selectedReferral = computed(() =>
  safeReferrals.value.find(r => r.id === selectedReferralId.value) ?? null
);

const submit = async () => {
  if (!selectedReferralId.value || !appointmentDate.value || !appointmentTime.value) {
    return;
  }

  const result = await createAppointment({
    referralId: selectedReferralId.value,
    date: appointmentDate.value,
    time: appointmentTime.value
  });

  if (result !== undefined) {
    successMessage.value = "Afspraak aangemaakt!";
      setTimeout(() => {
    successMessage.value = null;
  }, 2000);
    selectedReferralId.value = null;
    appointmentDate.value = "";
    appointmentTime.value = "";
    fetchReferrals(); // refresh select
  }
};

</script>

<template>
  <div class="p-6">
    <h1 class="text-2xl font-bold mb-4">Nieuwe afspraak</h1>

    <!-- Status -->
    <div v-if="loading" class="text-gray-500">
      Doorverwijzingen laden...
    </div>

    <div v-else-if="error" class="text-red-500">
      {{ error }}
    </div>

    <div
      v-if="successMessage"
      class="bg-green-200 text-green-800 p-2 rounded mb-4"
    >
      {{ successMessage }}
    </div>

    <!-- Formulier -->
    <div>
      <label for="referral" class="block mb-2">
        Kies een doorverwijzing:
      </label>

      <!-- Referral select -->
      <div v-if="safeReferrals.length > 0">
        <select
          v-model="selectedReferralId"
          id="referral"
          class="border p-2 rounded w-full"
        >
          <option :value="null" disabled>
            Selecteer een doorverwijzing
          </option>

          <option
            v-for="ref in safeReferrals"
            :key="ref.id"
            :value="ref.id"
            >
            {{ ref.treatmentName }} - {{ ref.status }}
            </option>
        </select>
      </div>

      <div v-else class="text-red-500">
        Er zijn geen actieve doorverwijzingen beschikbaar.
      </div>

      <!-- Datum & tijd -->
      <div v-if="selectedReferral" class="mt-4 space-y-2">
        <p>
          Je hebt doorverwijzing
          <strong>{{ selectedReferral.treatmentName }}</strong>
          geselecteerd.
        </p>
        <fieldset>
        <legend class="font-semibold">Datum en tijd</legend>
          <label class="date">Datum:</label>
          <input
            type="date"
            v-model="appointmentDate"
            :min="new Date(selectedReferral.createdAt).toISOString().split('T')[0]"
            class="border p-2 rounded w-full"
            required
          />

          <label class="time">Tijd:</label>
          <input
            type="time"
            v-model="appointmentTime"
            class="border p-2 rounded w-full"
            required
          />
        </fieldset>
        
        <button
        @click="submit"
        :disabled="creating || !appointmentDate || !appointmentTime"
        class="bg-black text-white px-4 py-2 rounded mt-2 w-full
                hover:bg-blue-600 disabled:opacity-50"
        >
        {{ creating ? 'Bezig met opslaan...' : 'Maak afspraak' }}
        </button>

        <p v-if="createError" class="text-red-500 mt-2">
          Kon de afspraak niet aanmaken.
        </p>
      </div>
    </div>
  </div>
</template>

