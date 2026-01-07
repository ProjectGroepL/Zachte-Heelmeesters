import { useQuery } from '@/composables/useApi'

export interface SpecialistAppointment {
  id: number
  patientName: string
  date: string
  status: 'PendingAccess' | 'Scheduled' | 'AccessDenied' | 'Completed' | 'Cancelled'
}

export function useSpecialistAppointments() {
  return useQuery<SpecialistAppointment[]>(
    '/specialist/appointments/my'
  )
}
