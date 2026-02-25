<script setup>
</script>

<template>
  <div class="trackResult">
    <img
      src="https://i.ytimg.com/vi/kPa7bsKwL-c/hq720.jpg?sqp=-oaymwEnCNAFEJQDSFryq4qpAxkIARUAAIhCGAHYAQHiAQoIGBACGAY4AUAB&rs=AOn4CLCP__7GEQfQgkh0AfV4704HTYiVUA"
      alt=""
      class="thumbnail"
    />
    <div class="trackInfo">
      <p class="title">Lady Gaga, Bruno Mars - Die With A Smile (Official Music Video)</p>
      <div class="uploaderInfo">
        <img
          src="https://yt3.ggpht.com/lM8HHBd5BZtP_-Dh1i3n1Z4teVW8v2KTG-hsS83yXUiK0K6krkk1XNEFbho3uwH-CS-FQpnlHw=s88-c-k-c0x00ffffff-no-rj"
          alt=""
          class="profilePictureUploader"
        />
        <p class="uploaderName">Lady Gaga</p>
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
  justify-content: space-between;
  gap: 3%;
}

.title {
  margin: 0;
}

.thumbnail {
  width: 50%;
  min-width: 100px;
}

.trackInfo {
  font-size: 0.8rem;
}

.uploaderInfo {
  display: flex;
  align-items: center;
}

.profilePictureUploader {
  width: 10%;
  border-radius: 50%;
  min-width: 20px;
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
  font-size: 0.7rem;
  margin-left: 5px;
}
</style>
