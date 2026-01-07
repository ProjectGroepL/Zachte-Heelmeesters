<script setup lang="ts">
    import { ref } from 'vue'
    import {
        usePatientAccessRequests,
        useDecideAccessRequest,
        useRevokeAccessRequest
    } from "@/composables/useAccessRequests"

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
        // We voeren de mutatie uit
        await decideMutation.mutate({ id, data: { approved } })
        
        // Alleen refetchen als het gelukt is
        if (!decideMutation.error.value) {
            await refetch()
        }
    }
</script>

<template>
  <main aria-labelledby="access-title" class="max-w-4xl mx-auto p-6">
    <h1 id="access-title" class="text-2xl font-bold mb-4">
      Toegangsverzoeken
    </h1>

    <div v-if="loading && !requests" role="status" aria-live="polite" class="text-gray-500">
      Verzoeken laden…
    </div>

    <div v-if="error" role="alert" class="p-4 mb-4 bg-red-50 text-red-600 rounded-lg border border-red-200">
      <strong>Fout bij laden:</strong> {{ error.message }}
    </div>

    <ul v-if="requests && requests.length > 0" class="space-y-4">
      <li
        v-for="r in requests"
        :key="r.id"
        class="p-5 border-2 border-gray-100 rounded-2xl shadow-sm bg-white"
      >
        <div class="flex flex-col gap-2">
          <h2 class="font-bold text-lg text-gray-900">
            Verzoek van {{ r.specialistName }}
          </h2>

          <div class="bg-blue-50 p-3 rounded-lg border border-blue-100">
            <p class="text-sm font-semibold text-blue-800 mb-1">Reden voor aanvraag:</p>
            <p class="text-gray-700 italic">"{{ r.reason }}"</p>
          </div>

          <div class="flex gap-3 mt-4">

          <!-- Pending: approve / deny -->
          <template v-if="r.status === 'Pending'">
            <button
              class="px-6 py-2 bg-green-600 text-white rounded-xl"
              @click="decide(r.id, true)"
            >
              Goedkeuren
            </button>

            <button
              class="px-6 py-2 border-2 border-red-200 text-red-600 rounded-xl"
              @click="decide(r.id, false)"
            >
              Weigeren
            </button>
          </template>

          <!-- Approved -->
          <template v-else-if="r.status === 'Approved'">
            <template v-if="revokeConfirmId === r.id">
              <div class="bg-red-50 border border-red-200 p-3 rounded-xl">
                <p class="text-sm text-red-800 mb-2">
                  Door het intrekken van toegang kan de specialist deze afspraak niet uitvoeren.
                  De afspraak wordt daarom geannuleerd
                </p>

                <div class="flex gap-2">
                  <button
                    class="px-4 py-1 bg-red-600 text-white rounded-lg"
                    @click="confirmRevoke(r.id)"
                  >
                    Bevestigen
                  </button>

                  <button
                    class="px-4 py-1 border rounded-lg"
                    @click="cancelRevoke"
                  >
                    Annuleren
                  </button>
                </div>
              </div>
            </template>

            <button
              v-else
              class="px-6 py-2 bg-red-600 text-white rounded-xl"
              @click="askRevoke(r.id)"
            >
              Toegang intrekken
            </button>
          </template>

          <!-- Denied / Revoked: info only -->
          <template v-else>
            <span class="text-sm text-gray-500 italic">
              Geen acties mogelijk
            </span>
          </template>

        </div>
          
        </div>
      </li>
    </ul>

    <p v-else-if="!loading" class="text-gray-500 bg-gray-50 p-8 rounded-2xl border-2 border-dashed text-center">
      Er zijn op dit moment geen openstaande verzoeken voor toegang tot uw dossier.
    </p>
  </main>
</template>