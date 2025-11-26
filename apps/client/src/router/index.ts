import { createRouter, createWebHistory } from 'vue-router'
import { routes, handleHotUpdate } from 'vue-router/auto-routes'

export const router = createRouter({
  history: createWebHistory(),
  routes
})

// Redirect non-existing auth routes to login
router.beforeEach((to, from, next) => {
  // Check if the route starts with /auth/
  if (to.path.startsWith('/auth/')) {
    // Get all valid auth routes from the router
    const validAuthRoutes = router.getRoutes()
      .filter(route => route.path.startsWith('/auth'))
      .map(route => route.path)

    // Add the base /auth route
    validAuthRoutes.push('/auth')

    // If it's not a valid auth route, redirect to login
    if (!validAuthRoutes.includes(to.path)) {
      next('/auth/login')
      return
    }
  }
  next()
})

// This will update routes at runtime without reloading the page
if (import.meta.hot) {
  handleHotUpdate(router)
}

export default router
