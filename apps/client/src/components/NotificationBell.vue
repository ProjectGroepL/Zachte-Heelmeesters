<script setup lang="ts">
import { Bell, BellOff } from 'lucide-vue-next'
import { computed, ref, onMounted, onUnmounted } from 'vue'
import { useNotifications, useMarkNotificationRead, useMarkAllRead } from '@/composables/useNotifications'

// In jouw useQuery heet de functie 'execute', dus die halen we hier op
const { data, execute } = useNotifications()
const markRead = useMarkNotificationRead()
const markAllRead = useMarkAllRead()

const isOpen = ref(false)
const containerRef = ref<HTMLElement | null>(null)

const unreadCount = computed(() =>
  (data.value ?? []).filter(n => !n.isRead).length
)

// Functie om alles als gelezen te markeren
const handleMarkAllRead = async () => {
  await markAllRead.mutate()
  await execute() // De data opnieuw ophalen
}

// Functie om één notificatie als gelezen te markeren bij klik
const handleMarkSingleRead = async (id: number, isRead: boolean) => {
  if (isRead) return
  await markRead.mutate(id)
  await execute() // De data opnieuw ophalen
}

const toggleDropdown = () => {
  isOpen.value = !isOpen.value
}

const closeDropdown = (e: MouseEvent) => {
  if (containerRef.value && !containerRef.value.contains(e.target as Node)) {
    isOpen.value = false
  }
}

onMounted(() => window.addEventListener('click', closeDropdown))
onUnmounted(() => window.removeEventListener('click', closeDropdown))
</script>

<template>
  <div class="relative" ref="containerRef">
    <button
      @click="toggleDropdown"
      type="button"
      class="relative p-2 text-muted-foreground hover:text-primary transition-colors focus:outline-none focus:ring-2 focus:ring-primary rounded-full"
    >
      <Bell class="w-5 h-5" />
      <span
        v-if="unreadCount > 0"
        class="absolute top-1 right-1 bg-red-600 text-white text-[10px] font-bold rounded-full h-4 w-4 flex items-center justify-center border-2 border-background"
      >
        {{ unreadCount }}
      </span>
    </button>

    <transition
      enter-active-class="transition duration-100 ease-out"
      enter-from-class="transform scale-95 opacity-0"
      enter-to-class="transform scale-100 opacity-100"
      leave-active-class="transition duration-75 ease-in"
      leave-from-class="transform scale-100 opacity-100"
      leave-to-class="transform scale-95 opacity-0"
    >
      <div
        v-if="isOpen"
        class="absolute left-full ml-2 bottom-0 w-80 bg-card border rounded-lg shadow-lg z-50 overflow-hidden"
      >
        <div class="p-3 border-b bg-muted/50 flex justify-between items-center">
          <h3 class="font-semibold text-sm">Notificaties</h3>
          <span class="text-xs text-muted-foreground">{{ unreadCount }} ongelezen</span>
        </div>

        <div class="max-h-[300px] overflow-y-auto">
          <div v-if="data?.length === 0" class="p-8 text-center text-muted-foreground">
            <BellOff class="w-8 h-8 mx-auto mb-2 opacity-20" />
            <p class="text-xs">Geen notificaties gevonden</p>
          </div>
          
          <ul v-else class="divide-y">
            <li 
              v-for="n in data" 
              :key="n.id" 
              @click="handleMarkSingleRead(n.id, n.isRead)"
              class="p-3 hover:bg-muted/50 cursor-pointer transition-colors relative"
              :class="{ 'bg-primary/5': !n.isRead }"
            >
              <div v-if="!n.isRead" class="absolute left-1 top-1/2 -translate-y-1/2 w-1.5 h-1.5 bg-primary rounded-full"></div>
              
              <p class="text-sm" :class="{ 'font-medium pl-2': !n.isRead, 'pl-2': n.isRead }">
                {{ n.message }}
              </p>
              <span class="text-[10px] text-muted-foreground pl-2">{{ new Date(n.createdAt).toLocaleString('nl-NL') }}</span>
            </li>
          </ul>
        </div>
        
        <div class="p-2 border-t text-center">
          <button 
            @click="handleMarkAllRead"
            :disabled="unreadCount === 0"
            class="text-xs text-primary hover:underline disabled:opacity-50 disabled:no-underline"
          >
            Markeer alles als gelezen
          </button>
        </div>
      </div>
    </transition>
  </div>
</template>