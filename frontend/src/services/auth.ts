// frontend/src/services/auth.ts
import { API_BASE } from './api'

export interface LoginRequest {
  username: string
  password: string
}

export interface RegisterRequest {
  username: string
  email: string
  password: string
}

export interface AuthResponse {
  token: string
  username: string
  userId: string
}

export const auth = {
  async login(request: LoginRequest): Promise<AuthResponse> {
    const response = await fetch(`${API_BASE}/auth/login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(request),
    })

    if (!response.ok) {
      throw new Error('Login failed')
    }

    return response.json()
  },

  async register(request: RegisterRequest): Promise<{ message: string }> {
    const response = await fetch(`${API_BASE}/auth/register`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(request),
    })

    if (!response.ok) {
      throw new Error('Registration failed')
    }

    return response.json()
  },

  isAuthenticated(): boolean {
    const token = localStorage.getItem('authToken')
    return Boolean(token)
  },

  isGuest(): boolean {
    return localStorage.getItem('isGuest') === 'true'
  },

  loginAsGuest() {
    const guestToken = 'guest_' + Date.now() + '_' + Math.random().toString(36).substr(2, 9)
    localStorage.setItem('authToken', guestToken)
    localStorage.setItem('username', 'Guest User')
    localStorage.setItem('userId', 'guest')
    localStorage.setItem('isGuest', 'true')
  },

  logout() {
    localStorage.removeItem('authToken')
    localStorage.removeItem('username')
    localStorage.removeItem('userId')
    localStorage.removeItem('isGuest')
  },
}
