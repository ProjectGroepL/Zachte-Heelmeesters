<script setup lang="ts">
import type { LucideIcon } from "lucide-vue-next"
import {
  Folder,
  Forward,
  MoreHorizontal,
  Stethoscope,
  Trash2,
} from "lucide-vue-next"

import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu'
import {
  SidebarGroup,
  SidebarGroupLabel,
  SidebarMenu,
  SidebarMenuAction,
  SidebarMenuButton,
  SidebarMenuItem,
  useSidebar,
} from '@/components/ui/sidebar'
import { useRouter } from "vue-router";

defineProps<{
  items: {
    title: string
    url: string
    icon?: LucideIcon
  }[]
}>()

const { currentRoute } = useRouter()

const isActive = (url: string) => currentRoute.value.path === url
</script>

<template>
  <SidebarGroup>
    <SidebarGroupLabel>Administratie</SidebarGroupLabel>
    <SidebarMenu>
      <SidebarMenuItem v-for="item in items" :key="item.title">
        <RouterLink :to="item.url" class="flex items-center gap-2 w-full">
          <SidebarMenuButton :tooltip="item.title"
            :class="isActive(item.url) ? 'bg-accent text-accent-foreground' : ''">
            <component :is="item.icon" v-if="item.icon" />
            <span>{{ item.title }}</span>
          </SidebarMenuButton>
        </RouterLink>
      </SidebarMenuItem>
    </SidebarMenu>
  </SidebarGroup>
</template>
