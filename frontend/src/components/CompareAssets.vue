<script setup lang="ts">
import { useAssetStore } from '@/stores/assetstore'

const store = useAssetStore()

const removeFromComparison = (id: string | undefined) => {
  if (!id) return
  const asset = store.comparisonAssets.find((a) => a.id === id)
  if (asset) store.toggleComparison(asset)
}

const findLowest = () => {
  if (!store.comparisonAssets.length) return null
  return Math.min(...store.comparisonAssets.map((a) => a.price || 0))
}

const findHighest = () => {
  if (!store.comparisonAssets.length) return null
  return Math.max(...store.comparisonAssets.map((a) => a.price || 0))
}
</script>

<template>
  <div v-if="store.comparisonAssets.length" class="comparison-container">
    <div class="comparison-header">
      <h3>Price Comparison</h3>
      <button class="clear-btn" @click="store.clearComparison">Clear All</button>
    </div>

    <div class="comparison-grid">
      <div v-for="asset in store.comparisonAssets" :key="asset.id" class="comparison-card">
        <div class="card-header">
          <h4>{{ asset.name }}</h4>
          <button class="remove-btn" @click="removeFromComparison(asset.id)">✕</button>
        </div>

        <div class="card-body">
          <div class="price-section" :class="{ lowest: asset.price === findLowest(), highest: asset.price === findHighest() }">
            <span class="price-label">Price:</span>
            <span class="price-value">${{ asset.price }}</span>
          </div>
          <div class="description-section">
            <span class="desc-label">Description:</span>
            <span class="desc-value">{{ asset.description }}</span>
          </div>
        </div>
      </div>
    </div>

    <div v-if="store.comparisonAssets.length > 1" class="comparison-stats">
      <div class="stat">
        <span class="label">Lowest:</span>
        <span class="value">${{ findLowest() }}</span>
      </div>
      <div class="stat">
        <span class="label">Highest:</span>
        <span class="value">${{ findHighest() }}</span>
      </div>
      <div class="stat">
        <span class="label">Average:</span>
        <span class="value">${{ (store.comparisonAssets.reduce((sum, a) => sum + (a.price || 0), 0) / store.comparisonAssets.length).toFixed(2) }}</span>
      </div>
    </div>
  </div>
</template>

<style scoped>
.comparison-container {
  background: linear-gradient(135deg, rgba(66, 184, 131, 0.1) 0%, rgba(51, 160, 111, 0.05) 100%);
  border: 2px solid rgba(66, 184, 131, 0.3);
  border-radius: 8px;
  padding: 1.5rem;
  margin-top: 1rem;
}

.comparison-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1.5rem;
  padding-bottom: 1rem;
  border-bottom: 1px solid rgba(66, 184, 131, 0.2);
}

.comparison-header h3 {
  margin: 0;
  color: #42b883;
  font-size: 1.25rem;
}

.clear-btn {
  padding: 8px 16px;
  background: #fee2e2;
  color: #dc2626;
  border: 1px solid #fecaca;
  border-radius: 6px;
  cursor: pointer;
  font-size: 13px;
  font-weight: 500;
  transition: all 0.2s;
}

.clear-btn:hover {
  background: #fecaca;
}

.comparison-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.comparison-card {
  background: white;
  border: 1px solid rgba(66, 184, 131, 0.2);
  border-radius: 8px;
  padding: 1.25rem;
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 0.5rem;
}

.card-header h4 {
  margin: 0;
  color: #1f2937;
  font-size: 1rem;
  flex: 1;
  word-break: break-word;
}

.remove-btn {
  background: none;
  border: none;
  font-size: 1.25rem;
  cursor: pointer;
  color: #9ca3af;
  padding: 0;
  transition: color 0.2s;
}

.remove-btn:hover {
  color: #dc2626;
}

.card-body {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.price-section {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.75rem;
  background: #f3f4f6;
  border-radius: 6px;
  transition: all 0.2s;
}

.price-section.lowest {
  background: linear-gradient(135deg, rgba(34, 197, 94, 0.15) 0%, rgba(20, 184, 166, 0.1) 100%);
  border: 1px solid rgba(34, 197, 94, 0.3);
}

.price-section.highest {
  background: linear-gradient(135deg, rgba(239, 68, 68, 0.15) 0%, rgba(250, 204, 21, 0.1) 100%);
  border: 1px solid rgba(239, 68, 68, 0.2);
}

.price-label {
  font-weight: 600;
  color: #6b7280;
  font-size: 0.875rem;
}

.price-value {
  font-weight: 700;
  color: #42b883;
  font-size: 1.25rem;
}

.description-section {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.desc-label {
  font-weight: 600;
  color: #6b7280;
  font-size: 0.75rem;
}

.desc-value {
  color: #6b7280;
  font-size: 0.875rem;
  line-height: 1.4;
}

.comparison-stats {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 1rem;
  padding-top: 1rem;
  border-top: 1px solid rgba(66, 184, 131, 0.2);
}

.stat {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.75rem;
  background: white;
  border-radius: 6px;
  border: 1px solid rgba(66, 184, 131, 0.15);
}

.stat .label {
  font-weight: 600;
  color: #6b7280;
  font-size: 0.875rem;
}

.stat .value {
  font-weight: 700;
  color: #42b883;
  font-size: 1.125rem;
}

@media (max-width: 768px) {
  .comparison-grid {
    grid-template-columns: 1fr;
  }

  .comparison-stats {
    grid-template-columns: 1fr;
  }
}
</style>
