<script setup lang="ts">
import type { Asset } from '@/services/api'

defineProps<{
  asset: Asset | null
}>()

const emit = defineEmits<{
  close: []
}>()

const formatDate = (dateStr?: string) => {
  if (!dateStr) return 'N/A'
  return new Date(dateStr).toLocaleDateString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  })
}
</script>

<template>
  <div v-if="asset" class="selected-asset">
    <div class="asset-detail-header">
      <h3>{{ asset.name }}</h3>
      <button @click="emit('close')" class="close-btn">✕</button>
    </div>
    <div class="asset-detail-body">
      <div class="detail-row">
        <span class="label">Description:</span>
        <span class="value">{{ asset.description }}</span>
      </div>
      <div class="detail-row">
        <span class="label">Price:</span>
        <span class="value">${{ asset.price }}</span>
      </div>
      <div class="detail-row">
        <span class="label">ID:</span>
        <span class="value mono">{{ asset.id }}</span>
      </div>
      <div class="detail-row">
        <span class="label">Created:</span>
        <span class="value">{{ formatDate(asset.createdAt) }}</span>
      </div>
    </div>
  </div>
  <div v-else class="empty-detail">
    <p>📋 Select an asset to view details</p>
  </div>
</template>

<style scoped>
.selected-asset,
.empty-detail {
  background: linear-gradient(135deg, rgba(66, 184, 131, 0.15) 0%, rgba(51, 160, 111, 0.08) 100%);
  border: 2px solid rgba(66, 184, 131, 0.3);
  border-radius: 8px;
  padding: 1.5rem;
  margin-bottom: 0.5rem;
}

.empty-detail {
  display: flex;
  align-items: center;
  justify-content: center;
  min-height: 120px;
  color: #9ca3af;
  font-size: 1rem;
}

.empty-detail p {
  margin: 0;
}

.asset-detail-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.asset-detail-header h3 {
  margin: 0;
  color: #42b883;
  font-size: 1.25rem;
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: #9ca3af;
  padding: 0;
  transition: color 0.2s;
}

.close-btn:hover {
  color: #6b7280;
}

.asset-detail-body {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.detail-row {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.5rem 0;
  border-bottom: 1px solid rgba(66, 184, 131, 0.1);
}

.detail-row:last-child {
  border-bottom: none;
}

.label {
  font-weight: 600;
  color: #374151;
  font-size: 0.875rem;
}

.value {
  color: #6b7280;
  font-size: 0.875rem;
}

.mono {
  font-family: 'Courier New', monospace;
  font-size: 0.8rem;
}
</style>
