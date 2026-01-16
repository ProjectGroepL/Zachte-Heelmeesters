import { useQuery } from '@/composables/useApi'

export interface UserListItem {
  id: number
  name: string
}

export function useUsers() {
  return useQuery<UserListItem[]>('/api/users')
}
