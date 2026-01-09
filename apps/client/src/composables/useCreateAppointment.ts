import api from "@/lib/api";
import { useMutation } from "@/composables/useApi";
import type { AppointmentCreateDto } from '@/types/appointment'


export function useCreateAppointment() {
  return useMutation<{ id: number }, AppointmentCreateDto>(
    (data) => api.post("/appointments", data).then(res => res.data)
  );
}
