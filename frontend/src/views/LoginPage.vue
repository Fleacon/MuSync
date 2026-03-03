<script setup>
import { ref } from 'vue'
import router from '../router'
import { useAuthStore } from '@/stores/auth'
import { useNotificationStore } from '@/stores/notificationStore.js'

const notification = useNotificationStore()

const newUsername = ref('')
const newPassword = ref('')
const confirmPassword = ref('')
const newRememberMe = ref(false)

const username = ref('')
const password = ref('')
const rememberMe = ref(false)

const processing = ref(false)

const authStore = useAuthStore()

async function registerUser() {
  processing.value = true
  if (newPassword.value !== confirmPassword.value) {
    notification.show('Error', 'Passwords do not match.', false)
    return
  }
  try {
    const response = await fetch('/api/Account/Register', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        username: newUsername.value,
        password: newPassword.value,
        rememberMe: newRememberMe.value,
      }),
      credentials: 'include',
    })
    const data = await response.json()
    if (response.ok) {
      newUsername.value = newPassword.value = confirmPassword.value = ''
      authStore.setAuth(data.username, data.providers)
      notification.show('Success', 'Account created successfully!')
      router.push('/account')
    } else {
      notification.show('Error', 'Registration failed: User already exists or invalid data', false)
    }
  } catch (err) {
    console.error(err)
    notification.show('Error', 'An error occurred during registration', false)
  }
  processing.value = false
}

async function loginUser() {
  processing.value = true
  try {
    const response = await fetch('/api/Account/Login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        username: username.value,
        password: password.value,
        rememberMe: rememberMe.value,
      }),
      credentials: 'include',
    })
    if (response.ok) {
      const data = await response.json()
      username.value = password.value = ''
      authStore.setAuth(data.username, data.providers)
      notification.show('Success', 'Logged in successfully!')
      router.push('/')
    } else {
      notification.show('Error', 'Login failed: Invalid username or password', false)
    }
  } catch (err) {
    console.error(err)
    notification.show('Error', 'An error occurred during login', false)
  }
  processing.value = false
}

function handleEnter() {
  if (username.value || password.value) {
    loginUser()
  } else if (newUsername.value || newPassword.value || confirmPassword.value) {
    registerUser()
  }
}
</script>

<template>
  <div class="main" @keydown.enter="handleEnter">
    <div class="container">
      <h3>Login</h3>
      <input
        type="text"
        name="username"
        id="username"
        placeholder="Username"
        v-model="username"
        class="inputField"
      />
      <input
        type="password"
        name="password"
        id="password"
        placeholder="Password"
        v-model="password"
        class="inputField"
      />
      <label for="rememberMe" class="checkboxContainer"
        ><input type="checkbox" name="rememberMe" v-model="rememberMe" />Remember me</label
      >
      <input type="button" value="Login" @click="loginUser" :disabled="processing" />
    </div>
    <div class="container">
      <h3>Create Account</h3>
      <input
        type="text"
        name="newUsername"
        id="newUsername"
        placeholder="Username"
        v-model="newUsername"
        class="inputField"
      />
      <input
        type="password"
        name="newPassword"
        id="newPassword"
        placeholder="Password"
        v-model="newPassword"
        class="inputField"
      />
      <input
        type="password"
        name="confirmPassword"
        id="confirmPassword"
        placeholder="Confirm Password"
        v-model="confirmPassword"
        class="inputField"
      />
      <label for="rememberMe" class="checkboxContainer"
        ><input type="checkbox" name="rememberMe" v-model="newRememberMe" /> Remember me
      </label>
      <input type="button" value="Register" @click="registerUser" :disabled="processing" />
    </div>
  </div>
</template>

<style scoped>
.main {
  display: flex;
  justify-content: center;
  align-items: center;
  flex-wrap: wrap;
  column-gap: 5%;
  row-gap: 2rem;
}

h3 {
  text-align: center;
  font-size: 1.5rem;
  margin-bottom: 1rem;
  margin-top: 1rem;
}

.container {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  padding: 20px;
  background-color: var(--secondary-color);
  border-radius: 18px;
  height: fit-content;
  width: 25%;
  min-width: fit-content;
}

.inputField {
  padding: 15px;
  padding-left: 1.2rem;
  border-radius: 1000px;
  border: none;
  width: 100%;
  font-size: 1rem;
  min-width: 150px;
}

input[type='button'] {
  margin-top: 0.5rem;
  padding: 15px 25px;
  border-radius: 1000px;
  border: none;
  background-color: var(--accent1-color);
  color: var(--accent2-color);
  cursor: pointer;
  font-weight: bold;
  font-size: 1.2rem;
  width: fit-content;
  align-self: center;
}

input[type='checkbox'] {
  appearance: none;
  width: 1.5rem;
  height: 1.5rem;
  aspect-ratio: 1/1;
  border: 1px solid var(--accent2-color);
  border-radius: 6px;
  cursor: pointer;
  background-color: transparent;
  margin-left: 1rem;
}

input[type='checkbox']:checked {
  background-color: var(--accent2-color);
}

label {
  display: flex;
  align-items: center;
  gap: 8px;
  font-weight: 200;
}

input[type='button']:disabled {
  opacity: 0.4;
  cursor: not-allowed;
}

.errorMessage {
  color: red;
  font-weight: bold;
  margin: 0;
}
</style>
