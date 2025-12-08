export interface ReferralRequest {
    patientId: number;
    treatmentId: number;
    notes?: string;
}

export interface Referral {
    id: number;
    patientName: string;
    treatmentName: string;
    createdAt: string;
    status: string;
}
interface DoctorPatient {
  patientId: number
  fullName: string
}