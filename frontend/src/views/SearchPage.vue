<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useAuthStore } from '../stores/auth'
import Selector from '@/components/Selector.vue'

const auth = useAuthStore()

const searchQuery = ref('')
const searching = ref(false)

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
  if (showSelector.value) return
  if (!searchQuery.value.trim()) return
  const selected = providers.value.filter((p) => p.selected)
  if (!selected.length) return

  searching.value = true
  try {
    const requests = selected.map((p) =>
      fetch(`/api/Provider/Search/${p.name}?q=${encodeURIComponent(searchQuery.value)}`, {
        credentials: 'include',
      }).then((res) => (res.ok ? res.json() : null)),
    )

    searchResults.value = (await Promise.all(requests)).filter(Boolean)
    showSelector.value = true
  } finally {
    searching.value = false
  }
}

function handleKeydown(e) {
  if (e.key === 'Enter') searchProviders()
}

onMounted(() => window.addEventListener('keydown', handleKeydown))
onUnmounted(() => window.removeEventListener('keydown', handleKeydown))
</script>

<template>
  <h1><span @click="$router.push('/')" style="cursor: pointer">Musync</span></h1>
  <div class="linksContainer">
    <a class="links" href="http://">Get Extension</a>
    <a class="links" target="_blank" href="https://github.com/Fleacon/MuSync">
      <i class="fa-brands fa-github"></i>Source Code
    </a>
  </div>
  <div class="interactionContainer" v-if="isLoggedIn">
    <div class="searchbar">
      <div class="searchInputWrapper">
        <input
          class="searchBox"
          type="text"
          placeholder="Search for Track..."
          v-model="searchQuery"
          :disabled="searching"
        />
        <button class="clearBtn" v-if="searchQuery" @click="searchQuery = ''" :disabled="searching">
          <i class="fa-solid fa-xmark" style="margin-right: 0; font-size: 1.1rem"></i>
        </button>
      </div>
      <input
        class="addButton"
        type="submit"
        value="Add"
        @click="searchProviders"
        :disabled="searching"
      />
    </div>
    <p style="text-align: center">Select Providers:</p>
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
          :disabled="searching"
        /><i class="fa-brands" :class="provider.icon"></i>{{ provider.label }}</label
      >
      <span class="providerSelect" @click="$router.push('/account')"
        ><i class="fa-solid fa-plus"></i>Add</span
      >
    </div>
    <div class="overlay" v-if="showSelector">
      <Selector class="selectorBox" @close="showSelector = false" :results="searchResults" />
    </div>
  </div>
  <div class="authenticationContainer" v-if="!isLoggedIn">
    <button @click="$router.push('/login')" class="logRegBtn">Login or Register</button>
  </div>
</template>

<style>
h1 {
  text-align: center;
  font-size: 5rem;
  margin-top: 0;
  margin-bottom: 2rem;
}

.links {
  border: 2px solid var(--accent2-color);
  width: fit-content;
  padding: 10px;
  border-radius: 1000px;
  display: inline-flex;
  align-items: center;
  text-decoration: none;
  transition:
    background-color 0.2s ease,
    filter 0.2s ease;
}

.linksContainer {
  display: flex;
  width: 100vw;
  justify-content: center;
  gap: 1rem;
  margin-bottom: 2rem;
  flex-wrap: nowrap;
  flex-shrink: 0;
}

.links:hover {
  background-color: color-mix(in srgb, var(--accent1-color) 40%, transparent);
}

.searchbar {
  display: flex;
  justify-content: center;
  gap: 10px;
  margin-bottom: 20px;
  width: 100vw;
}

.searchInputWrapper {
  position: relative;
  min-width: 15rem;
  width: 40%;
  display: flex;
  align-items: center;
}

.searchBox {
  width: 100%;
  padding: 10px;
  padding-right: 2.5rem;
  border-radius: 1000px;
  border: 2px solid var(--accent2-color);
}

.clearBtn {
  position: absolute;
  right: 0.75rem;
  background: transparent;
  border: none;
  cursor: pointer;
  color: gray;
  font-size: 1rem;
  display: flex;
  align-items: center;
  transition: filter 0.2s ease;
}

.searchBox:disabled {
  color: white;
}

.addButton {
  width: 5rem;
  padding: 10px;
  border-radius: 1000px;
  border: none;
  font-size: 1rem;
  font-weight: bold;
  background-color: var(--accent1-color);
  color: var(--accent2-color);
}

.providerContainer {
  display: flex;
  justify-content: center;
  width: 100vw;
  gap: 1rem;
}

.providerSelect {
  background-color: var(--accent1-color);
  padding: 10px;
  border-radius: 1000px;
  cursor: pointer;
  border: 2px solid transparent;
  transition:
    border-color 0.2s ease,
    background-color 0.2s ease;
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

.authenticationContainer {
  display: flex;
  justify-content: center;
  width: 100vw;
}

.logRegBtn {
  font-size: 1rem;
  padding: 10px 20px;
  border-radius: 10000px;
  border: none;
  background-color: var(--accent1-color);
  color: var(--accent2-color);
  cursor: pointer;
}

.searchBox:disabled,
.addButton:disabled {
  opacity: 0.4;
  cursor: not-allowed;
}

.providerSelect:has(input:disabled) {
  opacity: 0.4;
  cursor: not-allowed;
  pointer-events: none;
}

i {
  margin-right: 0.25rem;
}
</style>
