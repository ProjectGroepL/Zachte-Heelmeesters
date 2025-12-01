<script setup lang="ts">
import { cn } from '@/lib/utils'
import Logo from './Logo.vue'

type LogoSize = 'sm' | 'md' | 'lg' | 'xl'

const props = withDefaults(defineProps<{
  size?: LogoSize
  showText?: boolean
  class?: string
}>(), {
  size: 'md',
  showText: true
})

const textClasses: Record<LogoSize, string> = {
  sm: 'text-sm',
  md: 'text-lg',
  lg: 'text-xl',
  xl: 'text-2xl'
}

const containerClasses: Record<LogoSize, string> = {
  sm: 'px-2 py-1.5 space-x-1.5',
  md: 'px-4 py-3 space-x-2',
  lg: 'px-5 py-4 space-x-3',
  xl: 'px-6 py-5 space-x-4'
}
</script>

<template>
  <div :class="cn('flex items-center w-full', showText && containerClasses[size], props.class)">
    <Logo :size="size" />
    <span v-if="showText" :class="cn('text-primary font-bold leading-tight', textClasses[size])">
      Zachte Heelmeesters
    </span>
  </div>
</template>