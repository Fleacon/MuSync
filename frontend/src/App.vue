<script setup>
import { onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'

const auth = useAuthStore()

onMounted(async () => {
  await auth.checkAuth()
})
</script>

<template>
  <nav>
    <h1 id="logo" v-show="$route.path !== '/'" @click="$router.push('/')">MuSync</h1>
    <div class="account" @click="$router.push('/account')" v-if="auth.isAuthenticated">
      <p id="accountName">{{ auth.username }}</p>
      <i class="fa-solid fa-user" id="accountIcon"></i>
    </div>
  </nav>
  <main>
    <RouterView />
  </main>
</template>

<style>
html {
  height: 100%;
}

body {
  margin: 0;
  height: 100%;
  font-family: Inter, Arial, sans-serif;
  background-image: linear-gradient(
    var(--background-linear1-color),
    var(--background-linear2-color)
  );
  color: var(--accent2-color);
  background-repeat: no-repeat;
  background-attachment: fixed;
}

a {
  text-decoration: none;
  color: inherit;
}

#logo {
  cursor: pointer;
}

#accountIcon {
  font-size: 2em;
}

nav {
  width: 100vw;
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 100px;
}

.account {
  display: flex;
  align-items: center;
  position: absolute;
  right: 0;
}

.account:hover {
  cursor: pointer;
}

.no-select {
  -webkit-user-select: none;
  -moz-user-select: none;
  -ms-user-select: none;
  user-select: none;
}
</style>
