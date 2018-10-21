<template>
  <div class="authorize">
    <Page>
      <template slot="main">
        <LandingElement style="height: 100%">
          <template slot="first">
            <h2>Some words about registration process</h2>
            <br>
            Lorem ipsum dolor sit amet consectetur, adipisicing elit. Numquam et, voluptate dicta aut cumque ipsum minima
          </template>
          <template slot="second">
            <el-form>
              <el-form-item label="Subscription key">
                <el-input placeholder="Please input" v-model="subscriptionKey" width="100"></el-input>
              </el-form-item>
              <el-form-item>
                <el-button type="primary" @click="onSubmit">Sign in</el-button>
              </el-form-item>
            </el-form>
          </template>
        </LandingElement>
      </template>
    </Page>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';
import axios from 'axios';

import Page from '@/components/Page.vue';
import LandingElement from '@/components/LandingElement.vue';

@Component({
  components: {
    Page,
    LandingElement
  }
})
export default class Authorize extends Vue {
  public subscriptionKey: string = '';

  public onSubmit() {
    axios
      .post('HospitalAccount', `"${this.subscriptionKey}"`)
      .then((response) => {
        console.log(response);
      });
  }
}
</script>

<style lang="scss">
.authorize {
  .main-wrapper {
    background-color: #f6f7f9;
  }
}
</style>

