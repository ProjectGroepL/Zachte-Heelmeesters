<script setup lang="ts"> 
import { ref } from 'vue'
import { useCreateRole } from '@/composables/useCreateRole'

const name = ref('')
const description = ref('')
const { mutate, loading, error } = useCreateRole()

const submit = async () => {
    await mutate({
        Name: name.value,
        Description: description.value
    })

    name.value = ''
    description.value = ''

}
</script>

<template>
    <form @submit.prevent="submit" aria-labelledby="role-form-title">
        <h2 id="role-form-title">Create role</h2>

        <label for="role-name">Role name</label>
        <input 
            id="role-name"
            v-model="name"
            required
            />

            <label for="role-desc">Description</label>
            <input 
                id="role-desc"
                v-model="description"
            />

            <button :disabled="loading">
                Create
            </button>

            <p v-if="error" role="alert">Faild to Create role</p>
    </form>
</template>