import { useQuery } from '@/composables/useApi'
import type { AppointmentDto } from '@/types/appointment'

export function useSpecialistAppointments() {
  return useQuery<AppointmentDto[]>(
    '/specialist/appointments/my'
  )
}