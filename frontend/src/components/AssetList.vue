<script setup lang="ts">
import { useAssetStore } from '@/stores/assetstore'
import { computed, ref } from 'vue'
import UpdateAsset from './UpdateAsset.vue'
import PriceDistributionDetail from './PriceDistributionDetail.vue'
import type { Asset } from '@/services/api'
import { useToast } from 'vue-toastification'

const store = useAssetStore()
const editingAsset = ref<Asset | null>(null)
const lookupId = ref('')
const lookupLoading = ref(false)
const lookupError = ref('')
const lookupResult = ref<Asset | null>(null)
const toast = useToast()
const pageSizeOptions = [4, 8, 12, 24, 48]

const displayedAssets = computed(() => {
  return lookupResult.value ? [lookupResult.value] : store.assets
})

const assetGridKey = computed(() => {
  return `${lookupResult.value?.id ?? 'all'}-${store.currentPage}-${store.pageSize}-${displayedAssets.value.length}`
})

const assetGridStyle = computed(() => {
  if (lookupResult.value) {
    return {
      gridTemplateColumns: 'minmax(320px, 1fr)',
    }
  }

  if (store.pageSize <= 4) {
    return {
      gridTemplateColumns: 'repeat(2, minmax(320px, 1fr))',
    }
  }

  if (store.pageSize <= 8) {
    return {
      gridTemplateColumns: 'repeat(auto-fill, minmax(260px, 1fr))',
    }
  }

  return {
    gridTemplateColumns: 'repeat(auto-fill, minmax(220px, 1fr))',
  }
})

const assetCardClass = computed(() => {
  if (lookupResult.value) return 'asset-card-lookup'
  if (store.pageSize <= 4) return 'asset-card-large'
  if (store.pageSize <= 8) return 'asset-card-medium'
  return 'asset-card-compact'
})

const handleEdit = (asset: Asset) => {
  editingAsset.value = asset
}

const handleUpdateClose = () => {
  editingAsset.value = null
}

const handleDetailClose = () => {
  store.clearSelectedAsset()
}

const clearLookup = () => {
  lookupId.value = ''
  lookupError.value = ''
  lookupResult.value = null
}

const handleLookup = async () => {
  const trimmedId = lookupId.value.trim()
  if (!trimmedId) return

  lookupLoading.value = true
  lookupError.value = ''

  try {
    const asset = await store.fetchAsset(trimmedId)
    lookupResult.value = asset
  } catch (err: unknown) {
    lookupResult.value = null
    lookupError.value = err instanceof Error ? err.message : 'Failed to fetch asset.'
  } finally {
    lookupLoading.value = false
  }
}

const handlePageSizeChange = async (event: Event) => {
  const target = event.target as HTMLSelectElement | null
  const value = Number(target?.value ?? store.pageSize)

  if (!Number.isFinite(value)) return

  await store.setPageSize(value)
}

