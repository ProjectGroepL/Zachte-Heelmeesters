export type AccessRequestStatus =
| 'Pending'
| 'Approved'
| 'Denied'
| 'Revoked'

export interface AccessRequest {
    appointmentDate: string | number | Date
    id: number
    patientId: number 
    patientName: string
    specialistId: number 
    specialistName: string 
    reason: string
    status: AccessRequestStatus
    requestedAt: string
    date: string
}

export interface CreateAccessRequest {
  appointmentId: number
  reason: string
}

export interface DecideAccessRequest {
    approved: boolean 
}