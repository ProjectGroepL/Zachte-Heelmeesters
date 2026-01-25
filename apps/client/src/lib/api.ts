import axios, { AxiosError } from 'axios'

// Base API URL
const API_BASE_URL = import.meta.env.VITE_API_URL || 'https://localhost:7048/api'

// Flag to prevent multiple simultaneous refresh attempts
let isRefreshing = false
let failedQueue: Array<{ resolve: Function; reject: Function }> = []

const processQueue = (error: any, token: string | null = null) => {
  failedQueue.forEach(prom => {
    if (error) {
      prom.reject(error)
    } else {
      prom.resolve(token)
    }
  })
  
  failedQueue = []
}

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
      if (isRefreshing) {
        // If already refreshing, queue this request
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject })
        }).then(token => {
          originalRequest.headers.Authorization = `Bearer ${token}`
          return api(originalRequest)
        }).catch(err => {
          return Promise.reject(err)
        })
      }

      originalRequest._retry = true
      isRefreshing = true

      const refreshToken = localStorage.getItem('refresh_token')
      
      if (!refreshToken) {
        isRefreshing = false
        processQueue(new Error('No refresh token'), null)
        localStorage.removeItem('access_token')
        localStorage.removeItem('refresh_token')
        localStorage.removeItem('user_info')
        
        // Only redirect if not already on login page
        if (!window.location.pathname.startsWith('/auth')) {
          window.location.href = '/auth/login'
        }
        return Promise.reject(error)
      }

      try {
        const response = await axios.post(`${API_BASE_URL}/auth/refresh`, {
          refreshToken: refreshToken
        })

        if (response.status === 200 && response.data) {
          const newToken = response.data.token
          
          // Update stored tokens
          localStorage.setItem('access_token', newToken)
          localStorage.setItem('refresh_token', response.data.refreshToken)

          // Update the authorization header for the original request
          originalRequest.headers.Authorization = `Bearer ${newToken}`

          processQueue(null, newToken)
          isRefreshing = false

          // Retry the original request
          return api(originalRequest)
        }
      } catch (refreshError) {
        // Refresh failed, clear tokens and redirect to login
        processQueue(refreshError, null)
        isRefreshing = false
        
        localStorage.removeItem('access_token')
        localStorage.removeItem('refresh_token')
        localStorage.removeItem('user_info')
        
        // Only redirect if not already on login page
        if (!window.location.pathname.startsWith('/auth')) {
          window.location.href = '/auth/login'
        }
        return Promise.reject(refreshError)
      }
    }

    return Promise.reject(error)
  }
)

export default api