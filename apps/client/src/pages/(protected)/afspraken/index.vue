<script setup lang="ts">
import { ref, onMounted } from 'vue';

interface Referral {
  id: number;
  patientId: number;
  doctorId: number;
  treatmentId: number;
  notes?: string;
  status: string;
  createdAt: string;
}

interface Treatment {
  id: number;
  name: string;
  specialInstructions?: string;
}

interface Appointment {
  id: number;
  referralId: number;
  referral: Referral;
  treatment?: Treatment;
}

const appointments = ref<Appointment[]>([]);
const loading = ref(true);

const fetchAppointments = async () => {
  try {
    const res = await fetch('https://localhost:5001/api/appointments');
    appointments.value = await res.json();
  } catch (err) {
    console.error('Fout bij ophalen van afspraken:', err);
  } finally {
    loading.value = false;
  }
};

onMounted(fetchAppointments);
</script>

<template>
  <div class="p-6 flex flex-col space-y-6">
    <h1 class="text-2xl font-bold">Dashboard Afspraken</h1>

    <div v-if="loading" class="text-gray-500">Afspraken laden...</div>

    <div v-else>
      <p v-if="appointments.length === 0" class="text-gray-600">Er zijn nog geen afspraken.</p>

      <div v-else class="overflow-x-auto">
        <table class="min-w-full table-auto border border-gray-300">
          <thead class="bg-gray-100">
            <tr>
              <th class="border px-4 py-2">ID</th>
              <th class="border px-4 py-2">Doorverwijzing ID</th>
              <th class="border px-4 py-2">Behandeling</th>
              <th class="border px-4 py-2">Status</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="appointment in appointments" :key="appointment.id" class="hover:bg-gray-50">
              <td class="border px-4 py-2">{{ appointment.id }}</td>
              <td class="border px-4 py-2">{{ appointment.referralId }}</td>
              <td class="border px-4 py-2">{{ appointment.treatment?.name ?? '-' }}</td>
              <td class="border px-4 py-2">{{ appointment.referral.status }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</template>
