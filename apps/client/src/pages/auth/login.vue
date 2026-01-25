<script setup lang="ts">
import { ref, type HTMLAttributes } from "vue"
import { cn } from "@/lib/utils"
import { Button } from "@/components/ui/button"
import {
  Field,
  FieldDescription,
  FieldGroup,
  FieldLabel,
  FieldSeparator,
  FieldError
} from "@/components/ui/field"
import { Input } from "@/components/ui/input"
import { useAuth } from "@/composables/useAuth"
import { useRouter } from "vue-router"
import { z } from "zod"

const router = useRouter()
const { login, isAuthenticated } = useAuth()

const isLoading = ref(false)
const email = ref('')
const password = ref('')

const formError = ref<string | null>(null)
const errors = ref<Record<string, string[]>>({})

// Redirect to home if already authenticated
if (isAuthenticated()) {
  router.push('/')
}

// Clear field errors when user starts typing
const clearFieldError = (field: string) => {
  if (errors.value?.[field]) {
    delete errors.value[field]
  }
  // Also clear form error when user starts typing
  if (formError.value) {
    formError.value = null
  }
}

const loginSchema = z.object({
  email: z.string().min(1, "Email is verplicht").email("Ongeldig e-mailadres"),
  password: z.string().min(1, "Wachtwoord is verplicht"),
})

const handleSubmit = async (e: Event) => {
  e.preventDefault()

  // Start loading
  isLoading.value = true

  // Clear previous errors
  errors.value = {}
  formError.value = null

  const result = loginSchema.safeParse({
    email: email.value,
    password: password.value
  })

  if (!result.success) {
    errors.value = z.flattenError(result.error).fieldErrors
    isLoading.value = false
    return
  }

  // If validation passes, proceed with login logic
  const response = await login({
    email: email.value,
    password: password.value
  })

  if (response.success && response.data) {
    // Check if 2FA is required
    if (response.data.requiresTwoFactor && response.data.tempSessionId) {
      // Redirect to 2FA page with session ID
      console.log("Should redirect")
      router.push(`/auth/2fa?session=${response.data.tempSessionId}`)
    } else {
      // Normal login successful, redirect to home
      router.push('/')
    }
  } else {
    // Handle login errors
    if (response.status === 401) {
      formError.value = 'Onjuiste inloggegevens'
    } else {
      formError.value = response.message || 'Er is een fout opgetreden. Probeer het opnieuw.'
    }
  }

  isLoading.value = false
}

const props = defineProps<{
  class?: HTMLAttributes["class"]
}>()
</script>

<template>
  <form :class="cn('flex flex-col', props.class)" @submit="handleSubmit" novalidate>
    <FieldGroup class="gap-7">
      <div class="flex flex-col items-center gap-1 text-center">
        <h1 class="text-2xl font-bold">
          Inloggen op je account
        </h1>
        <p class="text-muted-foreground text-sm text-balance">
          Voer je gegevens in om in te loggen op je account
        </p>
      </div>
      <div v-if="formError"
        class="text-sm text-destructive bg-destructive/10 border border-destructive/20 rounded-md p-3">
        {{ formError }}
      </div>
      <Field>
        <FieldLabel for="email">
          E-mail
        </FieldLabel>
        <Input id="email" type="email" placeholder="m@voorbeeld.com" required @input="clearFieldError('email')"
          v-model="email" />
        <FieldError v-if="errors.email && errors.email[0]">{{ errors.email[0] }}</FieldError>
      </Field>
      <Field>
        <div class="flex items-center">
          <FieldLabel for="password">
            Wachtwoord
          </FieldLabel>
        </div>
        <Input id="password" type="password" required @input="clearFieldError('password')" v-model="password" />
        <FieldError v-if="errors.password && errors.password[0]">{{ errors.password[0] }}</FieldError>
      </Field>
      <Field>
        <Button type="submit">
          Inloggen
        </Button>
      </Field>
      <Field>
        <FieldDescription class="text-center">
          Nog geen account?
          <RouterLink to="/auth/registreren">Registreren</RouterLink>
        </FieldDescription>
      </Field>
    </FieldGroup>
  </form>
</template>
