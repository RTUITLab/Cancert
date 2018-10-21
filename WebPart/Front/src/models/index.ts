import axios from 'axios';

export class Algorithm {
  constructor(public id: string, public name: string, public version: string) {}
}

export class Hospital {
  constructor(public id: string, public name: string) {}
}

export class Record {
  constructor(
    public id: string,
    public hospitalId: string,
    public pagesCount: number,
    public size: number
  ) {}
}

export class Analyze {
  public record?: Record;
  public algorithm?: Algorithm;

  constructor(
    public id: string,
    public status: number,
    public mrRecordId: string,
    public mrAlgorithmId: string
  ) {}
}

export class State {
  public hospital?: Hospital;
  public algorithms: Algorithm[] = [];
  public records: Record[] = [];
  public analyze: Analyze[] = [];

  constructor() {
    if (
      localStorage.getItem('jwt') != null &&
      localStorage.getItem('hospital') != null
    ) {
      axios.defaults.headers.common.Authorization = `Bearer ${localStorage.getItem(
        'jwt'
      )}`;

      const h = JSON.parse(localStorage.getItem('hospital') || '');
      if (h) {
        this.hospital = new Hospital(h.id || '', h.name || '');
      }
    }
  }

  public authorize(key: string) {
    return new Promise((resolve, reject) => {
      axios
        .post('HospitalAccount', `"LongAccessTokenForCancerHospital"`)
        .then((response) => {
          const body = response.data;
          axios.defaults.headers.common.Authorization = `Bearer ${body.jwt}`;
          localStorage.setItem('jwt', body.jwt);
          localStorage.setItem('hospital', JSON.stringify(body.me));
          this.hospital = new Hospital(body.me.id, body.me.name);
          resolve();
        })
        .catch((error) => {
          reject();
        });
    });
  }

  public fetchAlgorithms() {
    return new Promise((resolve, reject) => {
      axios
        .get('mr/algorithms')
        .then((response) => {
          this.algorithms = response.data;
          resolve();
        })
        .catch((error) => reject());
    });
  }

  public fetchRecords() {
    return new Promise((resolve, reject) => {
      axios
        .get('mr/records')
        .then((response) => {
          this.records = response.data;
          resolve();
        })
        .catch(() => reject());
    });
  }

  public fetchAnalyze() {
    return new Promise((resolve, reject) => {
      axios
        .get('mr/analyze')
        .then((response) => {
          this.analyze = response.data;
          resolve();
        })
        .catch(() => reject());
    });
  }

  public getRecordAnalyze(record: Record) {
    return this.analyze.filter((a) => {
      if (a.mrRecordId === record.id) {
        a.record = record;
        a.algorithm = this.algorithms.find((al) => al.id === a.mrAlgorithmId);
        return true;
      } else {
        return false;
      }
    });
  }
}

export const currentState = new State();
