<template>
  <div class="landing-element">
    <el-row v-if="!oneColumn">
      <el-col :xs="24" :md="12" v-bind:class="getOrderClass(reversed)">
        <slot name="first"></slot>
      </el-col>
      <el-col :xs="24" :md="12" v-bind:class="getOrderClass(!reversed)">
        <slot name="second"></slot>
      </el-col>

    </el-row>
    <slot name="third"></slot>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';

@Component
export default class LandingElement extends Vue {
  @Prop({
    default: false
  })
  public reversed!: boolean;

  @Prop({
    default: false
  })
  public oneColumn!: boolean;

  public getOrderClass(reversed: boolean) {
    return {
      first: !reversed,
      second: reversed
    };
  }
}
</script>

<style lang="scss">
.landing-element {
  display: flex;
  background-color: #f6f7f9;

  & > .el-row {
    width: 1200px;
    margin-left: auto;
    margin-right: auto;
    margin-top: 20px;
    display: flex;

    & > .el-col {
      padding: 20px;
      font-size: 20px;

      * {
        max-width: 100%;
      }

      &.first {
        order: 1;
      }
      &.second {
        order: 2;
      }
    }
  }

  &:not(:last-child) {
    border-bottom: 1px solid hsl(211, 20%, 91%);
  }
  &:nth-child(2) {
    background-color: #ffffff;
  }
}
</style>


