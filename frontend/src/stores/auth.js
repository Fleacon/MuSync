import { defineStore } from 'pinia'

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: '',
    linkedProviders: [],
    isAuthenticated: false,
    hasCheckedAuth: false,
    favoriteProviders: [],
  }),

  getters: {
    username: (state) => state.user,
  },

  actions: {
    async checkAuth() {
      if (this.hasCheckedAuth) return

      await this.fetchAuth()
    },

    async fetchAuth() {
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

    async fetchPreferences() {
      try {
        const res = await fetch('/api/Preferences', {
          credentials: 'include',
        })
        if (!res.ok) return

        const prefs = await res.json()
        this.favoriteProviders = JSON.parse(prefs['favoriteProviders'] ?? '[]')
      } catch {
        this.favoriteProviders = []
      }
    },

    async handleUnauthorized() {
      this.hasCheckedAuth = false
      await this.fetchAuth()
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
      this.favoriteProviders = []
    },
  },
})
