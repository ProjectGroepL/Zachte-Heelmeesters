import { computed, ref } from 'vue'
import api from '@/lib/api'
import type { AppointmentDto } from '@/types/appointment'

export function useAppointment() {
    const appointments = ref<AppointmentDto[]>([])
    const loading = ref(false)

    const fetchAppointments = async () => {
        const res = await api.get('/appointments')
        appointments.value = res.data.appointments
    }

    const nextAppointment = computed(() => {
        if (!appointments.value.length) return null

        return appointments.value
            .slice()
            .sort((a, b) =>
                new Date(a.date).getTime() - new Date(b.date).getTime()
            )[0]
    })

    return {
        appointments,
        nextAppointment,
        fetchAppointments
    }
}