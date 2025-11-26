<script setup lang="ts">
import type { HTMLAttributes } from "vue"
import { cn } from "@/lib/utils"
import { Button } from "@/components/ui/button"
import {
  Field,
  FieldDescription,
  FieldGroup,
  FieldLabel,
  FieldSeparator,
} from "@/components/ui/field"
import { Input } from "@/components/ui/input"

const props = defineProps<{
  class?: HTMLAttributes["class"]
}>()

</script>

<template>
  <form :class="cn('flex flex-col', props.class)" novalidate>
    <FieldGroup class="gap-7">
      <div class="flex flex-col items-center gap-1 text-center">
        <h1 class="text-2xl font-bold">
          Inloggen op je account
        </h1>
        <p class="text-muted-foreground text-sm text-balance">
          Voer je gegevens in om in te loggen op je account
        </p>
      </div>
      <Field>
        <FieldLabel for="email">
          E-mail
        </FieldLabel>
        <Input id="email" type="email" placeholder="m@voorbeeld.com" required />
      </Field>
      <Field>
        <div class="flex items-center">
          <FieldLabel for="password">
            Wachtwoord
          </FieldLabel>
          <a href="#" class="ml-auto text-sm underline-offset-4 hover:underline">
            Wachtwoord vergeten?
          </a>
        </div>
        <Input id="password" type="password" required />
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
