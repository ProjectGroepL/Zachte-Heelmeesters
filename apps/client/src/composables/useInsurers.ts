import { useQuery } from '@/composables/useApi'
import type { UserSummaryDto } from '@/types/user'

export function useInsurers() {
  return useQuery<UserSummaryDto[]>(
    '/insurance/invoices/insurers'
  )
}