import { createRouter, createWebHistory } from 'vue-router'
import { routes, handleHotUpdate } from 'vue-router/auto-routes'
import { useAuth } from '@/composables/useAuth'

export const router = createRouter({
  history: createWebHistory(),
  routes
})

// Global navigation guard
router.beforeEach(async (to, from, next) => { // <-- mark async here
  // --- Redirect non-existing auth routes ---
  if (to.path.startsWith('/auth/')) {
    const validAuthRoutes = router.getRoutes()
      .filter(route => route.path.startsWith('/auth'))
      .map(route => route.path)
    validAuthRoutes.push('/auth')

    if (!validAuthRoutes.includes(to.path)) {
      return next('/auth/login')
    }
  }

  // --- Admin guard for /audits ---
  if (to.path === '/audits') {
    const { isAuthenticated, getUser } = useAuth()

    if (!isAuthenticated()) {
      return next('/auth/login') // not logged in
    }

    const res = await getUser() // ✅ now allowed
    const user = res.success ? res.data : null

    if (!user || (user as any).roleId !== 5) {
      return next('/') // redirect to home if not admin
    }
  }

  // Everything okay, proceed
  next()
})

// Hot module replacement for routes
if (import.meta.hot) {
  handleHotUpdate(router)
}

export default router
