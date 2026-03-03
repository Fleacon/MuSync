import { ref } from 'vue'
import { defineStore } from 'pinia'

export const useNotificationStore = defineStore('notification', () => {
  const title = ref('')
  const message = ref('')
  const visible = ref(false)
  const success = ref(true)
  let timer = null

  function show(t, m, isSuccess = true) {
    title.value = t
    message.value = m
    success.value = isSuccess
    visible.value = true

    if (timer) clearTimeout(timer)
    timer = setTimeout(() => (visible.value = false), 3000)
  }

  return { title, message, visible, success, show }
})
