<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { usePatientReferrals } from '@/composables/useReferral'
import { useSpecialists } from '@/composables/useSpecialists'
import { useCreateAppointment } from '@/composables/useCreateAppointment'
import { Card, CardHeader, CardTitle, CardContent, CardDescription } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import {
  Field,
  FieldLabel,
  FieldError,
  FieldGroup
} from '@/components/ui/field'
import { Combobox } from '@/components/ui/combobox'
import type { ComboboxOption } from '@/components/ui/combobox/Combobox.vue'
import { Calendar, Clock, ArrowLeft, Check } from 'lucide-vue-next'
import { z } from 'zod'

const router = useRouter()
const { data: referrals, loading: loadingReferrals, error: referralsError } = usePatientReferrals()
const { data: specialists, loading: loadingSpecialists, error: specialistsError } = useSpecialists()
const createAppointment = useCreateAppointment()

const referralId = ref('')
const specialistId = ref('')
const date = ref('')
const time = ref('')
const success = ref(false)
const errors = ref<Record<string, string[]>>({})
const formError = ref<string | null>(null)

const referralOptions = computed<ComboboxOption[]>(() =>
  (referrals.value ?? []).map(r => ({
    value: String(r.id),
    label: r.treatmentName
  }))
)

const specialistOptions = computed<ComboboxOption[]>(() =>
  (specialists.value ?? []).map(s => ({
    value: String(s.id),
    label: s.fullName
  }))
)

const appointmentSchema = z.object({
  referralId: z.string().min(1, "Selecteer een doorverwijzing"),
  specialistId: z.string().min(1, "Selecteer een specialist"),
  date: z.string().min(1, "Datum is verplicht"),
  time: z.string().min(1, "Tijd is verplicht"),
}).refine((data) => {
  if (!data.date || !data.time) return true // Let required validation handle empty fields

  const appointmentDateTime = new Date(`${data.date}T${data.time}`)
  const now = new Date()

  return appointmentDateTime > now
}, {
  message: "De datum en tijd van de afspraak moeten in de toekomst zijn",
  path: ["time"], // Show error on date field
})

const clearFieldError = (field: string) => {
  if (errors.value?.[field]) {
    delete errors.value[field]
  }
  if (formError.value) {
    formError.value = null
  }
}

const submit = async () => {
  success.value = false
  errors.value = {}
  formError.value = null

  const result = appointmentSchema.safeParse({
    referralId: referralId.value,
    specialistId: specialistId.value,
    date: date.value,
    time: time.value
  })

  if (!result.success) {
    errors.value = result.error.flatten().fieldErrors as Record<string, string[]>
    return
  }

  try {
    await createAppointment.mutate({
      referralId: Number(referralId.value),
      specialistId: Number(specialistId.value),
      date: date.value,
      time: time.value
    })

    success.value = true

    // Reset form
    referralId.value = ''
    specialistId.value = ''
    date.value = ''
    time.value = ''

    // Redirect after a short delay
    setTimeout(() => {
      router.push('/afspraken')
    }, 2000)
  } catch (e: any) {
    formError.value = e.message || 'Er is een fout opgetreden bij het aanmaken van de afspraak'
  }
}

</script>

<template>
  <div class="container mx-auto p-6 max-w-2xl space-y-6">
    <div class="flex items-center gap-4">
      <Button variant="ghost" size="icon" @click="router.push('/afspraken')">
        <ArrowLeft class="h-5 w-5" />
      </Button>
      <div>
        <h1 class="text-3xl font-bold">Nieuwe Afspraak Maken</h1>
        <p class="text-muted-foreground mt-1">Plan een afspraak met een specialist</p>
      </div>
    </div>

    <!-- Success Message -->
    <div v-if="success" class="flex gap-3 p-4 bg-green-50 border border-green-200 rounded-lg text-green-800">
      <Check class="h-5 w-5 shrink-0 mt-0.5" />
      <div class="text-sm">
        <strong class="font-semibold">Afspraak succesvol aangemaakt!</strong>
        <p class="mt-1">Je wordt doorgestuurd naar het overzicht...</p>
      </div>
    </div>

    <!-- Loading State -->
    <Card v-if="loadingReferrals || loadingSpecialists">
      <CardContent class="pt-6">
        <div class="flex flex-col items-center justify-center py-8 text-muted-foreground">
          <Clock class="h-12 w-12 mb-4 animate-pulse opacity-50" />
          <p class="font-medium">Gegevens laden...</p>
        </div>
      </CardContent>
    </Card>

    <!-- Error State -->
    <Card v-else-if="referralsError || specialistsError">
      <CardContent class="pt-6">
        <div class="text-sm text-destructive bg-destructive/10 border border-destructive/20 rounded-md p-4">
          Sommige gegevens konden niet worden geladen. Probeer het opnieuw.
        </div>
      </CardContent>
    </Card>

    <!-- Form -->
    <Card v-else>
      <CardHeader>
        <CardTitle class="flex items-center gap-2">
          <Calendar class="h-5 w-5" />
          Afspraak Details
        </CardTitle>
        <CardDescription>
          Vul de onderstaande gegevens in om een afspraak te maken
        </CardDescription>
      </CardHeader>
      <CardContent>
        <form @submit.prevent="submit" class="space-y-6">
          <FieldGroup>
            <!-- Form Error -->
            <div v-if="formError"
              class="text-sm text-destructive bg-destructive/10 border border-destructive/20 rounded-md p-3">
              {{ formError }}
            </div>

            <!-- Referral Selection -->
            <Field>
              <FieldLabel for="referral">Doorverwijzing *</FieldLabel>
              <Combobox v-model="referralId" :options="referralOptions" placeholder="Selecteer een doorverwijzing..."
                search-placeholder="Zoek doorverwijzing..." empty-message="Geen doorverwijzingen gevonden"
                @update:model-value="clearFieldError('referralId')" />
              <FieldError v-if="errors.referralId">{{ errors.referralId[0] }}</FieldError>
            </Field>

            <!-- Specialist Selection -->
            <Field>
              <FieldLabel for="specialist">Specialist *</FieldLabel>
              <Combobox v-model="specialistId" :options="specialistOptions" placeholder="Selecteer een specialist..."
                search-placeholder="Zoek specialist..." empty-message="Geen specialisten gevonden"
                @update:model-value="clearFieldError('specialistId')" />
              <FieldError v-if="errors.specialistId">{{ errors.specialistId[0] }}</FieldError>
            </Field>

            <!-- Date Input -->
            <Field>
              <FieldLabel for="date">Datum *</FieldLabel>
              <Input id="date" type="date" v-model="date" @input="clearFieldError('date')" />
              <FieldError v-if="errors.date">{{ errors.date[0] }}</FieldError>
            </Field>

            <!-- Time Input -->
            <Field>
              <FieldLabel for="time">Tijd *</FieldLabel>
              <Input id="time" type="time" v-model="time" @input="clearFieldError('time')" />
              <FieldError v-if="errors.time">{{ errors.time[0] }}</FieldError>
            </Field>

            <!-- Submit Button -->
            <div class="flex gap-3 pt-4">
              <Button type="submit" :disabled="createAppointment.loading.value" class="flex-1">
                <Calendar class="h-4 w-4 mr-2" />
                {{ createAppointment.loading.value ? 'Opslaan...' : 'Afspraak Maken' }}
              </Button>
              <Button type="button" variant="outline" @click="router.push('/afspraken')"
                :disabled="createAppointment.loading.value">
                Annuleren
              </Button>
            </div>
          </FieldGroup>
        </form>
      </CardContent>
    </Card>
  </div>
</template>
