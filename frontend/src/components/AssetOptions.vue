<script setup lang="ts">
import { useAssetStore } from '@/stores/assetstore'
import { ref, onMounted } from 'vue'

const store = useAssetStore()
const selectedAssetId = ref('')

onMounted(() => {
  store.fetchAssets()
})
</script>

<template>
  <div class="button-group">
    <h2 class="green">Choose from droplist</h2>

    <!-- lookup  data -->
    <!-- Fetch by ID form -->
    <select
      v-model="selectedAssetId"
      class="asset-select"
      @change="() => store.fetchAsset(selectedAssetId)"
    >
      <option value="">Select an asset</option>
      <option v-for="asset in store.assets" :key="asset.id" :value="asset.id">
        {{ asset.name }} - ${{ asset.price }}
      </option>
    </select>
  </div>
</template>

<style scoped>
.button-group {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

h2 {
  margin: 0;
}

.asset-select {
  padding: 10px 12px;
  border: 1px solid #cbd5e1;
  border-radius: 6px;
  font-size: 14px;
  background: white;
  cursor: pointer;
  transition: all 0.2s;
}

.asset-select:hover {
  border-color: #42b883;
}

.asset-select:focus {
  outline: none;
  border-color: #42b883;
  box-shadow: 0 0 0 3px rgba(66, 184, 131, 0.1);
}

.green {
  color: #42b883;
}

@media (max-width: 768px) {
  .button-group {
    gap: 0.75rem;
  }
}
</style>
