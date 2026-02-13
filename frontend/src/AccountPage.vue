<script setup>
import Cookies from 'js-cookie'
import { onMounted, ref } from 'vue'

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
</template>
