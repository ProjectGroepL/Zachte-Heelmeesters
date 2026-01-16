import { ref } from 'vue'
import { defineStore } from 'pinia'

export const useNotificationStore = defineStore('notifications', () => {
  const isSheetOpen = ref(false)

  function openSheet() {
    isSheetOpen.value = true
  }

  function closeSheet() {
    isSheetOpen.value = false
  }

  function toggleSheet() {
    isSheetOpen.value = !isSheetOpen.value
  }

  return {
    isSheetOpen,
    openSheet,
    closeSheet,
    toggleSheet
  }
})
