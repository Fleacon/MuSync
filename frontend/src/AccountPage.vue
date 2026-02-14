<script setup>
import Cookies from 'js-cookie'
import { onMounted, ref } from 'vue'
import ProviderAuth from './components/ProviderAuth.vue'

function logout() {
  Cookies.remove('Session')
  Cookies.remove('ProviderList')
  fetch('http://localhost:5123/api/Account/Logout', {
    method: 'POST',
    credentials: 'include',
  })
  localStorage.removeItem('username')
  localStorage.removeItem('providerList')
  window.dispatchEvent(new CustomEvent('logout-success'))
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
