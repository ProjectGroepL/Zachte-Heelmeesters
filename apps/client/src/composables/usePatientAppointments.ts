import { useQuery } from '@/composables/useApi'

export interface PatientAppointment {
  id: number
  date: string
  status: string
  specialistName?: string
}

interface PatientAppointmentsResponse {
  appointments: PatientAppointment[]
}

export function usePatientAppointments() {
  // We wijzen nu naar de specifieke 'completed' route
  return useQuery<PatientAppointmentsResponse>('/appointments/completed')
}