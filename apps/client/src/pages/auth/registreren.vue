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
  FieldError
} from "@/components/ui/field"
import { Input } from "@/components/ui/input"
import { Combobox } from "@/components/ui/combobox"
import { useAuth } from "@/composables/useAuth"
import { useRouter } from "vue-router"
import { z } from "zod"
import { useQuery } from "@/composables/useApi"
import { type GpName } from "@/types/GeneralPractitioner.d"

const router = useRouter()
const { register, isAuthenticated } = useAuth()

const isLoading = ref(false)

// Form fields
const firstName = ref('')
const middleName = ref('')
const lastName = ref('')
const email = ref('')
const phoneNumber = ref('')
const country = ref('')
const zipCode = ref('')
const houseNumber = ref('')
const houseNumberAddition = ref('')
const street = ref('')
const city = ref('')
const password = ref('')
const confirmPassword = ref('')

const formError = ref<string | null>(null)
const errors = ref<Record<string, string[]>>({})

const { data: gps, loading, error } = useQuery<GpName[]>("/DoctorPatients/general-practitioners")

const gpOptions = computed(() => {
  if (!gps.value) return []
  return gps.value
    .filter(gp => gp && gp.id !== undefined && gp.fullName)
    .map((gp) => ({
      label: gp.fullName.replace(/\s+/g, ' ').trim(), // Clean up extra spaces
      value: gp.id.toString()
    }))
})

// Redirect to home if already authenticated
if (isAuthenticated()) {
  router.push('/')
}

const selectedDoctor = ref<string | undefined>()

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

const registerSchema = z.object({
  firstName: z.string().min(1, "Voornaam is verplicht"),
  middleName: z.string().optional(),
  lastName: z.string().min(1, "Achternaam is verplicht"),
  email: z.string().min(1, "E-mailadres is verplicht").email("Ongeldig e-mailadres"),
  phoneNumber: z.string().min(1, "Telefoonnummer is verplicht"),
  country: z.string().min(1, "Land is verplicht"),
  zipCode: z.string().min(1, "Postcode is verplicht"),
  houseNumber: z.string().min(1, "Huisnummer is verplicht"),
  houseNumberAddition: z.string().optional(),
  street: z.string().min(1, "Straatnaam is verplicht"),
  city: z.string().min(1, "Plaats is verplicht"),
  password: z.string()
    .min(6, "Wachtwoord moet minimaal 6 karakters lang zijn")
    .regex(/[0-9]/, "Wachtwoord moet minimaal één cijfer bevatten")
    .regex(/[a-z]/, "Wachtwoord moet minimaal één kleine letter bevatten")
    .regex(/[A-Z]/, "Wachtwoord moet minimaal één hoofdletter bevatten"),
  confirmPassword: z.string().min(1, "Bevestig je wachtwoord"),
  selectedDoctor: z.string().min(1, "Huisarts is verplicht")
}).refine((data) => data.password === data.confirmPassword, {
  message: "Wachtwoorden komen niet overeen",
  path: ["confirmPassword"],
})

