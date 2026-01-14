import { useQuery  } from "./useApi";  
import type {RoleDto} from '@/types/role.ts'

export function useRoles() {
    return useQuery<RoleDto[]>(
         '/api/roles'
    )
}