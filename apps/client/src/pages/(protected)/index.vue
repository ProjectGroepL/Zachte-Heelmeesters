<script setup lang="ts">
import { ref, computed } from 'vue'
import { useAuth } from '@/composables/useAuth'
import { usePatientReferrals } from '@/composables/useReferral'
import { useAppointment } from '@/composables/useAppointments'
import type { AppointmentDto } from '@/types/appointment'
import type { Referral } from '@/types/referral'
import type { AxiosError } from 'axios'


const { getStoredUser, hasRole } = useAuth()

// 🔹 role
const isPatient = computed(() => hasRole('Patient'))

// Only initialize composables when patient
const referralQuery = isPatient.value ? usePatientReferrals() : null
const appointmentQuery = isPatient.value ? useAppointment() : null

// 🔹 expose reactive data safely
const referrals = computed(() => referralQuery?.data.value ?? [])
const referralsLoading = computed(() => referralQuery?.loading.value ?? false)
const referralsError = computed(() => referralQuery?.error.value ?? null)

const appointments = computed(() => appointmentQuery?.appointments.value ?? [])
const nextAppointment = computed(() => appointmentQuery?.nextAppointment.value ?? null)
const appointmentsLoading = computed(() => appointmentQuery?.loading.value ?? false)
const appointmentsError = computed(() => appointmentQuery?.error.value ?? null)


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
  if (user.firstName && user.lastName) {
    return `${user.firstName} ${user.lastName}`
  }
  return user.firstName ?? 'Gebruiker'
})

// 🔹 veilige computed voor template
const safeReferrals = computed(() => referrals.value ?? [])

// 🔹 reload page function
const reloadPage = () => {
  window.location.reload()
}
</script>

