import { useQuery, useMutation } from '@/composables/useApi'
import api from '@/lib/api'
import type { MedicalDocument, MedicalDocumentStatus } from '@/types/MedicalDocument'

export function useMyMedicalDocuments() {
  return useQuery<MedicalDocument[]>(
    '/patient/medical-documents'
  )
}

export function useUpdateMedicalDocumentStatus() {
  return useMutation<void, { id: number; status: MedicalDocumentStatus }>(
    ({ id, status }) =>
      api.put(`/patient/medical-documents/${id}/status`, { status })
  )
}

export function usePatientMedicalDocuments(patientId: number) {
  return useQuery<MedicalDocument[]>(
    `/specialist/patients/${patientId}/medical-documents`,
    { immediate: !!patientId }
  )
}
