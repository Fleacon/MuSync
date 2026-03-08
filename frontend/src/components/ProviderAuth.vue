<script setup>
import { computed } from 'vue'
import ToggleSlide from './ToggleSlide.vue'

const props = defineProps({
  provider: {
    type: String,
    required: true,
  },
  iconClass: {
    type: String,
    default: '',
  },
  connected: Boolean,
  username: {
    type: String,
    default: '',
  },
  profilePictureUrl: {
    type: String,
    default: '',
  },
  loading: {
    type: Boolean,
    default: true,
  },
  isFavorite: {
    type: Boolean,
    default: false,
  },
})

const providerEnumMap = {
  'YouTube Music': 'YouTubeMusic',
  Spotify: 'Spotify',
  SoundCloud: 'SoundCloud',
}

const providerEnum = computed(() => providerEnumMap[props.provider])

function add() {
  if (!providerEnum.value) return

  window.location.href = `/api/ProviderAuth/Login/${providerEnum.value}`
}

async function disconnect() {
  if (!providerEnum.value) return

  await fetch(`/api/Account/Disconnect/${providerEnum.value}`, {
    method: 'POST',
    credentials: 'include',
  })

  window.location.reload()
}

const emit = defineEmits(['update:isFavorite'])
</script>

<template>
  <div class="authContainer">
    <div class="providerInfoContainer">
      <div class="providerInfo">
        <i :class="iconClass + ' fa-brands'"></i>
        <div class="providerText">
          <span v-if="loading">Loading...</span>
          <span v-else>{{ connected ? username : 'Not connected' }}</span>
          <p class="providerName">{{ provider }}</p>
        </div>
        <img
          :src="profilePictureUrl"
          alt="Profile Picture"
          v-if="connected && !loading"
          class="profilePicture"
          referrerpolicy="no-referrer"
        />
      </div>
      <div class="favContainer" v-if="connected && !loading">
        <ToggleSlide
          :modelValue="isFavorite"
          @update:modelValue="emit('update:isFavorite', $event)"
        />
        <p>Mark as Favorite</p>
      </div>
    </div>
    <div class="providerButtons">
      <template v-if="!loading">
        <button class="removeButton" v-if="connected" @click="disconnect">Remove</button>
        <button class="addButton" v-if="!connected" @click="add">Add</button>
      </template>
    </div>
  </div>
</template>

<style scoped>
.authContainer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  background-color: var(--secondary-color);
  border-radius: 10px;
  padding: 0.5rem 1rem;
  min-width: 470px;
}

.providerInfoContainer {
  padding: 0.6rem;
}

.providerInfo {
  display: flex;
  align-items: center;
  gap: 20px;
}

.providerInfo > i {
  font-size: 3em;
}

.providerButtons {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.providerText > * {
  margin: 10px 0;
}

.providerButtons > button {
  padding: 10px 20px;
  border: none;
  border-radius: 10000px;
  cursor: pointer;
  font-size: 1rem;
}

.removeButton {
  background-color: red;
  color: white;
  font-weight: bold;
}

.addButton {
  background-color: var(--accent1-color);
  color: white;
  font-weight: bold;
}

.providerName {
  font-weight: 200;
}

.profilePicture {
  width: 50px;
  height: 50px;
  border-radius: 50%;
}

.favContainer {
  display: flex;
  align-items: center;
  gap: 1.3rem;
  margin-left: 0.1rem;
}

.favContainer > p {
  font-size: 1rem;
  font-weight: 300;
}

@media only screen and (orientation: portrait) {
  .authContainer {
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    min-width: fit-content;
    padding: 0.5rem 0.5rem;
  }
  .providerInfo {
    padding: 0.5rem 1rem;
    width: 100%;
  }
  .profilePicture {
    margin-left: auto;
  }
  .providerButtons {
    margin-bottom: 0.2rem;
  }
}
</style>
