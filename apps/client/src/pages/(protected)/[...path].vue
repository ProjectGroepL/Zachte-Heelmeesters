<script setup lang="ts">
import { Button } from '@/components/ui/button'
import { useRouter } from 'vue-router'
import { Home, ArrowLeft, Search } from 'lucide-vue-next'

const router = useRouter()

const goHome = () => {
  router.push('/')
}

const goBack = () => {
  router.back()
}
</script>

<template>
  <div class="flex flex-col items-center justify-center h-full text-center space-y-6">
    <!-- Error Code -->
    <div class="relative">
      <h1 class="text-7xl font-bold text-gray-200 select-none">404</h1>
      <div class="absolute inset-0 flex items-center justify-center">
        <Search class="h-12 w-12 text-gray-400" />
      </div>
    </div>

    <!-- Error Message -->
    <div class="space-y-3 max-w-sm">
      <h2 class="text-2xl font-semibold text-gray-900">
        Pagina niet gevonden
      </h2>
      <p class="text-gray-600 text-base">
        De pagina die je zoekt bestaat niet of is verplaatst.
        Controleer de URL of ga terug naar het dashboard.
      </p>
    </div>

    <!-- Action Buttons -->
    <div class="flex flex-col sm:flex-row gap-3">
      <Button @click="goHome" class="flex items-center gap-2 text-sm">
        <Home class="h-4 w-4" />
        Terug naar Overzicht
      </Button>
      <Button @click="goBack" variant="outline" class="flex items-center gap-2 text-sm">
        <ArrowLeft class="h-4 w-4" />
        Vorige pagina
      </Button>
    </div>

    <!-- Additional Help -->
    <div class="mt-8 p-4 bg-gray-50 rounded-lg max-w-sm">
      <h3 class="font-semibold text-gray-900 mb-1 text-sm">
        Hulp nodig?
      </h3>
      <p class="text-xs text-gray-600">
        Als je denkt dat dit een fout is, neem dan contact op met de beheerder
        of probeer het later opnieuw.
      </p>
    </div>
  </div>
</template>