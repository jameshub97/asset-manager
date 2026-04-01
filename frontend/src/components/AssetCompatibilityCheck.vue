<script setup lang="ts">
import { useAssetStore } from '@/stores/assetstore'
import {  onMounted } from 'vue'

const store = useAssetStore()


onMounted(() => {
  store.fetchAssets()
})


</script>

<template>
    <p class="green">Asset Compatibility Checker TBD</p>
</template>

<style scoped>
h1 {
  font-weight: 500;
  font-size: 2.6rem;
  position: relative;
  top: -10px;
}

h3 {
  font-size: 1.2rem;
}

.greetings h1,
.greetings h3 {
  text-align: center;
}

.button-group {
  display: flex;
  flex-direction: column;  /* Stack vertically */
  gap: 8px;                /* Space between buttons */
  align-items: flex-start; /* Align left (or center/stretch) */
}

@media (min-width: 1024px) {
  .greetings h1,
  .greetings h3 {
    text-align: left;
  }
}
</style>
