<script setup>
import { ref, computed } from 'vue'
import ProviderResult from './ProviderResult.vue'
import ProviderPlaylists from './Providerplaylists.vue'

const emit = defineEmits(['close'])

const props = defineProps({
  results: {
    type: Array,
    default: () => [],
  },
})

// step: 'tracks' | 'playlists'
const step = ref('tracks')

// Map of provider -> selected trackId (null means none selected for that provider)
const trackSelections = ref({})

// Map of provider -> selected playlistId
const playlistSelections = ref({})

// Playlist data fetched per provider: [{ provider, playlists: [...] }]
const playlistResults = ref([])
const loadingPlaylists = ref(false)

// Providers that actually have a track selected
const providersWithTrack = computed(() =>
  Object.entries(trackSelections.value)
    .filter(([, trackId]) => trackId !== null)
    .map(([provider]) => provider),
)

const canGoNext = computed(() => providersWithTrack.value.length > 0)

// True if at least one playlist is selected per provider that has a track
const canConfirm = computed(() =>
  providersWithTrack.value.every((provider) => playlistSelections.value[provider] != null),
)

function handleTrackSelected({ provider, trackId }) {
  trackSelections.value[provider] = trackId
}

function handlePlaylistSelected({ provider, playlistId }) {
  playlistSelections.value[provider] = playlistId
}

async function goToPlaylists() {
  if (!canGoNext.value) return

  loadingPlaylists.value = true
  playlistResults.value = []

  console.log('trackSelections:', JSON.stringify(trackSelections.value))
  console.log('providersWithTrack:', providersWithTrack.value)

  const requests = providersWithTrack.value.map(async (provider) => {
    try {
      const res = await fetch(`/api/Provider/Get/Playlists/${provider}`, {
        credentials: 'include',
      })
      if (!res.ok) throw new Error(`Failed for ${provider}`)
      const data = await res.json()
      return { provider: data.provider, playlists: data.playlists }
    } catch (e) {
      console.error(e)
      failedProviders.value.push(provider) // surface this in the UI
      return null
    }
  })

  const results = await Promise.all(requests)
  playlistResults.value = results.filter(Boolean)
  loadingPlaylists.value = false
  step.value = 'playlists'
}

function backToTracks() {
  step.value = 'tracks'
  playlistSelections.value = {}
  trackSelections.value = {}
}

function close() {
  emit('close')
}

function confirm() {
  // Build final selections: [{ provider, trackId, playlistId }]
  const selections = providersWithTrack.value.map((provider) => ({
    provider,
    trackId: trackSelections.value[provider],
    playlistId: playlistSelections.value[provider],
  }))
  console.log('Confirmed selections:', selections)
  fetch('/api/Provider/AddToPlaylists', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    credentials: 'include',
    body: JSON.stringify(selections),
  })
  emit('close')
}
</script>

<template>
  <div class="selector">
    <!-- Header -->
    <div class="selectorHeader">
      <button class="backBtn" v-if="step === 'playlists'" @click="backToTracks">
        <i class="fa-solid fa-arrow-left"></i>
      </button>
      <h3>{{ step === 'tracks' ? 'Select Track' : 'Select Playlist' }}</h3>
      <button class="closeBtn" @click="close">
        <i class="fa-solid fa-xmark" style="color: var(--accent2-color)"></i>
      </button>
    </div>

    <!-- Track selection step -->
    <template v-if="step === 'tracks'">
      <div class="selectorResultsContainer" :class="{ 'many-items': results.length > 3 }">
        <ProviderResult
          v-for="(result, index) in results"
          :key="index"
          :trackResult="result"
          @track-selected="handleTrackSelected"
        />
      </div>
      <button
        class="nextBtn"
        :class="{ 'nextBtn--disabled': !canGoNext }"
        :disabled="!canGoNext"
        @click="goToPlaylists"
      >
        {{ loadingPlaylists ? 'Loading…' : 'Go to Playlists' }}
      </button>
    </template>

    <!-- Playlist selection step -->
    <template v-else-if="step === 'playlists'">
      <div class="selectorResultsContainer" :class="{ 'many-items': playlistResults.length > 3 }">
        <ProviderPlaylists
          v-for="(result, index) in playlistResults"
          :key="index"
          :result="result"
          @playlist-selected="handlePlaylistSelected"
        />
      </div>
      <button
        class="nextBtn"
        :class="{ 'nextBtn--disabled': !canConfirm }"
        :disabled="!canConfirm"
        @click="confirm"
      >
        Confirm
      </button>
    </template>
  </div>
</template>

<style scoped>
.selector {
  background-color: var(--primary-color);
  width: 85vw;
  height: 90vh;
  display: flex;
  flex-direction: column;
  align-items: center;
}

.selectorHeader {
  position: relative;
  display: flex;
  justify-content: center;
  align-items: center;
  width: 100%;
  height: 15%;
}

.selectorResultsContainer {
  width: 100%;
}

.closeBtn {
  position: absolute;
  right: 1rem;
  background: transparent;
  border: none;
  font-size: 1.25rem;
  cursor: pointer;
}

.backBtn {
  position: absolute;
  left: 1rem;
  background: transparent;
  border: none;
  font-size: 1.25rem;
  cursor: pointer;
  color: var(--accent2-color);
}

.selectorResultsContainer {
  display: flex;
  gap: 10px;
  height: 70%;
  overflow-x: auto;
  padding: 0 40px;
  justify-content: center;
}

.selectorResultsContainer.many-items {
  justify-content: flex-start;
}

.nextBtn {
  width: 15%;
  padding: 10px;
  border-radius: 1000px;
  border: none;
  background-color: var(--accent1-color);
  color: var(--accent2-color);
  height: 10%;
  margin-top: auto;
  margin-bottom: auto;
  font-weight: bold;
  cursor: pointer;
  transition: opacity 0.15s ease;
}

.nextBtn--disabled {
  opacity: 0.4;
  cursor: not-allowed;
}
</style>
