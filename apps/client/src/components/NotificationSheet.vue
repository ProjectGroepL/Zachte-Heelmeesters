<script setup lang="ts">
import { computed } from 'vue'
import { useNotifications, useMarkAllRead, useMarkNotificationRead } from '@/composables/useNotifications'
import { useNotificationStore } from '@/stores/notifications'
import {
  Sheet,
  SheetContent,
  SheetDescription,
  SheetHeader,
  SheetTitle,
} from '@/components/ui/sheet'
import { Button } from '@/components/ui/button'
import { BellOff, Clock, AlertCircle } from 'lucide-vue-next'

const notificationStore = useNotificationStore()
const { data: notifications, loading, error, execute: refetchNotifications } = useNotifications()
const markAllRead = useMarkAllRead()
const markRead = useMarkNotificationRead()

const unreadCount = computed(() =>
  (notifications.value ?? []).filter(n => !n.isRead).length
)

async function handleMarkAllRead() {
  await markAllRead.mutate()
  await refetchNotifications()
}

async function handleMarkSingleRead(id: number, isRead: boolean) {
  if (isRead) return
  await markRead.mutate(id)
  await refetchNotifications()
}
</script>

<template>
  <Sheet v-model:open="notificationStore.isSheetOpen">
    <SheetContent class="w-[400px] sm:w-[540px]">
      <SheetHeader>
        <SheetTitle>Alle Notificaties</SheetTitle>
        <SheetDescription>
          {{ unreadCount }} ongelezen {{ unreadCount === 1 ? 'notificatie' : 'notificaties' }}
        </SheetDescription>
      </SheetHeader>

      <div class="mt-6 space-y-4">
        <div v-if="unreadCount > 0" class="flex justify-end">
          <Button variant="outline" size="sm" @click="handleMarkAllRead">
            Markeer alles als gelezen
          </Button>
        </div>

        <div v-if="loading" class="flex items-center justify-center py-12 text-muted-foreground">
          <div class="text-center space-y-2">
            <Clock class="h-8 w-8 mx-auto animate-pulse" />
            <p class="text-sm">Wordt geladen...</p>
          </div>
        </div>

        <div v-else-if="error" class="flex flex-col items-center justify-center py-12 text-destructive">
          <AlertCircle class="w-12 h-12 mb-4" />
          <p class="text-sm">{{ error }}</p>
        </div>

        <div v-else-if="!notifications || notifications.length === 0"
          class="flex flex-col items-center justify-center py-12 text-muted-foreground">
          <BellOff class="w-12 h-12 mb-4 opacity-20" />
          <p class="text-sm">Geen notificaties gevonden</p>
        </div>

        <div v-else class="space-y-3 max-h-[calc(100vh-200px)] overflow-y-auto">
          <div v-for="notification in notifications" :key="notification.id"
            @click="handleMarkSingleRead(notification.id, notification.isRead)"
            class="p-4 ml-2 border rounded-lg cursor-pointer transition-colors hover:bg-muted/50 relative"
            :class="{ 'bg-primary/5 border-primary/20': !notification.isRead }">
            <div v-if="!notification.isRead"
              class="absolute left-2 top-1/2 -translate-y-1/2 w-2 h-2 bg-primary rounded-full"></div>

            <p class="text-sm" :class="{ 'font-medium pl-3': !notification.isRead, 'pl-3': notification.isRead }">
              {{ notification.message }}
            </p>
            <span class="text-xs text-muted-foreground pl-3 mt-1 block">
              {{ new Date(notification.createdAt).toLocaleString('nl-NL') }}
            </span>
          </div>
        </div>
      </div>
    </SheetContent>
  </Sheet>
</template>
