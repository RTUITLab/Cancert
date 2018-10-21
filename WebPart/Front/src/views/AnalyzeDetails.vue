<template>
  <div class="analyze-details">
    <Page>
      <template slot="main">
        <LandingElement :oneColumn="true">
          <template slot="third">
            <el-row>
              <el-col>
                <div class="images" v-if="showImages">
                  <img v-for="i in pagesRange" :key="`img-${i}`" :src="getImageUrl(i)" width="200px" style="margin: 5px">
                </div>
              </el-col>
            </el-row>
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
import { currentState, Analyze } from '@/models';
import Axios from 'axios';

@Component({
  components: {
    Page,
    LandingElement
  }
})
export default class AnalyzeDetails extends Vue {
  public showImages: boolean = false;
  public analyze?: Analyze;

  public mounted() {
    const id = this.$route.params.id;

    this.analyze = currentState.analyze.find((a) => a.id === id);

    if (this.analyze == null) {
      return;
    }

    this.analyze.record = currentState.records.find(
      (r) => this.analyze != null && r.id === this.analyze.mrRecordId
    );

    this.analyze.algorithm = currentState.algorithms.find(
      (r) => this.analyze != null && r.id === this.analyze.mrAlgorithmId
    );

    this.showImages = true;
  }

  public getImageUrl(i: number): string {
    if (this.analyze == null) {
      return '';
    }
    return `${Axios.defaults.baseURL}mr/analyze/result/${this.analyze.id}/${i}`;
  }

  get pagesRange(): number[] {
    if (this.analyze == null || this.analyze.record == null) {
      return [];
    }

    return Array.apply(null, { length: this.analyze.record.pagesCount }).map(
      Number.call,
      Number
    );
  }
}
</script>
