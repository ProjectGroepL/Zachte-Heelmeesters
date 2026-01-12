import { ref, onMounted } from 'vue'
import api from '@/lib/api'
import type { Specialist } from '@/types/specialist'

export function useSpecialists() {
  const data = ref<Specialist[] | null>(null)
  const loading = ref(false)
  const error = ref<unknown>(null)

  const fetchSpecialists = async () => {
    loading.value = true
    try {
      const res = await api.get<Specialist[]>('/specialists')
      data.value = res.data
    } catch (e) {
      error.value = e
    } finally {
      loading.value = false
    }
  }

  onMounted(fetchSpecialists)

  return {
    data,
    loading,
    error
  }
}
