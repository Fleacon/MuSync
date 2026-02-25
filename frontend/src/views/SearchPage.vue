<script setup>
import { ref, computed } from 'vue'
import { useAuthStore } from '../stores/auth'
import Selector from '@/components/Selector.vue'

const auth = useAuthStore()

const searchQuery = ref('')

const providers = ref([
  {
    name: 'YouTubeMusic',
    icon: 'fa-youtube',
    label: 'YouTube Music',
    connected: false,
    selected: false,
  },
  { name: 'Spotify', icon: 'fa-spotify', label: 'Spotify', connected: false, selected: false },
])

const isLoggedIn = computed(() => auth.isAuthenticated)
auth.linkedProviders.forEach((provider) => {
  const p = providers.value.find((p) => p.name === provider)
  if (p) p.connected = true
})

const showSelector = ref(false)
const searchResults = ref([])

async function searchProviders() {
  if (!searchQuery.value.trim()) return
  const selected = providers.value.filter((p) => p.selected)
  if (!selected.length) return

  const requests = selected.map((p) =>
    fetch(`/api/Provider/Search/${p.name}?q=${encodeURIComponent(searchQuery.value)}`, {
      credentials: 'include',
    }).then((res) => (res.ok ? res.json() : null)),
  )

  searchResults.value = (await Promise.all(requests)).filter(Boolean)
  showSelector.value = true
}
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
      <input
        class="searchBox"
        type="text"
        placeholder="Search for Track..."
        v-model="searchQuery"
      />
      <input class="addButton" type="submit" value="Add" @click="searchProviders" />
    </div>
    <p style="text-align: center">Select Providers</p>
    <div class="providerContainer">
      <label
        class="providerSelect"
        v-for="provider in providers"
        :key="provider.name"
        :for="provider.name.toLowerCase()"
        ><input
          type="checkbox"
          :name="provider.name.toLowerCase()"
          :id="provider.name.toLowerCase()"
          v-model="provider.selected"
        /><i class="fa-brands" :class="provider.icon"></i>{{ provider.label }}</label
      >
    </div>
    <p>{{ providers.map((p) => `${p.label}: ${p.selected}`).join(', ') }}</p>
    <p>Query: {{ searchQuery }}</p>
    <div class="overlay" v-if="showSelector">
      <Selector class="selectorBox" @close="showSelector = false" :results="searchResults" />
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
  appearance: none;
  width: 0;
  height: 0;
  margin: 0;
  padding: 0;
}

.providerSelect:has(input:checked) {
  border: 2px solid var(--accent2-color);
}

.overlay {
  position: fixed;
  inset: 0; /* top:0; right:0; bottom:0; left:0 */
  background: rgba(0, 0, 0, 0.5);
  z-index: 9999;
  display: flex;
  justify-content: center;
  align-items: center;
}

.selectorBox {
  border-radius: 12px;
  padding: 5px;
}
</style>
