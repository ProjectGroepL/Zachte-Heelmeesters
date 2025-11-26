<script setup lang="ts">
import { Activity } from "lucide-vue-next"
import { cn } from '@/lib/utils'

type LogoSize = 'sm' | 'md' | 'lg' | 'xl'

const props = withDefaults(defineProps<{
  size?: LogoSize
  class?: string
}>(), {
  size: 'md'
})

const sizeClasses: Record<LogoSize, string> = {
  sm: 'size-6 p-1.5',
  md: 'size-8 p-2',
  lg: 'size-10 p-2.5',
  xl: 'size-12 p-3'
}
</script>

<template>
  <div
    :class="cn('bg-primary text-primary-foreground rounded-sm flex items-center justify-center', sizeClasses[size], props.class)">
    <Activity class="size-full" />
  </div>
</template>