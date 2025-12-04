<script setup lang="ts">
import { ref, type HTMLAttributes, onMounted } from "vue"
import { cn } from "@/lib/utils"
import { Button } from "@/components/ui/button"
import { ArrowLeft } from "lucide-vue-next"
import {
  Field,
  FieldDescription,
  FieldGroup,
  FieldLabel,
  FieldError
} from "@/components/ui/field"
import {
  InputOTP,
  InputOTPGroup,
  InputOTPSeparator,
  InputOTPSlot,
} from "@/components/ui/input-otp"
import { useAuth } from "@/composables/useAuth"
import { useRouter, useRoute } from "vue-router"
import { z } from "zod"
import { REGEXP_ONLY_DIGITS } from "vue-input-otp"

const router = useRouter()
const route = useRoute()
const { verifyTwoFactor, resendTwoFactor } = useAuth()

const isLoading = ref(false)
const isResending = ref(false)
const code = ref('')
const tempSessionId = ref<number | null>(null)

const formError = ref<string | null>(null)
const codeError = ref<string | null>(null)

// Get session ID from query params
onMounted(() => {
  const sessionParam = route.query.session
  if (sessionParam && typeof sessionParam === 'string') {
    tempSessionId.value = parseInt(sessionParam)
  } else {
    // No session ID, redirect to login
    router.replace('/auth/login')
  }
})

// Clear errors when user starts typing
const clearError = () => {
  if (codeError.value) {
    codeError.value = null
  }
  if (formError.value) {
    formError.value = null
  }
}

const codeSchema = z.object({
  code: z.string().min(6, "Code moet 6 cijfers zijn").max(6, "Code moet 6 cijfers zijn").regex(/^\d+$/, "Code mag alleen cijfers bevatten"),
})

const handleSubmit = async (e: Event) => {
  e.preventDefault()

  if (!tempSessionId.value) {
    formError.value = 'Ongeldige sessie. Probeer opnieuw in te loggen.'
    return
  }

  // Start loading
  isLoading.value = true

  // Clear previous errors
  codeError.value = null
  formError.value = null

  const result = codeSchema.safeParse({
    code: code.value
  })

  if (!result.success) {
    const flattenedErrors = z.flattenError(result.error).fieldErrors
    codeError.value = flattenedErrors.code?.[0] || 'Ongeldige code'
    isLoading.value = false
    return
  }

  // If validation passes, proceed with 2FA verification
  const response = await verifyTwoFactor({
    tempSessionId: tempSessionId.value,
    code: code.value
  })

  if (response.success && response.data) {
    // 2FA verification successful, redirect to home
    router.push('/')
  } else {
    // Handle verification errors
    if (response.status === 400) {
      formError.value = 'Ongeldige verificatiecode'
    } else {
      formError.value = 'Er is een fout opgetreden. Probeer het opnieuw.'
    }

    console.error('Resend 2FA error:', response.message)
  }

  isLoading.value = false
}

const handleResend = async () => {
  if (!tempSessionId.value || isResending.value) {
    return
  }

  isResending.value = true

  const response = await resendTwoFactor({
    tempSessionId: tempSessionId.value
  })

  if (response.success) {
    formError.value = null
    // Show success message temporarily
    formError.value = 'Code opnieuw verstuurd naar je e-mail'
    setTimeout(() => {
      if (formError.value === 'Code opnieuw verstuurd naar je e-mail') {
        formError.value = null
      }
    }, 3000)
  } else {
    formError.value = 'Kon code niet opnieuw versturen'
    console.error('Resend 2FA error:', response.message)
  }

  isResending.value = false
}

const props = defineProps<{
  class?: HTMLAttributes["class"]
}>()
</script>

<template>
  <div :class="cn('flex flex-col gap-6', props.class)">
    <RouterLink to="/auth/login" class="hover:underline text-accent-foreground">
      <div class="flex gap-x-2">
        <ArrowLeft />
        Terug naar inloggen
      </div>
    </RouterLink>
    <form @submit="handleSubmit" novalidate>
      <FieldGroup class="gap-7">
        <div class="flex flex-col items-center gap-1 text-center">
          <h1 class="text-2xl font-bold">
            Voer verificatiecode in
          </h1>
          <p class="text-muted-foreground text-sm text-balance">
            We hebben een code naar je e-mail gestuurd.
          </p>
        </div>
        <div v-if="formError"
          class="text-sm text-destructive bg-destructive/10 border border-destructive/20 rounded-md p-3">
          {{ formError }}
        </div>
        <Field>
          <FieldLabel for="otp" class="sr-only">
            Verificatiecode
          </FieldLabel>
          <InputOTP id="otp" :maxlength="6" class="flex w-full justify-center" :pattern="REGEXP_ONLY_DIGITS" required
            @input="clearError" v-model="code">
            <InputOTPGroup class="*:data-[slot=input-otp-slot]:rounded-md *:data-[slot=input-otp-slot]:border gap-x-2">
              <InputOTPSlot :index="0" />
              <InputOTPSlot :index="1" />
              <InputOTPSlot :index="2" />
              <InputOTPSlot :index="3" />
              <InputOTPSlot :index="4" />
              <InputOTPSlot :index="5" />
            </InputOTPGroup>
          </InputOTP>
          <FieldError v-if="codeError">{{ codeError }}</FieldError>
          <FieldDescription v-else class="text-center">
            Voer de 6-cijferige code in die naar je e-mail is gestuurd.
          </FieldDescription>
        </Field>
        <Button type="submit" :disabled="isLoading">
          {{ isLoading ? 'Verifiëren...' : 'Verifiëren' }}
        </Button>
        <FieldDescription class="text-center">
          Code niet ontvangen?
          <button type="button" @click="handleResend" :disabled="isResending" class="underline hover:no-underline">
            {{ isResending ? 'Versturen...' : 'Opnieuw versturen' }}
          </button>
        </FieldDescription>
      </FieldGroup>
    </form>
  </div>
</template>
