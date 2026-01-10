<script setup lang="ts">
import { useAppointmentMedicalDocuments } from '@/composables/useMedicalDocuments'

const props = defineProps<{
  appointmentId: number
}>()

const { data, loading } =
  useAppointmentMedicalDocuments(props.appointmentId)

  
</script>

<template>
  <div class="pl-4 border-l">
    <p v-if="loading">Documenten laden…</p>

    <p v-else-if="!loading && (!data || data.length === 0)" class="text-gray-400 italic">
      Geen documenten beschikbaar
    </p>

    <p v-if="!data || data.length === 0" class="text-gray-400 italic">
      Er zijn geen definitieve medische documenten beschikbaar.
    </p>

    <ul v-else class="space-y-3">
      <li
        v-for="doc in data"
        :key="doc.id"
        class="p-3 border rounded-lg"
      >
        <h3 class="font-semibold">{{ doc.title }}</h3>
        <p class="text-sm text-gray-700">{{ doc.content }}</p>
      </li>
    </ul>
  </div>
</template>
