<script setup lang="ts">
import { ref, watch } from 'vue'
import { useCreateAppointmentReport } from '@/composables/useAppointmentReport'
import { CheckCircle2 } from 'lucide-vue-next'

const props = defineProps<{ appointmentId: number }>()
const emit = defineEmits(['success'])

// We maken de mutation reactief op de prop
const { mutate, loading, error } = useCreateAppointmentReport(props.appointmentId)

const summary = ref('')
const items = ref([{ description: '', cost: 0 }])
const isSuccess = ref(false)

const addItem = () => items.value.push({ description: '', cost: 0 })
const removeItem = (index: number) => items.value.splice(index, 1)

const submit = async () => {
  try {
    await mutate({
      summary: summary.value,
      items: items.value
    })
    isSuccess.value = true
    setTimeout(() => {
      emit('success')
      summary.value = ''
      items.value = [{ description: '', cost: 0 }]
      isSuccess.value = false
    }, 2000)
  } catch (e) {
    console.error("Fout bij opslaan rapport", e)
  }
}
</script>

<template>
  <div class="space-y-6">
    <div v-if="isSuccess" class="bg-green-50 text-green-700 p-4 rounded-lg flex items-center gap-3">
      <CheckCircle2 class="w-5 h-5" />
      <span>Rapport succesvol opgeslagen en afspraak gemarkeerd als voltooid.</span>
    </div>

    <div v-else class="space-y-4">
      <div>
        <label class="text-sm font-semibold">Medische Samenvatting</label>
        <textarea
          v-model="summary"
          placeholder="Beschrijf de bevindingen en resultaten..."
          class="w-full border p-3 rounded-lg min-h-[120px] mt-1"
        />
      </div>

      <div class="space-y-3">
        <label class="text-sm font-semibold">Gemaakte Kosten / Verrichtingen</label>
        <div v-for="(item, i) in items" :key="i" class="flex gap-2 items-start">
          <input
            v-model="item.description"
            placeholder="Bijv. Consult, Bloedonderzoek..."
            class="flex-1 border p-2 rounded-md"
          />
          <div class="relative">
            <span class="absolute left-3 top-2 text-gray-400">€</span>
            <input
              v-model.number="item.cost"
              type="number"
              class="w-28 border p-2 pl-7 rounded-md"
            />
          </div>
          <button @click="removeItem(i)" v-if="items.length > 1" class="p-2 text-red-500">×</button>
        </div>
      </div>

      <div class="flex justify-between items-center pt-4">
        <button @click="addItem" class="text-sm font-medium text-primary hover:underline">
          + Onderdeel toevoegen
        </button>

        <button
          @click="submit"
          :disabled="loading || !summary"
          class="bg-primary text-primary-foreground px-6 py-2 rounded-lg font-bold disabled:opacity-50"
        >
          {{ loading ? 'Bezig met opslaan...' : 'Rapport Definitief Afronden' }}
        </button>
      </div>
    </div>
  </div>
</template>