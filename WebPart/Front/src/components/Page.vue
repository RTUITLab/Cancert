<template>
  <div class="page">
    <slot name="header">
      <div class="header">
        <Menu />
      </div>
    </slot>

    <div class="main-wrapper">
      <slot name="main"></slot>
    </div>

    <slot name="footer">
      <Footer />
    </slot>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';

import Menu from '@/components/Menu.vue';
import Footer from '@/components/Footer.vue';

@Component({
  components: {
    Menu,
    Footer
  }
})
export default class Page extends Vue {}
</script>

<style lang="scss">
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
  font-family: 'Helvetica Neue', Helvetica, 'PingFang SC', 'Hiragino Sans GB',
    'Microsoft YaHei', '微软雅黑', Arial, sans-serif;
  color: #333;
}

body {
  overflow-x: hidden;
}

.noselect {
  -moz-user-select: none;
  -webkit-user-select: none;
  -ms-user-select: none;
  -o-user-select: none;
  user-select: none;
}

.el-button {
  & > span {
    color: #fbfbfb;
  }
}

.page {
  .header {
    height: 60px;
    background: #37517e;
    color: #fbfbfb;
  }

  .main-wrapper {
    position: relative;
    min-height: calc(100vh - 160px);
  }
}
</style>