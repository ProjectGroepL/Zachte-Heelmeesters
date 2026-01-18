import { useMutation } from '@/composables/useApi'
import api from '@/lib/api'
import type { CreateInsuranceInvoiceDto } from '@/types/insurance'

export function useCreateInsuranceInvoice() {
  return useMutation<void, CreateInsuranceInvoiceDto>((dto) =>
    api.post('/insurance/invoices', dto).then(() => undefined)
  )
}