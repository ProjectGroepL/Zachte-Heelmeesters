<script setup lang="ts">
import { FileText, Receipt } from "lucide-vue-next"

import {
  SidebarGroup,
  SidebarGroupLabel,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
} from '@/components/ui/sidebar'

import { useRouter } from "vue-router"

const items = [
  {
    title: "Openstaande rapporten",
    url: "/admin/AdminReportsPage",
    icon: FileText,
  },
]

const { currentRoute } = useRouter()
const isActive = (url: string) => currentRoute.value.path === url
</script>

<template>
  <SidebarGroup>
    <SidebarGroupLabel>Administratie</SidebarGroupLabel>

    <SidebarMenu>
      <SidebarMenuItem
        v-for="item in items"
        :key="item.title"
      >
        <RouterLink
          :to="item.url"
          class="flex items-center gap-2 w-full"
        >
          <SidebarMenuButton
            :tooltip="item.title"
            :class="isActive(item.url)
              ? 'bg-accent text-accent-foreground'
              : ''"
          >
            <component :is="item.icon" />
            <span>{{ item.title }}</span>
          </SidebarMenuButton>
        </RouterLink>
      </SidebarMenuItem>
    </SidebarMenu>
  </SidebarGroup>
</template>
