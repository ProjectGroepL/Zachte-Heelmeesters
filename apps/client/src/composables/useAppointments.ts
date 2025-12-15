import { computed } from "vue";
import { useQuery } from "@/composables/useApi";
import type { AppointmentDto } from "@/types/appointment";

export function useAppointment() {
  const {
    data,
    loading,
    error,
    execute
  } = useQuery<any>("/appointments");

  /**
   * Altijd correcte lijst afspraken,
   * ongeacht response shape van backend
   */
  const appointments = computed<AppointmentDto[]>(() => {
    const raw = data.value;

    if (!raw) return [];

    // 1️⃣ Meest voorkomend: { appointments: [...] }
    if (Array.isArray(raw.appointments)) {
      return raw.appointments;
    }

    // 2️⃣ Soms direct array: [ {...}, {...} ]
    if (Array.isArray(raw)) {
      return raw;
    }

    // 3️⃣ Iets anders → geen afspraken
    return [];
  });

  /**
   * Eerstvolgende afspraak (op datum gesorteerd)
   */
  const nextAppointment = computed<AppointmentDto | null>(() => {
    if (!appointments.value.length) return null;

    return appointments.value
      .slice()
      .sort(
        (a, b) =>
          new Date(a.date).getTime() -
          new Date(b.date).getTime()
      )[0] ?? null;
  });

  return {
    appointments,
    nextAppointment,
    loading,
    error,
    fetchAppointments: execute
  };
}
