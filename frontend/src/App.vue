<script setup>
import { onMounted } from 'vue'
import { useAuthStore } from '@/stores/auth'
import Notification from './components/Notification.vue'
import heartSound from '@/assets/sounds/VeigarLaugh.mp3'

const auth = useAuthStore()

function playHeartSound() {
  const audio = new Audio(heartSound)
  audio.play()
}

onMounted(async () => {
  await auth.checkAuth()
})
</script>

<template>
  <nav>
    <h2 id="logo" v-show="$route.path !== '/'" @click="$router.push('/')">MuSync</h2>
    <div
      class="account"
      @click="$router.push('/account')"
      v-if="auth.isAuthenticated"
      style="margin-right: 1rem"
    >
      <p id="accountName">{{ auth.username }}</p>
      <i class="fa-solid fa-user" id="accountIcon"></i>
    </div>
  </nav>
  <main>
    <Notification />
    <RouterView />
  </main>
  <footer>
    <p>
      Made with <i class="fa-solid fa-heart" @click="playHeartSound" style="cursor: pointer"></i>
    </p>
  </footer>
</template>

<style>
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

main {
  width: 100%;
  height: 100%;
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

h2 {
  font-size: 2rem;
}

footer {
  width: 100vw;
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100px;
  margin-top: 5rem;
}
</style>
