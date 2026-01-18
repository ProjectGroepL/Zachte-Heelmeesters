<script setup lang="ts">
import { Receipt } from 'lucide-vue-next'
import {
  SidebarGroup,
  SidebarGroupLabel,
  SidebarMenu,
  SidebarMenuItem,
  SidebarMenuButton
} from '@/components/ui/sidebar'
import { useRouter } from 'vue-router'

const items = [
  {
    title: 'Declaraties',
    url: '/insurance/InsuranceDashboard',
    icon: Receipt
  }
]

const { currentRoute } = useRouter()
const isActive = (url: string) => currentRoute.value.path === url
</script>

<template>
  <SidebarGroup>
    <SidebarGroupLabel>Zorgverzekeraar</SidebarGroupLabel>

    <SidebarMenu>
      <SidebarMenuItem
        v-for="item in items"
        :key="item.title"
      >
        <RouterLink :to="item.url">
          <SidebarMenuButton
            :aria-current="isActive(item.url) ? 'page' : undefined"
          >
            <component :is="item.icon" aria-hidden="true" />
            <span>{{ item.title }}</span>
          </SidebarMenuButton>
        </RouterLink>
      </SidebarMenuItem>
    </SidebarMenu>
  </SidebarGroup>
</template>
