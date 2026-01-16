<script setup lang="ts">
import { ref, computed } from 'vue'
import { useAuth } from '@/composables/useAuth'
import { usePatientReferrals } from '@/composables/useReferral'
import { useAppointment } from '@/composables/useAppointments'
import { useNotifications } from '@/composables/useNotifications'
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card'
import { Calendar, Bell, FileText, Clock, AlertCircle } from 'lucide-vue-next'
import type { AppointmentDto } from '@/types/appointment'
import type { Referral } from '@/types/referral'
import type { AxiosError } from 'axios'
import { getFullNameFromUser } from '@/lib/utils'


const { getStoredUser, hasRole } = useAuth()

// 🔹 role
const isPatient = computed(() => hasRole('Patient'))

// Only initialize composables when patient
const referralQuery = isPatient.value ? usePatientReferrals() : null
const appointmentQuery = isPatient.value ? useAppointment() : null
const notificationsQuery = isPatient.value ? useNotifications() : null

// 🔹 expose reactive data safely
const referrals = computed(() => referralQuery?.data.value ?? [])
const referralsLoading = computed(() => referralQuery?.loading.value ?? false)
const referralsError = computed(() => referralQuery?.error.value ?? null)

const appointments = computed(() => appointmentQuery?.appointments.value ?? [])
const nextAppointment = computed(() => appointmentQuery?.nextAppointment.value ?? null)
const appointmentsLoading = computed(() => appointmentQuery?.loading.value ?? false)
const appointmentsError = computed(() => appointmentQuery?.error.value ?? null)

const notifications = computed(() => notificationsQuery?.data.value ?? [])
const notificationsLoading = computed(() => notificationsQuery?.loading.value ?? false)
const notificationsError = computed(() => notificationsQuery?.error.value ?? null)


// 🔹 local UI state
const appointmentExpanded = ref(false)
const expandedReferralId = ref<number | null>(null)
const expandedAppointmentId = ref<number | null>(null)
const expandedAppointmentKey = ref<string | null>(null)
const getAppointmentKey = (a: AppointmentDto) =>
  `${a.referralId}-${a.date}`

const toggleAppointmentByKey = (key: string) => {
  expandedAppointmentKey.value =
    expandedAppointmentKey.value === key ? null : key
}


const toggleAppointment = () => {
  appointmentExpanded.value = !appointmentExpanded.value
}
const toggleAppointmentById = (id: number) => {
  expandedAppointmentId.value =
    expandedAppointmentId.value === id ? null : id
}
const toggleReferral = (id: number) => {
  expandedReferralId.value =
    expandedReferralId.value === id ? null : id
}
// 🔹 user
const user = getStoredUser()

const userName = computed(() => {
  if (!user) return 'Gebruiker'

  return getFullNameFromUser(user);
})

// 🔹 veilige computed voor template
const safeReferrals = computed(() => referrals.value ?? [])

// 🔹 reload page function
const reloadPage = () => {
  window.location.reload()
}
</script>

