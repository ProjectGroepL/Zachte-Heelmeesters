<script setup lang="ts">
import { ref, watch } from 'vue';
import { Button } from '@/components/ui/button';

const counter = ref<number>(0);

// GP combobox state
const gps = ref<{ id: number; fullName: string }[]>([]);
const selectedGpId = ref<number | null>(null);
const search = ref<string>('');

// Fetch GPs from the API using fetch()
const fetchGps = async (searchTerm = '') => {
  const query = encodeURIComponent(searchTerm);
  const response = await fetch(`https://localhost:7048/api/patients/gps?search=${query}`);
  if (response.ok) {
    gps.value = await response.json();
    console.log('Fetched GPs:', gps.value); // debugging
  } else {
    console.error('Failed to fetch GPs');
  }
};

// Watch search term for live filtering
watch(search, (newVal) => {
  fetchGps(newVal);
});

// Assign selected GP to patient (example: patientId = 2)
const assignGp = async () => {
  if (!selectedGpId.value) return;

  const response = await fetch(`https://localhost:7048/api/patients/2/gp`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ generalPractitionerId: selectedGpId.value })
  });

  if (response.ok) {
    alert('Huisarts succesvol geselecteerd!');
  } else {
    alert('Er is een fout opgetreden');
  }
};

// Initial fetch
fetchGps();
</script>

<template>
  <div class="flex flex-col justify-center items-center space-y-4 h-full">
    <div class="flex flex-col items-center">
      <h1 class="text-2xl font-bold">Zachte Heelmeesters</h1>
      <p>Dit is het startproject van de zachte heelmeesters app, probeer de counter eens uit!</p>
    </div>

    <!-- Existing counter button -->
    <Button @click="counter++">
      Count is: {{ counter }}
    </Button>

    <!-- GP combobox -->
    <div class="flex flex-col items-start space-y-2">
      <label for="gp-select" class="font-medium">Huisarts koppelen</label>
      <input id="gp-select" v-model="search" type="text" placeholder="Search by name..."
        class="border rounded p-2 w-64" />
      <select v-model="selectedGpId" class="border rounded p-2 w-64">
        <option :value="null" disabled>Selecteer uw huisarts</option>
        <option v-for="gp in gps" :key="gp.id" :value="gp.id">
          {{ gp.fullName }}
        </option>
      </select>
      <Button @click="assignGp">Huisarts selecteren</Button>
    </div>
  </div>
</template>
