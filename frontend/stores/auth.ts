import { defineStore } from 'pinia'
import type { User, AuthResponse, LoginCredentials, RegisterData } from '~/types'

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

      try {
        const config = useRuntimeConfig()
        const response = await $fetch<AuthResponse>(`${config.public.apiBase}/auth/login`, {
          method: 'POST',
          body: credentials
        })

        this.token = response.token
        this.user = response.user

        // Store token in localStorage
        if (process.client) {
          localStorage.setItem('token', response.token)
          localStorage.setItem('user', JSON.stringify(response.user))
        }

        return response
      } catch (error: any) {
        this.error = error.data?.message || 'Login failed'
        throw error
      } finally {
        this.loading = false
      }
    },

    async register(data: RegisterData) {
      this.loading = true
      this.error = null

      try {
        const config = useRuntimeConfig()
        const response = await $fetch<AuthResponse>(`${config.public.apiBase}/auth/register`, {
          method: 'POST',
          body: data
        })

        this.token = response.token
        this.user = response.user

        // Store token in localStorage
        if (process.client) {
          localStorage.setItem('token', response.token)
          localStorage.setItem('user', JSON.stringify(response.user))
        }

        return response
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

        if (token && userStr) {
          this.token = token
          this.user = JSON.parse(userStr)
        }
      }
    }
  }
})
