<script setup lang="ts">
    import RoleForm from './RoleForm.vue'
    import RoleList from './RoleList.vue'
    import UserRoletable from './UserRoleTable.vue'

</script>

<template>
    <main>
        <RoleForm />
        <RoleList />
        <UserRoleTable :users="[]" />
    </main>
</template>