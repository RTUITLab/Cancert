<template>
  <ul class="menu">
    <li class="noselect" @click="$router.push('/home')"><span>Home</span></li>
    <li class="noselect" @click="$router.push('/about')"><span>About</span></li>
  </ul>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';

@Component
export default class Menu extends Vue {}
</script>

<style lang="scss">
.menu {
  margin-left: 10vw;
  margin-right: 10vw;

  list-style-type: none;
  padding: 0;
  height: 60px;
  box-shadow: 0px 1px 0px 0px #fbfbfb;

  display: flex;

  li {
    display: inline-flex;
    align-items: center;
    padding-left: 20px;
    padding-right: 20px;

    span {
      color: #fbfbfb;
    }

    &:hover {
      box-shadow: 0px -4px 0px 0px #fbfbfb inset;
      cursor: pointer;
    }
  }
}
</style>
