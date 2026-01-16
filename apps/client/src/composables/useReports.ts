import { useQuery } from '@/composables/useApi'
import api from '@/lib/api'
import type { ReportSummaryDto } from '@/types/report'

export function useReportSummary(appointmentId: number) {
  return useQuery<ReportSummaryDto>(
    `/reports/${appointmentId}/summary`,
    {
      transform: (data) => data
    }
  )
}
