<script setup lang="ts">
import { FilePlus, ClipboardList } from 'lucide-vue-next'
</script>

<template>
  <nav aria-label="Specialist navigatie" class="px-2">
    <ul class="space-y-1">
      <li>
        <RouterLink
          to="/specialist/access-request"
          class="flex items-center gap-2 px-3 py-2 rounded hover:bg-muted"
        >
          <FilePlus class="size-4" />
          <span>Toegang aanvragen</span>
        </RouterLink>
      </li>

      <li>
        <RouterLink
          to="/specialist/access-requests"
          class="flex items-center gap-2 px-3 py-2 rounded hover:bg-muted"
        >
          <ClipboardList class="size-4" />
          <span>Mijn aanvragen</span>
        </RouterLink>
      </li>
    </ul>
  </nav>
</template>
