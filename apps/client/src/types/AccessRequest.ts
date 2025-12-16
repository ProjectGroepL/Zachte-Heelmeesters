export type AccessRequestStatus =
| 'Pending'
| 'Approved'
| 'Denied'
| 'Revoked'

export interface AccessRequest {
    id: number
    patientId: number 
    patientName: string
    specialistId: number 
    specialistName: string 
    reason: string
    status: AccessRequestStatus
    requestedAt: string
}

export interface CreateAccessRequest {
    patientId: number 
    reason: string 
}

export interface DecideAccessRequest {
    approved: boolean 
}