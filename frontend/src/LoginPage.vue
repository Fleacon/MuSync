<template>
  <div class="main">
    <div class="container">
      <h2>Login</h2>
      <input type="text" name="username" id="username" placeholder="Username" />
      <input type="password" name="password" id="password" placeholder="Password" />
      <label for="rememberMe"><input type="checkbox" name="rememberMe" /> Remember me</label>
      <input type="button" value="Login" />
    </div>
    <div class="container">
      <h2>Register</h2>
      <input type="text" name="newEmail" id="newEmail" placeholder="Email" />
      <input
        type="text"
        class="newUsername"
        name="newUsername"
        id="newUsername"
        placeholder="Username"
      />
      <input
        type="password"
        class="newPassword"
        name="newPassword"
        id="newPassword"
        placeholder="Password"
      />
      <input
        type="password"
        name="confirmPassword"
        id="confirmPassword"
        placeholder="Confirm Password"
      />
      <label for="rememberMe"><input type="checkbox" name="rememberMe" /> Remember me</label>
      <input type="button" value="Register" />
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
