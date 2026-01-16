<script setup lang="ts">
import { ref } from 'vue'
import { useInsuranceInvoicesForInsurer,useSetCoveredAmount } from '@/composables/useInsuranceInvoicesForInsurer'

const {
  data: invoices,
  loading,
  error,
  execute: refetch
} = useInsuranceInvoicesForInsurer()
const { mutate } = useSetCoveredAmount()

const coverage = ref<Record<number, number>>({})
const validationError = ref<Record<number, string | null>>({})

async function approve(invoiceId: number, max: number) {
  const value = coverage.value[invoiceId]

  if (!validateCoverage(invoiceId, value, max)) return

  const confirmed = confirm(
    `Weet u zeker dat u € ${value} wilt vergoeden?`
  )

  if (!confirmed) return

  await mutate({
    invoiceId,
    coveredAmount: value!
  })

  await refetch()
}
function validateCoverage(
  invoiceId: number,
  value: number | undefined,
  max: number
) {
  if (value == null || Number.isNaN(value)) {
    validationError.value[invoiceId] = 'Vul een bedrag in'
    return false
  }

  if (value < 0) {
    validationError.value[invoiceId] = 'Bedrag mag niet negatief zijn'
    return false
  }

  if (value > max) {
    validationError.value[invoiceId] =
      `Maximale vergoeding is € ${max}`
    return false
  }

  validationError.value[invoiceId] = null
  return true
}
function clampCoverage(invoiceId: number, max: number) {
  let value = coverage.value[invoiceId]

  if (value == null || Number.isNaN(value)) {
    coverage.value[invoiceId] = 0
    return
  }

  if (value < 0) {
    coverage.value[invoiceId] = 0
  } else if (value > max) {
    coverage.value[invoiceId] = max
  }
}
</script>


<template>
  <section class="space-y-6">
    <!-- HEADER -->
    <header>
      <h1 class="text-2xl font-bold text-gray-900">
        Declaraties
      </h1>
      <p class="text-sm text-gray-500">
        Overzicht van openstaande en verwerkte declaraties
      </p>
    </header>

    <!-- LOADING / ERROR -->
    <p v-if="loading" class="text-gray-500">Laden…</p>
    <p v-else-if="error" class="text-red-600">Fout bij laden</p>

    <!-- TABLE -->
    <div
      v-else-if="invoices?.length"
      class="bg-white rounded-2xl shadow overflow-hidden"
    >
      <table class="w-full border-collapse">
        <thead class="bg-gray-50 text-left text-sm text-gray-600">
          <tr>
            <th class="px-4 py-3">Datum</th>
            <th class="px-4 py-3">Bedrag</th>
            <th class="px-4 py-3">Vergoeding</th>
            <th class="px-4 py-3">Patiënt betaalt</th>
          </tr>
        </thead>

        <tbody class="divide-y">
          <tr
            v-for="i in invoices"
            :key="i.invoiceId"
            class="hover:bg-gray-50"
          >
            <!-- DATE -->
            <td class="px-4 py-3">
              {{ new Date(i.date).toLocaleDateString() }}
            </td>

            <!-- TOTAL -->
            <td class="px-4 py-3 font-medium">
              € {{ i.amount }}
            </td>

            <!-- COVERED -->
            <td class="px-4 py-3">
              <template v-if="i.coveredAmount === null">
                <div>
                <input
                  type="number"
                  class="w-24 border rounded-md px-2 py-1 text-sm"
                  :class="validationError[i.invoiceId] && 'border-red-500'"
                  v-model.number="coverage[i.invoiceId]"
                  @input="validateCoverage(i.invoiceId, coverage[i.invoiceId], i.amount)"
                />

                <button
                  class="px-3 py-1 rounded-md bg-blue-600 text-white text-sm hover:bg-blue-700 disabled:opacity-50"
                  :disabled="!!validationError[i.invoiceId]"
                  @click="approve(i.invoiceId, i.amount)"
                >
                  Opslaan
                </button>

                <p
                  v-if="validationError[i.invoiceId]"
                  class="text-xs text-red-600 mt-1"
                >
                  {{ validationError[i.invoiceId] }}
                </p>
                </div>
              </template>

              <template v-else>
                <span class="text-green-600 font-medium">
                  € {{ i.coveredAmount }}
                </span>
              </template>
            </td>

            <!-- PATIENT PAYS -->
            <td class="px-4 py-3">
              <span
                :class="i.patientPays === 0
                  ? 'text-green-600'
                  : 'text-gray-700'"
              >
                € {{ i.patientPays }}
              </span>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- EMPTY -->
    <div
      v-else
      class="text-center py-12 text-gray-500"
    >
      Geen declaraties
    </div>
  </section>
</template>

