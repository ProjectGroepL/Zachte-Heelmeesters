<script setup lang="ts">
import {computed, ref, watch, onMounted} from 'vue'
import {Calendar} from '@/components/ui/calendar'
import {Button} from '@/components/ui/button'
import {Separator} from '@/components/ui/separator'
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogDescription,
  DialogFooter,
  DialogClose,
  DialogTrigger,
} from '@/components/ui/dialog'
import {Input} from '@/components/ui/input'
import {Label} from '@/components/ui/label'
import {toDate} from 'reka-ui/date'
import 'vue-cal/dist/vuecal.css'
import VueCal from 'vue-cal'
import api from "@/lib/api.ts";

// -------------------------------------------------------------------------------------
// Basis: weekberekening
// -------------------------------------------------------------------------------------

function getWeekStart(date: Date): Date {
  const d = new Date(date)
  const day = d.getDay() || 7 // zondag = 0 → 7
  if (day !== 1) d.setDate(d.getDate() - (day - 1)) // terug naar maandag
  d.setHours(0, 0, 0, 0)
  return d
}

const todayJs = new Date()
const weekStart = ref<Date>(getWeekStart(todayJs))

// -------------------------------------------------------------------------------------
// Calendar integratie (shadcn)
// -------------------------------------------------------------------------------------

const calendarDate = ref<any>(null)

watch(calendarDate, (val) => {
  if (!val) return
  const jsDate = toDate(val as any)
  weekStart.value = getWeekStart(jsDate)
})

// -------------------------------------------------------------------------------------
// Afspraken (dummy data) + helpers voor layout
// -------------------------------------------------------------------------------------

type Appointment = {
  id: number
  title: string
  start: Date
  end: Date
}

// helper om datum in huidige week te maken (offset=0 → maandag)
function createWeekDate(dayOffset: number, hour: number, minute = 0): Date {
  const base = new Date(weekStart.value)
  base.setDate(base.getDate() + dayOffset)
  base.setHours(hour, minute, 0, 0)
  return base
}

// dummy afspraken in huidige week
const appointments = ref<Appointment[]>([
  {
    id: 1,
    title: 'Intake – Patiënt A',
    start: createWeekDate(0, 9), // ma 09:00
    end: createWeekDate(0, 11), // ma 11:00 (2 uur blok)
  },
  {
    id: 2,
    title: 'Controle – Patiënt B',
    start: createWeekDate(1, 10), // di 10:00
    end: createWeekDate(1, 11), // di 11:00
  },
  {
    id: 3,
    title: 'Behandeling – Patiënt C',
    start: createWeekDate(2, 13, 30), // wo 13:30
    end: createWeekDate(2, 15), // wo 15:00 (1,5 uur)
  },
  {
    id: 4,
    title: 'Nabespreking – Patiënt D',
    start: createWeekDate(3, 9), // do 09:00
    end: createWeekDate(3, 10), // do 10:00
  },
  {
    id: 5,
    title: 'Consult – Patiënt E',
    start: createWeekDate(4, 16), // vr 16:00
    end: createWeekDate(4, 18), // vr 18:00 (loopt tot einde dag)
  },
])

const vueCalEvents = computed(() =>
    appointments.value.map((appt) => ({
      start: appt.start,
      end: appt.end,
      title: appt.title,
      id: appt.id,
      fullText: appt.title,
    })),
)

// -------------------------------------------------------------------------------------
// iCal dialog (popup) state + handlers
// -------------------------------------------------------------------------------------

const icalDialogOpen = ref(false)
const icalUrl = ref('')
const icalError = ref('')

onMounted(() => {
  try {
    const stored = localStorage.getItem('ical_url')
    if (stored) icalUrl.value = stored
  } catch {
    // ignore storage errors (e.g. private mode)
  }
})

