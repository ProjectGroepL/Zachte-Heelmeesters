// src/lib/jwt.ts
import { jwtDecode } from 'jwt-decode'

export interface JwtPayload {
  sub: string
  role?: string | string[]
}

export function getRolesFromToken(token: string): string[] {
  const decoded = jwtDecode<JwtPayload>(token)

  if (!decoded.role) return []
  return Array.isArray(decoded.role)
    ? decoded.role
    : [decoded.role]
}
