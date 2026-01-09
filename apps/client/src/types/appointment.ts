export type AppointmentStatus =
  | 'PendingAccess'
  | 'Scheduled'
  | 'AccessDenied'
  | 'Completed'
  | 'Cancelled'

export interface AppointmentDto {
  id: number
  referralId: number
  notes: string
  status: AppointmentStatus
  treatmentDescription: string
  treatmentInstructions?: string
  patientName: string
  date: string
}
export interface AppointmentCreateDto {
  referralId: number
  specialistId: number
  date: string
  time: string
}