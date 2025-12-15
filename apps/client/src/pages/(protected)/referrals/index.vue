<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useDocterReferrals } from '@/composables/useDoctorReferral'

const {
  data: referrals,
  loading,
  error
} = useDocterReferrals()


</script>

<template>
  <div class="p-6">
    <h2 class="text-xl font-bold mb-4">Doorverwijzingen</h2>

    <div v-if="loading">Laden...</div>
    <div v-else-if="error">Fout bij laden</div>

    <div v-else-if="!referrals?.length" class="p-4 text-muted">
      Geen doorverwijzingen gevonden.
    </div>

    <table v-else class="w-full border">
      <thead>
        <tr class="bg-gray-100">
          <th class="p-2">Patiënt</th>
          <th class="p-2">Behandeling</th>
          <th class="p-2">Aangemaakt op</th>
        </tr>
      </thead>

      <tbody>
        <tr v-for="r in referrals" :key="r.id" class="border-t">
          <td class="p-2">{{ r.patientName }}</td>
          <td class="p-2">{{ r.treatmentName }}</td>
          <td class="p-2">{{ new Date(r.createdAt).toLocaleDateString() }}</td>
        </tr>
      </tbody>
    </table>
  </div>
</template>
