import { defineStore } from 'pinia'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: '',
    linkedProviders: [],
    isAuthenticated: false,
    hasCheckedAuth: false,
  }),

  getters: {
    username: (state) => state.user,
  },

  actions: {
    async checkAuth() {
      if (this.hasCheckedAuth) return

      try {
        const res = await fetch('/api/Auth/Me', {
          credentials: 'include',
        })

        if (!res.ok) throw new Error()

        const data = await res.json()

        this.user = data.username
        this.linkedProviders = data.providers ?? []
        this.isAuthenticated = true
      } catch {
        this.user = null
        this.linkedProviders = []
        this.isAuthenticated = false
      } finally {
        this.hasCheckedAuth = true
      }
    },
    setAuth(user, providers) {
      this.user = user
      this.linkedProviders = providers ?? []
      this.isAuthenticated = true
      this.hasCheckedAuth = true
    },

    clearAuth() {
      this.user = null
      this.linkedProviders = []
      this.isAuthenticated = false
      this.hasCheckedAuth = true
    },
  },
})
