import { useQuery } from '@/composables/useApi'

export function useUserRoles(userId: number) {
    return useQuery<string[]>(`/api/roles/user/${userId}`)
}
