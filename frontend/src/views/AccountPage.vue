<script setup>
import Cookies from 'js-cookie'
import { onMounted, ref } from 'vue'
import ProviderAuth from '../components/ProviderAuth.vue'
import { useAuthStore } from '../stores/auth'
import router from '../router'

const authStore = useAuthStore()

async function logout() {
  Cookies.remove('Session')
  Cookies.remove('ProviderList')
  const response = await fetch('/api/Account/Logout', {
    method: 'POST',
    credentials: 'include',
  })
  if (response.ok) {
    console.log('Logged out successfully')
    authStore.clearAuth()
    router.push('/')
  } else {
    console.error('Logout failed:', response.status)
  }
}

const providers = ref([
  { name: 'YouTube Music', icon: 'fa-youtube', connected: false },
  { name: 'Spotify', icon: 'fa-spotify', connected: false },
  { name: 'SoundCloud', icon: 'fa-soundcloud', connected: false },
])

const providerEnumMap = {
  'YouTube Music': 'YouTubeMusic',
  Spotify: 'Spotify',
  SoundCloud: 'SoundCloud',
}

const providerEnum = (providerName) => providerEnumMap[providerName]

onMounted(async () => {
  try {
    const response = await fetch('/api/ProviderController/LinkedProviders', {
      credentials: 'include',
    })

    if (!response.ok) return

    const linkedProviders = await response.json()

    providers.value.forEach((localProvider) => {
      const match = linkedProviders.find((p) => p.provider === providerEnumMap[localProvider.name])

      if (match) {
        localProvider.connected = true
        localProvider.username = match.username || match.Username
      }
    })
  } catch (err) {
    console.error('Failed to load providers', err)
  }
})
</script>

<template>
  <button @click="logout">Logout</button>
  <div class="accountPage">
    <ProviderAuth
      v-for="value in providers"
      :key="value.name"
      :provider="value.name"
      :iconClass="value.icon"
      :connected="value.connected"
      :username="value.username"
    />
  </div>
</template>

<style>
.accountPage {
  display: flex;
  flex-direction: column;
  gap: 20px;
  width: 80%;
  margin: auto;
}
</style>
