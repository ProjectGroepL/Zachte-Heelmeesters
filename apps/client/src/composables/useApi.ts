import { ref, type Ref } from 'vue'
import api from '@/lib/api'
import type { AxiosError } from 'axios'

export interface UseQueryOptions<T = any> {
  immediate?: boolean
  onSuccess?: (data: T) => void
  onError?: (error: AxiosError) => void
  transform?: (data: T) => T
}

export interface UseQueryReturn<T> {
  data: Ref<T | null>
  loading: Ref<boolean>
  error: Ref<AxiosError | null>
  execute: () => Promise<T | null>
}

export function useQuery<T = any>(
  url: string,
  options: UseQueryOptions<T> = {}
): UseQueryReturn<T> {
  const { immediate = true, onSuccess, onError, transform } = options

  const data: Ref<T | null> = ref(null)
  const loading = ref(false)
  const error: Ref<AxiosError | null> = ref(null)

  const execute = async (): Promise<T | null> => {
    loading.value = true
    error.value = null

    try {
      const response = await api.get(url)
      const transformedData = transform ? transform(response.data) : response.data
      data.value = transformedData
      onSuccess?.(transformedData)
      return transformedData
    } catch (err) {
      console.log(err)
      const axiosError = err as AxiosError
      error.value = axiosError
      onError?.(axiosError)
      return null
    } finally {
      loading.value = false
    }
  }

  // Auto-execute on mount if immediate is true
  if (immediate) {
    execute()
  }

  return {
    data,
    loading,
    error,
    execute
  }
}

// Simple mutation hook
export function useMutation<TData = any, TVariables = any>(
  mutationFn: (variables: TVariables) => Promise<TData>
) {
  const data: Ref<TData | null> = ref(null)
  const loading = ref(false)
  const error: Ref<AxiosError | null> = ref(null)

  const mutate = async (variables: TVariables): Promise<TData | null> => {
    loading.value = true
    error.value = null

    try {
      const result = await mutationFn(variables)
      data.value = result
      return result
    } catch (err) {
      error.value = err as AxiosError
      return null
    } finally {
      loading.value = false
    }
  }

  return {
    data,
    loading,
    error,
    mutate
  }
}