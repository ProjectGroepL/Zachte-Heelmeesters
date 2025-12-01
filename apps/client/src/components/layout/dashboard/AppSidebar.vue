<script setup lang="ts">
import type { SidebarProps } from '@/components/ui/sidebar'

import {
  Activity,
  Calendar,
  ChartColumn,
  ClipboardClock,
  Users2,
} from "lucide-vue-next"
import NavMain from '@/components/layout/dashboard/NavMain.vue'
import NavUser from '@/components/layout/dashboard/NavUser.vue'

import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarHeader,
  SidebarRail,
  useSidebar
} from '@/components/ui/sidebar'
import { cn } from '@/lib/utils'
import { useRouter } from 'vue-router'
import NavAdmin from '@/components/layout/dashboard/NavAdmin.vue'
import Separator from '@/components/ui/separator/Separator.vue'

const props = withDefaults(defineProps<SidebarProps>(), {
  collapsible: "icon",
})

const { open } = useSidebar()

// This is sample data.
const data = {
  user: {
    name: "Jan Jansen",
    email: "jan.jansen@example.com",
    avatar: null
  },

  navMain: [
    {
      title: "Overzicht",
      url: "/",
      icon: ChartColumn
    },
    {
      title: "Afspraken",
      url: "/afspraken",
      icon: ClipboardClock,
    },
    {
      title: "Agenda",
      url: "/agenda",
      icon: Calendar,
    },
  ],
  projects: [
    {
      title: "Gebruikers",
      url: "/admin/gebruikers",
      icon: Users2,
    },
    {
      title: "Activiteiten",
      url: "/admin/activiteiten",
      icon: Activity,
    },
  ],
}
</script>

<template>
  <Sidebar v-bind="props">
    <SidebarHeader>
      <RouterLink to="/">
        <div :class="cn('flex items-center w-full', open && 'px-4 py-3 space-x-2 ')">
          <div :class="cn('bg-primary text-primary-foreground! rounded-sm size-8 p-2', open && 'size-10')">
            <Activity class="size-full" />
          </div>
          <span v-if="open" class="text-lg text-primary font-bold leading-tight">Zachte
            Heelmeesters</span>
        </div>
      </RouterLink>
    </SidebarHeader>
    <SidebarContent>
      <NavMain :items="data.navMain" />
      <div v-if="!open" class="w-full px-2">
        <Separator />
      </div>
      <NavAdmin :items="data.projects" />
    </SidebarContent>
    <SidebarFooter>
      <NavUser :user="data.user" />
    </SidebarFooter>
    <SidebarRail />
  </Sidebar>
</template>
