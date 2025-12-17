<script setup lang="ts">
import LogoWithText from '@/components/branding/LogoWithText.vue';
</script>

<template>
  <div class="grid h-svh lg:grid-cols-2">
    <div class="flex flex-col p-6 md:p-10 h-full overflow-hidden">
      <div class="shrink-0 mb-4">
        <LogoWithText class="flex justify-center" />
      </div>
      <div class="flex-1 overflow-hidden">
        <div class="h-full overflow-y-auto px-4">
          <div class="flex items-center justify-center min-h-full">
            <div class="w-full max-w-xs py-4">
              <RouterView />
            </div>
          </div>
        </div>
      </div>
    </div>
    <div class="bg-muted relative hidden lg:block">
      <img src="/assets/images/hospital-stock-photo.jpg" alt="Image"
        class="absolute inset-0 h-full w-full object-cover dark:brightness-[0.2] dark:grayscale">
    </div>
  </div>
</template>