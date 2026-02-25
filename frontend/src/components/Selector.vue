<script setup>
import { ref } from 'vue'
import ProviderResult from './ProviderResult.vue'

const emit = defineEmits(['close'])
const props = defineProps({
  results: {
    type: Array,
    default: () => [],
  },
})

function close() {
  emit('close')
}
</script>

<template>
  <div class="selector">
    <div class="selectorHeader">
      <h3>Select Track</h3>
      <button class="closeBtn" @click="close">
        <i class="fa-solid fa-xmark" style="text-align: center; color: var(--accent2-color)"></i>
      </button>
    </div>
    <div class="selectorResultsContainer" :class="{ 'many-items': results.length > 3 }">
      <ProviderResult v-for="(result, index) in results" :key="index" :result="result" />
    </div>
    <button class="nextBtn">Go to Playlists</button>
  </div>
</template>

<style>
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

.closeBtn {
  position: absolute;
  right: 1rem;
  background: transparent;
  border: none;
  font-size: 1.25rem;
  cursor: pointer;
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
  width: 15%;
  padding: 10px;
  border-radius: 1000px;
  border: none;
  background-color: var(--accent1-color);
  color: var(--accent2-color);
  height: 10%;
  margin-top: auto;
  margin-bottom: auto;
  font-weight: bold;
  cursor: pointer;
}
</style>