<template>
  <div class="flex flex-col items-center space-y-4" role="main" aria-label="Dashboard overzicht">

    <!-- ❌ Foutmelding -->
    <div v-if="referralsError" class="p-4 bg-red-100 text-red-700 rounded-xl" role="alert" aria-live="assertive">
      {{ referralsError }}

      <button class="mt-2 px-4 py-2 bg-red-600 text-white rounded" @click="reloadPage()"
        aria-label="Probeer dashboard opnieuw te laden">
        Opnieuw proberen
      </button>
    </div>

    <!-- ⏳ Loading -->
    <div v-else-if="referralsLoading" role="status" aria-live="polite">
      Dashboard wordt geladen...
    </div>

    <!-- ✔ Dashboard content -->
    <div v-else class="w-full flex flex-col items-center">

      <!-- 🌟 WELKOMSBERICHT (GROOT EN VOLLEDIGE BREEDTE) -->
      <article aria-label="Welkomsbericht" class="w-full flex justify-center mt-6">
        <div class="w-full max-w-4xl p-8 bg-white rounded-2xl shadow-lg" role="region" aria-labelledby="welcome-title">
          <h2 id="welcome-title" class="text-3xl font-bold text-blue-600">
            Welkom terug,
          </h2>
          <p class="text-gray-700 text-xl mt-2 font-medium">{{ userName }}</p>
        </div>
      </article>

      <!-- TUSSENRUIMTE -->
      <div class="h-10"></div>

      <!-- Alleen Patiënten -->
      <template v-if="isPatient">

        <!-- AFSRPRAKEN IN GRID (3 op een rij) -->
        <section class="w-full max-w-6xl px-4" aria-label="Afspraken-overzicht">

          <h2 class="text-2xl font-bold text-blue-600 mb-4">
            Jouw afspraken
          </h2>

          <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">

            <!-- Één afspraak-kaart -->
            <!-- Één afspraak-kaart -->
            <div class="p-6 bg-white rounded-2xl shadow hover:shadow-xl transition cursor-pointer" role="button"
              tabindex="0" aria-controls="next-appointment-details" :aria-expanded="appointmentExpanded"
              @click="toggleAppointment" @keydown.enter="toggleAppointment" @keydown.space.prevent="toggleAppointment"
              :class="appointmentExpanded ? 'ring-2 ring-blue-400' : ''">
              <h3 class="text-xl font-semibold text-blue-600">
                Eerstvolgende afspraak
              </h3>

              <p v-if="appointmentsLoading" class="text-gray-500 mt-2">
                Wordt geladen...
              </p>

              <p v-if="appointmentsError" class="text-red-500 mt-2">
                {{ appointmentsError }}
              </p>

              <template v-if="nextAppointment">
                <p class="text-gray-700 mt-2 font-medium">
                  {{ nextAppointment.treatmentDescription }}
                </p>
                <p class="text-gray-500">
                  Datum: {{ new Date(nextAppointment.date).toLocaleString('nl-NL') }}
                </p>

                <div v-if="appointmentExpanded" id="next-appointment-details" role="region"
                  aria-label="Details van de afspraak" class="mt-4 border-t pt-3 text-gray-700 space-y-2 text-sm">
                  <p class="font-bold text-blue-500">Instructies</p>
                  <p>{{ nextAppointment.treatmentInstructions || 'Geen instructies.' }}</p>
                  <p class="text-xs italic text-gray-400">
                    Druk op Enter of Spatie om te sluiten.
                  </p>
                </div>

                <!-- ✅ Add the link here for nextAppointment -->
                <router-link :to="{ name: '/(protected)/afspraken/[id]', params: { id: nextAppointment.id } }"
                  class="mt-4 inline-block text-blue-500 hover:text-blue-700">
                  Bekijk details
                </router-link>
              </template>

              <template v-else>
                <p class="text-gray-400 mt-2">Geen geplande afspraken.</p>
              </template>
            </div>


            <div v-for="a in appointments
              .filter(ap =>
                ap.referralId !== nextAppointment?.referralId &&
                ap.status !== 'AccessDenied' &&
                ap.status !== 'Cancelled'
              )" :key="getAppointmentKey(a)" role="button" tabindex="0"
              :aria-expanded="expandedAppointmentKey === getAppointmentKey(a)"
              :aria-controls="`appointment-${getAppointmentKey(a)}`"
              @click="toggleAppointmentByKey(getAppointmentKey(a))"
              @keydown.enter="toggleAppointmentByKey(getAppointmentKey(a))"
              @keydown.space.prevent="toggleAppointmentByKey(getAppointmentKey(a))"
              class="p-6 bg-white rounded-2xl shadow hover:shadow-xl transition cursor-pointer"
              :class="expandedAppointmentKey === getAppointmentKey(a) ? 'ring-2 ring-blue-400' : ''">
              <h3 class="text-xl font-semibold text-blue-600">
                Afspraak
              </h3>

              <p class="text-gray-700 mt-2 font-medium">
                {{ a.treatmentDescription }}
              </p>

              <p class="text-gray-500">
                Datum: {{ new Date(a.date).toLocaleString('nl-NL') }}
              </p>

              <div v-if="expandedAppointmentKey === getAppointmentKey(a)" :id="`appointment-${getAppointmentKey(a)}`"
                role="region" aria-label="Details van afspraak"
                class="mt-4 border-t pt-3 text-sm text-gray-700 space-y-2">
                <p class="font-semibold text-blue-500">Instructies</p>
                <p>{{ a.treatmentInstructions || 'Geen instructies.' }}</p>
              </div>

              <!-- ✅ Link for this specific appointment -->
              <router-link :to="{ name: '/(protected)/afspraken/[id]', params: { id: a.id } }"
                class="mt-4 inline-block text-blue-500 hover:text-blue-700">
                Bekijk details
              </router-link>
            </div>

          </div>
        </section>

        <!-- EXTRA RUIMTE -->
        <div class="h-16"></div>

        <!-- 📄 DOORVERWIJZINGEN IN GRID -->
        <section class="w-full max-w-6xl px-4" aria-label="Doorverwijzingen-overzicht">

          <h2 class="text-2xl font-bold text-blue-600 mb-4">
            Doorverwijzingen
          </h2>

          <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">

            <!-- Kaart per verwijzing --> <!-- maakt een modal dat je er altijd invlijft misschien met modal strap.-->
            <div v-for="ref in safeReferrals" :key="ref.id" role="button" tabindex="0"
              :aria-expanded="expandedReferralId === ref.id" :aria-controls="`referral-${ref.id}`"
              @click="toggleReferral(ref.id)" @keydown.enter="toggleReferral(ref.id)"
              @keydown.space.prevent="toggleReferral(ref.id)"
              class="p-6 bg-white rounded-2xl shadow hover:shadow-xl transition cursor-pointer"
              :class="expandedReferralId === ref.id ? 'ring-2 ring-blue-400' : ''">
              <h3 class="text-xl font-semibold text-blue-600">
                {{ ref.treatmentName }}
              </h3>

              <p class="text-gray-700 mt-1 font-medium">
                Patiënt: {{ ref.patientName }}
              </p>

              <p class="text-gray-500 text-sm">
                Status: {{ ref.status }}
              </p>

              <div v-if="expandedReferralId === ref.id" :id="`referral-${ref.id}`" role="region"
                aria-label="Details van doorverwijzing" class="mt-4 border-t pt-3 text-sm text-gray-700 space-y-2">
                <p class="font-semibold text-blue-500">Aangemaakt op</p>
                <p>{{ new Date(ref.createdAt).toLocaleString('nl-NL') }}</p>
              </div>
            </div>

            <!-- Geen verwijzingen -->
            <div v-if="safeReferrals.length === 0"
              class="col-span-full p-8 text-center bg-gray-50 border border-gray-200 rounded-2xl text-gray-500">
              Geen doorverwijzingen.
            </div>

          </div>
        </section>

      </template>

      <!-- Medewerkers -->
      <template v-else>
        <div class="mt-6 text-center text-gray-500" aria-label="Dashboard-medewerker-overzicht">
          Dit is het dashboard voor medewerkers.
        </div>
      </template>

    </div>
  </div>
</template>
