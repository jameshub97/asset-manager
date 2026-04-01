import { defineStore } from 'pinia'
import { api, type Asset } from '@/services/api'

export const useAssetStore = defineStore('assets', {
  state: () => ({
    assets: [] as Asset[],
    loading: false as boolean,
    error: null as string | null,
    selectedAsset: null as Asset | null,
  }),

  getters: {
    getAssetById: (state) => (id: string) => {
      return state.assets.find(asset => asset.id === id)
    },
    assetCount: (state) => state.assets.length,
    isLoading: (state) => state.loading,
  },

  actions: {
    async fetchAssets() {
      this.loading = true
      this.error = null
      try {
        this.assets = await api.getAssets()
      } catch (err: unknown) {
        this.error = err instanceof Error ? err.message : 'Failed to fetch assets'
        console.error('Failed to fetch assets:', err)
      } finally {
        this.loading = false
      }
    },

    async fetchAsset(id: string) {
      this.loading = true
      this.error = null
      try {
        this.selectedAsset = await api.getAsset(id)
        return this.selectedAsset
      } catch (err: unknown) {
        this.error = err instanceof Error ? err.message : `Failed to fetch asset ${id}`
        throw err
      } finally {
        this.loading = false
      }
    },

    async createAsset(assetData: Omit<Asset, 'id' | 'createdAt'>) {
      // ✅ Validation before API call
      if (!assetData.name?.trim()) {
        this.error = 'Name is required'
        throw new Error('Name is required')
      }
      if (!assetData.description?.trim()) {
        this.error = 'Description is required'
        throw new Error('Description is required')
      }
      if (assetData.price === undefined || assetData.price === null) {
        this.error = 'Price is required'
        throw new Error('Price is required')
      }
      if (assetData.price < 0) {
        this.error = 'Price must be 0 or greater'
        throw new Error('Price must be 0 or greater')
      }

      this.loading = true
      this.error = null

      try {
        const newAsset = await api.createAsset(assetData)
        this.assets.push(newAsset)
        return newAsset
      } catch (err: unknown) {
        this.error = err instanceof Error ? err.message : 'Failed to create asset'
        throw err
      } finally {
        this.loading = false
      }
    },

    async updateAsset(id: string, assetData: Partial<Asset>) {
      this.loading = true
      try {
        const updatedAsset = await api.updateAsset(id, assetData)
        const index = this.assets.findIndex(a => a.id === id)
        if (index !== -1) {
          this.assets[index] = updatedAsset
        }
        if (this.selectedAsset?.id === id) {
          this.selectedAsset = updatedAsset
        }
        return updatedAsset
      } catch (err: unknown) {
        this.error = err instanceof Error ? err.message : 'Failed to update asset'
        throw err
      } finally {
        this.loading = false
      }
    },

    async deleteAsset(id: string) {
      this.loading = true
      try {
        await api.deleteAsset(id)
        this.assets = this.assets.filter(a => a.id !== id)
        if (this.selectedAsset?.id === id) {
          this.selectedAsset = null
        }
      } catch (err: unknown) {
        this.error = err instanceof Error ? err.message : 'Failed to delete asset'
        throw err
      } finally {
        this.loading = false
        await this.fetchAssets()

      }
    },

    clearError() {
      this.error = null
    },
  },
})
