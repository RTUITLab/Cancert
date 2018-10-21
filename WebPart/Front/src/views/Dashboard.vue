<template>
  <div class="dashboard">
    <Page>
      <template slot="main">
        <LandingElement>
          <template slot="first">
            
          </template>
          <template slot="second">
            
          </template>
        </LandingElement>
      </template>
    </Page>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator';

import Page from '@/components/Page.vue';
import LandingElement from '@/components/LandingElement.vue';

@Component({
  components: {
    Page,
    LandingElement
  }
})
export default class Dashboard extends Vue {}
</script>

<style lang="scss">
</style>
