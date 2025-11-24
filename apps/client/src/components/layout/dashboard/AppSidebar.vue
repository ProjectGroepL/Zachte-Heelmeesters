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
import NavProjects from '@/components/layout/dashboard/NavAdmin.vue'
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
      icon: ChartColumn,
      isActive: true,
    },
    {
      title: "Afspraken",
      url: "/afspraken",
      icon: ClipboardClock,
      isActive: false,
    },
    {
      title: "Agenda",
      url: "/agenda",
      icon: Calendar,
      isActive: false,
    },
  ],
  projects: [
    {
      name: "Gebruikers",
      url: "#",
      icon: Users2,
    },
    {
      name: "Activiteiten",
      url: "#",
      icon: Activity,
    },
  ],
}
</script>

<template>
  <Sidebar v-bind="props">
    <SidebarHeader>
      <a>
        <div :class="cn('flex items-center w-full', open && 'px-4 py-3 space-x-2 ')">
          <div :class="cn('bg-primary text-primary-foreground! rounded-sm size-8 p-2', open && 'size-10')">
            <Activity class="size-full" />
          </div>
          <span v-if="open" class="text-lg text-primary font-bold leading-tight">Zachte
            Heelmeesters</span>
        </div>
      </a>
    </SidebarHeader>
    <SidebarContent>
      <NavMain :items="data.navMain" />
      <NavProjects :admin-items="data.projects" />
    </SidebarContent>
    <SidebarFooter>
      <NavUser :user="data.user" />
    </SidebarFooter>
    <SidebarRail />
  </Sidebar>
</template>
