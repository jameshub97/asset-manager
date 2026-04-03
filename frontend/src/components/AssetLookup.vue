<!-- components/AssetLookup.vue -->
<template>
  <div class="asset-lookup">
      <h2 class="green">Lookup an asset</h2>

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
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

h2 {
  margin: 0;
}

.green {
  color: #42b883;
}

.lookup-control {
  display: flex;
  gap: 0.75rem;
}

.lookup-control input {
  flex: 1;
  padding: 10px 12px;
  border: 1px solid #cbd5e1;
  border-radius: 6px;
  font-size: 14px;
  transition: all 0.2s;
}

.lookup-control input:focus {
  outline: none;
  border-color: #42b883;
  box-shadow: 0 0 0 3px rgba(66, 184, 131, 0.1);
}

.lookup-control input.is-loading {
  background: #f1f5f9;
}

.lookup-control button {
  padding: 10px 20px;
  background: #42b883;
  color: white;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-size: 14px;
  font-weight: 500;
  transition: all 0.2s;
  white-space: nowrap;
}

.lookup-control button:hover:not(:disabled) {
  background: #33a06f;
}

.lookup-control button:disabled {
  background: #94a3b8;
  cursor: not-allowed;
  opacity: 0.6;
}

.error-message {
  padding: 10px 12px;
  background: #fee2e2;
  color: #dc2626;
  border: 1px solid #fecaca;
  border-radius: 6px;
  font-size: 13px;
}

@media (max-width: 768px) {
  .lookup-control {
    gap: 0.5rem;
  }

  .lookup-control button {
    padding: 10px 16px;
  }
}
</style>
