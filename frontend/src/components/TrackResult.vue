<script setup>
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
  selected: Boolean,
  trackId: {
    type: String,
    default: '',
  },
  provider: {
    type: String,
    default: '',
  },
})
</script>

<template>
  <div class="trackResult">
    <div class="imgContainer">
      <img
        :src="thumbnailUrl"
        alt="Track thumbnail"
        class="thumbnail"
        :class="provider === 'YouTubeMusic' ? 'youtubeThumbnail' : ''"
      />
    </div>
    <div class="trackInfo">
      <div class="titleContainer">
        <p class="title" v-html="title"></p>
      </div>
      <div class="uploaderInfo">
        <img
          :src="uploaderProfilePictureUrl"
          alt="Uploader profile picture"
          class="profilePictureUploader"
        />
        <p class="uploaderName">{{ uploaderName }}</p>
      </div>
    </div>
  </div>
</template>

<style>
.trackResult {
  display: flex;
  align-items: center;
  height: fit-content;
  background-color: var(--accent1-color);
  padding: 10px;
  cursor: pointer;
  gap: 1rem;
  height: 20vh;
  width: 100%;
}

.title {
  margin: 0;
}

.imgContainer {
  display: flex;
  height: 100%;
  width: 45%;
  flex-shrink: 0;
  justify-content: center;
  align-items: center;
}

.thumbnail {
  height: 100%;
  aspect-ratio: 1/1;
  object-fit: cover;
}

.youtubeThumbnail {
  aspect-ratio: 16/9;
  width: 100%;
  object-fit: cover;
}

.trackInfo {
  font-size: 1rem;
  width: 100%;
  height: 100%;
}

.uploaderInfo {
  display: flex;
  align-items: center;
  width: 100%;
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
}
</style>