const handleDelete = async (asset: Asset) => {
  if (!asset.id) return

  try {
    await store.deleteAsset(asset.id)
    if (lookupResult.value?.id === asset.id) {
      clearLookup()
    }
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
      <div class="main-layout">
        <div class="left-column">
          <div class="table-toolbar">
            <div class="table-toolbar-copy">
              <h3>Assets</h3>
              <p>Search by asset ID directly in the table.</p>
            </div>

            <div class="lookup-inline">
              <label class="page-size-control">
                <span>Items per page</span>
                <select
                  :value="store.pageSize"
                  :disabled="store.loading || Boolean(lookupResult)"
                  @change="handlePageSizeChange"
                >
                  <option v-for="option in pageSizeOptions" :key="option" :value="option">
                    {{ option }}
                  </option>
                </select>
              </label>

              <input
                v-model="lookupId"
                type="text"
                placeholder="Enter asset ID"
                :disabled="lookupLoading"
                @keydown.enter.prevent="handleLookup"
              />
              <button
                class="toolbar-btn toolbar-btn-primary"
                :disabled="!lookupId.trim() || lookupLoading"
                @click="handleLookup"
              >
                {{ lookupLoading ? 'Fetching...' : 'Fetch' }}
              </button>
              <button
                v-if="lookupResult || lookupError || lookupId"
                class="toolbar-btn toolbar-btn-secondary"
                @click="clearLookup"
              >
                Clear
              </button>
            </div>
          </div>

          <p v-if="lookupError" class="lookup-error">{{ lookupError }}</p>

          <div :key="assetGridKey" class="assets-grid" :style="assetGridStyle">
            <div
              v-for="asset in displayedAssets"
              :key="asset.id"
              class="asset-card"
              :class="[assetCardClass, { active: store.selectedAsset?.id === asset.id }]"
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

          <div v-if="store.totalCount > 0 && !lookupResult" class="pagination">
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
              {{ store.pageSize }} per page
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

        <div class="right-column">
          <PriceDistributionDetail
            :asset="store.selectedAsset"
            @close="handleDetailClose"
          />
        </div>
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
  overflow: hidden;
}

.main-layout {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1.5rem;
  flex: 1;
  overflow: hidden;
  min-height: 0;
}

.left-column {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
  overflow-y: auto;
  min-height: 0;
}

.right-column {
  display: flex;
  flex-direction: column;
  overflow-y: auto;
  min-height: 0;
}

.table-toolbar {
  display: flex;
  justify-content: space-between;
  align-items: flex-end;
  gap: 1rem;
  padding: 1rem;
  border: 1px solid rgba(66, 184, 131, 0.18);
  border-radius: 14px;
  background: linear-gradient(180deg, rgba(255, 255, 255, 0.95) 0%, rgba(240, 253, 250, 0.9) 100%);
}

.table-toolbar-copy h3 {
  margin: 0;
  font-size: 1.1rem;
  color: #1f2937;
}

.table-toolbar-copy p {
  margin: 0.35rem 0 0;
  color: #6b7280;
  font-size: 0.875rem;
}

.lookup-inline {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  flex-wrap: wrap;
  justify-content: flex-end;
}

.page-size-control {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: #4b5563;
  font-size: 0.9rem;
  font-weight: 600;
}

.page-size-control select {
  padding: 0.65rem 0.8rem;
  border: 1px solid #cbd5e1;
  border-radius: 10px;
  background: white;
  font-size: 0.9rem;
  color: #1f2937;
}

.page-size-control select:focus,
.lookup-inline input:focus {
  outline: none;
  border-color: #42b883;
  box-shadow: 0 0 0 3px rgba(66, 184, 131, 0.12);
}

.lookup-inline input {
  min-width: 240px;
  padding: 0.7rem 0.85rem;
  border: 1px solid #cbd5e1;
  border-radius: 10px;
  font-size: 0.95rem;
  transition: all 0.2s;
}

.toolbar-btn {
  padding: 0.7rem 0.9rem;
  border-radius: 10px;
  border: 1px solid transparent;
  cursor: pointer;
  font-size: 0.9rem;
  font-weight: 600;
  transition: all 0.2s;
}

.toolbar-btn-primary {
  background: #42b883;
  color: white;
}

.toolbar-btn-primary:hover:not(:disabled) {
  background: #33a06f;
}

.toolbar-btn-secondary {
  background: white;
  color: #374151;
  border-color: #d1d5db;
}

.toolbar-btn-secondary:hover {
  border-color: #9ca3af;
}

.toolbar-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.lookup-error {
  margin: -0.5rem 0 0;
  padding: 0.85rem 1rem;
  border-radius: 10px;
  background: #fee2e2;
  border: 1px solid #fecaca;
  color: #b91c1c;
  font-size: 0.9rem;
}

/* Assets Grid */
.assets-grid {
  display: grid;
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

.asset-card-large,
.asset-card-lookup {
  min-height: 240px;
  padding: 1.5rem;
}

.asset-card-medium {
  min-height: 210px;
}

.asset-card-compact {
  min-height: 180px;
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

@media (max-width: 1024px) {
  .main-layout {
    grid-template-columns: 1fr;
  }

  .table-toolbar {
    flex-direction: column;
    align-items: stretch;
  }

  .lookup-inline {
    justify-content: stretch;
  }

  .lookup-inline input {
    min-width: 0;
    width: 100%;
  }

  .assets-grid {
    grid-template-columns: repeat(auto-fill, minmax(220px, 1fr)) !important;
  }
}

@media (max-width: 768px) {
  .assets-grid {
    grid-template-columns: 1fr !important;
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
