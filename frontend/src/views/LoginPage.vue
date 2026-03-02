<script setup>
import { ref } from 'vue'
import router from '../router'
import { useAuthStore } from '@/stores/auth'

const newUsername = ref('')
const newPassword = ref('')
const confirmPassword = ref('')
const newRememberMe = ref(false)

const username = ref('')
const password = ref('')
const rememberMe = ref(false)

const error = ref('')
const success = ref('')
const connected = ref('dunno')

const authStore = useAuthStore()

async function registerUser() {
  error.value = ''
  success.value = ''
  if (newPassword.value !== confirmPassword.value) {
    alert('Passwords do not match!')
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
    connected.value = 'yuh uh'
    if (response.ok) {
      success.value = 'Registration successful!'
      newUsername.value = newPassword.value = confirmPassword.value = ''
      authStore.setAuth(data.username, data.providers)
      router.push('/account')
    } else {
      const errorData = await response.json()
      error.value = errorData.message || `Server error: ${response.status}`
    }
  } catch (err) {
    connected.value = 'disconnected'
    error.value = 'Network error. Is server running?'
    console.error(err)
  }
}

async function loginUser() {
  error.value = ''
  success.value = ''
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
      success.value = 'Login successful!'
      username.value = password.value = ''
      authStore.setAuth(data.username, data.providers)
      router.push('/')
    } else {
      const errorData = await response.json()
      error.value = errorData.message || `Server error: ${response.status}`
    }
  } catch (err) {
    error.value = 'Network error. Is server running?'
    console.error(err)
  }
}
</script>

<template>
  <div class="main">
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
        ><input type="checkbox" name="rememberMe" v-model="rememberMe" /> Remember me</label
      >
      <input type="button" value="Login" @click="loginUser" />
    </div>
    <div class="container">
      <h3>Create Account</h3>

      <div v-if="connected === 'connected'" class="connect">✅ Connected</div>
      <div v-if="error" class="error">{{ error }}</div>
      <div v-if="success" class="success">{{ success }}</div>

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
      <input type="button" value="Register" @click="registerUser" />
    </div>
  </div>
</template>

<style scoped>
.main {
  display: flex;
  justify-content: center;
  align-items: center;
  flex-wrap: wrap;
  gap: 5%;
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
  gap: 15px;
  padding: 20px;
  background-color: var(--secondary-color);
  border-radius: 18px;
  height: fit-content;
  width: 25%;
}

.inputField {
  padding: 15px;
  padding-left: 1.2rem;
  border-radius: 1000px;
  border: none;
  background-color: var(--accent1-color);
  width: 100%;
  color: var(--accent2-color);
  font-size: 1rem;
}

.inputField:focus {
  outline: 1px solid var(--accent2-color);
}

.inputField::placeholder {
  color: #d9d9d9;
  opacity: 0.5;
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
  width: 24px;
  height: 24px;
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
</style>
