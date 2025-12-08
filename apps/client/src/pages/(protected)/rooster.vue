<script setup lang="ts">
import { computed, ref, watch } from 'vue'
import type { DateValue } from 'reka-ui'
import { getLocalTimeZone, today } from '@internationalized/date'
import { toDate } from 'reka-ui/date'
import { Calendar } from '@/components/ui/calendar'
import { Button } from '@/components/ui/button'
import { Separator } from '@/components/ui/separator'

// Bereken maandag van de gekozen week
function getWeekStart(date: Date): Date {
  const d = new Date(date)
  const day = d.getDay() || 7
  if (day !== 1) d.setDate(d.getDate() - (day - 1))
  d.setHours(0, 0, 0, 0)
  return d
}

const todayJs = new Date()
const weekStart = ref<Date>(getWeekStart(todayJs))

// Calendar-model (DateValue uit reka-ui)
const calendarDate = ref<any>(null)

// Sync Calendar → weekStart
watch(
    calendarDate,
    (val) => {
      if (!val) return
      const jsDate = toDate(val as DateValue)
      weekStart.value = getWeekStart(jsDate)
    },
    { immediate: false },
)

const startHour = 8
const endHour = 18
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

// Generiek type, herbruikbaar los van 'specialist'
type Appointment = {
  id: number
  title: string
  start: Date
  end: Date
}

// Helper om een datum in deze week te maken (offset = 0 is maandag)
function createWeekDate(dayOffset: number, hour: number, minute = 0): Date {
  const base = new Date(weekStart.value)
  base.setDate(base.getDate() + dayOffset)
  base.setHours(hour, minute, 0, 0)
  return base
}

const appointments = ref<Appointment[]>([
  // Dummy data; in de praktijk vul je dit met data uit je API
  {
    id: 1,
    title: 'Intake – Patiënt A',
    start: createWeekDate(0, 9),  // Maandag 09:00
    end: createWeekDate(0, 10),   // Maandag 10:00
  },
  {
    id: 2,
    title: 'Controle – Patiënt B',
    start: createWeekDate(1, 11), // Dinsdag 11:00
    end: createWeekDate(1, 17),   // Dinsdag 12:00
  },
  {
    id: 3,
    title: 'Behandeling – Patiënt C',
    start: createWeekDate(2, 14), // Woensdag 14:00
    end: createWeekDate(2, 15, 30), // Woensdag 15:30
  },
  {
    id: 4,
    title: 'Nabespreking – Patiënt D',
    start: createWeekDate(3, 10), // Donderdag 10:00
    end: createWeekDate(3, 11),   // Donderdag 11:00
  },
  {
    id: 5,
    title: 'Consult – Patiënt E',
    start: createWeekDate(4, 16), // Vrijdag 16:00
    end: createWeekDate(4, 17),   // Vrijdag 17:00
  },
])


// Hoogte van één uur-slot in pixels (moet overeenkomen met je CSS h-16 → ±64px)
const SLOT_HEIGHT = 64

function isSameDay(a: Date, b: Date) {
  return (
    a.getFullYear() === b.getFullYear() &&
    a.getMonth() === b.getMonth() &&
    a.getDate() === b.getDate()
  )
}

// Alle afspraken voor één dag
function getAppointmentsForDay(day: Date) {
  return appointments.value.filter((appt) => isSameDay(appt.start, day))
}

// Bepaal top/height voor een afspraak in de dagkolom
function getAppointmentStyle(appt: Appointment) {
  const startHour = appt.start.getHours() + appt.start.getMinutes() / 60
  const endHour = appt.end.getHours() + appt.end.getMinutes() / 60
  const durationHours = Math.max(endHour - startHour, 0.25) // min 15 min, voorkomt 0

  const topPx = (startHour - startHourBase.value) * SLOT_HEIGHT
  const heightPx = durationHours * SLOT_HEIGHT

  return {
    top: `${topPx}px`,
    height: `${heightPx}px`,
  }
}

// Basisuur voor de eerste rij (zelfde als startHour)
const startHourBase = computed(() => startHour)

function getAppointmentsForDayAndHour(day: Date, hour: number) {
  return appointments.value.filter((appt) => {
    return (
        isSameDay(appt.start, day) &&
        appt.start.getHours() <= hour &&
        appt.end.getHours() > hour
    )
  })
}

function onAddAgenda() {
  // Deze actie kun je per rol anders invullen
  alert('Agenda toevoegen (hier kun je een dialoog of route openen).')
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

        <!-- Lichaam met tijdslots -->
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

          <!-- Lichaam met tijdslots -->
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

              <!-- Afspraken in deze dag -->
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

    <!-- Rechterzijde: date picker + Add Agenda -->
    <aside class="w-80 border-l bg-slate-50 p-6 flex flex-col gap-4">
      <div class="flex items-center justify-between">
        <h2 class="text-lg font-semibold">Week selectie</h2>
      </div>

      <Button
          type="button"
          size="sm"
          class="justify-center"
          @click="onAddAgenda"
      >
        Agenda toevoegen
      </Button>

      <Separator />

      <div class="space-y-2">
        <p class="block text-sm font-medium text-slate-700">
          Kies een datum (bepaalt de week)
        </p>

        <!-- shadcn / reka-ui Calendar -->
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