const API_BASE = 'http://localhost:5001/api' // Change from 5171 to 5001

export interface Asset {
  id?: string
  name: string
  description: string
  price?: number
  createdAt?: string
}

export const api = {
  async request(endpoint: string, options?: RequestInit) {
    const response = await fetch(`${API_BASE}${endpoint}`, {
      ...options,
      headers: {
        'Content-Type': 'application/json',
        ...options?.headers,
      },
    })

    if (!response.ok) {
      throw new Error(`API error: ${response.status}`)
    }

    // Handle responses with no content (e.g., 204 No Content)
    if (response.status === 204 || response.headers.get('content-length') === '0') {
      return
    }

    return response.json()
  },

  // Asset endpoints
  getAssets(): Promise<Asset[]> {
    return this.request('/assets')
  },

  getAsset(id: string): Promise<Asset> {
    return this.request(`/assets/${id}`)
  },

  createAsset(data: Omit<Asset, 'id'>): Promise<Asset> {
    return this.request('/assets', {
      method: 'POST',
      body: JSON.stringify(data),
    })
  },

  updateAsset(id: string, data: Partial<Asset>): Promise<Asset> {
    return this.request(`/assets/${id}`, {
      method: 'PUT',
      body: JSON.stringify(data),
    })
  },

  deleteAsset(id: string): Promise<void> {
    return this.request(`/assets/${id}`, {
      method: 'DELETE',
    })
  },
}
