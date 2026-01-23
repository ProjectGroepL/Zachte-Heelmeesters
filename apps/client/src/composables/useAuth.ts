import type {
  LoginCredentials,
  LoginResponse,
  User,
  RefreshTokenRequest,
  RefreshTokenResponse,
  RegisterData,
  TwoFactorVerifyRequest,
  ResendRequest,
  TwoFactorResponse,
  ApiResponse
} from "@/types/Auth"

import api from "@/lib/api"
import { useRouter } from "vue-router";

export const useAuth = () => {
  const router = useRouter();

  const register = async (registerData: RegisterData): Promise<ApiResponse<LoginResponse>> => {
    try {
      const response = await api.post('/auth/register', registerData)

      if (response.status === 200 && response.data) {
        // Store both tokens for immediate login after registration
        localStorage.setItem('access_token', response.data.token)
        localStorage.setItem('refresh_token', response.data.refreshToken)

        // Store user info
        if (response.data.user) {
          localStorage.setItem('user_info', JSON.stringify(response.data.user))
        }
      }

      return {
        success: response.status === 200,
        status: response.status,
        data: response.status === 200 ? response.data : undefined
      }
    } catch (error: any) {
      return {
        success: false,
        status: error.response?.status || 500,
        message: error.response?.data?.message || 'Registration failed'
      }
    }
  }

  const login = async (credentials: LoginCredentials): Promise<ApiResponse<LoginResponse>> => {
    try {
      const response = await api.post('/auth/login', credentials)

      if (response.status === 200 && response.data) {
        // Check if 2FA is required
        if (response.data.requiresTwoFactor) {
          // Don't store tokens yet, wait for 2FA verification
          return {
            success: true,
            status: response.status,
            data: response.data
          }
        }

        // Store both tokens for normal login
        localStorage.setItem('access_token', response.data.token)
        localStorage.setItem('refresh_token', response.data.refreshToken)

        // Store user info for quick access without JWT decoding
        if (response.data.user) {
          localStorage.setItem('user_info', JSON.stringify(response.data.user))
        }
      }

      return {
        success: response.status === 200,
        status: response.status,
        data: response.status === 200 ? response.data : undefined
      }
    } catch (error: any) {
      return {
        success: false,
        status: error.response?.status || 500,
        message: error.response?.data?.message || 'Login failed'
      }
    }
  }

  const verifyTwoFactor = async (verifyData: TwoFactorVerifyRequest): Promise<ApiResponse<TwoFactorResponse>> => {
    try {
      const response = await api.post('/auth/verify-2fa', verifyData)

      if (response.status === 200 && response.data) {
        // Store tokens after successful 2FA verification
        localStorage.setItem('access_token', response.data.token)
        localStorage.setItem('refresh_token', response.data.refreshToken)

        // Store user info
        if (response.data.user) {
          localStorage.setItem('user_info', JSON.stringify(response.data.user))
        }
      }

      return {
        success: response.status === 200,
        status: response.status,
        data: response.status === 200 ? response.data : undefined
      }
    } catch (error: any) {
      return {
        success: false,
        status: error.response?.status || 500,
        message: error.response?.data?.message || '2FA verification failed'
      }
    }
  }

  const resendTwoFactor = async (resendData: ResendRequest): Promise<ApiResponse<void>> => {
    try {
      const response = await api.post('/auth/resend-2fa', resendData)

      return {
        success: response.status === 200,
        status: response.status,
        message: response.data?.message || 'Code resent successfully'
      }
    } catch (error: any) {
      return {
        success: false,
        status: error.response?.status || 500,
        message: error.response?.data?.message || 'Failed to resend code'
      }
    }
  }

  const refreshToken = async (): Promise<boolean> => {
    try {
      const storedRefreshToken = localStorage.getItem('refresh_token')
      if (!storedRefreshToken) {
        return false
      }

      const response = await api.post('/auth/refresh', {
        refreshToken: storedRefreshToken
      } as RefreshTokenRequest)

      if (response.status === 200 && response.data) {
        const data = response.data as RefreshTokenResponse
        localStorage.setItem('access_token', data.token)
        localStorage.setItem('refresh_token', data.refreshToken)
        return true
      }

      return false
    } catch {
      // If refresh fails, remove tokens and redirect to login
      localStorage.removeItem('access_token')
      localStorage.removeItem('refresh_token')
      router.replace('/auth/login')
      return false
    }
  }

  // Logs out on the server (for audit + refresh token invalidation)
  // and always logs out locally
  const logout = async (): Promise<void> => {
    try {
      // IMPORTANT: token must still exist here
      await api.post('/auth/logout')
    } catch (error) {
      // Never block logout
      console.warn('Server logout failed, continuing client logout', error)
    } finally {
      // Remove tokens from localStorage
      localStorage.removeItem('access_token')
      localStorage.removeItem('refresh_token')
      localStorage.removeItem('user_info')

      // Redirect to login page
      router.replace('/auth/login')
    }
  }


  const getUser = async (): Promise<ApiResponse<User>> => {
    try {
      const response = await api.get('/auth/me')
      return {
        success: response.status === 200,
        status: response.status,
        data: response.status === 200 ? response.data : undefined
      }
    } catch (error: any) {
      return {
        success: false,
        status: error.response?.status || 500,
        message: error.response?.data?.message || 'Failed to get user'
      }
    }
  }

  const hasRole = (roleName: string) => {
    const user = getStoredUser()
    if (!user) return false

    console.log(user.role, roleName.toLowerCase())

    // Case 1: user.role = Role[]
    if (Array.isArray(user.role)) {
      return user.role.some(
        (r: any) => r?.name?.toLowerCase() === roleName.toLowerCase()
      )
    }

    // Case 2: user.role = { name: string }
    if (user.role && (user.role as any).name) {
      return (user.role as any).name.toLowerCase() === roleName.toLowerCase()
    }

    // Case 3: legacy string
    if (typeof (user as any).role === "string") {
      return (user as any).role.toLowerCase() === roleName.toLowerCase()
    }

    return false
  }

  const isAuthenticated = (): boolean => {
    const token = localStorage.getItem('access_token')
    return !!token
  }

  // Get user info from localStorage (stored during login) or fallback to JWT
  const getStoredUser = (): User | null => {
    const stored = localStorage.getItem('user_info')
    if (!stored) return null
    try {
      return JSON.parse(stored)
    } catch {
      return null
    }
  }

  return {
    register,
    login,
    verifyTwoFactor,
    resendTwoFactor,
    logout,
    refreshToken,
    isAuthenticated,
    getUser,
    getStoredUser,
    hasRole
  }
}