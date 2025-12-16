<script setup lang="ts">
import { computed, watch } from 'vue' // Voeg watch toe
import { useRoute } from 'vue-router'
import { usePatientMedicalDocuments } from '@/composables/useMedicalDocuments'

const route = useRoute()

// 1. Fix de TypeScript error op route.params
// Door 'as any' of een interface te gebruiken weet TS dat 'id' bestaat
const patientId = computed(() => {
  const params = route.params as { id?: string }
  const id = params.id
  return id ? Number(id) : 0
})

// 2. Initialiseer de query
const { data, loading, execute } = usePatientMedicalDocuments(patientId.value)

// 3. Zorg dat de data ververst als de route verandert (bijv. van patient 1 naar 2)
watch(patientId, () => {
  execute()
})

</script>


<template>
  <main
    aria-labelledby="patient-documents-title"
    :aria-busy="loading"
    class="max-w-5xl mx-auto p-6"
  >
    <h1 id="patient-documents-title" class="text-2xl font-bold mb-4">
      Medisch dossier van patiënt
    </h1>

    <p v-if="loading" role="status" aria-live="polite">
      Medische documenten worden geladen…
    </p>

    <p v-else-if="!data || data?.length === 0" class="text-gray-500">
    Deze patiënt heeft geen gedeelde medische documenten.
    </p>

    <section
      v-else
      aria-label="Gedeelde medische documenten"
      class="space-y-4"
    >
      <article
        v-for="doc in data ?? []" 
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

        <time
          class="block mt-2 text-sm text-gray-500"
          :datetime="doc.createdAt"
        >
          Aangemaakt op
          {{ new Date(doc.createdAt).toLocaleDateString('nl-NL') }}
        </time>
      </article>
    </section>
  </main>
</template>

