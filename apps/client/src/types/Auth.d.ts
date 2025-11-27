// User related types
export interface User {
  id: number
  firstName: string
  middleName?: string
  lastName: string
  email: string
  phoneNumber: string
  street: string
  houseNumber: string
  houseNumberAddition?: string
  zipCode: string
  city: string
  country: string
  twoFactorEnabled: boolean
  role: string
}

// Auth request types
export interface LoginCredentials {
  email: string
  password: string
}

export interface RegisterData {
  firstName: string
  middleName?: string
  lastName: string
  email: string
  phoneNumber: string
  street: string
  houseNumber: string
  houseNumberAddition?: string
  zipCode: string
  city: string
  country: string
  password: string
}

export interface RefreshTokenRequest {
  refreshToken: string
}

export interface TwoFactorVerifyRequest {
  tempSessionId: number
  code: string
}

export interface ResendRequest {
  tempSessionId: number
}

// Auth response types
export interface LoginResponse {
  token: string
  refreshToken: string
  user: User
  requiresTwoFactor?: boolean
  tempSessionId?: number
}

export interface RefreshTokenResponse {
  token: string
  refreshToken: string
  user: User
}

export interface TwoFactorResponse {
  token: string
  refreshToken: string
  user: User
}

// API response wrapper
export interface ApiResponse<T> {
  success: boolean
  status: number
  data?: T
  message?: string
}