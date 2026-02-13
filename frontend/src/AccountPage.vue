<script setup>
import Cookies from 'js-cookie'
import { onMounted, ref } from 'vue'

function logout() {
  Cookies.remove('Session')
  fetch('http://localhost:5123/api/Account/Logout', {
    method: 'POST',
    credentials: 'include',
  })
}
</script>

<template>
  <button @click="logout">Logout</button>
</template>
