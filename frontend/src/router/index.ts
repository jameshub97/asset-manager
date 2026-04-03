// frontend/src/router/index.ts
import { createRouter, createWebHistory } from 'vue-router'
import { auth } from '@/services/auth'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/login',
      name: 'login',
      component: () => import('@/components/Login.vue'),
    },
    {
      path: '/home',
      name: 'home',
      redirect: '/assets',
    },
    {
      path: '/',
      redirect: '/login',
    },
    // Keep other routes if needed
    {
      path: '/assets',
      name: 'assets',
      component: () => import('@/components/HomePage.vue'),
      meta: { requiresAuth: true },
    },
  ],
})

router.beforeEach((to, from, next) => {
  // If route requires auth and user is not authenticated
  if (to.meta.requiresAuth && !auth.isAuthenticated()) {
    next('/login')
  }
  // If user is logged in and tries to go to login page
  else if (to.path === '/login' && auth.isAuthenticated()) {
    next('/assets')
  }
  else {
    next()
  }
})

export default router
