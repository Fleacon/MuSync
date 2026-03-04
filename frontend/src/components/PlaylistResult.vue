<script setup>
const props = defineProps({
  provider: {
    type: String,
    default: '',
  },
  thumbnailUrl: {
    type: String,
    default: '',
  },
  title: {
    type: String,
    default: '',
  },
  playlistId: {
    type: String,
    default: '',
  },
  selected: {
    type: Boolean,
    default: false,
  },
})

const emit = defineEmits(['select'])

function handleClick() {
  emit('select', props.playlistId)
}
</script>

<template>
  <div
    class="playlistResult"
    :class="{ 'playlistResult--selected': selected }"
    @click="handleClick"
  >
    <div class="selectedIndicator" v-if="selected">
      <i class="fa-solid fa-circle-check"></i>
    </div>
    <div class="imgContainer">
      <img
        :src="thumbnailUrl"
        alt="Playlist thumbnail"
        class="thumbnail"
        :class="provider === 'YouTubeMusic' ? 'youtubeThumbnail' : ''"
      />
    </div>
    <div class="playlistinfo">
      <p class="playlistTitle">{{ title }}</p>
    </div>
  </div>
</template>

<style scoped>
.playlistResult {
  position: relative;
  display: flex;
  align-items: center;
  background-color: var(--accent1-color);
  padding: 10px;
  cursor: pointer;
  gap: 1rem;
  height: 20vh;
  min-height: 20vh;
  max-height: 300px;
  width: 100%;
  border: 2px solid transparent;
  border-radius: 8px;
  transition:
    border-color 0.15s ease,
    background-color 0.15s ease;
}

.playlistResult:hover {
  filter: brightness(1.15);
  background-color: var(--accent1-color);
}

.playlistResult--selected {
  border-color: var(--accent2-color);
  background-color: color-mix(in srgb, var(--accent1-color) 70%, var(--primary-color));
}

.playlistResult .selectedIndicator {
  position: absolute;
  top: 6px;
  right: 8px;
  font-size: 1.1rem;
  color: var(--accent2-color);
  line-height: 1;
}

.imgContainer {
  display: flex;
  height: 100%;
  width: 45%;
  flex-shrink: 0;
  justify-content: center;
  align-items: center;
}

.playlistinfo {
  font-size: 1rem;
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
  justify-content: center;
}

.playlistTitle {
  font-weight: bold;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  text-overflow: ellipsis;
  margin: 0;
}

.thumbnail {
  height: 100%;
  min-height: 100px;
  object-fit: cover;
  aspect-ratio: 1/1;
}

.youtubeThumbnail {
  aspect-ratio: 16/9;
  width: 100%;
  height: auto;
  min-height: 50px;
  max-height: 100%;
  object-fit: cover;
}
</style>
