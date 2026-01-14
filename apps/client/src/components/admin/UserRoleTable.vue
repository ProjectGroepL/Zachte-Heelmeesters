<script setup lang="ts">
    defineProps<{
        users: {
            id: number 
            name:string 
            roles: string[]
        }[]
    }>()
</script>

<template> 
    <table aria-label="User roles">
        <thead>
            <tr>
                <th scope="col">Users</th>
                <th scope="col">Roles</th>
            </tr>
        </thead>

        <tbody>
            <tr v-for="u in users" :key="u.id">
                <th scope="row">{{  u.name  }}</th>
                <td>{{  u.roles.join(', ') }}</td>
            </tr>

        </tbody>
    </table>
</template>