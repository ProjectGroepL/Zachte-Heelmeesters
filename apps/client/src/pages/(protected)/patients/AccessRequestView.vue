<script setup lang="ts">
    import {
        usePatientAccessRequests,
        useDecideAccessRequest
    } from "@/composables/useAccessRequests"

    const {
        data: requests,
        loading,
        error,
        execute: refetch
    } = usePatientAccessRequests();

    const decideMutation = useDecideAccessRequest();

    const decide = async (id: number, approved: boolean) => {
        await decideMutation.mutate({ id, data: { approved } })
        if (!decideMutation.error.value) {
            await refetch()
        }
    }
</script>

<template>
  <main aria-labelledby="access-title" class="max-w-4xl mx-auto p-6">
    <h1 id="access-title" class="text-2xl font-bold mb-4">
      Toegangsverzoeken
    </h1>

    <div v-if="loading" role="status" aria-live="polite">
      Verzoeken laden…
    </div>

    <div v-if="error" role="alert">
      {{ error.message }}
    </div>

    <ul v-if="requests?.length" class="space-y-4">
      <li
        v-for="r in requests"
        :key="r.id"
        class="p-4 border rounded-xl"
      >
        <fieldset>
          <legend class="font-semibold">
            Verzoek van {{ r.specialistName }}
          </legend>

          <p class="text-sm text-gray-600">
            Reden: {{ r.reason }}
          </p>

          <div class="flex gap-2 mt-4">
            <button
              class="px-4 py-2 bg-green-600 text-white rounded"
              @click="decide(r.id, true)"
              :disabled="decideMutation.loading.value"
            >
              Goedkeuren
            </button>

            <button
              class="px-4 py-2 bg-red-600 text-white rounded"
              @click="decide(r.id, false)"
              :disabled="decideMutation.loading.value"
            >
              Weigeren
            </button>
          </div>
        </fieldset>
      </li>
    </ul>

    <p v-else class="text-gray-500">
      Geen openstaande verzoeken.
    </p>
  </main>
</template>