<script setup>
import { ref } from 'vue'

const newUsername = ref('')
const newPassword = ref('')
const confirmPassword = ref('')

const username = ref('')
const password = ref('')

const error = ref('')
const success = ref('')
const connected = ref('dunno')

async function registerUser() {
  error.value = ''
  success.value = ''
  if (newPassword.value !== confirmPassword.value) {
    alert('Passwords do not match!')
    return
  }
  try {
    const response = await fetch('http://localhost:5123/api/Account/Register', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        username: newUsername.value,
        password: newPassword.value,
      }),
    })
    const data = await response.json()
    connected.value = 'yuh uh'
    if (response.ok) {
      success.value = 'Registration successful!'
      newUsername.value = newPassword.value = confirmPassword.value = ''
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
    const response = await fetch('http://localhost:5123/api/Account/Login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        username: username.value,
        password: password.value,
      }),
      credentials: 'include',
    })
    if (response.ok) {
      const data = await response.json()
      localStorage.setItem('token', data.token)
      success.value = 'Login successful!'
      username.value = password.value = ''
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
      <h2>Login</h2>
      <input type="text" name="username" id="username" placeholder="Username" v-model="username" />
      <input
        type="password"
        name="password"
        id="password"
        placeholder="Password"
        v-model="password"
      />
      <label for="rememberMe"><input type="checkbox" name="rememberMe" /> Remember me</label>
      <input type="button" value="Login" @click="loginUser" />
    </div>
    <div class="container">
      <h2>Register</h2>

      <div v-if="connected === 'connected'" class="connect">✅ Connected</div>
      <div v-if="error" class="error">{{ error }}</div>
      <div v-if="success" class="success">{{ success }}</div>

      <input
        type="text"
        name="newUsername"
        id="newUsername"
        placeholder="Username"
        v-model="newUsername"
      />
      <input
        type="password"
        name="newPassword"
        id="newPassword"
        placeholder="Password"
        v-model="newPassword"
      />
      <input
        type="password"
        name="confirmPassword"
        id="confirmPassword"
        placeholder="Confirm Password"
        v-model="confirmPassword"
      />
      <label for="rememberMe"><input type="checkbox" name="rememberMe" /> Remember me</label>
      <input type="button" value="Register" @click="registerUser" />
    </div>
  </div>
</template>

<style>
.main {
  display: flex;
  justify-content: center;
  align-items: center;
  flex-wrap: wrap;
  gap: 5%;
}

.container {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 10px;
  padding: 20px;
  background-color: var(--secondary-color);
  border-radius: 18px;
  height: fit-content;
}
</style>
