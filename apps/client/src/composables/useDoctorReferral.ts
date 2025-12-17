import {useQuery} from '@/composables/useApi'
import type { Referral } from '@/types/referral'

export function useDocterReferrals() {
    return useQuery<Referral[]>('/referrals', {
        transform: (raw) =>
        (raw || []).map(r => ({
            id: r.id ?? r.id,
            patientName: r.patientName ?? r.patientName ?? '',
            treatmentName: r.treatmentName ?? r.treatmentName ?? '',
            createdAt: (r.createdAt ?? r.createdAt)?.toString(),
            status: r.status ?? r.status ?? 'open'
        }))
    })
}