<script setup lang="ts">
import { ref, computed } from 'vue'
import api from '@/lib/api'

interface AuditTrail {
    id: number
    userId: number | null
    ipAddress: string
    details: string | null
    timestamp: string
    method: string
    path: string
    statusCode: number
    userAgent: string | null
}

const selectedDate = ref<string | null>(null)
const logs = ref<AuditTrail[]>([])
const isLoading = ref(false)

const sortedLogs = computed(() => {
    return [...logs.value].sort(
        (a, b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime()
    )
})

async function loadLogs() {
    if (!selectedDate.value) return

    isLoading.value = true
    try {
        const response = await api.get<AuditTrail[]>('/audittrail', {
            params: { date: new Date(selectedDate.value!).toISOString() }
        })

        logs.value = response.data
    } catch (err) {
        console.error('Failed to load audit logs', err)
    } finally {
        isLoading.value = false
    }
}

function downloadCsv() {
    if (!logs.value.length) return

    const headers = [
        'Timestamp',
        'UserId',
        'Method',
        'Path',
        'StatusCode',
        'IpAddress',
        'UserAgent',
        'Details'
    ]

    const rows = logs.value.map(l => [
        l.timestamp,
        l.userId ?? '',
        l.method,
        l.path,
        l.statusCode,
        l.ipAddress,
        l.userAgent ?? '',
        l.details ?? ''
    ])

    const csv = [headers, ...rows]
        .map(row => row.map(v => `"${String(v).replace(/"/g, '""')}"`).join(','))
        .join('\n')

    const blob = new Blob([csv], { type: 'text/csv;charset=utf-8;' })
    const url = URL.createObjectURL(blob)

    const link = document.createElement('a')
    link.href = url
    link.download = `audit-log-${selectedDate.value}.csv`
    link.click()

    URL.revokeObjectURL(url)
}
</script>

<template>
    <div class="p-6 space-y-4">
        <h1 class="text-2xl font-semibold">Audit Logs</h1>
        <div class="flex items-center gap-4">
            <input type="date" v-model="selectedDate" class="border rounded px-3 py-2" />

            <button class="px-4 py-2 rounded bg-blue-600 text-white" @click="loadLogs">
                Load logs
            </button>

            <button v-if="logs.length" class="px-4 py-2 rounded bg-gray-700 text-white" @click="downloadCsv">
                Download CSV
            </button>
        </div>

        <div v-if="isLoading">Loading…</div>

        <div v-else-if="!logs.length" class="text-gray-500">
            No logs loaded
        </div>

        <ul v-else class="space-y-2">
            <li v-for="log in sortedLogs" :key="log.id" class="border rounded p-3 text-sm">
                <div class="font-mono text-xs text-gray-500">
                    {{ new Date(log.timestamp).toLocaleString() }}
                </div>
                <div class="flex flex-wrap gap-2">
                    <span class="font-semibold">{{ log.method }}</span>
                    <span>{{ log.path }}</span>
                    <span class="text-gray-600">({{ log.statusCode }})</span>
                </div>
                <div class="text-gray-600">
                    User: {{ log.userId ?? 'Anonymous' }} | IP: {{ log.ipAddress }}
                </div>
                <div v-if="log.details" class="italic text-gray-700">
                    {{ log.details }}
                </div>
            </li>
        </ul>

    </div>
</template>
