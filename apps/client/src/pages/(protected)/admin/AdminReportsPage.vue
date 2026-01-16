<!-- src/pages/admin/AdminReportsPage.vue -->
<script setup lang="ts">
import { ref } from 'vue'
import { useAdminReports } from '@/composables/useAdminReports'
import CreateInsuranceInvoiceModal from './CreateInsuranceInvoiceModal.vue'

const { data: reports, loading, error, execute } = useAdminReports()
const selectedReportId = ref<number | null>(null)
</script>

<template>
  <main class="max-w-6xl mx-auto p-6 space-y-6">
    <!-- PAGE HEADER -->
    <header>
      <h1 class="text-2xl font-bold text-gray-900">
        Openstaande rapporten
      </h1>
      <p class="text-sm text-gray-500">
        Rapporten die nog gefactureerd moeten worden aan de verzekeraar
      </p>
    </header>

    <!-- LOADING -->
    <div v-if="loading" class="text-gray-500">
      Rapporten laden…
    </div>

    <!-- ERROR -->
    <div
      v-else-if="error"
      class="p-4 bg-red-50 border border-red-200 rounded-lg text-red-700"
    >
      Er is iets misgegaan bij het laden van de rapporten.
    </div>

    <!-- EMPTY STATE -->
    <div
      v-else-if="reports && reports.length === 0"
      class="p-8 border-2 border-dashed rounded-xl text-center text-gray-500"
    >
      Geen openstaande rapporten 🎉
    </div>

    <!-- TABLE -->
    <div
      v-else
      class="overflow-hidden border border-gray-200 rounded-xl bg-white shadow-sm"
    >
      <table class="min-w-full text-sm">
        <thead class="bg-gray-50 text-gray-600 uppercase text-xs">
          <tr>
            <th class="px-6 py-3 text-left">Patiënt</th>
            <th class="px-6 py-3 text-left">Datum</th>
            <th class="px-6 py-3 text-right">Totaal</th>
            <th class="px-6 py-3 text-right"></th>
          </tr>
        </thead>

        <tbody class="divide-y">
          <tr
            v-for="r in reports"
            :key="r.reportId"
            class="hover:bg-gray-50"
          >
            <td class="px-6 py-4 font-medium text-gray-900">
              {{ r.patientName }}
            </td>

            <td class="px-6 py-4 text-gray-600">
              {{ new Date(r.createdAt).toLocaleDateString() }}
            </td>

            <td class="px-6 py-4 text-right font-semibold">
              € {{ r.totalCost.toFixed(2) }}
            </td>

            <td class="px-6 py-4 text-right">
              <button
                class="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition"
                @click="selectedReportId = r.reportId"
              >
                Factuur maken
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- MODAL -->
    <CreateInsuranceInvoiceModal
      v-if="selectedReportId"
      :report-id="selectedReportId"
      @close="() => {
        selectedReportId = null
        execute()
      }"
    />
  </main>
</template>
