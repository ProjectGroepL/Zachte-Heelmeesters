import { useQuery, useMutation } from '@/composables/useApi'
import api from '@/lib/api'
import type { AppointmentReport } from '@/types/AppointmentReport'

export function useAppointmentReport(appointmentId: number) {
  return useQuery<AppointmentReport>(
    `/specialist/appointments/${appointmentId}/report`,
    { immediate: !!appointmentId }
  )
}

export function useCreateAppointmentReport(appointmentId: number) {
  return useMutation(async (payload: {
    summary: string
    items: { description: string; cost: number }[]
  }) => {
    const { data } = await api.post(
      `/specialist/appointments/${appointmentId}/report`,
      payload
    )
    return data
  })
}
