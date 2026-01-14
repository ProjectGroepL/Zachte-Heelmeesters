import { useMutation } from '@/composables/useApi'
import api from '@/lib/api'
import type { RoleCreateDto, RoleDto} from '@/types/role'

export function useCreateRole() {
    return useMutation<RoleDto, RoleCreateDto>((dto) =>
    api.post('/api/roles/create', dto).then(r => r.data))
}