import { useQuery } from '@/composables/useApi'
import type { AdminReportOverviewDto } from '@/types/admin'

export function useAdminReports() {
  return useQuery<AdminReportOverviewDto[]>(
    '/admin/reports'
  )
}