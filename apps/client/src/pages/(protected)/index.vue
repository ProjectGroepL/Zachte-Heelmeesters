<script setup lang="ts">
import { ref } from 'vue';
import { Button } from '@/components/ui/button';

const counter = ref<number>(0);
</script>

<template>
  <div class="flex flex-col justify-center items-center space-y-4 h-full">
    <div class="flex flex-col items-center">
      <h1 class="text-2xl font-bold">Zachte Heelmeesters</h1>
      <p>Dit is het startproject van de zachte heelmeesters app, probeer de counter eens uit!</p>
    </div>
    <Button @click="counter++">
      Count is: {{ counter }}
    </Button>
  </div>
</template>