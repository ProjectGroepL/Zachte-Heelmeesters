import api from "@/lib/api";
import { useMutation } from "@/composables/useApi";

interface CreateAppointmentRequest {
  referralId: number;
  date: string; // YYYY-MM-DD
  time: string; // HH:mm
}

export function useCreateAppointment() {
  return useMutation<{ id: number }, CreateAppointmentRequest>(
    (data) => api.post("/appointments", data).then(res => res.data)
  );
}