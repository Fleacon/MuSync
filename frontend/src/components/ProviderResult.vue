<script setup>
import TrackResult from './TrackResult.vue'
import { ref, computed, onMounted, onUnmounted, watch, nextTick } from 'vue'

const providerIcons = {
  YouTubeMusic: { icon: 'fa-youtube', label: 'YouTube Music' },
  Spotify: { icon: 'fa-spotify', label: 'Spotify' },
  SoundCloud: { icon: 'fa-soundcloud', label: 'SoundCloud' },
}

const props = defineProps({
  trackResult: {
    type: Object,
    default: () => ({}),
  },
})

const containerRef = ref(null)
const isOverflowing = ref(false)

let resizeObserver = null

function checkOverflow() {
  const el = containerRef.value
  if (!el) return
  isOverflowing.value = el.scrollWidth > el.clientWidth
}

const emit = defineEmits(['track-selected'])

const selectedTrackId = ref(null)

const providerMeta = computed(
  () =>
    providerIcons[props.trackResult.provider] ?? {
      icon: 'fa-music',
      label: props.trackResult.provider,
    },
)

function handleTrackSelect(trackId) {
  if (selectedTrackId.value === trackId) {
    selectedTrackId.value = null
    emit('track-selected', { provider: props.trackResult.provider, trackId: null })
  } else {
    selectedTrackId.value = trackId
    emit('track-selected', { provider: props.trackResult.provider, trackId })
  }
}

onMounted(() => {
  resizeObserver = new ResizeObserver(checkOverflow)
  if (containerRef.value) resizeObserver.observe(containerRef.value)
  checkOverflow()
})

onUnmounted(() => resizeObserver?.disconnect())

watch(
  () => props.results,
  () => {
    nextTick(checkOverflow)
  },
)
</script>

<template>
  <div class="providerResult">
    <div class="providerLogo">
      <i class="fa-brands" :class="providerMeta.icon" style="font-size: 2rem"></i>
      <p style="font-size: 1em">{{ providerMeta.label }}</p>
    </div>
    <div class="resultList">
      <TrackResult
        v-for="track in trackResult.tracks"
        :key="track.id"
        :thumbnailUrl="track.thumbnailUrl"
        :title="track.title"
        :uploaderName="track.uploaderName"
        :uploaderProfilePictureUrl="track.uploaderImgUrl"
        :trackId="track.id"
        :provider="trackResult.provider"
        :selected="selectedTrackId === track.id"
        @select="handleTrackSelect"
      />
    </div>
  </div>
</template>

<style>
.providerResult {
  display: flex;
  flex-direction: column;
  align-items: center;
  background-color: var(--secondary-color);
  border-radius: 12px;
  height: 100%;
  width: 40%;
  min-width: 300px;
  gap: 5px;
  padding: 10px 20px 20px 20px;
}

.providerLogo {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 5px;
}

.resultList {
  display: flex;
  flex-direction: column;
  gap: 10px;
  width: 100%;
  overflow-y: auto;
  scrollbar-color: #708399 transparent;
  align-items: center;
}
</style>
