<script setup lang="ts">
import type { HTMLAttributes } from "vue"
import { ref, computed } from "vue"
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
import { Combobox } from "@/components/ui/combobox"

const props = defineProps<{
  class?: HTMLAttributes["class"]
}>()

// Mock data for doctors - replace with actual API call later
const doctors = [
  { value: "dr-janssen", label: "Dr. P. Janssen" },
  { value: "dr-de-boer", label: "Dr. M. de Boer" },
  { value: "dr-van-der-meer", label: "Dr. A. van der Meer" },
  { value: "dr-bakker", label: "Dr. S. Bakker" },
  { value: "dr-visser", label: "Dr. L. Visser" },
]

const selectedDoctor = ref("")
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

      <!-- Personal Information -->
      <FieldSeparator />
      <div class="text-sm font-bold">Persoonlijke gegevens</div>

      <Field>
        <FieldLabel for="firstName">
          Voornaam
        </FieldLabel>
        <Input id="firstName" type="text" placeholder="Jan" required />
      </Field>
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <Field>
          <FieldLabel for="middleName">
            Tussenvoegsel
            <span class="text-muted-foreground font-normal text-xs">(Optioneel)</span>
          </FieldLabel>
          <Input id="middleName" type="text" placeholder="van der" />
        </Field>
        <Field>
          <FieldLabel for="lastName">
            Achternaam
          </FieldLabel>
          <Input id="lastName" type="text" placeholder="Janssen" required />
        </Field>
      </div>

      <Field>
        <FieldLabel for="email">
          E-mailadres
        </FieldLabel>
        <Input id="email" type="email" placeholder="jan@voorbeeld.com" required />
      </Field>

      <Field>
        <FieldLabel for="phoneNumber">
          Telefoonnummer
        </FieldLabel>
        <Input id="phoneNumber" type="tel" placeholder="+31 6 12345678" required />
      </Field>

      <!-- Address Information -->
      <FieldSeparator />
      <div class="text-sm font-bold">Adresgegevens</div>

      <Field>
        <FieldLabel for="country">
          Land
        </FieldLabel>
        <Input id="country" type="text" required />
      </Field>

      <Field>
        <FieldLabel for="postalCode">
          Postcode
        </FieldLabel>
        <Input id="postalCode" type="text" placeholder="1234 AB" required />
      </Field>

      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <Field>
          <FieldLabel for="houseNumber">
            Huisnummer
          </FieldLabel>
          <Input id="houseNumber" type="text" placeholder="123" required />
        </Field>
        <Field>
          <FieldLabel for="houseNumberAddition">
            Toevoeging
            <span class="text-muted-foreground font-normal text-xs">(Optioneel)</span>
          </FieldLabel>
          <Input id="houseNumberAddition" type="text" placeholder="A" />
        </Field>
      </div>

      <Field class="md:col-span-2">
        <FieldLabel for="street">
          Straatnaam
        </FieldLabel>
        <Input id="street" type="text" placeholder="Hoofdstraat" required />
      </Field>

      <Field>
        <FieldLabel for="city">
          Plaats
        </FieldLabel>
        <Input id="city" type="text" placeholder="Amsterdam" required />
      </Field>

      <!-- Doctor Selection -->
      <FieldSeparator />
      <div class="text-sm font-bold">Huisartsenpraktijk</div>

      <Field>
        <FieldLabel>Selecteer je huisarts</FieldLabel>
        <Combobox v-model="selectedDoctor" :options="doctors" placeholder="Selecteer een huisarts..."
          search-placeholder="Zoek een huisarts..." empty-message="Geen huisarts gevonden." />
        <FieldDescription>
          Kies de huisarts waar u uw afspraken mee maakt.
        </FieldDescription>
      </Field>

      <!-- Password Information -->
      <FieldSeparator />
      <div class="text font-bold">Wachtwoord</div>

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
        <Button type="submit" class="w-full">
          Account aanmaken
        </Button>
      </Field>
      <Field>
        <FieldDescription class="px-6 text-center">
          Al een account? <RouterLink to="/auth/login"
            class="text-primary underline underline-offset-4 hover:text-primary/80">Inloggen</RouterLink>
        </FieldDescription>
      </Field>
    </FieldGroup>
  </form>
</template>
