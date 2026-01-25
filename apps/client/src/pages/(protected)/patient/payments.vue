<script setup lang="ts">
import { usePatientInvoices } from '@/composables/usePatientInvoices'
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card'
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '@/components/ui/table'
import { Badge } from '@/components/ui/badge'
import { Receipt, Clock, Wallet } from 'lucide-vue-next'

const { data: invoices, loading } = usePatientInvoices()
</script>

<template>
  <div class="container mx-auto p-6 space-y-6">
    <div>
      <h1 class="text-3xl font-bold">Mijn betalingen</h1>
      <p class="text-muted-foreground mt-1">
        Overzicht van openstaande en verwerkte betalingen
      </p>
    </div>

    <Card>
      <CardHeader>
        <CardTitle class="flex items-center gap-2">
          <Receipt class="h-5 w-5" />
          Betalingen Overzicht
        </CardTitle>
      </CardHeader>
      <CardContent>
        <!-- Loading -->
        <div v-if="loading" class="flex flex-col items-center justify-center py-12 text-muted-foreground">
          <Clock class="h-12 w-12 mb-4 animate-pulse opacity-50" />
          <p class="font-medium">Betalingen laden...</p>
        </div>

        <!-- Table -->
        <div v-else-if="invoices?.length" class="rounded-md border">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Datum</TableHead>
                <TableHead>Omschrijving</TableHead>
                <TableHead>Bedrag</TableHead>
                <TableHead>Status</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              <TableRow
                v-for="i in invoices"
                :key="i.invoiceId"
              >
                <TableCell>
                  {{ new Date(i.date).toLocaleDateString('nl-NL') }}
                </TableCell>
                <TableCell class="font-medium">
                  Zorgkosten
                </TableCell>
                <TableCell class="font-semibold">
                  € {{ i.patientPays }}
                </TableCell>
                <TableCell>
                  <Badge
                    :variant="i.patientPays === 0 ? 'default' : 'secondary'"
                    :class="i.patientPays === 0
                      ? 'bg-green-100 text-green-700 hover:bg-green-100'
                      : 'bg-yellow-100 text-yellow-700 hover:bg-yellow-100'"
                  >
                    {{ i.patientPays === 0 ? 'Voldaan' : 'Openstaand' }}
                  </Badge>
                </TableCell>
              </TableRow>
            </TableBody>
          </Table>
        </div>

        <!-- Empty State -->
        <div v-else class="flex flex-col items-center justify-center py-12 text-muted-foreground">
          <Wallet class="h-12 w-12 mb-4 opacity-50" />
          <p class="font-medium">Geen betalingen gevonden</p>
          <p class="text-sm mt-2">Er zijn geen openstaande betalingen.</p>
        </div>
      </CardContent>
    </Card>
  </div>
</template>
