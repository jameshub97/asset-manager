<script setup lang="ts">
import { useAssetStore } from '@/stores/assetstore'
import { onMounted } from 'vue'

const store = useAssetStore()

onMounted(() => {
  store.fetchAssets()
})
</script>

<template>
  <div class="button-group">
      <h2 class="green">Asset List</h2>
    <div v-if="store.assets.length">
      <div v-if="store.selectedAsset">
        <h2>{{ store.selectedAsset.name }}</h2>
        <p>{{ store.selectedAsset.description }}</p>
        <p>Price: ${{ store.selectedAsset.price }}</p>
        <p>ID: {{ store.selectedAsset.id }}</p>
      </div>
      <ul>
        <li v-for="asset in store.assets" :key="asset.id">
          {{ asset.name }} - ${{ asset.price }}
          <button @click="asset.id && store.fetchAsset(asset.id)" :disabled="!asset.id">
            View
          </button>
          <button @click="asset.id && store.deleteAsset(asset.id)" :disabled="!asset.id">
            Delete
          </button>
        </li>
      </ul>
    </div>
    <p v-else>No assets loaded</p>
  </div>
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
  flex-direction: column; /* Stack vertically */
  gap: 8px; /* Space between buttons */
  align-items: flex-start; /* Align left (or center/stretch) */
}

@media (min-width: 1024px) {
  .greetings h1,
  .greetings h3 {
    text-align: left;
  }
}
</style>
