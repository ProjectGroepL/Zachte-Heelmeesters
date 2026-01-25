import { useQuery, useMutation } from '@/composables/useApi'
import api from '@/lib/api'
import type { Notification } from '@/types/Notification'

export function useNotifications() {
  // Only fetch notifications if user has a token
  const hasToken = !!localStorage.getItem('access_token')
  
  return useQuery<Notification[]>('/notifications', {
    immediate: hasToken // Only auto-fetch if authenticated
  })
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