function submitIcal() {
  icalError.value = ''

  if (!icalUrl.value.trim()) {
    icalError.value = 'Voer een URL in.'
    return
  }

  try {
    new URL(icalUrl.value)
  } catch {
    icalError.value = 'Dit lijkt geen geldige URL te zijn.'
    return
  }

  console.log('iCal URL opslaan:', icalUrl.value)
  saveIcalWithAuth(icalUrl.value).catch((err) => {
    icalError.value = err.message || 'Er is een fout opgetreden bij het opslaan.'
    return
  })

  try {
    localStorage.setItem('ical_url', icalUrl.value)
  } catch {
    // ingore the errors
  }
  
  icalDialogOpen.value = false
}
async function saveIcalWithAuth(url: string) {
  const token = localStorage.getItem('access_token')
  if (!token) throw new Error('No auth token available — user must be logged in')

  try {
    const res = await api.post('/SpecialistIcal', { url }, {
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`,
      },
    })
    return res.data
  } catch (err: any) {
    if (err.response) {
      const server = err.response.data?.message ?? err.response.data
      throw new Error(server ?? `Save failed: ${err.response.status}`)
    }
    throw err
  }
}

</script>

<template>
  <div class="flex h-full">
    <!-- Linkerzijde: weekrooster -->
    <div class="flex-1 overflow-auto p-6 space-y-4">
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-semibold">Rooster</h1>
          <p class="text-sm text-slate-600">
            Overzicht van je afspraken per week.
          </p>
        </div>
      </div>

      <Separator/>

      <VueCal
          class="rounded-md"
          default-view="week"
          today-button
          hide-view-selector
          :selected-date="calendarDate"
          :time-from="0 * 60"
          :time-to="24 * 60"
          :events="vueCalEvents"
          :disable-views="['years', 'year', 'month', 'day']"
          locale="nl"
      >
        <template #event="slotProps">
          <div :title="slotProps.event.fullText">
            {{ slotProps.event.title }}
          </div>
        </template>
      </VueCal>
    </div>

    <!-- Rechterzijde: iCal-popup + date picker -->
    <aside class="w-80 border-l bg-slate-50 p-6 flex flex-col gap-4">
      <div class="flex items-center justify-between">
        <h2 class="text-lg font-semibold">Week selectie</h2>
      </div>
      
      <Dialog v-model:open="icalDialogOpen">
        <DialogTrigger as-child>
          <Button
              type="button"
              size="sm"
              class="justify-center"
          >
            Agenda toevoegen
          </Button>
        </DialogTrigger>

        <DialogContent class="sm:max-w-md">
          <DialogHeader>
            <DialogTitle>Agenda koppelen via iCal</DialogTitle>
            <DialogDescription>
              Plak hier de iCal-URL van je agenda. We gebruiken deze om je
              rooster automatisch bij te werken.
            </DialogDescription>
          </DialogHeader>

          <div class="space-y-3 mt-3">
            <div class="space-y-1">
              <Label for="ical-url">iCal URL</Label>
              <Input
                  id="ical-url"
                  type="url"
                  placeholder="https://voorbeeld.nl/agenda.ics"
                  v-model="icalUrl"
              />
              <p v-if="icalError" class="text-xs text-red-500">
                {{ icalError }}
              </p>
            </div>

            <p class="text-xs text-slate-500">
              Deel deze link alleen met vertrouwde systemen. De link kan toegang
              geven tot je agenda.
            </p>
          </div>

          <DialogFooter class="mt-4 flex justify-end gap-2">
            <DialogClose as-child>
              <Button type="button" variant="outline">
                Annuleren
              </Button>
            </DialogClose>
            <!-- Geen DialogClose hier; sluiten gebeurt in submitIcal() -->
            <Button type="button" @click="submitIcal">
              Opslaan
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      <Separator/>
      
      <Calendar
          v-model="calendarDate"
          locale="nl-NL"
          layout="month-and-year"
          class="rounded-md border bg-white"
      />
    </aside>
  </div>
</template>