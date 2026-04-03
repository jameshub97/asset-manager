<script setup lang="ts">
import { useAssetStore } from '@/stores/assetstore'
import { onMounted, ref } from 'vue'
import UpdateAsset from './UpdateAsset.vue'
import AssetDetail from './AssetDetail.vue'
import CompareAssets from './CompareAssets.vue'
import type { Asset } from '@/services/api'
import { useToast } from 'vue-toastification'

const store = useAssetStore()
const editingAsset = ref<Asset | null>(null)
const toast = useToast()

onMounted(() => {
  store.fetchAssets()
})

const handleEdit = (asset: Asset) => {
  editingAsset.value = asset
}

const handleUpdateClose = () => {
  editingAsset.value = null
}

const handleDetailClose = () => {
  store.selectedAsset = null
}

const handleDelete = async (asset: Asset) => {
  if (!asset.id) return

  try {
    await store.deleteAsset(asset.id)
    toast.success(`Deleted ${asset.name}.`)
  } catch (err: unknown) {
    const message = err instanceof Error ? err.message : 'Failed to delete asset.'
    toast.error(message)
  }
}
</script>

<template>
  <div class="asset-list-container">
    <div v-if="store.assets.length" class="list-content">
      <!-- Asset Detail Section -->
      <AssetDetail
        :asset="store.selectedAsset"
        @close="handleDetailClose"
      />

      <!-- Compare Section -->
      <CompareAssets />
      <div class="assets-grid">
        <div
          v-for="asset in store.assets"
          :key="asset.id"
          class="asset-card"
          :class="{ active: store.selectedAsset?.id === asset.id }"
        >
          <div class="asset-header">
            <h4>{{ asset.name }}</h4>
            <span class="price">${{ asset.price }}</span>
          </div>
          <p class="description">{{ asset.description }}</p>
          <div class="asset-actions">
            <button
              @click="asset.id && store.fetchAsset(asset.id)"
              :disabled="!asset.id"
              class="btn-view"
            >
              View
            </button>
            <button
              @click="handleEdit(asset)"
              :disabled="!asset.id"
              class="btn-edit"
            >
              Edit
            </button>
            <button
              @click="asset.id && store.toggleComparison(asset)"
              :disabled="!asset.id"
              :class="['btn-compare', { active: store.isInComparison(asset.id) }]"
            >
              {{ store.isInComparison(asset.id) ? '✓ Compare' : 'Compare' }}
            </button>
            <button
              @click="handleDelete(asset)"
              :disabled="!asset.id"
              class="btn-delete"
            >
              Delete
            </button>
          </div>
        </div>
      </div>

      <div v-if="store.totalCount > 0" class="pagination">
        <button
          class="pagination-btn"
          :disabled="store.loading || !store.currentPage || store.currentPage <= 1"
          @click="store.goToPage(store.currentPage - 1)"
        >
          Previous
        </button>

        <p class="pagination-meta">
          Page {{ store.currentPage }} of {{ store.totalPages }}
          <span class="dot">•</span>
          {{ store.totalCount }} total assets
        </p>

        <button
          class="pagination-btn"
          :disabled="store.loading || !store.totalPages || store.currentPage >= store.totalPages"
          @click="store.goToPage(store.currentPage + 1)"
        >
          Next
        </button>
      </div>
    </div>
    <div v-else class="empty-state">
      <p>📭 No assets loaded</p>
    </div>

    <!-- Update Asset Modal -->
    <UpdateAsset
      v-if="editingAsset"
      :asset="editingAsset"
      @close="handleUpdateClose"
    />
  </div>
</template>

<style scoped>
.asset-list-container {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
  height: 100%;
}

h2 {
  margin: 0;
  font-size: 1.5rem;
}

.list-content {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
  flex: 1;
  overflow-y: auto;
}

/* Assets Grid */
.assets-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
  gap: 1rem;
  flex: 1;
  align-content: start;
}

.asset-card {
  padding: 1.25rem;
  background: linear-gradient(135deg, rgba(66, 184, 131, 0.08) 0%, rgba(51, 160, 111, 0.03) 100%);
  border: 1px solid rgba(66, 184, 131, 0.2);
  border-radius: 8px;
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  transition: all 0.2s;
  cursor: pointer;
}

.asset-card:hover {
  border-color: rgba(66, 184, 131, 0.4);
  box-shadow: 0 4px 12px rgba(66, 184, 131, 0.1);
  transform: translateY(-2px);
}

.asset-card.active {
  border-color: #42b883;
  background: linear-gradient(135deg, rgba(66, 184, 131, 0.15) 0%, rgba(51, 160, 111, 0.08) 100%);
  box-shadow: 0 4px 12px rgba(66, 184, 131, 0.2);
}

.asset-header {
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
  gap: 0.5rem;
}

.asset-header h4 {
  margin: 0;
  flex: 1;
  color: #1f2937;
  font-size: 1rem;
}

.price {
  background: #42b883;
  color: white;
  padding: 0.25rem 0.75rem;
  border-radius: 4px;
  font-weight: 600;
  font-size: 0.875rem;
  white-space: nowrap;
}

.description {
  margin: 0;
  color: #6b7280;
  font-size: 0.875rem;
  line-height: 1.4;
  flex: 1;
}

.asset-actions {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 0.5rem;
  margin-top: 0.5rem;
}

.btn-view,
.btn-edit,
.btn-compare,
.btn-delete {
  padding: 0.5rem 0.75rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 0.825rem;
  font-weight: 500;
  transition: all 0.2s;
}

.btn-view {
  background: #42b883;
  color: white;
}

.btn-view:hover:not(:disabled) {
  background: #33a06f;
}

.btn-edit {
  background: #dbeafe;
  color: #0369a1;
}

.btn-edit:hover:not(:disabled) {
  background: #bfdbfe;
}

.btn-compare {
  background: #f3e8ff;
  color: #7c3aed;
}

.btn-compare:hover:not(:disabled) {
  background: #ede9fe;
}

.btn-compare.active {
  background: #7c3aed;
  color: white;
}

.btn-delete {
  background: #fee2e2;
  color: #dc2626;
}

.btn-delete:hover:not(:disabled) {
  background: #fecaca;
}

.btn-view:disabled,
.btn-edit:disabled,
.btn-compare:disabled,
.btn-delete:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.empty-state {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 200px;
  color: #9ca3af;
  font-size: 1.125rem;
  text-align: center;
}

.pagination {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 0.75rem;
  padding: 0.75rem 0;
  border-top: 1px solid rgba(66, 184, 131, 0.18);
}

.pagination-btn {
  padding: 0.5rem 0.9rem;
  border-radius: 6px;
  border: 1px solid rgba(66, 184, 131, 0.35);
  background: white;
  color: #1f2937;
  font-size: 0.875rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s;
}

.pagination-btn:hover:not(:disabled) {
  border-color: #42b883;
  color: #0f766e;
}

.pagination-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.pagination-meta {
  margin: 0;
  color: #4b5563;
  font-size: 0.875rem;
  text-align: center;
}

.dot {
  margin: 0 0.4rem;
  color: #9ca3af;
}

@media (max-width: 768px) {
  .assets-grid {
    grid-template-columns: 1fr;
  }

  .asset-card {
    padding: 1rem;
  }

  .pagination {
    flex-direction: column;
    align-items: stretch;
  }

  .pagination-btn {
    width: 100%;
  }
}
</style>