const handleSubmit = async (e: Event) => {
  e.preventDefault()

  // Start loading
  isLoading.value = true

  // Clear previous errors
  errors.value = {}
  formError.value = null

  const result = registerSchema.safeParse({
    firstName: firstName.value,
    middleName: middleName.value || undefined,
    lastName: lastName.value,
    email: email.value,
    phoneNumber: phoneNumber.value,
    country: country.value,
    zipCode: zipCode.value,
    houseNumber: houseNumber.value,
    houseNumberAddition: houseNumberAddition.value || undefined,
    street: street.value,
    city: city.value,
    password: password.value,
    confirmPassword: confirmPassword.value,
    selectedDoctor: selectedDoctor.value || ""
  })

  if (!result.success) {
    errors.value = z.flattenError(result.error).fieldErrors
    isLoading.value = false
    return
  }

  // If validation passes, proceed with registration
  const response = await register({
    firstName: firstName.value,
    middleName: middleName.value || undefined,
    lastName: lastName.value,
    email: email.value,
    phoneNumber: phoneNumber.value,
    country: country.value,
    zipCode: zipCode.value,
    houseNumber: houseNumber.value,
    houseNumberAddition: houseNumberAddition.value || undefined,
    street: street.value,
    city: city.value,
    password: password.value,
    generalPractitionerId: selectedDoctor.value || ""
  })

  if (response.success && response.data) {
    // Registration successful, redirect to home
    router.push('/')
  } else {
    // Handle registration errors
    if (response.status === 400) {
      formError.value = response.message || 'Registratie mislukt. Controleer je gegevens.'
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
    <FieldGroup class="gap-4">
      <div class="flex flex-col items-center gap-1 text-center">
        <h1 class="text-2xl font-bold">
          Maak je account aan
        </h1>
        <p class="text-muted-foreground text-sm text-balance">
          Vul je gegevens in om je account te maken
        </p>
      </div>

      <div v-if="formError"
        class="text-sm text-destructive bg-destructive/10 border border-destructive/20 rounded-md p-3">
        {{ formError }}
      </div>

      <!-- Personal Information -->
      <FieldSeparator />
      <div class="text-sm font-bold">Persoonlijke gegevens</div>

      <Field>
        <FieldLabel for="firstName">
          Voornaam
        </FieldLabel>
        <Input id="firstName" type="text" placeholder="Jan" required v-model="firstName"
          @input="clearFieldError('firstName')" />
        <FieldError v-if="errors.firstName && errors.firstName[0]">{{ errors.firstName[0] }}</FieldError>
      </Field>
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <Field>
          <FieldLabel for="middleName">
            Tussenvoegsel
            <span class="text-muted-foreground font-normal text-xs">(Optioneel)</span>
          </FieldLabel>
          <Input id="middleName" type="text" placeholder="van der" v-model="middleName"
            @input="clearFieldError('middleName')" />
          <FieldError v-if="errors.middleName && errors.middleName[0]">{{ errors.middleName[0] }}</FieldError>
        </Field>
        <Field>
          <FieldLabel for="lastName">
            Achternaam
          </FieldLabel>
          <Input id="lastName" type="text" placeholder="Janssen" required v-model="lastName"
            @input="clearFieldError('lastName')" />
          <FieldError v-if="errors.lastName && errors.lastName[0]">{{ errors.lastName[0] }}</FieldError>
        </Field>
      </div>

      <Field>
        <FieldLabel for="email">
          E-mailadres
        </FieldLabel>
        <Input id="email" type="email" placeholder="jan@voorbeeld.com" required v-model="email"
          @input="clearFieldError('email')" />
        <FieldError v-if="errors.email && errors.email[0]">{{ errors.email[0] }}</FieldError>
      </Field>

      <Field>
        <FieldLabel for="phoneNumber">
          Telefoonnummer
        </FieldLabel>
        <Input id="phoneNumber" type="tel" placeholder="+31 6 12345678" required v-model="phoneNumber"
          @input="clearFieldError('phoneNumber')" />
        <FieldError v-if="errors.phoneNumber && errors.phoneNumber[0]">{{ errors.phoneNumber[0] }}</FieldError>
      </Field>

      <!-- Address Information -->
      <FieldSeparator />
      <div class="text-sm font-bold">Adresgegevens</div>

      <Field>
        <FieldLabel for="country">
          Land
        </FieldLabel>
        <Input id="country" type="text" required v-model="country" @input="clearFieldError('country')" />
        <FieldError v-if="errors.country && errors.country[0]">{{ errors.country[0] }}</FieldError>
      </Field>

      <Field>
        <FieldLabel for="zipCode">
          Postcode
        </FieldLabel>
        <Input id="zipCode" type="text" placeholder="1234 AB" required v-model="zipCode"
          @input="clearFieldError('zipCode')" />
        <FieldError v-if="errors.zipCode && errors.zipCode[0]">{{ errors.zipCode[0] }}</FieldError>
      </Field>

      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <Field>
          <FieldLabel for="houseNumber">
            Huisnummer
          </FieldLabel>
          <Input id="houseNumber" type="text" placeholder="123" required v-model="houseNumber"
            @input="clearFieldError('houseNumber')" />
          <FieldError v-if="errors.houseNumber && errors.houseNumber[0]">{{ errors.houseNumber[0] }}</FieldError>
        </Field>
        <Field>
          <FieldLabel for="houseNumberAddition">
            Toevoeging
            <span class="text-muted-foreground font-normal text-xs">(Optioneel)</span>
          </FieldLabel>
          <Input id="houseNumberAddition" type="text" v-model="houseNumberAddition"
            @input="clearFieldError('houseNumberAddition')" />
          <FieldError v-if="errors.houseNumberAddition && errors.houseNumberAddition[0]">{{
            errors.houseNumberAddition[0] }}</FieldError>
        </Field>
      </div>

      <Field class="md:col-span-2">
        <FieldLabel for="street">
          Straatnaam
        </FieldLabel>
        <Input id="street" type="text" placeholder="Hoofdstraat" required v-model="street"
          @input="clearFieldError('street')" />
        <FieldError v-if="errors.street && errors.street[0]">{{ errors.street[0] }}</FieldError>
      </Field>

      <Field>
        <FieldLabel for="city">
          Plaats
        </FieldLabel>
        <Input id="city" type="text" placeholder="Amsterdam" required v-model="city" @input="clearFieldError('city')" />
        <FieldError v-if="errors.city && errors.city[0]">{{ errors.city[0] }}</FieldError>
      </Field>

      <!-- Doctor Selection -->
      <FieldSeparator />
      <div class="text-sm font-bold">Huisartsenpraktijk</div>

      <Field>
        <FieldLabel>Selecteer je huisarts</FieldLabel>
        <Combobox v-model="selectedDoctor" :options="gpOptions" placeholder="Selecteer een huisarts..."
          search-placeholder="Zoek een huisarts..." empty-message="Geen huisarts gevonden."
          @update:model-value="clearFieldError('selectedDoctor')" :disabled="loading" />
        <FieldError v-if="errors.selectedDoctor && errors.selectedDoctor[0]">{{ errors.selectedDoctor[0] }}</FieldError>
        <FieldDescription v-else>
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
        <Input id="password" type="password" required v-model="password" @input="clearFieldError('password')" />
        <FieldError v-if="errors.password && errors.password[0]">{{ errors.password[0] }}</FieldError>
        <FieldDescription v-else class="space-y-1">
          <span class="block">Wachtwoord moet bevatten:</span>
          <ul class="list-disc list-inside space-y-0.5 ml-2">
            <li>Minimaal 6 karakters</li>
            <li>Minimaal één hoofdletter</li>
            <li>Minimaal één kleine letter</li>
            <li>Minimaal één cijfer</li>
          </ul>
        </FieldDescription>
      </Field>
      <Field>
        <FieldLabel for="confirmPassword">
          Bevestig wachtwoord
        </FieldLabel>
        <Input id="confirmPassword" type="password" required v-model="confirmPassword"
          @input="clearFieldError('confirmPassword')" />
        <FieldError v-if="errors.confirmPassword && errors.confirmPassword[0]">{{ errors.confirmPassword[0] }}
        </FieldError>
        <FieldDescription v-else>Bevestig je wachtwoord.</FieldDescription>
      </Field>

      <Field>
        <Button type="submit" class="w-full" :disabled="isLoading">
          {{ isLoading ? 'Account aanmaken...' : 'Account aanmaken' }}
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
