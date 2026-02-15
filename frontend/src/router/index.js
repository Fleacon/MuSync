import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/auth'

import SearchPage from '../SearchPage.vue'
import HomePage from '../HomePage.vue'
import AccountPage from '../AccountPage.vue'
import LoginPage from '../LoginPage.vue'

const routes = [
  { path: '/', component: HomePage },
  { path: '/search', component: SearchPage },
  { path: '/account', component: AccountPage },
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
