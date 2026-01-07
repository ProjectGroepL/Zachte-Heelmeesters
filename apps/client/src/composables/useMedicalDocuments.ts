import { useQuery, useMutation } from '@/composables/useApi'
import api from '@/lib/api'
import type { MedicalDocument, MedicalDocumentStatus } from '@/types/MedicalDocument'

export const useCreateMedicalDocument = () =>
  useMutation(async (payload: {
    patientId: number
    title: string
    content: string
  }) => {
    const { data } = await api.post(
      '/doctor/medical-documents', // 👈 JUIST
      payload
    )
    return data
  })

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

export function useDoctorMedicalDocuments() {
  return useQuery<MedicalDocument[]>(
    '/doctor/medical-documents'
  )
}

export function useUpdateDoctorMedicalDocumentStatus() {
  return useMutation<
    void,
    { id: number; status: MedicalDocumentStatus }
  >(({ id, status }) =>
    api.put(`/doctor/medical-documents/${id}/status`, { status })
  )
}

