<script setup>
import { useNotificationStore } from '../stores/notificationStore.js'
const notification = useNotificationStore()
</script>

<template>
  <Transition name="slide">
    <div class="notification" v-if="notification.visible" :class="{ error: !notification.success }">
      <i
        class="fa-solid"
        :class="notification.success ? 'fa-check' : 'fa-xmark'"
        style="font-size: 2rem"
      ></i>
      <div class="resultMessages">
        <p class="title">{{ notification.title }}</p>
        <p class="message">{{ notification.message }}</p>
      </div>
    </div>
  </Transition>
</template>

<style scoped>
.notification {
  position: fixed;
  top: 2rem;
  left: 50%;
  transform: translateX(-50%);
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1rem;
  background-color: var(--accent1-color);
  color: var(--accent2-color);
  width: 30%;
  min-width: 250px;
  border-radius: 15px;
  z-index: 99999;
}

.error {
  background-color: #c0392b;
  color: white;
}

.slide-enter-active,
.slide-leave-active {
  transition:
    top 0.3s ease,
    opacity 0.3s ease;
}

.slide-enter-from,
.slide-leave-to {
  top: -5rem;
  opacity: 0;
}

.title {
  font-weight: bold;
  font-size: 1.2rem;
}
</style>
