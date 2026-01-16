<script setup lang="ts">
import { ref, computed } from 'vue'
import { useAppointment } from '@/composables/useAppointments'
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { Checkbox } from '@/components/ui/checkbox'
import { Label } from '@/components/ui/label'
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogFooter,
    DialogHeader,
    DialogTitle,
} from '@/components/ui/dialog'
import {
    Table,
    TableBody,
    TableCell,
    TableHead,
    TableHeader,
    TableRow,
} from '@/components/ui/table'
import { Calendar, Clock, AlertCircle, Info } from 'lucide-vue-next'
import { RouterLink } from 'vue-router'

const { appointments, loading, error } = useAppointment()
const showCancelled = ref(false)
const selectedAppointment = ref<any>(null)
const isDialogOpen = ref(false)

const openAppointmentDetails = (appointment: any) => {
    selectedAppointment.value = appointment
    isDialogOpen.value = true
}

const isEffectivelyCancelled = (status: string) =>
    status === 'Cancelled' || status === 'AccessDenied'

const displayStatus = (status: string) => {
    if (status === 'AccessDenied') return 'Geannuleerd'
    if (status === 'PendingAccess') return 'In afwachting'
    if (status === 'Scheduled') return 'Gepland'
    return status
}

const filteredAppointments = computed(() =>
    appointments.value.filter(a => showCancelled.value || !isEffectivelyCancelled(a.status))
)

const hasCancelledAppointments = computed(() =>
    appointments.value.some(a => isEffectivelyCancelled(a.status))
)

</script>


