import api from '@/lib/api'
import { useQuery, useMutation } from '@/composables/useApi'
import type { Referral, ReferralRequest } from '@/types/referral'
import { mapReferrals } from '@/composables/mapReferrals'

/* ======================
   GET referrals (patient)
====================== */
export function usePatientReferrals() {
  return useQuery<Referral[]>('/referrals/patient', {
    immediate: true,
    transform: mapReferrals
  })
}

/* ======================
   GET referrals (doctor)
====================== */
export function useDoctorReferrals() {
  return useQuery<Referral[]>('/referrals', {
    transform: mapReferrals
  })
}

/* ======================
   GET single referral
====================== */
export function useReferralById(id: number) {
  return useQuery<Referral>(`/referrals/${id}`, {
    immediate: !!id
  })
}

/* ======================
   CREATE referral (POST)
====================== */
export function useCreateReferral() {
  return useMutation<Referral, ReferralRequest>(
    (data) => api.post('/referrals', data).then(res => res.data)
  )
}

export const useTreatments = () =>
  useQuery<{ id: number; name: string }[]>('/referrals/treatments')