import {useQuery, useMutation } from "@/composables/useApi"
import api from '@/lib/api'
import type {
    AccessRequest,
    CreateAccessRequest,
    DecideAccessRequest
} from "@/types/AccessRequest"

// GET pending requests for patient
export function usePatientAccessRequests() {
    return useQuery<AccessRequest[]>('/patient/access-requests')
}
// POST approve/deny
export function useDecideAccessRequest(){
    return useMutation<void, {id: number; data: DecideAccessRequest }>(
        ({ id, data}) =>
            api.post(`/patient/access-request/${id}/decision`, data)
        .then(res => res.data)
    )
}

// Specialist Post create request
export function useCreateAccessRequest(){
    return useMutation<AccessRequest, CreateAccessRequest>(
        (data) => 
            api.post('specialist/access-request', data)
            .then(res => res.data)
    )
}