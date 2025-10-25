import { defineStore } from 'pinia'
import type { User, AuthResponse, LoginCredentials, RegisterData } from '~/types'

// Helper to convert PascalCase response to camelCase
function normalizeUser(user: any): User {
  return {
    id: user.Id || user.id,
    email: user.Email || user.email,
    firstName: user.FirstName || user.firstName,
    lastName: user.LastName || user.lastName,
    fullName: user.FullName || user.fullName,
    tenantId: user.TenantId || user.tenantId,
    roles: user.Roles || user.roles || [],
    createdAt: user.CreatedAt || user.createdAt
  }
}

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: null as User | null,
    token: null as string | null,
    loading: false,
    error: null as string | null
  }),

  getters: {
    isAuthenticated: (state) => !!state.token,
    isAdmin: (state) => state.user?.roles.includes('Admin') ?? false,
    isManager: (state) => state.user?.roles.includes('Manager') ?? false
  },

  actions: {
    async login(credentials: LoginCredentials) {
      this.loading = true
      this.error = null

      const { withLoading } = useApiLoading()

      try {
        const response = await withLoading(async () => {
          const config = useRuntimeConfig()
          return await $fetch<AuthResponse>(`${config.public.apiBase}/auth/login`, {
            method: 'POST',
            body: credentials,
            headers: {
              'Content-Type': 'application/json'
            }
          })
        }, 'Signing in...')

        // Backend sends PascalCase (Token, User), handle both cases
        const token = (response as any).Token || (response as any).token
        const userRaw = (response as any).User || (response as any).user

        if (!token || !userRaw) {
          console.error('Invalid response - token:', !!token, 'user:', !!userRaw, 'response:', response)
          throw new Error('Invalid response from server')
        }

        const user = normalizeUser(userRaw)

        this.token = token
        this.user = user

        // Store token in localStorage
        if (process.client) {
          localStorage.setItem('token', token)
          localStorage.setItem('user', JSON.stringify(user))
        }

        return { token, user }
      } catch (error: any) {
        console.error('Login error:', error)
        this.error = error.data?.message || error.message || 'Login failed'
        throw error
      } finally {
        this.loading = false
      }
    },

    async register(data: RegisterData) {
      this.loading = true
      this.error = null

      const { withLoading } = useApiLoading()

      try {
        const response = await withLoading(async () => {
          const config = useRuntimeConfig()
          return await $fetch<AuthResponse>(`${config.public.apiBase}/auth/register`, {
            method: 'POST',
            body: data
          })
        }, 'Creating account...')

        // Backend sends PascalCase (Token, User), handle both cases
        const token = (response as any).Token || (response as any).token
        const userRaw = (response as any).User || (response as any).user

        const user = normalizeUser(userRaw)

        this.token = token
        this.user = user

        // Store token in localStorage
        if (process.client) {
          localStorage.setItem('token', token)
          localStorage.setItem('user', JSON.stringify(user))
        }

        return { token, user }
      } catch (error: any) {
        this.error = error.data?.message || 'Registration failed'
        throw error
      } finally {
        this.loading = false
      }
    },

    async fetchCurrentUser() {
      if (!this.token) return

      try {
        const config = useRuntimeConfig()
        const user = await $fetch<User>(`${config.public.apiBase}/auth/me`, {
          headers: {
            Authorization: `Bearer ${this.token}`
          }
        })

        this.user = user
        if (process.client) {
          localStorage.setItem('user', JSON.stringify(user))
        }
      } catch (error) {
        this.logout()
      }
    },

    logout() {
      this.user = null
      this.token = null
      this.error = null

      if (process.client) {
        localStorage.removeItem('token')
        localStorage.removeItem('user')
      }

      navigateTo('/login')
    },

    initializeAuth() {
      if (process.client) {
        const token = localStorage.getItem('token')
        const userStr = localStorage.getItem('user')

        if (token && userStr && userStr !== 'undefined' && userStr !== 'null') {
          try {
            this.token = token
            this.user = JSON.parse(userStr)
          } catch (error) {
            // Clear invalid data
            localStorage.removeItem('token')
            localStorage.removeItem('user')
            this.token = null
            this.user = null
          }
        }
      }
    }
  }
})
