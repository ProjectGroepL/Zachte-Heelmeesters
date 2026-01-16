// src/composables/usePatientInvoices.ts
import { useQuery } from '@/composables/useApi'
import type { PatientInvoiceView } from '@/types/patient'

export function usePatientInvoices() {
  return useQuery<PatientInvoiceView[]>(
    '/patient/invoices'
  )
}
