import { useQuery } from '@/composables/useApi'
import type { AppointmentReport } from '@/types/AppointmentReport'

export function usePatientAppointmentReport(appointmentId: number) {
  return useQuery<AppointmentReport>(
    `/patient/appointments/${appointmentId}/report`,
    { immediate: !!appointmentId }
  )
}
