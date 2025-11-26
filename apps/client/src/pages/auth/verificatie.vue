<script setup lang="ts">
import type { HTMLAttributes } from "vue"
import { cn } from "@/lib/utils"
import { Button } from "@/components/ui/button"
import { ArrowLeft } from "lucide-vue-next"
import {
  Field,
  FieldDescription,
  FieldGroup,
  FieldLabel,
} from "@/components/ui/field"
import {
  InputOTP,
  InputOTPGroup,
  InputOTPSeparator,
  InputOTPSlot,
} from "@/components/ui/input-otp"

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
    <form novalidate>
      <FieldGroup>
        <div class="flex flex-col items-center gap-1 text-center">
          <h1 class="text-2xl font-bold">
            Voer verificatiecode in
          </h1>
          <p class="text-muted-foreground text-sm text-balance">
            We hebben een code naar je e-mail gestuurd.
          </p>
        </div>
        <Field>
          <FieldLabel for="otp" class="sr-only">
            Verificatiecode
          </FieldLabel>
          <InputOTP id="otp" :maxlength="6" class="flex w-full justify-center" required>
            <InputOTPGroup class="*:data-[slot=input-otp-slot]:rounded-md *:data-[slot=input-otp-slot]:border gap-x-2">
              <InputOTPSlot :index="0" />
              <InputOTPSlot :index="1" />
              <InputOTPSlot :index="2" />
              <InputOTPSlot :index="3" />
              <InputOTPSlot :index="4" />
              <InputOTPSlot :index="5" />
            </InputOTPGroup>
          </InputOTP>
          <FieldDescription class="text-center">
            Voer de 6-cijferige code in die naar je e-mail is gestuurd.
          </FieldDescription>
        </Field>
        <Button type="submit">
          Verifiëren
        </Button>
        <FieldDescription class="text-center">
          Code niet ontvangen? <a href="#">Opnieuw versturen</a>
        </FieldDescription>
      </FieldGroup>
    </form>
  </div>
</template>
