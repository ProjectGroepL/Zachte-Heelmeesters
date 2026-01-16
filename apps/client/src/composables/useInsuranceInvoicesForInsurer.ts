// src/composables/useInsuranceInvoicesForInsurer.ts
import { useQuery, useMutation } from '@/composables/useApi'
import api from '@/lib/api';
import type { InsuranceInvoiceForInsurer } from '@/types/insurance'

export function useInsuranceInvoicesForInsurer() {
  return useQuery<InsuranceInvoiceForInsurer[]>(
    '/insurance/invoices'
  )
}
export function useSetCoveredAmount() {
  return useMutation<void, { invoiceId: number; coveredAmount: number }>(
    ({ invoiceId, coveredAmount }: { invoiceId: number; coveredAmount: number }) =>
      api.post(`/insurance/invoices/${invoiceId}/coverage`, {
      coveredAmount
    })
  )
}