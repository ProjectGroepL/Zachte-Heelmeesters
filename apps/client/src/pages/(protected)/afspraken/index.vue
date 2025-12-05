<script setup lang="ts">
import { ref, onMounted } from 'vue'

interface AppointmentDto {
    referralId: number
    patientName: string
    notes: string
    status: string
    treatmentDescription: string
    treatmentInstructions: string
    date: string
}

const appointments = ref<AppointmentDto[]>([])
const loading = ref(true)
const error = ref<string | null>(null)

const fetchAppointments = async () => {
    loading.value = true
    error.value = null

    try {
        const token = localStorage.getItem('access_token')
        if (!token) throw new Error('Niet ingelogd')

        // fetch uitvoeren
        const res = await fetch('https://localhost:7048/api/appointments', {
            headers: {
                'Authorization': `Bearer ${token}`,
                'Content-Type': 'application/json'
            }
        })

        if (!res.ok) throw new Error(`HTTP error! status: ${res.status}`)

        // hier mag je res.json() aanroepen
        const data = await res.json()
        console.log('Server response:', data)

        // Vue state updaten
        appointments.value = data.appointments
    } catch (err: any) {
        console.error('Fout bij ophalen van afspraken:', err)
        error.value = 'Er is een fout opgetreden bij het laden van de afspraken.'
    } finally {
        loading.value = false
    }
}


onMounted(fetchAppointments)

</script>


<template>
    <div class="p-6 flex flex-col space-y-6">
        <div class="flex items-center justify-between">
            <h1 class="text-2xl font-bold">Mijn Afspraken</h1>

            <router-link to="/afspraken/create" class="px-4 py-2 bg-black text-white rounded hover:bg-blue-700">
                Nieuwe afspraak maken
            </router-link>
        </div>


        <div v-if="loading" class="text-gray-500">Afspraken laden...</div>
        <div v-else-if="error" class="text-red-500">{{ error }}</div>
        <div v-else>
            <p v-if="appointments.length === 0" class="text-gray-600">Er zijn nog geen afspraken.</p>

            <div v-else class="overflow-x-auto">
                <table class="min-w-full table-auto border border-gray-300">
                    <thead class="bg-gray-100">
                        <tr>
                            <th class="border px-4 py-2">Doorverwijzing ID</th>
                            <th class="border px-4 py-2">Behandeling</th>
                            <th class="border px-4 py-2">Instructies</th>
                            <th class="border px-4 py-2">Status</th>
                            <th class="border px-4 py-2">Notities</th>
                            <th class="border px-4 py-2">Datum</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr v-for="appointment in appointments" :key="appointment.referralId" class="hover:bg-gray-50">
                            <td class="border px-4 py-2">{{ appointment.referralId }}</td>
                            <td class="border px-4 py-2">{{ appointment.treatmentDescription }}</td>
                            <td class="border px-4 py-2">{{ appointment.treatmentInstructions }}</td>
                            <td class="border px-4 py-2">{{ appointment.status }}</td>
                            <td class="border px-4 py-2">{{ appointment.notes }}</td>
                            <td class="border px-4 py-2">{{ new Date(appointment.date).toLocaleString('nl-NL') }}</td>

                        </tr>
                    </tbody>
                </table>
            </div>
        </div>

    </div>
</template>