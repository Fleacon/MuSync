<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue'
import ProviderResult from './ProviderResult.vue'
import ProviderPlaylists from './Providerplaylists.vue'
import { useNotificationStore } from '../stores/notificationStore.js'

const notification = useNotificationStore()

const emit = defineEmits(['close'])

const props = defineProps({
  results: {
    type: Array,
    default: () => [],
  },
})

const step = ref('tracks')
const trackSelections = ref({})
const playlistSelections = ref({})
const playlistResults = ref([])
const loadingPlaylists = ref(false)

function handleKeydown(e) {
  if (e.key === 'Escape') close()
}

onMounted(() => window.addEventListener('keydown', handleKeydown))
onUnmounted(() => window.removeEventListener('keydown', handleKeydown))

const providersWithTrack = computed(() =>
  Object.entries(trackSelections.value)
    .filter(([, trackId]) => trackId !== null)
    .map(([provider]) => provider),
)

const canGoNext = computed(() => providersWithTrack.value.length > 0)
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
  const requests = providersWithTrack.value.map(async (provider) => {
    try {
      const res = await fetch(`/api/Provider/Get/Playlists/${provider}`, {
        credentials: 'include',
      })
      if (!res.ok) throw new Error(`Failed for ${provider}`)
      const data = await res.json()
      return { provider: data.provider, playlists: data.playlists }
    } catch (e) {
      failedProviders.value.push(provider)
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

async function confirm() {
  const selections = providersWithTrack.value.map((provider) => ({
    provider,
    trackId: trackSelections.value[provider],
    playlistId: playlistSelections.value[provider],
  }))

  try {
    const res = await fetch('/api/Provider/AddToPlaylists', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'include',
      body: JSON.stringify(selections),
    })
    if (res.ok) {
      notification.show('Success!', 'Track added to playlist.')
    } else {
      notification.show('Failed', 'Something went wrong.', false)
    }
  } catch {
    notification.show('Failed', 'Network error.', false)
  }
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
  width: 4rem;
  min-width: fit-content;
  padding: 0 2rem;
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
  transition:
    filter 0.2s ease,
    opacity 0.2s ease;
}

.nextBtn--disabled {
  opacity: 0.4;
  cursor: not-allowed;
}

.closeBtn,
.backBtn {
  transition: filter 0.2s ease;
}

@media only screen and (orientation: portrait) {
  .selectorResultsContainer {
    justify-content: flex-start;
  }
}
</style>
