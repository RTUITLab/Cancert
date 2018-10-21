import Vue from 'vue';
import Router from 'vue-router';
import Home from './views/Home.vue';

Vue.use(Router);

export default new Router({
  mode: 'history',
  base: process.env.BASE_URL,
  routes: [
    {
      path: '/dashboard',
      name: 'dashboard',
      component: () =>
        import(/* webpackChunkName: "about" */ './views/Dashboard.vue')
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