<template>
    <div class="container mx-auto p-6 space-y-6">
        <div class="flex items-center justify-between">
            <h1 class="text-3xl font-bold">Mijn Afspraken</h1>
            <Button as-child>
                <RouterLink to="/afspraken/create">
                    <Calendar class="h-4 w-4 mr-2" />
                    Nieuwe afspraak maken
                </RouterLink>
            </Button>
        </div>

        <!-- Info Banner -->
        <div v-if="hasCancelledAppointments" class="flex gap-3 p-4 bg-yellow-50 border border-yellow-200 rounded-lg">
            <Info class="h-5 w-5 text-yellow-600 shrink-0 mt-0.5" />
            <div class="text-sm text-yellow-800">
                <strong class="font-semibold">Waarom is een afspraak geannuleerd?</strong>
                <p class="mt-1">
                    Omdat je de specialist geen toegang hebt gegeven tot je medische gegevens,
                    kan deze niet met voldoende zekerheid handelen.
                    Daarom is de afspraak automatisch geannuleerd.
                </p>
            </div>
        </div>

        <!-- Toggle Cancelled -->
        <div class="flex items-center gap-2">
            <Checkbox v-model="showCancelled" id="show-cancelled" />
            <Label for="show-cancelled" class="text-sm cursor-pointer">
                Toon geannuleerde afspraken
            </Label>
        </div>

        <!-- Content Card -->
        <Card>
            <CardHeader>
                <CardTitle class="flex items-center gap-2">
                    <Calendar class="h-5 w-5" />
                    Afspraken Overzicht
                </CardTitle>
            </CardHeader>
            <CardContent>
                <!-- Loading State -->
                <div v-if="loading" class="flex flex-col items-center justify-center py-12 text-muted-foreground">
                    <Clock class="h-12 w-12 mb-4 animate-pulse opacity-50" />
                    <p class="font-medium">Afspraken laden...</p>
                </div>

                <!-- Error State -->
                <div v-else-if="error" class="flex flex-col items-center justify-center py-12 text-destructive">
                    <AlertCircle class="h-12 w-12 mb-4" />
                    <p class="font-medium">{{ error }}</p>
                </div>

                <!-- Empty State -->
                <div v-else-if="appointments.length === 0"
                    class="flex flex-col items-center justify-center py-12 text-muted-foreground">
                    <Calendar class="h-12 w-12 mb-4 opacity-50" />
                    <p class="font-medium">Geen afspraken gevonden</p>
                    <p class="text-sm mt-2">Er zijn nog geen afspraken gepland.</p>
                </div>

                <!-- Table -->
                <div v-else class="rounded-md border">
                    <Table>
                        <TableHeader>
                            <TableRow>
                                <TableHead>Doorverwijzing ID</TableHead>
                                <TableHead>Status</TableHead>
                                <TableHead>Behandeling</TableHead>
                                <TableHead>Datum</TableHead>
                            </TableRow>
                        </TableHeader>
                        <TableBody>
                            <TableRow 
                                v-for="appointment in filteredAppointments" 
                                :key="appointment.referralId" 
                                @click="openAppointmentDetails(appointment)"
                                class="cursor-pointer hover:bg-muted/50"
                                :class="{
                                    'opacity-50 bg-muted/30': isEffectivelyCancelled(appointment.status)
                                }"
                            >
                                <TableCell class="font-medium">
                                    {{ appointment.referralId }}
                                </TableCell>
                                <TableCell>
                                    <span
                                        class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                                        :class="{
                                            'bg-green-100 text-green-800': appointment.status === 'Scheduled',
                                            'bg-yellow-100 text-yellow-800': appointment.status === 'PendingAccess',
                                            'bg-gray-100 text-gray-600': appointment.status === 'Cancelled',
                                            'bg-red-100 text-red-800': appointment.status === 'AccessDenied'
                                        }">
                                        {{ displayStatus(appointment.status) }}
                                    </span>
                                </TableCell>
                                <TableCell>{{ appointment.treatmentDescription }}</TableCell>
                                <TableCell class="whitespace-nowrap">
                                    {{ new Date(appointment.date).toLocaleString('nl-NL') }}
                                </TableCell>
                            </TableRow>
                        </TableBody>
                    </Table>
                </div>
            </CardContent>
        </Card>

        <!-- Appointment Details Dialog -->
        <Dialog v-model:open="isDialogOpen">
            <DialogContent class="max-w-2xl">
                <DialogHeader>
                    <DialogTitle>Afspraak Details</DialogTitle>
                    <DialogDescription>
                        Doorverwijzing #{{ selectedAppointment?.referralId }}
                    </DialogDescription>
                </DialogHeader>
                <div v-if="selectedAppointment" class="space-y-4">
                    <div>
                        <h3 class="text-sm font-semibold mb-1">Behandeling</h3>
                        <p class="text-sm text-muted-foreground">{{ selectedAppointment.treatmentDescription }}</p>
                    </div>
                    <div>
                        <h3 class="text-sm font-semibold mb-1">Datum & Tijd</h3>
                        <p class="text-sm text-muted-foreground">
                            {{ new Date(selectedAppointment.date).toLocaleString('nl-NL') }}
                        </p>
                    </div>
                    <div>
                        <h3 class="text-sm font-semibold mb-1">Status</h3>
                        <span
                            class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium"
                            :class="{
                                'bg-green-100 text-green-800': selectedAppointment.status === 'Scheduled',
                                'bg-yellow-100 text-yellow-800': selectedAppointment.status === 'PendingAccess',
                                'bg-gray-100 text-gray-600': selectedAppointment.status === 'Cancelled',
                                'bg-red-100 text-red-800': selectedAppointment.status === 'AccessDenied'
                            }">
                            {{ displayStatus(selectedAppointment.status) }}
                        </span>
                    </div>
                    <div>
                        <h3 class="text-sm font-semibold mb-1">Instructies</h3>
                        <p class="text-sm text-muted-foreground whitespace-pre-wrap">
                            {{ selectedAppointment.treatmentInstructions || 'Geen instructies beschikbaar' }}
                        </p>
                    </div>
                    <div>
                        <h3 class="text-sm font-semibold mb-1">Notities</h3>
                        <p class="text-sm text-muted-foreground whitespace-pre-wrap">
                            {{ selectedAppointment.notes || 'Geen notities beschikbaar' }}
                        </p>
                    </div>
                </div>
                <DialogFooter>
                    <Button @click="isDialogOpen = false">Sluiten</Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    </div>
</template>