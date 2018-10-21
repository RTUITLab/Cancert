import Vue from 'vue';
import Router, { NavigationGuard } from 'vue-router';
import Home from './views/Home.vue';

import { currentState } from '@/models';

Vue.use(Router);

const guard: NavigationGuard = (to, from, next) => {
  if (currentState.hospital == null) {
    next('/home');
    return;
  }

  next();
};

const router = new Router({
  mode: 'history',
  base: process.env.BASE_URL,
  routes: [
    {
      path: '/dashboard',
      name: 'dashboard',
      component: () =>
        import(/* webpackChunkName: "about" */ './views/Dashboard.vue'),
      beforeEnter: guard
    },
    {
      path: '/dashboard/analyze/:id',
      name: 'analyze',
      component: () =>
        import(/* webpackChunkName: "about" */ './views/AnalyzeDetails.vue'),
      beforeEnter: guard
    },
    {
      path: '/documentation',
      name: 'documentation',
      component: () =>
        import(/* webpackChunkName: "about" */ './views/Documentation.vue')
    },
    {
      path: '/authorize',
      name: 'authorize',
      component: () =>
        import(/* webpackChunkName: "about" */ './views/Authorize.vue')
    },
    {
      path: '/*',
      name: 'home',
      component: Home
    }
  ]
});

export default router;
