<script setup>
import { computed } from 'vue'

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
})

const providerEnumMap = {
  'YouTube Music': 'YouTubeMusic',
  Spotify: 'Spotify',
}

const providerEnum = computed(() => providerEnumMap[props.provider])

function add() {
  if (!providerEnum.value) return

  window.location.href = `/api/ProviderAuth/Login/${providerEnum.value}`
}

async function disconnect() {
  if (!providerEnum.value) return

  await fetch(`/api/ProviderAuth/Disconnect/${providerEnum.value}`, {
    method: 'POST',
    credentials: 'include',
  })

  window.location.reload()
}

async function refreshToken() {
  if (!providerEnum.value) return

  await fetch(`/api/ProviderAuth/Refresh/${providerEnum.value}`, {
    method: 'POST',
    credentials: 'include',
  })

  window.location.reload()
}
</script>

<template>
  <div class="authContainer">
    <div class="providerInfo">
      <i :class="iconClass + ' fa-brands'"></i>
      <div class="providerText">
        <p class="providerUsername">{{ connected ? username : 'Not connected' }}</p>
        <p class="providerName">{{ provider }}</p>
      </div>
    </div>
    <div class="providerButtons">
      <button @click="refreshToken">refreshToken</button>
      <button class="removeButton" v-if="connected" @click="disconnect">Remove</button>
      <button class="accent1-color" v-if="!connected" @click="add">Add</button>
    </div>
  </div>
</template>

<style>
.authContainer {
  display: flex;
  justify-content: space-between;
  align-items: center;
  background-color: var(--secondary-color);
  border-radius: 10px;
  padding: 0px 20px;
}

.providerInfo {
  display: flex;
  align-items: center;
  gap: 20px;
  padding: 10px;
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
  background-color: var(--accent1-color);
  color: var(--accent2-color);
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
</style>
