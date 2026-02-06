import { createRouter, createWebHistory } from 'vue-router'

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

export default router
