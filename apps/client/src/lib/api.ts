import axios, { AxiosError } from 'axios'

// Base API URL
const API_BASE_URL = import.meta.env.VITE_API_URL || 'https://localhost:7048/api'

// Create axios instance
const api = axios.create({
  baseURL: API_BASE_URL,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
  },
})

// Request interceptor to add auth token
api.interceptors.request.use(
  (config) => {
    // Get token from localStorage (or wherever you store it)
    const token = localStorage.getItem('access_token')

    if (token) {
      config.headers.Authorization = `Bearer ${token}`
    }

    return config
  },
  (error) => {
    return Promise.reject(error)
  }
)

// Response interceptor for error handling and token refresh
api.interceptors.response.use(
  (response) => {
    return response
  },
  async (error: AxiosError) => {
    const originalRequest = error.config as any

    // Don't intercept 401 errors for auth endpoints
    const isAuthEndpoint = originalRequest.url?.startsWith('/auth')

    // Handle unauthorized for non-auth endpoints only
    if (error.response?.status === 401 && !originalRequest._retry && !isAuthEndpoint) {
      originalRequest._retry = true

      try {
        // Try to refresh the token
        const refreshToken = localStorage.getItem('refresh_token')
        if (!refreshToken) {
          throw new Error('No refresh token available')
        }

        const response = await axios.post(`${API_BASE_URL}/auth/refresh`, {
          refreshToken: refreshToken
        })

        if (response.status === 200 && response.data) {
          // Update stored tokens
          localStorage.setItem('access_token', response.data.token)
          localStorage.setItem('refresh_token', response.data.refreshToken)

          // Update the authorization header for the original request
          originalRequest.headers.Authorization = `Bearer ${response.data.token}`

          // Retry the original request
          return api(originalRequest)
        }
      } catch (refreshError) {
        // Refresh failed, redirect to login
        localStorage.removeItem('access_token')
        localStorage.removeItem('refresh_token')
        window.location.href = '/auth/login'
        return Promise.reject(refreshError)
      }
    }

    return Promise.reject(error)
  }
)

export async function saveSpecialistIcal(url: string) {
    const response = await api.post('/SpecialistIcal', { url })
    return response.data as { message: string; url: string }
}

export default api