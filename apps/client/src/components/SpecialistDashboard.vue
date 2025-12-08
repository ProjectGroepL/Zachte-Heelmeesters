<script setup lang="ts">
import { useRouter } from 'vue-router'

const router = useRouter()

function goToSchedule() {
  router.push('/rooster')
}

</script>

<template>
  <section class="p-6">
    <h1 class="text-2xl font-semibold mb-4">Dashboard specialist</h1>

    <div class="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
      <button
          type="button"
          class="rounded-lg border bg-white shadow-sm p-4 text-left hover:bg-slate-50 transition"
          @click="goToSchedule"
      >
        <h2 class="font-medium text-lg mb-1">Rooster &amp; afspraken</h2>
        <p class="text-sm text-slate-600">
          Bekijk je weekrooster en alle afspraken in één overzicht.
        </p>
      </button>
    </div>
  </section>
</template>
