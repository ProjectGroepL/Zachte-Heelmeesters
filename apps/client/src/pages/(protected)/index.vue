<script setup lang="ts">
import {onMounted, ref} from 'vue';
import { Button } from '@/components/ui/button';
import SpecialistDashboard from '@/components/SpecialistDashboard.vue'
import { useAuth } from '@/composables/useAuth'

const { isAuthenticated, getUser } = useAuth()

import type { User } from '@/types/Auth'
const user = ref<User | null>(null)

onMounted(async () => {
  if (!isAuthenticated()) return
  const res = await getUser()
  if (res.success) user.value = res.data!
})

const counter = ref<number>(0);
</script>

<template>
  <SpecialistDashboard v-if="user" />
  <div v-else class="flex flex-col justify-center items-center space-y-4 h-full">
    <div class="flex flex-col items-center">
      <h1 class="text-2xl font-bold">Zachte Heelmeesters</h1>
      <p>Dit is het startproject van de zachte heelmeesters app, probeer de counter eens uit!</p>
    </div>
    <Button @click="counter++">
      Count is: {{ counter }}
    </Button>
  </div>
</template>