import api from "@/lib/api"
import type { Referral, ReferralRequest } from '@/types/referral'

export function useReferral()
{
    async function getDoctorReferrals(): Promise<Referral[]> {
        const response = await api.get('/referrals')
        return response.data
    }

    async function getReferrals(): Promise<Referral[]> {
        const reponse = await api.get('/referrals/patient')
        const raw = reponse.data || []

        // Debug: show raw server payload so we can confirm field names and values
        console.debug('[useReferral] raw /referrals response:', raw)

        // Normalize server DTO (PascalCase) -> client expected camelCase
        const mapped: Referral[] = (raw as any[]).map(r => ({
            id: r.Id ?? r.id,
            patientName: r.PatientName ?? r.patientName ?? r.patient ?? '',
            treatmentName: r.TreatmentName ?? r.treatmentName ?? r.treatment ?? '',
            createdAt: (r.CreatedAt ?? r.createdAt ?? r.created_at ?? '')?.toString(),
            status: r.Status ?? r.status ?? 'open'
        }))

        return mapped
    }

    async function createReferral(data: ReferralRequest)
    {
        const response = await api.post('/referrals', data)
        return response.data
    }

    async function getReferral(id: number): Promise<Referral> {
        const reponse = await api.get(`/referrals/${id}`)
        return reponse.data
    }

    return { getReferrals, createReferral, getReferral, getDoctorReferrals}
}
