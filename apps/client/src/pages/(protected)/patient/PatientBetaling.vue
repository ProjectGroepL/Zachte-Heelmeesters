<script setup lang="ts">
import { usePatientInvoices } from '@/composables/usePatientInvoices'

const { data: invoices, loading } = usePatientInvoices()
</script>

<template>
  <section class="space-y-6">
    <!-- HEADER -->
    <header>
      <h1 class="text-2xl font-bold text-gray-900">
        Mijn betalingen
      </h1>
      <p class="text-sm text-gray-500">
        Overzicht van openstaande en verwerkte betalingen
      </p>
    </header>

    <!-- LOADING -->
    <p v-if="loading" class="text-gray-500">
      Laden…
    </p>

    <!-- TABLE -->
    <div
      v-else-if="invoices?.length"
      class="bg-white rounded-2xl shadow overflow-hidden"
    >
      <table class="w-full border-collapse">
        <thead class="bg-gray-50 text-left text-sm text-gray-600">
          <tr>
            <th class="px-4 py-3">Datum</th>
            <th class="px-4 py-3">Omschrijving</th>
            <th class="px-4 py-3">Bedrag</th>
            <th class="px-4 py-3">Status</th>
          </tr>
        </thead>

        <tbody class="divide-y">
          <tr
            v-for="i in invoices"
            :key="i.invoiceId"
            class="hover:bg-gray-50"
          >
            <td class="px-4 py-3">
              {{ new Date(i.date).toLocaleDateString() }}
            </td>

            <td class="px-4 py-3 text-gray-700">
              Zorgkosten
            </td>

            <td class="px-4 py-3 font-medium">
              € {{ i.patientPays }}
            </td>

            <td class="px-4 py-3">
              <span
                class="px-2 py-1 rounded-full text-xs font-medium"
                :class="i.patientPays === 0
                  ? 'bg-green-100 text-green-700'
                  : 'bg-yellow-100 text-yellow-700'"
              >
                {{ i.patientPays === 0 ? 'Voldaan' : 'Openstaand' }}
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
      Geen openstaande betalingen
    </div>
  </section>
</template>
