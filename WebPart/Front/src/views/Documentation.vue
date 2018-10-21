<template>
  <div class="documentation">
    <Page>
      <template slot="main">

        <LandingElement v-for="(apiCall, index) in apiCalls" :key="index">
          <template slot="first">
            <div class="api-url">
              <span v-bind:class="getMethodClass(apiCall.method)"></span>
              <span class="base-url"></span>{{apiCall.url}}
            </div>
            <br>
            <el-input type="textarea" readonly :value="apiCall.input" v-if="apiCall.input" autosize resize="none"></el-input>

          </template>
          <template slot="second">
            <pre>{{apiCall.output}}</pre>
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

interface ApiCall {
  method: string;
  url: string;
  input?: string;
  output: string;
}

@Component({
  components: {
    Page,
    LandingElement
  }
})
export default class Documentation extends Vue {
  public getMethodClass(method: string) {
    const result: any = {};
    result[`method-${method}`] = true;
    return result;
  }

  get apiCalls(): ApiCall[] {
    return [
      {
        method: 'post',
        url: '/api/HospitalAccount',
        input: `"LongAccessToken"`,
        output: `{
    "jwt": "longJwt",
    "me": {
        "id": "6f6238d7-b7a5-4496-9ebe-cc0b6675c6db",
        "name": "Cancer Hospital"
    }
}`
      },
      {
        method: 'get',
        url: '/api/mr/algorithms',
        output: `[
    {
        "id": "e60fa65d-3e66-4b83-b9ac-e9c6abeccae6",
        "name": "SampleAlg",
        "version": "0.0.1"
    }
]`
      },
      {
        method: 'get',
        url: '/api/mr/records',
        output: `[
    {
        "id": "9841e088-9491-4d78-f694-08d636ee95ec",
        "hospitalId": "6f6238d7-b7a5-4496-9ebe-cc0b6675c6db"
    }
]`
      },
      {
        method: 'get',
        url: '/api/mr/records/{{cancerId}}',
        output: `[
    {
        "id": "9841e088-9491-4d78-f694-08d636ee95ec",
        "hospitalId": "6f6238d7-b7a5-4496-9ebe-cc0b6675c6db"
    }
]`
      },
      {
        method: 'get',
        url: '/api/mr/analyze',
        output: `[
    {
        "id": "1dffdcbc-718c-431c-cbcf-08d637041ac4",
        "status": -1,
        "mrRecordId": "9841e088-9491-4d78-f694-08d636ee95ec",
        "mrAlgorithmId": "e60fa65d-3e66-4b83-b9ac-e9c6abeccae6"
    }
]`
      },
      {
        method: 'post',
        url: '/api/mr',
        input: '{{form-data files}}',
        output: ``
      },
      {
        method: 'post',
        url: '/api/mr/analyze/queue/{{cancerRecordId}}',
        input: `{
	"algorithms":[
		"{{cancerAlgId}}"
	]
}`,
        output: ``
      }
    ];
  }
}
</script>

<style lang="scss">
.documentation {
  .api-url {
    font-family: monospace;

    .method {
      padding: 5px;
      color: #fbfbfb;
      width: 40px;
      margin-right: 3px;
    }

    .method-get::after {
      @extend .method;
      background-color: #67c23a;
      content: 'GET';
    }
    .method-post::after {
      @extend .method;
      background-color: #409eff;
      content: 'POST';
    }
    .method-put::after {
      @extend .method;
      background-color: #e6a23c;
      content: 'PUT';
    }
    .method-delete::after {
      @extend .method;
      background-color: #f56c6c;
      content: 'DELETE';
    }

    .base-url {
      font-weight: bold;
    }
    .base-url::before {
      content: '{{apiUrl';
    }
    .base-url::after {
      content: '}}';
    }
  }
}
</style>
