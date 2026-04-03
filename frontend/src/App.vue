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
body {
  min-height: 100%;
  width: 100%;
  background: #ffffff;
}

#app {
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
  display: flex;
  flex-direction: column;
  width: 100%;
  min-height: 100vh;
  height: auto;
  background: #ffffff;
  overflow: hidden;
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
    width: 100%;
  }
}

@media (max-width: 768px) {
  #app {
    width: 100%;
  }
}
</style>

