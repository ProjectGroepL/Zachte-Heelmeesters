import type { Referral } from '@/types/referral'

export function mapReferrals(raw: any): Referral[] {
  if (!raw) return []

  const list = Array.isArray(raw) ? raw : []

  return list.map(r => ({
    id: r.id,
    treatmentName: r.treatmentDescription, // 🔥 HIER ZAT DE KLOPPER
    patientName: '', // zit niet in deze DTO
    createdAt: r.createdAt,
    status: r.status
  }))
}
