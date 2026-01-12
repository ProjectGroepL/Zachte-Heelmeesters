<script setup lang="ts">
import { ref } from 'vue'
import { useDoctorPatients } from '@/composables/useDoctorPatients'
import { useCreateMedicalDocument } from '@/composables/useMedicalDocuments'

const { data: patients, loading } = useDoctorPatients()
const createDoc = useCreateMedicalDocument()

const patientId = ref<number | null>(null)
const title = ref('')
const content = ref('')

const success = ref(false)
const errorMessage = ref<string | null>(null)

const submit = async () => {
  if (!patientId.value) return

  success.value = false
  errorMessage.value = null

  try {
    await createDoc.mutate({
      patientId: patientId.value,
      title: title.value,
      content: content.value
    })

    // ✅ Success feedback
    success.value = true

    // Reset form
    patientId.value = null
    title.value = ''
    content.value = ''
  } catch (e) {
    errorMessage.value = 'Opslaan van het document is mislukt.'
  }
}
</script>

<template>
  <main
    class="max-w-2xl mx-auto p-6"
    aria-labelledby="doc-title"
  >
    <h1 id="doc-title" class="text-2xl font-bold mb-4">
      Medisch document aanmaken
    </h1>

    <!-- ✅ Success message -->
    <div
      v-if="success"
      class="mb-4 p-4 bg-green-100 border border-green-400 text-green-700 rounded"
      role="status"
      aria-live="polite"
    >
      ✅ Medisch document is succesvol opgeslagen.
    </div>

    <!-- ❌ Error message -->
    <div
      v-if="errorMessage"
      class="mb-4 p-4 bg-red-100 border border-red-400 text-red-700 rounded"
      role="alert"
    >
      {{ errorMessage }}
    </div>

    <form @submit.prevent="submit" class="space-y-4">
      <!-- Patient -->
      <div>
        <label for="patient" class="block font-medium">
          Patiënt
        </label>
        <select
          id="patient"
          v-model="patientId"
          class="border p-2 rounded w-full"
          :disabled="loading || createDoc.loading.value"
          required
        >
          <option disabled :value="null">
            Selecteer patiënt
          </option>
          <option
            v-for="p in patients ?? []"
            :key="p.patientId"
            :value="p.patientId"
          >
            {{ p.fullName }}
          </option>
        </select>
      </div>

      <!-- Title -->
      <div>
        <label for="title" class="block font-medium">
          Titel
        </label>
        <input
          id="title"
          v-model="title"
          class="border p-2 rounded w-full"
          required
        />
      </div>

      <!-- Content -->
      <div>
        <label for="content" class="block font-medium">
          Inhoud
        </label>
        <textarea
          id="content"
          v-model="content"
          class="border p-2 rounded w-full h-32"
          required
        />
      </div>

      <!-- Submit -->
      <button
        type="submit"
        class="bg-blue-600 text-white px-4 py-2 rounded w-full disabled:opacity-50"
        :disabled="createDoc.loading.value"
        :aria-busy="createDoc.loading.value"
      >
        {{ createDoc.loading.value ? 'Opslaan…' : 'Opslaan' }}
      </button>
    </form>
  </main>
</template>