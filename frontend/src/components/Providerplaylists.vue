<script setup>
import PlaylistResult from './PlaylistResult.vue'
import { computed, ref } from 'vue'

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

const emit = defineEmits(['playlist-selected'])

const selectedPlaylistIds = ref(new Set()) // <-- was a single ref, now a Set

const providerMeta = computed(
  () => providerIcons[props.result.provider] ?? { icon: 'fa-music', label: props.result.provider },
)

function handlePlaylistSelect(playlistId) {
  const next = new Set(selectedPlaylistIds.value)
  if (next.has(playlistId)) {
    next.delete(playlistId)
  } else {
    next.add(playlistId)
  }
  selectedPlaylistIds.value = next
  emit('playlist-selected', {
    provider: props.result.provider,
    playlistIds: [...selectedPlaylistIds.value],
  })
}
</script>

<template>
  <div class="providerResult">
    <div class="providerLogo">
      <i class="fa-brands" :class="providerMeta.icon" style="font-size: 2rem"></i>
      <p style="font-size: 1em">{{ providerMeta.label }}</p>
    </div>
    <div class="resultList">
      <PlaylistResult
        v-for="playlist in result.playlists"
        :provider="result.provider"
        :key="playlist.id"
        :thumbnailUrl="playlist.thumbnailUrl"
        :title="playlist.title"
        :playlistId="playlist.id"
        :selected="selectedPlaylistIds.has(playlist.id)"
        @select="handlePlaylistSelect"
      />
    </div>
  </div>
</template>
