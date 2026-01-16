import { useQuery, useMutation } from '@/composables/useApi'
import api from '@/lib/api'
import type { InsuranceInvoiceDto, CreateInsuranceInvoiceDto } from '@/types/insurance'

export function useInsuranceInvoices() {
  return useQuery<InsuranceInvoiceDto[]>('/api/insurance/invoices', {
    transform: (data) => data
  })
}

export function useCreateInsuranceInvoice() {
  return useMutation<void, CreateInsuranceInvoiceDto>((dto) =>
    api.post('/insurance/invoices', dto).then(() => undefined)
  )
}
