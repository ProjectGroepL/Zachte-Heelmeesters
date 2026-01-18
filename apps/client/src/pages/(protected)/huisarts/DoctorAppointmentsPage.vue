<script setup lang="ts">
import { ref, onMounted } from 'vue'
import AppointmentCard from '@/components/AppointmentCard.vue'
import RequestAccessModal from '@/components/RequestAccessModal.vue'
import api from '@/lib/api'

const appointments = ref<any[]>([])
const selectedAppointment = ref<any | null>(null)

async function loadAppointments() {
  const res = await api.get('/doctor/appointments')
  appointments.value = res.data
}

onMounted(loadAppointments)
</script>

<template>
  <section class="space-y-6">
    <h1 class="text-2xl font-bold">Afspraken & Rapporten</h1>

    <!-- EMPTY STATE -->
    <p v-if="appointments.length === 0" class="text-muted-foreground">
      Geen afspraken gevonden
    </p>

    <!-- LIST -->
    <div v-else class="grid gap-4">
      <AppointmentCard
        v-for="a in appointments"
        :key="a.id"
        :appointment="a"
        @request-access="selectedAppointment = a"
      />
    </div>

    <!-- MODAL -->
    <RequestAccessModal
  v-if="selectedAppointment"
  :appointmentId="selectedAppointment.id"
  @close="() => {
    selectedAppointment = null
    loadAppointments()
  }"
/>
  </section>
</template>
