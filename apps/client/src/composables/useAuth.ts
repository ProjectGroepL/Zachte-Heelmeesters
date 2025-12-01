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

  //TODO add server logout endpoint to invalidate refresh tokens
  const logout = (): void => {

    // Remove tokens from localStorage
    localStorage.removeItem('access_token')
    localStorage.removeItem('refresh_token')

    // Redirect to login page
    router.replace('/auth/login')
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

  const isAuthenticated = (): boolean => {
    const token = localStorage.getItem('access_token')
    return !!token
  }

  return {
    register,
    login,
    verifyTwoFactor,
    resendTwoFactor,
    logout,
    refreshToken,
    isAuthenticated,
    getUser
  }
}