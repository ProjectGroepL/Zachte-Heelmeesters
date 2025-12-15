import { computed } from "vue";
import { useQuery } from "@/composables/useApi";
import type { AppointmentDto } from "@/types/appointment";

interface AppointmentsResponse {
  userId: number;
  appointments: AppointmentDto[];
}

export function useAppointment() {
  const {
    data,
    loading,
    error,
    execute
  } = useQuery<AppointmentsResponse>("/appointments");

  /**
   * Publieke fetch-functie.
   * Alias van useQuery.execute(), maar duidelijker in gebruik.
   */
  const fetchAppointments = execute;

  const appointments = computed<AppointmentDto[]>(() => {
    return data.value?.appointments ?? [];
  });

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
