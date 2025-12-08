<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import { Calendar } from '@/components/ui/calendar'
import { Button } from '@/components/ui/button'
import { Separator } from '@/components/ui/separator'
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
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { toDate } from 'reka-ui/date'

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

const startHour = 8
const endHour = 18
const SLOT_HEIGHT = 64 // visuele hoogte per uur (px), matcht h-16

const hours = computed(() =>
    Array.from({ length: endHour - startHour }, (_, i) => startHour + i),
)

const weekDays = computed(() => {
  const start = weekStart.value
  return Array.from({ length: 7 }, (_, i) => {
    const d = new Date(start)
    d.setDate(start.getDate() + i)
    return d
  })
})

const weekStartDate = computed(() => weekDays.value[0])
const weekEndDate = computed(() => weekDays.value[6])

function formatDateLabel(d: Date) {
  return d.toLocaleDateString('nl-NL', {
    weekday: 'short',
    day: 'numeric',
    month: 'short',
  })
}

function formatHourLabel(h: number) {
  return `${h.toString().padStart(2, '0')}:00`
}

// -------------------------------------------------------------------------------------
// Calendar integratie (shadcn/reka-ui) – los getypt om type-issues te vermijden
// -------------------------------------------------------------------------------------

// Gebruik 'any' zodat v-model met Calendar geen TS-mismatch geeft
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

function isSameDay(a: Date, b: Date) {
  return (
      a.getFullYear() === b.getFullYear() &&
      a.getMonth() === b.getMonth() &&
      a.getDate() === b.getDate()
  )
}

function getAppointmentsForDay(day: Date) {
  return appointments.value.filter((appt) => isSameDay(appt.start, day))
}

// basisuur voor de grid (zelfde als startHour)
const startHourBase = computed(() => startHour)

// bereken top/hoogte in px voor een afspraak binnen één dagkolom
function getAppointmentStyle(appt: Appointment) {
  const startH = appt.start.getHours() + appt.start.getMinutes() / 60
  const endH = appt.end.getHours() + appt.end.getMinutes() / 60
  const durationHours = Math.max(endH - startH, 0.25) // min 15 min

  const topPx = (startH - startHourBase.value) * SLOT_HEIGHT
  const heightPx = durationHours * SLOT_HEIGHT

  return {
    top: `${topPx}px`,
    height: `${heightPx}px`,
  }
}

// -------------------------------------------------------------------------------------
// iCal dialog (popup) state + handlers
// -------------------------------------------------------------------------------------

const icalDialogOpen = ref(false)   // <-- nieuw
const icalUrl = ref('')
const icalError = ref('')

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

  // hier later je API-call
  console.log('iCal URL opslaan:', icalUrl.value)

  // Alleen bij geldige input sluiten
  icalDialogOpen.value = false
}

// -------------------------------------------------------------------------------------
// Overige
// -------------------------------------------------------------------------------------

function onAddAgenda() {
  // Niet meer gebruikt direct; de knop is nu de DialogTrigger,
  // maar hou deze eventueel voor toekomstige logica.
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

      <Separator />

      <p class="text-sm text-slate-600">
        Week van
        <strong v-if="weekStartDate">
          {{ weekStartDate.toLocaleDateString('nl-NL') }}
        </strong>
        t/m
        <strong v-if="weekEndDate">
          {{ weekEndDate.toLocaleDateString('nl-NL') }}
        </strong>
      </p>

      <div class="border rounded-lg overflow-hidden bg-white">
        <!-- Kop met dagen -->
        <div class="grid" :style="{ gridTemplateColumns: `80px repeat(7, 1fr)` }">
          <div
              class="bg-slate-50 border-b border-r h-12 flex items-center justify-center text-xs font-medium text-slate-500"
          >
            Tijd
          </div>
          <div
              v-for="(d, i) in weekDays"
              :key="i"
              class="bg-slate-50 border-b border-r h-12 flex flex-col items-center justify-center text-xs font-medium"
          >
            <span>{{ formatDateLabel(d) }}</span>
          </div>
        </div>

        <!-- Lichaam met uren en dagkolommen -->
        <div
            class="grid"
            :style="{ gridTemplateColumns: `80px repeat(7, 1fr)` }"
        >
          <!-- Linker kolom: tijdlabels -->
          <div class="border-r bg-slate-50/60 relative">
            <div
                v-for="hour in hours"
                :key="hour"
                class="h-16 px-2 flex items-start justify-end text-xs text-slate-500 pt-1"
            >
              {{ formatHourLabel(hour) }}
            </div>
          </div>

          <!-- Dagkolommen -->
          <div
              v-for="(day, dayIndex) in weekDays"
              :key="dayIndex"
              class="border-r relative bg-white"
              :style="{ height: `${(endHour - startHour) * SLOT_HEIGHT}px` }"
          >
            <!-- Achtergrond-uurvakken -->
            <div
                v-for="hour in hours"
                :key="hour"
                class="h-16 border-t last:border-b border-slate-200/80"
            />

            <!-- Afspraken in deze dag (één blok, doorlopend over meerdere rijen) -->
            <div
                v-for="appt in getAppointmentsForDay(day)"
                :key="appt.id"
                class="absolute left-1 right-1 rounded bg-blue-500/80 text-white text-xs p-1 overflow-hidden hover:bg-blue-600 cursor-pointer shadow-sm"
                :style="getAppointmentStyle(appt)"
            >
              <div class="font-semibold truncate">
                {{ appt.title }}
              </div>
              <div class="text-[10px]">
                {{
                  appt.start.toLocaleTimeString('nl-NL', {
                    hour: '2-digit',
                    minute: '2-digit',
                  })
                }}
                -
                {{
                  appt.end.toLocaleTimeString('nl-NL', {
                    hour: '2-digit',
                    minute: '2-digit',
                  })
                }}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Rechterzijde: iCal-popup + date picker -->
    <aside class="w-80 border-l bg-slate-50 p-6 flex flex-col gap-4">
      <div class="flex items-center justify-between">
        <h2 class="text-lg font-semibold">Week selectie</h2>
      </div>

      <!-- Dialog als echte popup -->
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

      <Separator />

      <div class="space-y-2">
        <p class="block text-sm font-medium text-slate-700">
          Kies een datum (bepaalt de week)
        </p>

        <Calendar
            v-model="calendarDate"
            locale="nl-NL"
            layout="month-and-year"
            class="rounded-md border bg-white"
        />

        <p class="text-xs text-slate-500">
          De gekozen datum wordt afgerond naar maandag van die week.
        </p>
      </div>
    </aside>
  </div>
</template>