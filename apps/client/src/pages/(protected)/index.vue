<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { Button } from '@/components/ui/button';
import { useAuth } from '@/composables/useAuth';
import { useReferral } from '@/composables/userReferral'
import type { AlignVerticalDistributeCenter } from 'lucide-vue-next';
import type { Referral } from '@/types/referral';
import { useAppointment } from '@/composables/useAppointments'
import type { AppointmentDto } from '@/types/appointment';

const { getStoredUser } = useAuth()
const { getReferrals } = useReferral()
const { fetchAppointments, nextAppointment } = useAppointment()


const userName = ref<string>('')
const referrals = ref<Referral[]>([])


onMounted(async () => {
  const user = getStoredUser()

  if (user?.firstName && user?.lastName) {
    userName.value = `${user.firstName} ${user.lastName}`
  } else if (user?.firstName) {
    userName.value = user.firstName
  } else {
    userName.value = 'Gebruiker'
  }

  referrals.value = await getReferrals()
  await fetchAppointments()
})

</script>

<template>
  <div class="flex flex-col justify-center items-center space-y-4 h-full">
    <div class="flex flex-col items-center">
      <h1 class="text-2xl font-bold">Zachte Heelmeesters</h1>
      <p>Dit is het startproject van de zachte heelmeesters app</p>
    </div>
    
    <artical> 
      <div 
        id="Welcomemessage"
        role="button"
        tabindex="0"
        aria-label="Welkomsbericht"
        class="
          w-full max-w-xl p-4 bg-white rounded-xl shadow 
          hover:shadow-lg transition cursor-pointer
          focus:ring focus:ring-blue-400
          transition
        "
      >
        <h2 class="text-xl font-semibold">Welkom</h2>
        <p class="text-gray-600 mt-1">{{ userName }}</p>
      </div>
    </artical>

    <artical>
     <div
      class="
        w-full max-w-xl p-4 bg-white rounded-xl shadow 
        hover:shadow-lg transition cursor-pointer mt-4"
      
      >

      <h2 class="text-xl font-semibold">Eerstvolgende afspraak</h2>

      <div v-if="nextAppointment">
        <p class="text-gray-600 mt-1">
          Behandeling: {{ nextAppointment.treatmentDescription }}
        </p>
        <p class="text-gray-500 text-sm">
          Datum: {{ new Date(nextAppointment.date).toLocaleString('nl-NL') }}
        </p>
      </div>
        
      <div v-else class="text-gray-400 text-sm mt-1">
        Je hebt nog geen geplande afspraken.
      </div>
    </div>
    </artical>

    <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4 mt-6 w-full px-6">
      <div 
        v-for="item in referrals"
        
        :key="item.id"
        class="p-4 bg-white rounded-xl shadow hover:shadow-lg transition cursor-pointer"
      >
        <h2 class="text-lg font-semibold">{{ item.treatmentName }}</h2>
        <p class="text-sm text-gray-600">Patiënt: {{ item.patientName }}</p>
        <p class="text-xs text-gray-500">Status: {{ item.status }}</p>
      </div>

      <div v-if="referrals.length === 0"
            class="col-span-full p-6 text-center bg-gray-50 rounded-xl border border-gray-200">
            <p class="text-gray-600">Je hebt nog geen actieve doorverwijzingen.</p>
        </div>
    </div>
      
  </div>
</template>