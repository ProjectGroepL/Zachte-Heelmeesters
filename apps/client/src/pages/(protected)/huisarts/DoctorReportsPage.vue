<script setup lang="ts">
import { onMounted, ref } from 'vue'
import api from '@/lib/api'

const reports = ref<any[]>([])

onMounted(async () => {
  const res = await api.get('/doctor/reports')
  reports.value = res.data
})
</script>

<template>
  <section class="space-y-6">
    <h1 class="text-2xl font-bold">Toegankelijke rapporten</h1>

    <p v-if="reports.length === 0" class="text-muted-foreground">
      U heeft nog geen goedgekeurde rapporten.
    </p>

    <div v-else class="space-y-4">
      <div
        v-for="r in reports"
        :key="r.id"
        class="border rounded-xl p-4 bg-card"
      >
        <h2 class="font-semibold">Rapport #{{ r.id }}</h2>

        <p class="text-sm text-muted-foreground">
          Aangemaakt op {{ new Date(r.createdAt).toLocaleDateString() }}
        </p>

        <p class="mt-2">{{ r.summary }}</p>

        <ul class="mt-2 list-disc ml-5 text-sm">
          <li v-for="i in r.items" :key="i">{{ i }}</li>
        </ul>
      </div>
    </div>
  </section>
</template>
