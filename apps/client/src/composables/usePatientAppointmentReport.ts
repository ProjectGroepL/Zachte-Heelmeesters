import { useQuery } from '@/composables/useApi'
import type { AppointmentReport } from '@/types/AppointmentReport'
import type { PatientReportDto } from '@/types/report'

// export function usePatientAppointmentReport(appointmentId: number) {
//   return useQuery<AppointmentReport>(
//     `/patient/appointments/${appointmentId}/report`,
//     { immediate: !!appointmentId }
//   )
// }

export function usePatientAppointmentReport(appointmentId: number) {
  return useQuery<PatientReportDto>(
    `/reports/appointment/${appointmentId}`,
    {
      transform: data => data
    }
  )
}
