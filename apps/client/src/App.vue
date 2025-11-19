<script setup lang="ts">
import { ref } from 'vue';

const counter = ref<number>(0);
</script>

<template>
  <div class="w-screen h-screen flex justify-center items-center">
    <button class="px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600 transition" @click="counter++">
      Count is: {{ counter }}
    </button>
  </div>
</template>