<script setup lang="ts">
import { ref } from 'vue'
import {
  usePatientAccessRequests,
  useDecideAccessRequest,
  useRevokeAccessRequest
} from "@/composables/useAccessRequests"
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
import { ShieldCheck, Clock, AlertCircle, Info } from 'lucide-vue-next'
import {
  Alert,
  AlertDescription,
  AlertTitle,
} from '@/components/ui/alert'

const {
  data: requests,
  loading,
  error,
  execute: refetch
} = usePatientAccessRequests();

const decideMutation = useDecideAccessRequest();
const revokeMutation = useRevokeAccessRequest()
const revokeConfirmId = ref<number | null>(null)

const revoke = async (id: number) => {
  await revokeMutation.mutate({ id })
  if (!revokeMutation.error.value) {
    await refetch()
  }
}
const askRevoke = (id: number) => {
  revokeConfirmId.value = id
}

const cancelRevoke = () => {
  revokeConfirmId.value = null
}

const confirmRevoke = async (id: number) => {
  await revoke(id)
  revokeConfirmId.value = null
}
const decide = async (id: number, approved: boolean) => {
  await decideMutation.mutate({ id, data: { approved } })

  if (!decideMutation.error.value) {
    await refetch()
  }
}
</script>

<template>
  <div class="container mx-auto p-6 space-y-6">
    <h1 class="text-3xl font-bold">Toegangsverzoeken</h1>

    <Card>
      <CardHeader>
        <CardTitle class="flex items-center gap-2">
          <ShieldCheck class="h-5 w-5" />
          Verzoeken Overzicht
        </CardTitle>
      </CardHeader>
      <CardContent>
        <!-- Loading -->
        <div v-if="loading && !requests" class="flex flex-col items-center justify-center py-12 text-muted-foreground">
          <Clock class="h-12 w-12 mb-4 animate-pulse opacity-50" />
          <p class="font-medium">Verzoeken laden...</p>
        </div>

        <!-- Error -->
        <Alert v-if="error" variant="destructive" class="mb-4">
          <AlertCircle class="h-4 w-4" />
          <AlertTitle>Fout bij laden</AlertTitle>
          <AlertDescription>{{ error.message }}</AlertDescription>
        </Alert>

        <!-- Empty State -->
        <div v-if="!loading && (!requests || requests.length === 0)"
          class="flex flex-col items-center justify-center py-12 text-muted-foreground">
          <ShieldCheck class="h-12 w-12 mb-4 opacity-50" />
          <p class="font-medium">Geen openstaande verzoeken</p>
          <p class="text-sm mt-2">Er zijn op dit moment geen openstaande verzoeken voor toegang tot uw dossier.</p>
        </div>

        <!-- Requests List -->
        <div v-if="requests && requests.length > 0" class="space-y-4">
          <Card v-for="r in requests" :key="r.id" class="border">
            <CardContent class="p-6">
              <div class="space-y-4">
                <h2 class="text-lg font-semibold">
                  Verzoek van {{ r.specialistName }}
                </h2>

                <Alert class="bg-blue-50 border-blue-200">
                  <Info class="h-4 w-4 text-blue-600" />
                  <AlertTitle class="text-blue-800">Reden voor aanvraag</AlertTitle>
                  <AlertDescription class="text-blue-900 italic">
                    "{{ r.reason }}"
                  </AlertDescription>
                </Alert>

                <!-- Pending: approve / deny -->
                <div v-if="r.status === 'Pending'" class="flex gap-3">
                  <Button @click="decide(r.id, true)" class="bg-green-600 hover:bg-green-700">
                    Goedkeuren
                  </Button>
                  <Button @click="decide(r.id, false)" variant="destructive">
                    Weigeren
                  </Button>
                </div>

                <!-- Approved -->
                <div v-else-if="r.status === 'Approved'">
                  <Alert v-if="revokeConfirmId === r.id" variant="destructive" class="mb-3">
                    <AlertCircle class="h-4 w-4" />
                    <AlertDescription>
                      Door het intrekken van toegang kan de specialist deze afspraak niet uitvoeren.
                      De afspraak wordt daarom geannuleerd.
                    </AlertDescription>
                    <div class="flex gap-2 mt-3">
                      <Button size="sm" variant="destructive" @click="confirmRevoke(r.id)">
                        Bevestigen
                      </Button>
                      <Button size="sm" variant="outline" @click="cancelRevoke">
                        Annuleren
                      </Button>
                    </div>
                  </Alert>

                  <Button v-else variant="destructive" @click="askRevoke(r.id)">
                    Toegang intrekken
                  </Button>
                </div>

                <!-- Denied / Revoked: info only -->
                <p v-else class="text-sm text-muted-foreground italic">
                  Geen acties mogelijk
                </p>
              </div>
            </CardContent>
          </Card>
        </div>
      </CardContent>
    </Card>
  </div>
</template>