<template>
  <div class="container mx-auto p-6 space-y-6" role="main" aria-label="Dashboard overzicht">

    <!-- Dashboard Title -->
    <div>
      <h1 class="text-3xl font-bold">Dashboard</h1>
    </div>

    <!-- ❌ Foutmelding -->
    <Card v-if="referralsError" class="border-destructive" role="alert" aria-live="assertive">
      <CardContent class="pt-6">
        <p class="text-destructive mb-4">{{ referralsError }}</p>
        <button class="px-4 py-2 bg-destructive text-destructive-foreground rounded-md hover:bg-destructive/90"
          @click="reloadPage()" aria-label="Probeer dashboard opnieuw te laden">
          Opnieuw proberen
        </button>
      </CardContent>
    </Card>

    <!-- ⏳ Loading -->
    <div v-else-if="referralsLoading" role="status" aria-live="polite">
      Dashboard wordt geladen...
    </div>

    <!-- ✔ Dashboard content -->
    <div v-else class="space-y-6">

      <!-- Welkomsbericht -->
      <Card aria-label="Welkomsbericht">
        <CardHeader>
          <CardTitle class="text-2xl">Welkom, {{ userName }}</CardTitle>
        </CardHeader>
      </Card>

      <!-- Alleen Patiënten -->
      <template v-if="isPatient">

        <!-- Two-column layout matching wireframe -->
        <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">

          <!-- Left column: Afspraken (spans 2 columns) -->
          <Card class="lg:col-span-2 flex flex-col" aria-label="Afspraken-overzicht">
            <CardHeader>
              <CardTitle class="flex items-center gap-2">
                <Calendar class="h-5 w-5" />
                Afspraken
              </CardTitle>
            </CardHeader>
            <CardContent class="flex-1 min-h-[500px]">
              <!-- Loading -->
              <div v-if="appointmentsLoading" class="flex items-center justify-center h-full text-muted-foreground">
                <div class="text-center space-y-2">
                  <Clock class="h-8 w-8 mx-auto animate-pulse" />
                  <p>Wordt geladen...</p>
                </div>
              </div>

              <!-- Error -->
              <div v-else-if="appointmentsError" class="flex items-center justify-center h-full text-destructive">
                <div class="text-center space-y-2">
                  <AlertCircle class="h-8 w-8 mx-auto" />
                  <p>{{ appointmentsError }}</p>
                </div>
              </div>

              <!-- Afspraken lijst -->
              <div v-else class="space-y-4">
                <!-- Eerstvolgende afspraak -->
                <div v-if="nextAppointment"
                  class="p-4 border rounded-lg hover:bg-accent cursor-pointer transition-colors" role="button"
                  tabindex="0" aria-controls="next-appointment-details" :aria-expanded="appointmentExpanded"
                  @click="toggleAppointment" @keydown.enter="toggleAppointment"
                  @keydown.space.prevent="toggleAppointment">
                  <div class="space-y-2">
                    <p class="font-semibold">{{ nextAppointment.treatmentDescription }}</p>
                    <p class="text-sm text-muted-foreground">
                      {{ new Date(nextAppointment.date).toLocaleString('nl-NL') }}
                    </p>

                    <!-- Uitklapbare details -->
                    <div v-if="appointmentExpanded" id="next-appointment-details" role="region"
                      aria-label="Details van de afspraak" class="mt-4 pt-4 border-t space-y-2">
                      <p class="font-semibold text-sm">Instructies</p>
                      <p class="text-sm text-muted-foreground">
                        {{ nextAppointment.treatmentInstructions || 'Geen instructies.' }}
                      </p>
                    </div>
                  </div>
                </div>

                <!-- Andere afspraken -->
                <div v-for="a in appointments
                  .filter(ap =>
                    ap.referralId !== nextAppointment?.referralId &&
                    ap.status !== 'AccessDenied' &&
                    ap.status !== 'Cancelled'
                  )" :key="getAppointmentKey(a)"
                  class="p-4 border rounded-lg hover:bg-accent cursor-pointer transition-colors" role="button"
                  tabindex="0" :aria-expanded="expandedAppointmentKey === getAppointmentKey(a)"
                  :aria-controls="`appointment-${getAppointmentKey(a)}`"
                  @click="toggleAppointmentByKey(getAppointmentKey(a))"
                  @keydown.enter="toggleAppointmentByKey(getAppointmentKey(a))"
                  @keydown.space.prevent="toggleAppointmentByKey(getAppointmentKey(a))">
                  <div class="space-y-2">
                    <p class="font-semibold">{{ a.treatmentDescription }}</p>
                    <p class="text-sm text-muted-foreground">
                      {{ new Date(a.date).toLocaleString('nl-NL') }}
                    </p>

                    <div v-if="expandedAppointmentKey === getAppointmentKey(a)"
                      :id="`appointment-${getAppointmentKey(a)}`" role="region" aria-label="Details van afspraak"
                      class="mt-4 pt-4 border-t space-y-2">
                      <p class="font-semibold text-sm">Instructies</p>
                      <p class="text-sm text-muted-foreground">
                        {{ a.treatmentInstructions || 'Geen instructies.' }}
                      </p>
                    </div>
                  </div>
                </div>

                <!-- Geen afspraken -->
                <div v-if="!nextAppointment && appointments.length === 0"
                  class="flex items-center justify-center h-full text-muted-foreground">
                  <div class="text-center space-y-3">
                    <Calendar class="h-12 w-12 mx-auto opacity-50" />
                    <p class="font-medium">Geen geplande afspraken</p>
                    <p class="text-sm">Je hebt momenteel geen afspraken staan.</p>
                  </div>
                </div>
              </div>
            </CardContent>
          </Card>

          <!-- Right column: Meldingen + Doorverwijzingen -->
          <div class="space-y-6 flex flex-col">

            <!-- Meldingen -->
            <Card class="flex flex-col flex-1" aria-label="Meldingen-overzicht">
              <CardHeader>
                <CardTitle class="flex items-center gap-2">
                  <Bell class="h-5 w-5" />
                  Meldingen
                </CardTitle>
              </CardHeader>
              <CardContent class="flex-1 min-h-[200px]">
                <!-- Loading -->
                <div v-if="notificationsLoading" class="flex items-center justify-center h-full text-muted-foreground">
                  <div class="text-center space-y-2">
                    <Clock class="h-6 w-6 mx-auto animate-pulse" />
                    <p class="text-sm">Wordt geladen...</p>
                  </div>
                </div>

                <!-- Error -->
                <div v-else-if="notificationsError" class="flex items-center justify-center h-full text-destructive">
                  <div class="text-center space-y-2">
                    <AlertCircle class="h-6 w-6 mx-auto" />
                    <p class="text-sm">{{ notificationsError }}</p>
                  </div>
                </div>

                <!-- Meldingen lijst -->
                <div v-else-if="notifications.length > 0" class="space-y-3">
                  <div v-for="notification in notifications.slice(0, 5)" :key="notification.id"
                    class="p-3 border rounded-md hover:bg-accent transition-colors">
                    <p class="text-sm font-medium">{{ notification.message }}</p>
                    <p class="text-xs text-muted-foreground mt-1">
                      {{ new Date(notification.createdAt).toLocaleDateString('nl-NL') }}
                    </p>
                  </div>
                </div>

                <!-- Geen meldingen -->
                <div v-else class="flex items-center justify-center h-full text-muted-foreground">
                  <div class="text-center space-y-2">
                    <Bell class="h-8 w-8 mx-auto opacity-50" />
                    <p class="text-sm font-medium">Geen nieuwe meldingen</p>
                  </div>
                </div>
              </CardContent>
            </Card>

            <!-- Doorverwijzingen -->
            <Card class="flex flex-col flex-1" aria-label="Doorverwijzingen-overzicht">
              <CardHeader>
                <CardTitle class="flex items-center gap-2">
                  <FileText class="h-5 w-5" />
                  Doorverwijzingen
                </CardTitle>
              </CardHeader>
              <CardContent class="flex-1 min-h-[200px]">
                <div v-if="safeReferrals.length > 0" class="space-y-3">
                  <div v-for="ref in safeReferrals.slice(0, 5)" :key="ref.id"
                    class="p-3 border rounded-md hover:bg-accent cursor-pointer transition-colors" role="button"
                    tabindex="0" :aria-expanded="expandedReferralId === ref.id" :aria-controls="`referral-${ref.id}`"
                    @click="toggleReferral(ref.id)" @keydown.enter="toggleReferral(ref.id)"
                    @keydown.space.prevent="toggleReferral(ref.id)">
                    <p class="text-sm font-medium">{{ ref.treatmentName }}</p>
                    <p class="text-xs text-muted-foreground">{{ ref.status }}</p>

                    <div v-if="expandedReferralId === ref.id" :id="`referral-${ref.id}`" role="region"
                      aria-label="Details van doorverwijzing" class="mt-3 pt-3 border-t space-y-1">
                      <p class="text-xs text-muted-foreground">
                        Aangemaakt: {{ new Date(ref.createdAt).toLocaleString('nl-NL') }}
                      </p>
                    </div>
                  </div>
                </div>

                <!-- Geen verwijzingen -->
                <div v-else class="flex items-center justify-center h-full text-muted-foreground">
                  <div class="text-center space-y-2">
                    <FileText class="h-8 w-8 mx-auto opacity-50" />
                    <p class="text-sm font-medium">Geen doorverwijzingen</p>
                  </div>
                </div>
              </CardContent>
            </Card>

          </div>

        </div>

      </template>

      <!-- Medewerkers -->
      <template v-else>
        <Card aria-label="Dashboard-medewerker-overzicht">
          <CardContent class="pt-6">
            <p class="text-muted-foreground text-center">
              Dit is het dashboard voor medewerkers.
            </p>
          </CardContent>
        </Card>
      </template>

    </div>
  </div>
</template>
