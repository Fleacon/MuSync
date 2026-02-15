import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/auth'

import SearchPage from '../views/SearchPage.vue'
import AccountPage from '../views/AccountPage.vue'
import LoginPage from '../views/LoginPage.vue'

const routes = [
  { path: '/', component: SearchPage },
  { path: '/account', component: AccountPage, meta: { requiresAuth: true } },
  { path: '/login', component: LoginPage },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

router.beforeEach(async (to) => {
  const auth = useAuthStore()

  await auth.checkAuth()

  if (to.meta.requiresAuth && !auth.isAuthenticated) {
    return '/login'
  }

  if (to.path === '/login' && auth.isAuthenticated) {
    return '/account'
  }
})

export default router
