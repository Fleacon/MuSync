import { createRouter, createWebHistory } from 'vue-router'

import SearchPage from '../SearchPage.vue'
import HomePage from '../HomePage.vue'

const routes = [
  { path: '/', component: HomePage },
  { path: '/search', component: SearchPage },
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

export default router
