import Vue from 'vue';
import Router from 'vue-router';
import Home from './views/Home.vue';

Vue.use(Router);

export default new Router({
  mode: 'history',
  base: process.env.BASE_URL,
  routes: [
    {
      path: '/about',
      name: 'about',
      component: () =>
        import(/* webpackChunkName: "about" */ './views/About.vue')
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
