<script setup>
import { onMounted, ref } from 'vue'
import ProviderAuth from '../components/ProviderAuth.vue'
import { useAuthStore } from '../stores/auth'
import router from '../router'
import { useNotificationStore } from '@/stores/notificationStore'

const authStore = useAuthStore()
const notification = useNotificationStore()
var confirmDeletion = false

async function logout() {
  const response = await fetch('/api/Account/Logout', {
    method: 'POST',
    credentials: 'include',
  })
  if (response.ok) {
    notification.show('Success', 'Logged out successfully!')
    authStore.clearAuth()
    router.push('/')
  } else {
    notification.show('Error', 'Logout failed. Please try again.', false)
  }
}

async function deleteAccount() {
  // Option A: simple double-click confirmation (no callback needed)
  async function deleteAccount() {
    if (!confirmDeletion) {
      notification.show('Are you sure?', 'Click Delete Account again to confirm.', true)
      confirmDeletion = true
      return
    }

    confirmDeletion = false
    const response = await fetch('/api/Account/Delete', {
      method: 'DELETE',
      credentials: 'include',
    })

    if (response.ok) {
      notification.show('Success', 'Account deleted successfully!')
      await logout()
    } else {
      notification.show('Error', 'Failed to delete account. Please try again.', false)
    }
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
  await Promise.all(
    providers.value.map(async (localProvider) => {
      const enumValue = providerEnum(localProvider.name)
      localProvider.loading = true

      try {
        const response = await fetch(`/api/Provider/UserData/${enumValue}`, {
          credentials: 'include',
        })

        if (!response.ok) return

        const data = await response.json()
        if (!data || data.provider === 'Invalid') return

        localProvider.connected = true
        localProvider.username = data.username
        localProvider.profilePictureUrl = data.profilePictureUrl
      } catch (err) {
        console.error(`Failed loading ${localProvider.name}`, err)
      } finally {
        localProvider.loading = false
      }
    }),
  )
})
</script>

<template>
  <div class="accountPage">
    <div class="providerList">
      <ProviderAuth
        v-for="value in providers"
        :key="value.name"
        :provider="value.name"
        :iconClass="value.icon"
        :connected="value.connected"
        :username="value.username"
        :profilePictureUrl="value.profilePictureUrl"
        :loading="value.loading"
      />
    </div>
    <div class="accountOptions">
      <button @click="logout" class="logoutBtn">Logout</button>
      <button @click="deleteAccount" class="deleteAccountBtn">Delete Account</button>
    </div>
  </div>
</template>

<style scoped>
.accountPage {
  display: flex;
  flex-direction: column;
  align-items: center;
  width: 100%;
  margin: auto;
  gap: 1rem;
}

.providerList {
  display: flex;
  flex-direction: column;
  gap: 20px;
  width: 80%;
}

h3 {
  margin: 0.5rem 0;
}

.accountOptions {
  display: flex;
  gap: 20px;
  margin-top: 1rem;
}

button {
  padding: 10px 20px;
  border: none;
  border-radius: 10000px;
  cursor: pointer;
  font-size: 1rem;
  padding: 10px 20px;
  border: none;
  background-color: var(--accent1-color);
  color: white;
  font-weight: bold;
}

.deleteAccountBtn {
  background-color: red;
}
</style>
