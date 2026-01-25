<script setup lang="ts">
import {
  useMyMedicalDocuments,
  useUpdateMedicalDocumentStatus
} from '@/composables/useMedicalDocuments'
import { computed, ref } from 'vue'
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card'
import { Button } from '@/components/ui/button'
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
import { FileText, Clock, FolderOpen } from 'lucide-vue-next'

const { data, loading, execute } = useMyMedicalDocuments()
const updateStatus = useUpdateMedicalDocumentStatus()

const visibleDocuments = computed(() =>
  (data.value ?? []).filter(doc => doc.status !== 'Draft')
)

const selectedDocument = ref<any>(null)
const isDialogOpen = ref(false)

const openDocumentDetails = (doc: any) => {
  selectedDocument.value = doc
  isDialogOpen.value = true
}

const changeStatus = async (id: number, status: 'Final' | 'Archived') => {
  await updateStatus.mutate({ id, status })
  await execute()
  isDialogOpen.value = false
}
</script>

<template>
  <div class="container mx-auto p-6 space-y-6">
    <h1 class="text-3xl font-bold">Mijn medische documenten</h1>

    <Card>
      <CardHeader>
        <CardTitle class="flex items-center gap-2">
          <FileText class="h-5 w-5" />
          Documenten Overzicht
        </CardTitle>
      </CardHeader>
      <CardContent>
        <!-- Loading -->
        <div v-if="loading" class="flex flex-col items-center justify-center py-12 text-muted-foreground">
          <Clock class="h-12 w-12 mb-4 animate-pulse opacity-50" />
          <p class="font-medium">Documenten laden...</p>
        </div>

        <!-- Empty state -->
        <div v-else-if="!visibleDocuments || visibleDocuments.length === 0"
          class="flex flex-col items-center justify-center py-12 text-muted-foreground">
          <FolderOpen class="h-12 w-12 mb-4 opacity-50" />
          <p class="font-medium">Geen documenten gevonden</p>
          <p class="text-sm mt-2">Er zijn nog geen medische documenten.</p>
        </div>

        <!-- Documents list -->
        <div v-else class="rounded-md border">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Titel</TableHead>
                <TableHead>Status</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              <TableRow 
                v-for="doc in visibleDocuments" 
                :key="doc.id"
                @click="openDocumentDetails(doc)"
                class="cursor-pointer hover:bg-muted/50"
              >
                <TableCell class="font-medium">
                  {{ doc.title }}
                </TableCell>
                <TableCell>
                  <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium" :class="{
                    'bg-yellow-100 text-yellow-800': doc.status === 'Draft',
                    'bg-green-100 text-green-800': doc.status === 'Final',
                    'bg-gray-100 text-gray-600': doc.status === 'Archived'
                  }">
                    {{ doc.status }}
                  </span>
                </TableCell>
              </TableRow>
            </TableBody>
          </Table>
        </div>
      </CardContent>
    </Card>

    <!-- Document Details Dialog -->
    <Dialog v-model:open="isDialogOpen">
      <DialogContent class="max-w-2xl">
        <DialogHeader>
          <DialogTitle>{{ selectedDocument?.title }}</DialogTitle>
          <DialogDescription>
            Document details
          </DialogDescription>
        </DialogHeader>
        <div v-if="selectedDocument" class="space-y-4">
          <div>
            <h3 class="text-sm font-semibold mb-1">Inhoud</h3>
            <p class="text-sm text-muted-foreground whitespace-pre-wrap">{{ selectedDocument.content }}</p>
          </div>
          <div>
            <h3 class="text-sm font-semibold mb-1">Status</h3>
            <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium" :class="{
              'bg-yellow-100 text-yellow-800': selectedDocument.status === 'Draft',
              'bg-green-100 text-green-800': selectedDocument.status === 'Final',
              'bg-gray-100 text-gray-600': selectedDocument.status === 'Archived'
            }">
              {{ selectedDocument.status }}
            </span>
          </div>
        </div>
        <DialogFooter>
          <div class="flex gap-2">
            <Button 
              v-if="selectedDocument?.status === 'Draft'" 
              @click="changeStatus(selectedDocument.id, 'Final')"
            >
              Finaliseren
            </Button>
            <Button 
              v-if="selectedDocument?.status === 'Final'" 
              variant="secondary"
              @click="changeStatus(selectedDocument.id, 'Archived')"
            >
              Archiveren
            </Button>
            <Button variant="outline" @click="isDialogOpen = false">Sluiten</Button>
          </div>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  </div>
</template>
