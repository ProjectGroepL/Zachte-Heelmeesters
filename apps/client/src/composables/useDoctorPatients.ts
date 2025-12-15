import { useQuery } from '@/composables/useApi'

interface DoctorPatient {
  patientId: number
  fullName: string
}

export function useDoctorPatients() {
  return useQuery<DoctorPatient[]>('/DoctorPatients', {
    transform: (raw: any[]) =>
      (raw || [])
        .map(p => ({
          patientId: p.patientId ?? p.PatientId ?? null,
          fullName:
            p.fullName ??
            ((p.firstName ?? '') + ' ' + (p.lastName ?? '')).trim()
        }))
        .filter(p => p.patientId != null)
  })
}
