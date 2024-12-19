// router/index.js

import { createRouter, createWebHistory } from 'vue-router';
import Login from '../views/LoginPage.vue';
import Home from '../views/HomePage.vue';
import ProfilePage from '@/views/ProfilePage.vue';

const routes = [
  { path: '/', redirect: '/login' },
  { path: '/login', component: Login },
  { path: '/home', name:'HomePage', component: Home, meta: { requiresAuth: true } },
  { path: '/profile', name:'ProfilePage', component: ProfilePage}
];

const router = createRouter({
  history: createWebHistory(), 
  routes
});

router.beforeEach((to, from, next) => {
  const isAuthenticated = !!localStorage.getItem('token');
  if (to.matched.some(record => record.meta.requiresAuth) && !isAuthenticated) {
    next('/login');
  } else {
    next();
  }
});

export default router;
