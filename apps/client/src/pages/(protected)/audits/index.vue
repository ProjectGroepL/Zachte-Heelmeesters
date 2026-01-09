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
    <div class="p-6 space-y-4" aria-labelledby="audit-logs-title">
        <h1 id="audit-logs-title" class="text-2xl font-semibold">
            Audit Logs
        </h1>

        <div class="flex items-center gap-4">
            <input type="date" v-model="selectedDate" class="border rounded px-3 py-2"
                aria-label="Select date to view audit logs" />

            <button class="px-4 py-2 rounded bg-blue-600 text-white" @click="loadLogs"
                aria-label="Load audit logs for selected date">
                Load logs
            </button>

            <button v-if="logs.length" class="px-4 py-2 rounded bg-gray-700 text-white" @click="downloadCsv"
                aria-label="Download audit logs as CSV">
                Download CSV
            </button>
        </div>

        <div v-if="isLoading" aria-live="polite">
            Loading…
        </div>

        <div v-else-if="!logs.length" class="text-gray-500" aria-live="polite">
            No logs loaded
        </div>

        <ul v-else class="space-y-2" role="list" aria-label="Audit log entries">
            <li v-for="log in sortedLogs" :key="log.id" class="border rounded p-3 text-sm" role="listitem"
                :aria-label="`Audit log entry for ${log.method} ${log.path}`">
                <div class="font-mono text-xs text-gray-500" aria-label="Timestamp">
                    {{ new Date(log.timestamp).toLocaleString() }}
                </div>

                <div class="flex flex-wrap gap-2">
                    <span class="font-semibold" aria-label="HTTP method">
                        {{ log.method }}
                    </span>
                    <span aria-label="Request path">
                        {{ log.path }}
                    </span>
                    <span class="text-gray-600" aria-label="HTTP status code">
                        ({{ log.statusCode }})
                    </span>
                </div>

                <div class="text-gray-600" aria-label="User and IP address">
                    User: {{ log.userId ?? 'Anonymous' }} | IP: {{ log.ipAddress }}
                </div>

                <div v-if="log.details" class="italic text-gray-700" aria-label="Additional log details">
                    {{ log.details }}
                </div>
            </li>
        </ul>
    </div>
</template>
