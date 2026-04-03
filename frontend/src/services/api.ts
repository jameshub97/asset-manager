export const API_BASE = 'http://localhost:5001/api'

export interface Asset {
  id?: string
  name: string
  description: string
  price?: number
  createdAt?: string
  userId?: string
}

export interface PagedResponse<T> {
  items: T[]
  page: number
  pageSize: number
  totalCount: number
  totalPages: number
  hasNextPage: boolean
  hasPreviousPage: boolean
}

type JsonRecord = Record<string, unknown>

const isRecord = (value: unknown): value is JsonRecord => {
  return typeof value === 'object' && value !== null
}

const toNumber = (value: unknown, fallback: number): number => {
  if (typeof value === 'number') return value
  if (typeof value === 'string') {
    const parsed = Number(value)
    return Number.isFinite(parsed) ? parsed : fallback
  }
  return fallback
}

const normalizeAsset = (value: unknown): Asset => {
  const raw = isRecord(value) ? value : {}

  return {
    id: (raw.id as string | undefined) ?? (raw.Id as string | undefined),
    name: String(raw.name ?? raw.Name ?? ''),
    description: String(raw.description ?? raw.Description ?? ''),
    price: toNumber(raw.price ?? raw.Price, 0),
    createdAt: (raw.createdAt as string | undefined) ?? (raw.CreatedAt as string | undefined),
    userId: (raw.userId as string | undefined) ?? (raw.UserId as string | undefined),
  }
}

const normalizePagedAssets = (value: unknown): PagedResponse<Asset> | Asset[] => {
  if (Array.isArray(value)) {
    return value.map(normalizeAsset)
  }

  const raw = isRecord(value) ? value : {}
  const rawItems = raw.items ?? raw.Items
  const items = Array.isArray(rawItems) ? rawItems.map(normalizeAsset) : []

  return {
    items,
    page: toNumber(raw.page ?? raw.Page, 1),
    pageSize: toNumber(raw.pageSize ?? raw.PageSize, items.length || 1),
    totalCount: toNumber(raw.totalCount ?? raw.TotalCount, items.length),
    totalPages: toNumber(raw.totalPages ?? raw.TotalPages, 1),
    hasNextPage: Boolean(raw.hasNextPage ?? raw.HasNextPage),
    hasPreviousPage: Boolean(raw.hasPreviousPage ?? raw.HasPreviousPage),
  }
}

export const api = {
  async request(endpoint: string, options?: RequestInit) {
    const token = localStorage.getItem('authToken')
    const headers = new Headers(options?.headers)
    headers.set('Content-Type', 'application/json')

    if (token) {
      headers.set('Authorization', `Bearer ${token}`)
    }

    const response = await fetch(`${API_BASE}${endpoint}`, {
      ...options,
      headers,
    })

    if (!response.ok) {
      if (response.status === 401) {
        localStorage.removeItem('authToken')
        localStorage.removeItem('username')
        throw new Error('Unauthorized. Please log in again.')
      }
      throw new Error(`API error: ${response.status}`)
    }

    if (response.status === 204 || response.headers.get('content-length') === '0') {
      return
    }

    return response.json()
  },

  async getAssets(page = 1, pageSize = 8): Promise<PagedResponse<Asset> | Asset[]> {
    const payload = await this.request(`/assets?page=${page}&pageSize=${pageSize}`)
    return normalizePagedAssets(payload)
  },

  async getAsset(id: string): Promise<Asset> {
    const payload = await this.request(`/assets/${id}`)
    return normalizeAsset(payload)
  },

  async createAsset(data: Omit<Asset, 'id'>): Promise<Asset> {
    const payload = await this.request('/assets', {
      method: 'POST',
      body: JSON.stringify(data),
    })
    return normalizeAsset(payload)
  },

  async updateAsset(id: string, data: Partial<Asset>): Promise<Asset> {
    const payload = await this.request(`/assets/${id}`, {
      method: 'PUT',
      body: JSON.stringify(data),
    })
    return normalizeAsset(payload)
  },

  deleteAsset(id: string): Promise<void> {
    return this.request(`/assets/${id}`, {
      method: 'DELETE',
    })
  },
}
