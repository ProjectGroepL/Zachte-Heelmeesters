import { defineStore } from 'pinia'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    token: '' as string,
    roles: [] as string[],
  }),

  getters: {
    isAdmin: (state) => state.roles.includes('Systeembeheerder'),
  },

  actions: {
    setAuth(token: string, roles: string[]) {
      this.token = token
      this.roles = roles
    },

    clear() {
      this.token = ''
      this.roles = []
    },
  },
})
