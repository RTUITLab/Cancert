<template>
  <div class="footer" align="center">
    <b>No rights reserved&nbsp;|&nbsp;reide740@gmail.com</b>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';

@Component
export default class Footer extends Vue {}
</script>


<style lang="scss">
.footer {
  background-color: #2e2532;
  height: 60px;
  line-height: 60px;

  b {
    color: #6c666f;
  }

  &::after {
    position: relative;
    display: block;
    left: 0;
    right: 0;

    width: 100%;
    height: 40px;
    line-height: 40px;

    background-color: #201a23;
    color: #6c666f;

    text-align: center;
    content: 'JUNCTIONxBudapest 2018';
  }
}
</style>
