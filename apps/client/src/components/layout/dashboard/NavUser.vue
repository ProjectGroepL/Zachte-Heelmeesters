<script setup lang="ts">
import { ref, computed, onMounted } from "vue"
import api from '@/lib/api'
import { useRouter } from 'vue-router'
import {
  BadgeCheck,
  Bell,
  ChevronsUpDown,
  CreditCard,
  LogOut,
  Settings,
  Sparkles,
} from "lucide-vue-next"
import { useNotifications } from '@/composables/useNotifications'
import { Badge } from '@/components/ui/badge'
import { useNotificationStore } from '@/stores/notifications'

import {
  Avatar,
  AvatarFallback,
  AvatarImage,
} from '@/components/ui/avatar'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuGroup,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu'
import {
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  useSidebar,
} from '@/components/ui/sidebar'
import { useAuth } from '@/composables/useAuth'
import type { User } from '@/types/Auth'
import { getFullNameFromUser } from "@/lib/utils"

const { isMobile } = useSidebar()
const { getUser } = useAuth()
const router = useRouter()

const user = ref<User | null>(null)
const isLoading = ref(true)

// Notifications
const { data: notifications } = useNotifications()
const notificationStore = useNotificationStore()
const unreadCount = computed(() =>
  (notifications.value ?? []).filter(n => !n.isRead).length
)

// Computed values for display
const displayName = computed(() => {
  if (!user.value) return 'Loading...'
  return getFullNameFromUser(user.value)
})

const initials = computed(() => {
  if (!user.value) return 'LO'
  return `${user.value.firstName.charAt(0)}${user.value.lastName.charAt(0)}`.toUpperCase()
})

// Fetch user data on component mount
onMounted(async () => {
  try {
    const response = await getUser()
    if (response.success && response.data) {
      user.value = response.data
    }
  } catch (error) {
    console.error('Failed to fetch user data:', error)
  } finally {
    isLoading.value = false
  }
})

async function handleLogout() {
  try {
    await api.post('/auth/logout')
  } catch {
    // ignore errors — logout must always succeed
  } finally {
    localStorage.removeItem('access_token')
    localStorage.removeItem('refresh_token')
    localStorage.removeItem('user_info')
    router.push('/auth/login')
  }
}
</script>

<template>
  <SidebarMenu>
    <SidebarMenuItem>
      <DropdownMenu>
        <DropdownMenuTrigger as-child>
          <SidebarMenuButton size="lg"
            class="data-[state=open]:bg-sidebar-accent data-[state=open]:text-sidebar-accent-foreground">
            <Avatar class="h-8 w-8 rounded-lg">
              <AvatarFallback class="rounded-lg">
                {{ initials }}
              </AvatarFallback>
            </Avatar>
            <div class="grid flex-1 text-left text-sm leading-tight">
              <span class="truncate font-medium">{{ displayName }}</span>
              <span class="truncate text-xs">{{ user?.email || 'Loading...' }}</span>
            </div>
            <ChevronsUpDown class="ml-auto size-4" />
          </SidebarMenuButton>
        </DropdownMenuTrigger>
        <DropdownMenuContent class="w-[--reka-dropdown-menu-trigger-width] min-w-56 rounded-lg"
          :side="isMobile ? 'bottom' : 'right'" align="end" :side-offset="4">
          <DropdownMenuLabel class="p-0 font-normal">
            <div class="flex items-center gap-2 px-1 py-1.5 text-left text-sm">
              <Avatar class="h-8 w-8 rounded-lg">
                <AvatarFallback class="rounded-lg">
                  {{ initials }}
                </AvatarFallback>
              </Avatar>
              <div class="grid flex-1 text-left text-sm leading-tight">
                <span class="truncate font-semibold">{{ displayName }}</span>
                <span class="truncate text-xs">{{ user?.email || 'Loading...' }}</span>
              </div>
            </div>
          </DropdownMenuLabel>
          <DropdownMenuSeparator />
          <DropdownMenuGroup>
            <DropdownMenuItem class="cursor-pointer" @click="notificationStore.openSheet()">
              <Bell />
              <span class="flex-1">Meldingen</span>
              <Badge v-if="unreadCount > 0"
                class="ml-auto h-5 w-5 p-0 bg-primary flex items-center justify-center text-[10px]">
                {{ unreadCount }}
              </Badge>
            </DropdownMenuItem>
            <DropdownMenuItem>
              <Settings />
              Instellingen
            </DropdownMenuItem>
          </DropdownMenuGroup>
          <DropdownMenuSeparator />
          <DropdownMenuItem @click="handleLogout">
            <LogOut />
            Log out
          </DropdownMenuItem>
        </DropdownMenuContent>
      </DropdownMenu>
    </SidebarMenuItem>
  </SidebarMenu>
</template>