export default defineNuxtPlugin(() => {
  const { start, finish, setText } = useLoading()

  // Route-specific loading messages
  const routeMessages: Record<string, string> = {
    '/': 'Loading Home...',
    '/login': 'Signing in...',
    '/register': 'Creating account...',
    '/dashboard': 'Loading Dashboard...',
    '/dashboard/tickets': 'Loading Tickets...',
    '/dashboard/tickets/new': 'Creating Ticket...',
    '/dashboard/users': 'Loading Users...'
  }

  const router = useRouter()

  router.beforeEach((to, from) => {
    if (to.path !== from.path) {
      const message = routeMessages[to.path] || 'Loading...'
      start(message)
    }
  })

  router.afterEach(() => {
    // Add a small delay for better UX
    setTimeout(() => {
      finish()
    }, 200)
  })
})
