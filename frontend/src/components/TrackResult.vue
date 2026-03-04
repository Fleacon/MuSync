<script setup>
import { ref, onMounted, onUnmounted } from 'vue'

const props = defineProps({
  thumbnailUrl: {
    type: String,
    default: '',
  },
  title: {
    type: String,
    default: '',
  },
  uploaderName: {
    type: String,
    default: '',
  },
  uploaderProfilePictureUrl: {
    type: String,
    default: '',
  },
  selected: {
    type: Boolean,
    default: false,
  },
  trackId: {
    type: String,
    default: '',
  },
  provider: {
    type: String,
    default: '',
  },
})

const emit = defineEmits(['select'])

function handleClick() {
  emit('select', props.trackId)
}

const imgContainer = ref(null)
const thumbnailStyle = ref({ height: '100%', width: 'auto' })

let resizeObserver = null

function updateThumbnailSize() {
  if (!imgContainer.value) return
  const { offsetWidth, offsetHeight } = imgContainer.value
  if (offsetWidth >= offsetHeight) {
    thumbnailStyle.value = { height: '100%', width: 'auto' }
  } else {
    thumbnailStyle.value = { width: '100%', height: 'auto' }
  }
}

onMounted(() => {
  resizeObserver = new ResizeObserver(updateThumbnailSize)
  resizeObserver.observe(imgContainer.value)
  updateThumbnailSize()
})

onUnmounted(() => {
  resizeObserver?.disconnect()
})
</script>

<template>
  <div class="trackResult" :class="{ 'trackResult--selected': selected }" @click="handleClick">
    <div class="selectedIndicator" v-if="selected">
      <i class="fa-solid fa-circle-check"></i>
    </div>
    <div class="imgContainer" ref="imgContainer">
      <img
        :src="thumbnailUrl"
        alt="Track thumbnail"
        class="thumbnail"
        :class="provider === 'YouTubeMusic' ? 'youtubeThumbnail' : ''"
        :style="provider !== 'YouTubeMusic' ? thumbnailStyle : {}"
      />
    </div>
    <div class="trackInfo">
      <div class="titleContainer">
        <p class="title" v-html="title"></p>
      </div>
      <div class="uploaderInfo">
        <img
          v-if="uploaderProfilePictureUrl"
          :src="uploaderProfilePictureUrl"
          alt="Uploader profile picture"
          class="profilePictureUploader"
        />
        <p class="uploaderName">{{ uploaderName }}</p>
      </div>
    </div>
  </div>
</template>

<style scoped>
.trackResult {
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
    border-color 0.2s ease,
    background-color 0.2s ease;
}

.trackResult:hover {
  filter: brightness(1.15);
  background-color: var(--accent1-color); /* remove the color-mix */
}

.trackResult--selected {
  border-color: var(--accent2-color);
  background-color: color-mix(in srgb, var(--accent1-color) 70%, var(--primary-color));
}

.selectedIndicator {
  position: absolute;
  top: 6px;
  right: 8px;
  font-size: 1.1rem;
  color: var(--accent2-color);
  line-height: 1;
}

.title {
  margin: 0;
}

.imgContainer {
  display: flex;
  height: 100%;
  width: 45%;
  min-width: 90px;
  min-height: 90px;
  flex-shrink: 0;
  justify-content: center;
  align-items: center;
  overflow: hidden;
}

.thumbnail {
  object-fit: cover;
}

.youtubeThumbnail {
  aspect-ratio: 16/9;
  width: 100%;
  height: auto;
  min-height: 50px;
  max-height: 100%;
  object-fit: cover;
}

.trackInfo {
  font-size: 1rem;
  width: 100%;
  height: 100%;
  min-width: 0;
  overflow: hidden;
}

.uploaderInfo {
  display: flex;
  align-items: center;
  width: 100%;
  overflow: hidden;
  min-width: 0;
}

.profilePictureUploader {
  height: 2rem;
  aspect-ratio: 1/1;
  border-radius: 50%;
}

.titleContainer {
  height: 55%;
  display: flex;
  align-items: center;
}

.title {
  font-weight: bold;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  text-overflow: ellipsis;
}

.uploaderName {
  font-weight: 200;
  font-size: 0.8rem;
  margin-left: 0.5rem;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  min-width: 0;
}
</style>
