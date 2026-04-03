import { defineStore } from 'pinia'
import { api, type Asset } from '@/services/api'

export const useAssetStore = defineStore('assets', {
  state: () => ({
    assets: [] as Asset[],
    loading: false as boolean,
    error: null as string | null,
    selectedAsset: null as Asset | null,
    comparisonAssets: [] as Asset[],
    currentPage: 1 as number,
    pageSize: 8 as number,
    totalCount: 0 as number,
    totalPages: 1 as number,
  }),

  getters: {
    getAssetById: (state) => (id: string) => {
      return state.assets.find((asset) => asset.id === id)
    },
    assetCount: (state) => state.totalCount,
    isLoading: (state) => state.loading,
    isInComparison: (state) => (id: string | undefined) => {
      if (!id) return false
      return state.comparisonAssets.some((a) => a.id === id)
    },
  },

  actions: {
    async fetchAssets(page?: number) {
      this.loading = true
      this.error = null
      try {
        const targetPage = page ?? this.currentPage
        const response = await api.getAssets(targetPage, this.pageSize)

        // Support paged responses and legacy array responses.
        if (Array.isArray(response)) {
          this.assets = response
          this.currentPage = 1
          this.pageSize = response.length || this.pageSize
          this.totalCount = response.length
          this.totalPages = 1
        } else {
          this.assets = response.items
          this.currentPage = response.page
          this.pageSize = response.pageSize
          this.totalCount = response.totalCount
          this.totalPages = response.totalPages
        }
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
        await this.fetchAssets(1)
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
        const index = this.assets.findIndex((a) => a.id === id)
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
      this.error = null

      try {
        await api.deleteAsset(id)

        // ✅ Clear selected if it was deleted
        if (this.selectedAsset?.id === id) {
          this.selectedAsset = null
        }

        const nextPage = this.assets.length === 1 && this.currentPage > 1
          ? this.currentPage - 1
          : this.currentPage

        await this.fetchAssets(nextPage)
      } catch (err: unknown) {
        console.error('Delete error for ID:', id, err)
        this.error = err instanceof Error ? err.message : 'Failed to delete asset'
        console.error('Delete failed:', err)
        throw err
      } finally {
        this.loading = false
      }
    },

    toggleComparison(asset: Asset) {
      if (!asset.id) return
      const index = this.comparisonAssets.findIndex((a) => a.id === asset.id)
      if (index !== -1) {
        this.comparisonAssets.splice(index, 1)
      } else {
        if (this.comparisonAssets.length < 5) {
          this.comparisonAssets.push(asset)
        }
      }
    },

    clearComparison() {
      this.comparisonAssets = []
    },

    async goToPage(page: number) {
      if (this.loading) return
      if (page < 1 || page > this.totalPages || page === this.currentPage) return
      await this.fetchAssets(page)
    },

    clearError() {
      this.error = null
    },
  },
})
