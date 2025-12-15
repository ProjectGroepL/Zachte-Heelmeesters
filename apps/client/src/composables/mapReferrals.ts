import type { Referral } from '@/types/referral'

export function mapReferrals(raw: any[]): Referral[]{
    return (raw || []).map(r => ({
    id: r.Id ?? r.id,
    patientName: r.PatientName ?? r.patientName ?? '',
    treatmentName: r.TreatmentName ?? r.treatmentName ?? '',
    createdAt: (r.CreatedAt ?? r.createdAt)?.toString(),
    status: r.Status ?? r.status ?? 'open'
  }))
}