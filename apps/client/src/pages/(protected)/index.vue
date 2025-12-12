<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useAuth } from '@/composables/useAuth';
import { useReferral } from '@/composables/userReferral';
import { useAppointment } from '@/composables/useAppointments';
import type { Referral } from '@/types/referral';

const { getStoredUser, hasRole } = useAuth();
const { getReferrals } = useReferral();
const {
  fetchAppointments,
  nextAppointment,
  loading: appointmentsLoading,
  error: appointmentsError,
  appointments
} = useAppointment();

const userName = ref('');
const referrals = ref<Referral[]>([]);
const loadingDashboard = ref(false);
const dashboardError = ref<string | null>(null);
const appointmentExpanded = ref(false)
const user = getStoredUser();
const expandedReferralId = ref<number | null>(null);

const toggleReferral = (id: number) => {
  expandedReferralId.value = expandedReferralId.value === id ? null : id;
};
// Rol-checks
const isPatient = computed(() => hasRole('Patient'))

const reloadPage = () => {
  window.location.reload();
};

onMounted(async () => {
  console.log("🚀 Dashboard mounted()");
  loadingDashboard.value = true;
  dashboardError.value = null;

  try {
    if (user?.firstName && user?.lastName) {
      userName.value = `${user.firstName} ${user.lastName}`;
      console.log("➡ User:", user);
    } else {
      userName.value = user?.firstName || "Gebruiker";
    }

    const result = await getReferrals();
    console.log("➡ Referrals received:", result);
    await fetchAppointments();
    referrals.value = result;
    console.log("➡ Stored referrals:", referrals.value)
  } catch (e) {
    dashboardError.value = "Dashboard kan niet geladen worden.";
  } finally {
    loadingDashboard.value = false;
  }
});
</script>

<template>
  <div 
    class="flex flex-col items-center space-y-4"
    role="main"
    aria-label="Dashboard overzicht"
  >

    <!-- ❌ Foutmelding -->
    <div 
      v-if="dashboardError" 
      class="p-4 bg-red-100 text-red-700 rounded-xl"
      role="alert"
      aria-live="assertive"
    >
      {{ dashboardError }}

      <button 
        class="mt-2 px-4 py-2 bg-red-600 text-white rounded"
        @click="reloadPage()"
        aria-label="Probeer dashboard opnieuw te laden"
      >
        Opnieuw proberen
      </button>
    </div>

    <!-- ⏳ Loading -->
    <div 
      v-else-if="loadingDashboard"
      role="status"
      aria-live="polite"
    >
      Dashboard wordt geladen...
    </div>

    <!-- ✔ Dashboard content -->
    <div v-else class="w-full flex flex-col items-center">

      <!-- 🌟 WELKOMSBERICHT (GROOT EN VOLLEDIGE BREEDTE) -->
      <article aria-label="Welkomsbericht" class="w-full flex justify-center mt-6">
        <div 
          class="w-full max-w-4xl p-8 bg-white rounded-2xl shadow-lg"
          role="region"
          aria-labelledby="welcome-title"
        >
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

        <!-- 📅 AFSRPRAKEN IN GRID (3 op een rij) -->
        <section class="w-full max-w-6xl px-4" aria-label="Afspraken-overzicht">

          <h2 class="text-2xl font-bold text-blue-600 mb-4">
            Jouw afspraken
          </h2>

          <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">

            <!-- Één afspraak-kaart -->
            <div 
              class="p-6 bg-white rounded-2xl shadow hover:shadow-xl transition cursor-pointer"
              @click="appointmentExpanded = !appointmentExpanded"
              :class="appointmentExpanded ? 'ring-2 ring-blue-400' : ''"
            >
              <h3 class="text-xl font-semibold text-blue-600">
                Eerstvolgende afspraak
              </h3>

              <!-- Loading -->
              <p v-if="appointmentsLoading" class="text-gray-500 mt-2">
                Wordt geladen...
              </p>

              <!-- Error -->
              <p v-if="appointmentsError" class="text-red-500 mt-2">
                {{ appointmentsError }}
              </p>

              <!-- Inhoud -->
              <template v-if="nextAppointment">
                <p class="text-gray-700 mt-2 font-medium">
                  {{ nextAppointment.treatmentDescription }}
                </p>
                <p class="text-gray-500">
                  Datum: {{ new Date(nextAppointment.date).toLocaleString('nl-NL') }}
                </p>

                <!-- Uitklapbaar -->
                <div 
                  v-if="appointmentExpanded"
                  class="mt-4 border-t pt-3 text-gray-700 space-y-2 text-sm"
                >
                  <p class="font-bold text-blue-500">Instructies</p>
                  <p>{{ nextAppointment.treatmentInstructions || "Geen instructies." }}</p>

                  <p class="text-xs italic text-gray-400">Klik opnieuw om te sluiten.</p>
                </div>
              </template>

              <!-- Geen afspraak -->
              <template v-else>
                <p class="text-gray-400 mt-2">
                  Geen geplande afspraken.
                </p>
              </template>

            </div>

            <div
              :key="a.referralId"
              v-for="a in appointments.filter(ap => ap.referralId !== nextAppointment?.referralId)"
              class="p-6 bg-white rounded-2xl shadow hover:shadow-xl transition"
            >
              <h3 class="text-xl font-semibold text-blue-600">
                Afspraak
              </h3>

              <p class="text-gray-700 mt-2 font-medium">
                {{ a.treatmentDescription }}
              </p>

              <p class="text-gray-500">
                Datum: {{ new Date(a.date).toLocaleString('nl-NL') }}
              </p>
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

            <!-- Kaart per verwijzing -->
            <div
              v-for="ref in referrals"
              :key="ref.id"
              @click="toggleReferral(ref.id)"
              class="p-6 bg-white rounded-2xl shadow hover:shadow-xl transition cursor-pointer"
              :class="expandedReferralId === ref.id ? 'ring-2 ring-blue-400' : ''"
            >
              <h3 class="text-xl font-semibold text-blue-600">
                {{ ref.treatmentName }}
              </h3>

              <p class="text-gray-700 mt-1 font-medium">
                Patiënt: {{ ref.patientName }}
              </p>

              <p class="text-gray-500 text-sm">
                Status: {{ ref.status }}
              </p>

              <!-- Uitklap -->
              <div 
                v-if="expandedReferralId === ref.id"
                class="mt-4 border-t pt-3 text-sm text-gray-700 space-y-2"
              >
                <p class="font-semibold text-blue-500">Aangemaakt op</p>
                <p>{{ new Date(ref.createdAt).toLocaleString("nl-NL") }}</p>

                <p class="text-xs text-gray-400 italic">
                  Klik opnieuw om te sluiten.
                </p>
              </div>
            </div>

            <!-- Geen verwijzingen -->
            <div 
              v-if="referrals.length === 0" 
              class="col-span-full p-8 text-center bg-gray-50 border border-gray-200 rounded-2xl text-gray-500"
            >
              Geen doorverwijzingen.
            </div>

          </div>
        </section>

      </template>

      <!-- Medewerkers -->
      <template v-else>
        <div class="mt-6 text-center text-gray-500">
          Dit is het dashboard voor medewerkers.
        </div>
      </template>

    </div>
  </div>
</template>



