import { useMutation, useQuery } from '@/composables/useApi'
import api from '@/lib/api'
import type { AccessRequest } from "@/types/AccessRequest"

export interface CreateAccessRequestRequest {
  patientId: number
  reason: string
}

export function useRequestAccess() {
  return useMutation<void, CreateAccessRequestRequest>((data) =>
    api.post('/specialist/access-request', data).then(res => res.data)
  )
}

export function useSpecialistAccessRequests() {
  return useQuery<AccessRequest[]>(
    '/specialist/access-request/my'
  )
}
