export default defineNuxtRouteMiddleware((to, from) => {
  const authStore = useAuthStore()

  if (process.client) {
    authStore.initializeAuth()
  }

  if (!authStore.isAuthenticated) {
    return navigateTo('/login')
  }
})
