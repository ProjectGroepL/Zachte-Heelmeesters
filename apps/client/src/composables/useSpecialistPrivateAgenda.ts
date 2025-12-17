import { useQuery } from '@/composables/useApi'

export type SpecialistPrivateAgendaItem = {
  uid: string
  start: string 
  end: string
  userId: number
}

export function useSpecialistPrivateAgendaById(id: number) {
  return useQuery<SpecialistPrivateAgendaItem[]>(`/SpecialistPrivateAgenda/${id}`, {
    immediate: !!id,
  })
}