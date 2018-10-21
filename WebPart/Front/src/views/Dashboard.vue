<template>
  <div class="dashboard">
    <Page>
      <template slot="main">
        <LandingElement>
          <template slot="first">
            <h3>Records</h3>
            <br>

            <el-card class="box-card" v-for="record in records" :key="record.id">
              <div class="text item">
                <el-row :gutter="24">
                  <el-col :span="8">
                    <b>{{ record.pagesCount }} page{{ record.pagesCount === 1 ? '' : 's' }}</b>
                  </el-col>
                  <el-col :span="16" align="right">
                    <el-select v-model="algorithm" placeholder="Algorithm" size="mini" style="display: inline; float: left">
                      <el-option v-for="item in algorithms" :key="item.name" :label="`${item.name} ${item.version}`" :value="item">
                      </el-option>
                    </el-select>
                    <el-button size="mini" type="primary" @click="queueAnalize(record)">Queue analize</el-button>
                  </el-col>
                </el-row>
                <el-row>
                  <el-col>
                    <pre>Total size: {{ record.size }} bytes</pre>
                  </el-col>
                </el-row>

                <div v-for="analyze in getRecordAnalyze(record)" :key="analyze.id" class="analyze">
                  <el-row :gutter="24">
                    <el-col :span="20" class="progress-col">
                      <el-progress :percentage="analyze.status"></el-progress>
                    </el-col>
                    <el-col :span="4" v-if="analyze.status === 100">
                      <el-button type="success" size="mini" @click="$router.push(`/dashboard/analyze/${analyze.id}`)">Open</el-button>
                    </el-col>
                  </el-row>
                </div>
              </div>
            </el-card>

          </template>
          <template slot="second">
            <span width="100" align="right">
              <h2>{{hospitalName}}</h2>
            </span>
            <el-form>
              <el-form-item>
                <input type="file" ref="fileInput" multiple>
                <el-button @click="onSubmit" type="primary" size="small">Submit</el-button>
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

import Page from '@/components/Page.vue';
import LandingElement from '@/components/LandingElement.vue';
import { currentState, Algorithm, Record } from '@/models';
import axios from 'axios';

@Component({
  components: {
    Page,
    LandingElement
  }
})
export default class Dashboard extends Vue {
  public algorithms: Algorithm[] = [];
  public records: Record[] = [];

  public attachments: File[] = [];
  public algorithm?: Algorithm;

  public fetchAll() {
    Promise.all([
      currentState.fetchAlgorithms(),
      currentState.fetchRecords(),
      currentState.fetchAnalyze()
    ]).then(() => {
      this.algorithms = currentState.algorithms;
      this.algorithm = this.algorithms[0];
      this.records = currentState.records;
    });
  }

  public mounted() {
    this.fetchAll();
    setInterval(() => {
      this.fetchAll();
    }, 5000);
  }

  public getRecordAnalyze(record: Record) {
    return currentState.getRecordAnalyze(record);
  }

  public getAction(): string {
    return axios.defaults.baseURL + 'mr';
  }

  public queueAnalize(record: Record) {
    axios
      .post(`mr/analyze/queue/${record.id}`, {
        algorithms: [this.algorithms[0].id]
      })
      .then(() => {
        Promise.all([
          currentState.fetchRecords(),
          currentState.fetchAnalyze()
        ]).then(() => {
          this.records = currentState.records;
        });
      });
  }

  public onSubmit() {
    if (this.$refs.fileInput == null) {
      return;
    }

    const formData: FormData = new FormData();

    const files = (this.$refs.fileInput as any).files;
    for (let i = 0; i < files.length; ++i) {
      formData.append('files', files[i]);
    }

    axios
      .post('mr', formData, {
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      })
      .then(() => (this.attachments = []));

    return true;
  }

  get hospitalName(): string {
    return currentState.hospital ? currentState.hospital.name : '';
  }
}
</script>

<style lang="scss">
.dashboard {
  .box-card {
    .progress-col {
      line-height: 28px;
    }

    .analyze:not(:last-child) {
      margin-bottom: 0.5em;
    }
  }
}
</style>
