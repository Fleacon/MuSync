<script setup>
import { ref, computed } from 'vue'
import router from '../router'
import { useAuthStore } from '../stores/auth'

const auth = useAuthStore()

const providers = ref([
  { name: 'YouTube Music', icon: 'fa-youtube', connected: false },
  { name: 'Spotify', icon: 'fa-spotify', connected: false },
  { name: 'SoundCloud', icon: 'fa-soundcloud', connected: false },
])

const isLoggedIn = computed(() => auth.isAuthenticated)
//const providers = computed(() => auth.linkedProviders)
</script>

<template>
  <h1 @click="$router.push('/')">Musync</h1>
  <div class="linksContainer">
    <p class="links"><a href="http://">Get Extension</a></p>
    <p class="links">
      <a href="http://"><i class="fa-brands fa-github"></i>Source Code</a>
    </p>
  </div>
  <div class="interactionContainer" v-if="isLoggedIn">
    <div class="searchbar">
      <input class="searchBox" type="text" placeholder="Search for Track..." />
      <input class="addButton" type="submit" value="Add" />
    </div>
    <p style="text-align: center">Select Providers</p>
    <div class="providerContainer">
      <label class="providerSelect" for="ytMusic"
        ><input type="checkbox" name="ytMusic" id="ytMusic" /><i class="fa-brands fa-youtube"></i
        >YouTube Music</label
      >
      <label class="providerSelect" for="spotify"
        ><input type="checkbox" name="spotify" id="spotify" /><i class="fa-brands fa-spotify"></i
        >Spotify</label
      >
      <label class="providerSelect" for="soundCloud"
        ><input type="checkbox" name="soundCloud" id="soundCloud" /><i
          class="fa-brands fa-soundcloud"
        ></i
        >SoundCloud</label
      >
    </div>
  </div>
  <div class="authenticationContainer" v-if="!isLoggedIn">
    <button @click="$router.push('/login')">Login or Register</button>
  </div>
</template>

<style>
h1 {
  text-align: center;
  cursor: pointer;
}

.links {
  border: 2px solid var(--accent2-color);
  width: fit-content;
  padding: 10px;
  border-radius: 1000px;
}

.linksContainer {
  display: flex;
  width: 100vw;
  justify-content: center;
  gap: 20px;
  margin-bottom: 20px;
}

.searchbar {
  display: flex;
  justify-content: center;
  gap: 10px;
  margin-bottom: 20px;
  width: 100vw;
}

.searchBox {
  width: 40%;
  padding: 10px;
  border-radius: 1000px;
  border: 2px solid var(--accent2-color);
}

.addButton {
  width: 10%;
  padding: 10px;
  border-radius: 1000px;
  border: none;
  background-color: var(--accent1-color);
  color: var(--accent2-color);
}

.providerContainer {
  display: flex;
  justify-content: center;
  width: 100vw;
}

.providerSelect {
  background-color: var(--accent1-color);
  margin-left: 20px;
  margin-right: 10px;
  padding: 10px;
  border-radius: 1000px;
  cursor: pointer;
  border: 2px solid transparent;
}

input {
  cursor: pointer;
}

.providerSelect > input {
  display: none;
}

.providerSelect:has(input:checked) {
  border: 2px solid var(--accent2-color);
}
</style>
