<template>
  <div class="home-page">
    <page>
      <div slot="header" class="header">
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 100" preserveAspectRatio="none" class="fill-dark">
          <polygon points="0,100 100,100 0,0" />
        </svg>
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 100" preserveAspectRatio="none" class="fill-light">
          <polygon points="0,100 100,0 100,100" />
        </svg>

        <div style="position: absolute; top: 0; width: 100vw">
          <Menu />

          <el-row>
            <el-col style="display: flex">
              <h1>World Free From Cancer</h1>
            </el-col>
            <el-col align="center">
              <el-button type="primary" round @click="$router.push('/authorize')">Sign in</el-button>
            </el-col>
          </el-row>
        </div>
      </div>

      <div slot="main" class="main">
        <LandingElement>
          <template slot="first">
            <img src="https://picsum.photos/600/300" alt="Test">
          </template>
          <template slot="second">
            <h1>First sample text</h1>
            Lorem, ipsum dolor sit amet consectetur adipisicing elit. Aliquam exercitationem, quasi ipsum consectetur perspiciatis dignissimos ut obcaecati eligendi quidem aliquid necessitatibus omnis quisquam natus, nesciunt saepe incidunt quos ex officia?
          </template>
        </LandingElement>
        <LandingElement :reversed="true">
          <template slot="first">
            <img src="https://picsum.photos/600/300" alt="Test">
          </template>
          <template slot="second">
            <h1>First sample text</h1>
            Lorem, ipsum dolor sit amet consectetur adipisicing elit. Aliquam exercitationem, quasi ipsum consectetur perspiciatis dignissimos ut obcaecati eligendi quidem aliquid necessitatibus omnis quisquam natus, nesciunt saepe incidunt quos ex officia?
          </template>
        </LandingElement>
      </div>
    </page>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';

import Page from '@/components/Page.vue';
import Menu from '@/components/Menu.vue';
import LandingElement from '@/components/LandingElement.vue';

@Component({
  components: {
    Page,
    Menu,
    LandingElement
  }
})
export default class About extends Vue {}
</script>


<style lang="scss">
.home-page {
  .header {
    width: 100vw;
    height: 340px;
    background: #37517e;
    color: #fbfbfb;

    .el-button {
      z-index: 1000;
      font-size: 20px;
      font-weight: bold;
    }

    h1 {
      z-index: 1000;
      font-size: 30px;
      font-weight: bold;
      margin-top: 50px;
      padding: 20px;
      margin-left: 10vw;
      color: #fbfbfb;
    }

    @media (min-width: 576px) {
      h1 {
        margin-left: auto;
        margin-right: auto;
      }
    }

    @media (min-width: 768px) {
      h1 {
        margin-left: auto;
        margin-right: auto;
        font-size: 40px;
      }
    }

    @media (min-width: 992px) {
      h1 {
        margin-left: auto;
        margin-right: auto;
        font-size: 60px;
      }
    }

    svg {
      position: relative;
      display: block;
      left: 0;

      width: 100vw;
    }

    .fill-light {
      fill: #405f96;
      height: 20vw;
      top: calc(340px - 30vw);
    }

    .fill-dark {
      fill: darken(#405f96, 15);
      height: 10vw;
      top: calc(340px - 10vw);
    }
  }
}
</style>

