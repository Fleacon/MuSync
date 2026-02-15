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
</script>

<template>
  <button @click="logout">Logout</button>
  <div class="accountPage">
    <ProviderAuth provider="YouTube Music" iconClass="fa-brands fa-youtube" />
    <ProviderAuth provider="Spotify" iconClass="fa-brands fa-spotify" />
    <ProviderAuth provider="SoundCloud" iconClass="fa-brands fa-soundcloud" />
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
