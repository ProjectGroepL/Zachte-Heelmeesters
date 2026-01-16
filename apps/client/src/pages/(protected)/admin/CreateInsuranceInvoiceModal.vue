<!-- src/pages/admin/CreateInsuranceInvoiceModal.vue -->
<script setup lang="ts">
import { ref } from 'vue'
import { useInsurers } from '@/composables/useInsurers'
import { useCreateInsuranceInvoice } from '@/composables/useCreateInsuranceInvoice'

const props = defineProps<{
  reportId: number
}>()

const emit = defineEmits(['close'])

const insurerId = ref<number | null>(null)

const { data: insurers, loading: insurersLoading } = useInsurers()
const { mutate, loading } = useCreateInsuranceInvoice()

async function submit() {
  if (!insurerId.value) return

  await mutate({
    appointmentReportId: props.reportId,
    insurerId: insurerId.value,
  })

  emit('close')
}
</script>

<template>
  <!-- BACKDROP -->
  <div class="fixed inset-0 bg-black/40 flex items-center justify-center z-50">
    <!-- MODAL -->
    <div class="bg-white w-full max-w-md rounded-2xl shadow-lg p-6 space-y-6">
      <!-- HEADER -->
      <header>
        <h2 class="text-xl font-bold text-gray-900">
          Factuur aanmaken
        </h2>
        <p class="text-sm text-gray-500">
          Koppel dit rapport aan een zorgverzekeraar
        </p>
      </header>

      <!-- FORM -->
      <div class="space-y-2">
        <label class="block text-sm font-medium text-gray-700">
          Verzekeraar
        </label>

        <select
          v-model="insurerId"
          class="w-full border rounded-lg px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
        >
          <option disabled :value="null">
            Selecteer een verzekeraar
          </option>

          <option
            v-for="i in insurers"
            :key="i.id"
            :value="i.id"
          >
            {{ i.displayName }}
          </option>
        </select>

        <p v-if="insurersLoading" class="text-xs text-gray-500">
          Verzekeraars laden…
        </p>
      </div>

      <!-- ACTIONS -->
      <footer class="flex justify-end gap-3">
        <button
          class="px-4 py-2 rounded-lg border text-gray-700 hover:bg-gray-50"
          @click="$emit('close')"
        >
          Annuleren
        </button>

        <button
          class="px-4 py-2 rounded-lg bg-blue-600 text-white hover:bg-blue-700 disabled:opacity-50"
          :disabled="!insurerId || loading"
          @click="submit"
        >
          Opslaan
        </button>
      </footer>
    </div>
  </div>
</template>
