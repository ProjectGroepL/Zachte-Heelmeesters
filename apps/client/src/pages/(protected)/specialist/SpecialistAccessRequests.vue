<script setup lang="ts">
import { useSpecialistAccessRequests } from '@/composables/useSpecialistAccessRequests'
import { onMounted, onUnmounted } from 'vue'

const { data, loading, error, execute: refetch } =
  useSpecialistAccessRequests()

let interval: number | undefined

onMounted(() => {
  interval = window.setInterval(refetch, 100_000)
})

onUnmounted(() => {
  if (interval) clearInterval(interval)
})
</script>

<template>
  <main class="max-w-5xl mx-auto p-6" aria-labelledby="title">
    <h1 id="title" class="text-2xl font-bold mb-4">
      Mijn toegangsaanvragen
    </h1>

    <div v-if="loading" role="status">Laden…</div>
    <div v-else-if="error" role="alert" class="text-red-600">
      Fout bij laden
    </div>

    <table v-else class="w-full border rounded">
      <thead class="bg-gray-100">
        <tr>
          <th class="p-2 text-left">Afspraak</th>
          <th class="p-2 text-left">Reden</th>
          <th class="p-2 text-left">Status</th>
          <th class="p-2 text-left">Datum</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="r in data" :key="r.id" class="border-t">
          <td class="p-2">
            {{ r.patientName }}
          </td>
          
          <td class="p-2">{{ r.reason }}</td>
          <td class="p-2 font-medium">
            <span
              :class="{
                'text-yellow-600': r.status === 'Pending',
                'text-green-600': r.status === 'Approved',
                'text-red-600': r.status === 'Denied',
                'text-gray-500': r.status === 'Revoked'
              }"
            >
              {{ r.status }}
            </span>
          </td>
          <td class="p-2">
            {{ new Date(r.requestedAt).toLocaleDateString('nl-NL') }}
          </td>
        </tr>
      </tbody>
    </table>
  </main>
</template>
