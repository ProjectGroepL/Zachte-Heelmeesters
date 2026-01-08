import { useQuery, useMutation } from '@/composables/useApi'
import api from '@/lib/api'
import type { Notification } from '@/types/Notification'

export function useNotifications() {
  return useQuery<Notification[]>('/notifications')
}

export function useMarkNotificationRead() {
  return useMutation<void, number>((id) =>
    api.put(`/notifications/${id}/read`)
  )
}

// Voeg deze functie toe onderaan je composable bestand
export function useMarkAllRead() {
  return useMutation<void, void>(() =>
    api.put('/notifications/mark-all-read')
  )
}