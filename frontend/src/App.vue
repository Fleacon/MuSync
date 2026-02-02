<script setup></script>

<template>
  <h1>Musync</h1>
  <nav>
    <RouterLink to="/">Home</RouterLink>
    <RouterLink to="/account">account</RouterLink>
  </nav>
  <main>
    <RouterView />
  </main>
</template>

<style>
:root {
  --primary-color: #0c2b4e;
  --secondary-color: #1a3d64;
  --accent1-color: #1d546c;
  --accent2-color: #f4f4f4;
  --background-linear1-color: var(--primary-color);
  --background-linear2-color: #0b2435;
}

html {
  height: 100%;
}

body {
  margin: 0;
  height: 100%;
  font-family: Inter, Arial, sans-serif;
  background-image: linear-gradient(
    var(--background-linear1-color),
    var(--background-linear2-color)
  );
  color: var(--accent2-color);
  background-repeat: no-repeat;
  background-attachment: fixed;
}
</style>
