<script setup>
import TrackResult from './TrackResult.vue'
</script>

<template>
  <div class="providerResult">
    <div class="providerLogo">
      <i class="fa-brands fa-youtube" style="font-size: 2rem"></i>
      <p style="font-size: 1em">Youtube Music</p>
    </div>
    <div class="resultList">
      <TrackResult v-for="i in 20" :key="i" />
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
  width: 50%;
  min-width: 30%;
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
