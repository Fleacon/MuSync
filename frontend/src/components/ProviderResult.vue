<script setup>
import TrackResult from './TrackResult.vue'
import { computed } from 'vue'

const providerIcons = {
  YouTubeMusic: { icon: 'fa-youtube', label: 'YouTube Music' },
  Spotify: { icon: 'fa-spotify', label: 'Spotify' },
  SoundCloud: { icon: 'fa-soundcloud', label: 'SoundCloud' },
}

const props = defineProps({
  result: {
    type: Object,
    default: () => ({}),
  },
})

const providerMeta = computed(
  () => providerIcons[props.result.provider] ?? { icon: 'fa-music', label: props.result.provider },
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
        v-for="track in result.tracks"
        :key="track.id"
        :thumbnailUrl="track.thumbnailUrl"
        :title="track.title"
        :uploaderName="track.uploaderName"
        :uploaderProfilePictureUrl="track.uploaderImgUrl"
        :trackId="track.id"
        :provider="result.provider"
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
  width: 45%;
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
