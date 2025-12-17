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
import NavDoctor from '@/components/layout/dashboard/NavDoctor.vue'

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
import { useAuth } from '@/composables/useAuth'
import { onMounted, ref } from 'vue'


const props = withDefaults(defineProps<SidebarProps>(), {
  collapsible: "icon",
})

const { open } = useSidebar()

const { isAuthenticated, getUser } = useAuth()

import type { User } from '@/types/Auth'
const user = ref<User | null>(null)

onMounted(async () => {
  if (!isAuthenticated()) return
  const res = await getUser()
  if (res.success) user.value = res.data!
})

const hasRole = (roleName: string) => {
  if (!user.value) return false

  // Case 1: server returns an array `role: Role[]`
  const maybeRolesArray = (user.value as any).role
  if (Array.isArray(maybeRolesArray)) {
    return maybeRolesArray.some((r: any) => (r && (r.name === roleName || r === roleName)))
  }

  // Case 2: server returns a single role string `Role` or `role` (legacy)
  const maybeRoleString = (user.value as any).Role || (user.value as any).role
  if (typeof maybeRoleString === 'string') {
    return maybeRoleString === roleName || maybeRoleString.toLowerCase() === roleName.toLowerCase()
  }

  return false
}

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
       <!-- Doctor -->
      <NavDoctor v-if="hasRole('Huisarts')" />
      <NavAdmin :items="data.projects" />
    </SidebarContent>
    <SidebarFooter>
      <NavUser :user="data.user" />
    </SidebarFooter>
    <SidebarRail />
  </Sidebar>
</template>
