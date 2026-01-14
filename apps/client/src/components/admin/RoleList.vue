<script setup lang="ts">
    import { useRoles } from '@/composables/useRoles'

    const { data: roles, loading } = useRoles()
</script>
<template>
    <section aria-labelledby="roles-title">
        <h2 id="roles-title">Roles</h2>

        <p v-if="loading">Loading roles.. </p>

        <ul v-else role="list">
            <li v-for="role in roles" :key="role.Id">
                <strong>{{  role.Name }}</strong> - {{ role.Description }}
            </li>
        </ul>

    </section>
</template>