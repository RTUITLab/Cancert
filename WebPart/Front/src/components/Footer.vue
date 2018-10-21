<template>
  <div class="footer">
    <el-row>
      <el-col :xs="24" :md="12" align="right">
        <b>No rights reserved&nbsp;|&nbsp;</b>
      </el-col>
      <el-col :xs="24" :md="12">
        <b>reide740@gmail.com</b>
      </el-col>
    </el-row>
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

  & > .el-row {
    line-height: 60px;
    & > .el-col b {
      color: #6c666f;
    }
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
