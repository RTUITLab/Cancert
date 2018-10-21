<template>
  <div class="authorize">
    <Page>
      <template slot="main">
        <LandingElement style="height: 100%">
          <template slot="first">
            <h2>Join Us Today!</h2>
            <br>
            In order to get your subscription key contact us via email or telephone.<br><br>
            anton.pushkin2@gmail.com <br>
            +7(962)956-34-35
          </template>
          <template slot="second">
            <el-form>
              <el-form-item label="Subscription key">
                <el-input placeholder="Please enter the key" v-model="subscriptionKey" width="100"></el-input>
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

