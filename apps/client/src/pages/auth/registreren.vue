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
    <FieldGroup class="gap-4">
      <div class="flex flex-col items-center gap-1 text-center">
        <h1 class="text-2xl font-bold">
          Maak je account aan
        </h1>
        <p class="text-muted-foreground text-sm text-balance">
          Vul je gegevens in om je account te maken
        </p>
      </div>
      <Field>
        <FieldLabel for="name">
          Volledige naam
        </FieldLabel>
        <Input id="name" type="text" placeholder="Jan Janssen" required />
      </Field>
      <Field>
        <FieldLabel for="email">
          E-mail
        </FieldLabel>
        <Input id="email" type="email" placeholder="m@voorbeeld.com" required />
      </Field>
      <Field>
        <FieldLabel for="password">
          Wachtwoord
        </FieldLabel>
        <Input id="password" type="password" required />
        <FieldDescription>
          Moet minimaal 8 karakters lang zijn.
        </FieldDescription>
      </Field>
      <Field>
        <FieldLabel for="confirm-password">
          Bevestig wachtwoord
        </FieldLabel>
        <Input id="confirm-password" type="password" required />
        <FieldDescription>Bevestig je wachtwoord.</FieldDescription>
      </Field>
      <Field>
        <Button type="submit">
          Account aanmaken
        </Button>
      </Field>
      <Field>
        <FieldDescription class="px-6 text-center">
          Al een account? <RouterLink to="/auth/login">Inloggen</RouterLink>
        </FieldDescription>
      </Field>
    </FieldGroup>
  </form>
</template>
