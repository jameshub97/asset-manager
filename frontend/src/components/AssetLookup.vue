<!-- components/AssetLookup.vue -->
<template>
  <h2 class="green">Lookup an asset</h2>
  <div class="asset-lookup">
    <div class="lookup-control">
      <input
        v-model="searchId"
        @input="onInput"
        placeholder="Enter asset ID"
        :class="{ 'is-loading': loading }"
      />
      <button @click="search" :disabled="!searchId || loading">
        {{ loading ? '...' : 'Fetch' }}
      </button>
    </div>

    <div v-if="error" class="error-message">
      {{ error }}
    </div>

    <slot v-if="result" name="result" :asset="result" />
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useAssetStore } from '@/stores/assetstore'
import type { Asset } from '@/services/api'

const store = useAssetStore()
const searchId = ref('')
const loading = ref(false)
const error = ref('')
const result = ref<Asset | null>(null)

let timeoutId: ReturnType<typeof setTimeout>

function onInput() {
  // Clear previous timeout
  if (timeoutId) clearTimeout(timeoutId)

  // Clear states
  error.value = ''
  result.value = null

  // Debounce search
  if (searchId.value.trim()) {
    timeoutId = setTimeout(search, 400)
  }
}

async function search() {
  if (!searchId.value.trim()) return

  loading.value = true
  error.value = ''

  try {
    await store.fetchAsset(searchId.value)
    result.value = store.selectedAsset

    if (!result.value) {
      error.value = `No asset found with ID: ${searchId.value}`
    }
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Failed to fetch'
  } finally {
    loading.value = false
  }
}

// Expose for parent use
defineExpose({
  clear: () => {
    searchId.value = ''
    result.value = null
    error.value = ''
  },
})
</script>

<style scoped>
.asset-lookup {
  margin: 16px 0;
}

.lookup-control {
  display: flex;
  gap: 8px;
}

.lookup-control input {
  flex: 1;
  padding: 8px 12px;
  border: 1px solid #cbd5e1;
  border-radius: 6px;
}

.lookup-control input.is-loading {
  background: #f1f5f9;
}

.lookup-control button {
  padding: 8px 16px;
  background: #42b883;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
}

.lookup-control button:disabled {
  background: #94a3b8;
  cursor: not-allowed;
}

.error-message {
  margin-top: 8px;
  padding: 8px;
  background: #fee2e2;
  color: #dc2626;
  border-radius: 4px;
  font-size: 13px;
}
</style>
