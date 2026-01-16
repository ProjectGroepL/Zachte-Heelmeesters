// src/composables/useSendReportToAdmin.ts
import { useMutation } from '@/composables/useApi'
import api from '@/lib/api'

export function useSendReportToAdmin() {
  return useMutation<void, { reportId: number }>((v) =>
    api.post(`/reports/${v.reportId}/send-to-admin`).then(() => undefined)
  )
}