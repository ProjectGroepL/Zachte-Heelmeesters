import { computed, ref } from "vue";
import api from "@/lib/api";
import type { AppointmentDto } from "@/types/appointment";

interface AppointmentsResponse {
    userId: number;
    appointments: AppointmentDto[];
}

export function useAppointment() {
    const appointments = ref<AppointmentDto[]>([]);
    const loading = ref(false);
    const error = ref<string | null>(null);

    const fetchAppointments = async () => {
        loading.value = true;
        error.value = null;

        try {
            const res = await api.get<AppointmentsResponse>("/appointments"); //reponse formaat moet veranderd worden als we usequery gebruiken waardoor het hier niet kan.
            appointments.value = res.data.appointments;
        } catch (err) {
            error.value = "Afspraken konden niet geladen worden.";
        } finally {
            loading.value = false;
        }
    };

    const nextAppointment = computed<AppointmentDto | null>(() => {
        if (!appointments.value.length) return null;

        return (
            appointments.value
                .slice()
                .sort(
                    (a, b) =>
                        new Date(a.date).getTime() -
                        new Date(b.date).getTime()
                )[0] ?? null
        );
    });

    return {
        appointments,
        nextAppointment,
        loading,
        error,
        fetchAppointments
    };
}