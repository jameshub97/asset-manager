import { defineStore } from 'pinia'
import { api, type Asset } from '@/services/api'  // ← Fixed import

export const useAssetStore = defineStore('assets', {
  // State - where data lives
  state: () => ({
  assets: [] as Asset[],
  loading: false as boolean,
  error: null as string | null,
  selectedAsset: null as Asset | null,
  }),

  // Getters - computed properties
  getters: {
    // Get asset by ID
    getAssetById: (state) => (id: string) => {
      return state.assets.find(asset => asset.id === id)
    },

    // Count assets
    assetCount: (state) => state.assets.length,

    // Loading state for UI
    isLoading: (state) => state.loading,
  },

  // Actions - functions that modify state and call API
  actions: {
    // Fetch all assets
    async fetchAssets() {
      this.loading = true
      this.error = null

      try {
        this.assets = await api.getAssets()
      } catch (error) {
        this.error = "error.message"
        console.error('Failed to fetch assets:', error)
      } finally {
        this.loading = false
      }
    },

    // Fetch single asset
    async fetchAsset(id: string) {
      this.loading = true
      this.error = null

      try {
        this.selectedAsset = await api.getAsset(id)
        return this.selectedAsset
      } catch (error) {
        this.error = "error.message"
        throw error
      } finally {
        this.loading = false
      }
    },

    // Create new asset
    async createAsset(assetData: Omit<Asset, 'id' | 'createdAt'>) {
      this.loading = true

      try {
        const newAsset = await api.createAsset(assetData)
        this.assets.push(newAsset)
        return newAsset
      } catch (error) {
        this.error = "error.message"
        throw error
      } finally {
        this.loading = false
      }
    },

    // Update existing asset
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
      } catch (error) {
        this.error = "error.message"
        throw error
      } finally {
        this.loading = false
      }
    },

    // Delete asset
    async deleteAsset(id: string) {
      this.loading = true

      try {
        await api.deleteAsset(id)
        this.assets = this.assets.filter(a => a.id !== id)
        if (this.selectedAsset?.id === id) {
          this.selectedAsset = null
        }
      } catch (error) {
        this.error = "error.message"
        throw error
      } finally {
        this.loading = false
      }
    },

    // Clear error
    clearError() {
      this.error = null
    },
  },
})
