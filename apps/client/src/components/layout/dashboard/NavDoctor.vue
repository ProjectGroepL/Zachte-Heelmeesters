<script setup lang="ts">
import type { LucideIcon } from "lucide-vue-next"
import { Stethoscope, Plus } from "lucide-vue-next"

import {
  SidebarGroup,
  SidebarGroupLabel,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
} from '@/components/ui/sidebar'

import { useRouter } from "vue-router"

const items = [
  { title: "Doorverwijzingen", url: "/referrals", icon: Stethoscope },
  { title: "Nieuwe Doorverwijzing", url: "/referrals/new", icon: Plus },
  { title: "Medisch document Maken", url: "/huisarts/CreateMedicalDocument", icon: Plus },
  { title: "Medisch document Finalize", url: "/huisarts/MyMedicalDocuments", icon: Plus },
  { title: "EindRapport aanvraag", url: "/huisarts/DoctorAppointmentsPage", icon: Plus },
    { title: "EindRapport overzicht", url: "/huisarts/DoctorReportsPage", icon: Plus },
]

const { currentRoute } = useRouter()
const isActive = (url: string) => currentRoute.value.path === url
</script>

<template>
  <SidebarGroup>
    <SidebarGroupLabel>Huisarts</SidebarGroupLabel>
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
