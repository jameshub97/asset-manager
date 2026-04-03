<script setup lang="ts">
import { RouterView } from 'vue-router'
</script>

<template>
  <div id="app">
    <RouterView />
  </div>
</template>

<style>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

html,
body,
#app {
  min-height: 100%;
  width: 100%;
}

#app {
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
  display: flex;
  flex-direction: column;
  max-width: 80vw;
  margin: 0 auto;
  width: 100%;
  min-height: 100vh;
  height: auto;
  background: #f9fafb;
}

nav {
  padding: 1rem;
  background: #42b883;
  display: flex;
  gap: 1rem;
}

nav a {
  color: white;
  text-decoration: none;
  padding: 0.5rem 1rem;
  border-radius: 4px;
}

nav a:hover {
  background: #33a06f;
}

nav a.router-link-active {
  background: #2c7a5a;
}

@media (max-width: 1200px) {
  #app {
    max-width: 90vw;
  }
}

@media (max-width: 768px) {
  #app {
    max-width: 100vw;
  }
}
</style>

