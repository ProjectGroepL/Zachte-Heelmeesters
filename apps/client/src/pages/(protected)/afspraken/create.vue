<script setup lang="ts">
import { ref, onMounted, computed } from 'vue'
import api from '@/lib/api'

// DTO interface for TypeScript
interface ReferralDto {
    id: number
    treatmentDescription: string
    instructions?: string
    status: string
    notes?: string
}

// reactive state
const referrals = ref<ReferralDto[]>([])
const selectedReferralId = ref<number | null>(null)
const loading = ref(true)
const error = ref<string | null>(null)
const appointmentDate = ref<string>('') // YYYY-MM-DD
const appointmentTime = ref<string>('') // HH:MM

// computed property for selected referral instead of number
const selectedReferral = computed(() => {
    return referrals.value.find(r => r.id === selectedReferralId.value) || null
})

// fetch function
const fetchReferrals = async () => {
    loading.value = true
    error.value = null

    try {
        const res = await api.get('/referrals/patient')
        referrals.value = res.data
    } catch (err: any) {
        console.error('Fout bij ophalen van referrals:', err)
        error.value = 'Er is een fout opgetreden bij het laden van de referrals.'
    } finally {
        loading.value = false
    }
}

// Submit functie
const createAppointment = async () => {
    if (!selectedReferralId.value || !appointmentDate.value || !appointmentTime.value) {
        alert('Selecteer een doorverwijzing, datum en tijd')
        return
    }

    loading.value = true
    error.value = null

    try {
        const token = localStorage.getItem('access_token')
        if (!token) throw new Error('Niet ingelogd')

        // Optioneel: combineer datum + tijd tot één string of laat backend zelf behandelen
        const payload = {
            referralId: selectedReferralId.value,
            date: appointmentDate.value,
            time: appointmentTime.value
        }

        await api.post('/appointments', payload, {
            headers: { Authorization: `Bearer ${token}` }
        })

        alert('Afspraak aangemaakt!')
        // reset formulier
        selectedReferralId.value = null
        appointmentDate.value = ''
        appointmentTime.value = ''
    } catch (err: any) {
        console.error(err)
        error.value = 'Kon de afspraak niet aanmaken'
    } finally {
        loading.value = false
    }
}

// call fetch on mount
onMounted(fetchReferrals)
</script>

<template>
    <div class="p-6">
        <h1 class="text-2xl font-bold mb-4">Nieuwe afspraak</h1>

        <!-- Statusmeldingen -->
        <div v-if="loading" class="text-gray-500">Referrals laden...</div>
        <div v-else-if="error" class="text-red-500">{{ error }}</div>

        <!-- Formulier -->
        <div v-else>
            <label for="referral" class="block mb-2">Kies een doorverwijzing:</label>
            <select v-model="selectedReferralId" id="referral" class="border p-2 rounded w-full">
                <option value="" disabled selected>Selecteer een referral</option>
                <option v-for="ref in referrals" :key="ref.id" :value="ref.id">
                    {{ ref.treatmentDescription }} - {{ ref.status }}
                </option>
            </select>

            <!-- Datum en tijd selectie -->
            <div v-if="selectedReferralId" class="mt-4 space-y-2">
                <p>Je hebt doorverwijzing {{ selectedReferral?.treatmentDescription }} geselecteerd.</p>

                <label class="block">Datum:</label>
                <input type="date" v-model="appointmentDate" class="border p-2 rounded w-full" required />

                <label class="block">Tijd:</label>
                <input type="time" v-model="appointmentTime" class="border p-2 rounded w-full" required />

                <button @click="createAppointment" class="bg-black text-white px-4 py-2 rounded mt-2 w-full hover:bg-blue-600">
                    Maak afspraak
                </button>

            </div>
        </div>
    </div>
</template>
