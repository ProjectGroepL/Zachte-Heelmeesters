import api from "@/lib/api"
import type { Referral, ReferralRequest } from '@/types/referral'

export function useReferral()
{
    async function getReferrals(): Promise<Referral[]> {
        const response = await api.get('/referrals/patient')
        const raw = response.data || []

        console.debug('[useReferral] raw /referrals/patient:', raw)

        return raw.map((r: { id: any; treatmentDescription: any; createdAt: any; status: any }) => ({
            id: r.id,
            patientName: "",  // komt niet terug in dit endpoint
            treatmentName: r.treatmentDescription,
            createdAt: r.createdAt,
            status: r.status
        }))
    }

    async function createReferral(data: ReferralRequest) {
        return await api.post('/referrals', data)
    }

    async function getReferral(id: number): Promise<Referral> {
        const response = await api.get(`/referrals/${id}`)
        return response.data
    }

    return { getReferrals, createReferral, getReferral }
}
