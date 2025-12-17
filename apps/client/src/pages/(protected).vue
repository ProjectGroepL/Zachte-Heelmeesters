<script setup lang="ts">
import {
  SidebarInset,
  SidebarProvider,
  SidebarTrigger,
} from '@/components/ui/sidebar'
import {
  Breadcrumb,
  BreadcrumbItem,
  BreadcrumbLink,
  BreadcrumbList,
  BreadcrumbPage,
  BreadcrumbSeparator,
} from '@/components/ui/breadcrumb'
import { Separator } from '@/components/ui/separator'
import AppSidebar from '@/components/layout/dashboard/AppSidebar.vue'
import { useRouter } from 'vue-router'
import { House } from 'lucide-vue-next'
import { computed } from 'vue'
import { useAuth } from "@/composables/useAuth";

const { isAuthenticated } = useAuth();
const router = useRouter();

// If the user is not authenticated, redirect to login page
if (!isAuthenticated()) {
  router.push('/auth/login');
}

const filteredPathSegments = computed(() => {
  const pathSegments = router.currentRoute.value.path.split('/')

  // Remove empty segments
  const filtered = pathSegments.map((segment) => segment.trim()).filter((segment) => segment.length > 0)

  filtered.unshift('dashboard')
  return filtered
})
</script>

<template>
  <SidebarProvider>
    <AppSidebar />
    <SidebarInset class="flex flex-col h-screen">
      <header
        class="flex h-16 shrink-0 items-center gap-2 transition-[width,height] ease-linear group-has-data-[collapsible=icon]/sidebar-wrapper:h-12">
        <div class="flex items-center gap-2 px-4">
          <SidebarTrigger class="-ml-1" />
          <Separator orientation="vertical" class="mr-2 data-[orientation=vertical]:h-4" />
          <Breadcrumb>
            <BreadcrumbList class="capitalize">
              <template v-for="(segment, index) in filteredPathSegments.slice(0, -1)" :key="`breadcrumb-${index}`">
                <BreadcrumbItem class="hidden md:block">
                  <BreadcrumbLink class="flex items-center"
                    :href="`/${filteredPathSegments.slice(1, index + 1).join('/')}`">
                    <House v-if="index === 0" class="mr-1 h-4 w-4" />
                    {{ segment }}
                  </BreadcrumbLink>
                </BreadcrumbItem>

                <BreadcrumbSeparator v-if="index <= filteredPathSegments.length - 2" class="hidden md:block" />
              </template>

              <BreadcrumbItem>
                <BreadcrumbPage>{{ filteredPathSegments[filteredPathSegments.length - 1] }}</BreadcrumbPage>
              </BreadcrumbItem>

            </BreadcrumbList>
          </Breadcrumb>
        </div>
      </header>
      <div class="flex-1 overflow-y-auto p-4 pt-0">
        <RouterView />
      </div>
    </SidebarInset>
  </SidebarProvider>
</template>