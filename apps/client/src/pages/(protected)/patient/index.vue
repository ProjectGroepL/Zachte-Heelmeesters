<script setup lang="ts">
import {
  useMyMedicalDocuments,
  useUpdateMedicalDocumentStatus
} from '@/composables/useMedicalDocuments'
import { 
  usePatientAccessRequests, 
  useDecideAccessRequest 
} from '@/composables/useAccessRequests'

const { data, loading, execute } = useMyMedicalDocuments()
const updateStatus = useUpdateMedicalDocumentStatus()

const changeStatus = async (id: number, status: 'Final' | 'Archived') => {
  await updateStatus.mutate({ id, status })
  await execute()
}
</script>

<template>
  <main
    aria-labelledby="documents-title"
    :aria-busy="loading"
    class="max-w-4xl mx-auto p-6"
  >
    <h1 id="documents-title" class="text-2xl font-bold mb-4">
      Mijn medische documenten
    </h1>

    <!-- Loading -->
    <p v-if="loading" role="status" aria-live="polite">
      Documenten worden geladen…
    </p>

    <!-- Empty state -->
    <p v-else-if="!data || data.length === 0" class="text-gray-500">
      Er zijn nog geen medische documenten.
    </p>

    <!-- Documents list -->
    <section
      v-else
      aria-label="Lijst met medische documenten"
      class="space-y-4"
    >
      <article
        v-for="doc in data"
        :key="doc.id"
        class="border rounded-xl p-4"
        role="region"
        :aria-labelledby="`doc-title-${doc.id}`"
      >
        <h2
          :id="`doc-title-${doc.id}`"
          class="text-lg font-semibold"
        >
          {{ doc.title }}
        </h2>

        <p class="mt-2 text-gray-700">
          {{ doc.content }}
        </p>

        <p class="mt-2 text-sm text-gray-600">
          Status:
          <strong>{{ doc.status }}</strong>
        </p>

        <div class="mt-4 flex gap-2">
          <button
            v-if="doc.status === 'Draft'"
            type="button"
            class="px-4 py-2 rounded bg-blue-600 text-white"
            @click="changeStatus(doc.id, 'Final')"
            :aria-label="`Document ${doc.title} finaliseren`"
          >
            Finaliseren
          </button>

          <button
            v-if="doc.status === 'Final'"
            type="button"
            class="px-4 py-2 rounded bg-gray-600 text-white"
            @click="changeStatus(doc.id, 'Archived')"
            :aria-label="`Document ${doc.title} archiveren`"
          >
            Archiveren
          </button>
        </div>
      </article>
    </section>
  </main>
</template>
