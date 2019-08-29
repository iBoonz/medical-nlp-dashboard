import { ConvertableFromApi } from './api/convertable-from-api.model';

export class ReferralInfo extends ConvertableFromApi<ReferralInfo> {
  patientName: string;
  physicianName: string;
  remarks: string;
  createdOn: Date;
  status: number;
  id:number;
  constructor(data?: any) {
    super(data);
  }

  readFromObject(): void {
    this.patientName = this.objectFromApi.patientName;
    this.physicianName = this.objectFromApi.physicianName;
    this.remarks = this.objectFromApi.remarks;
    this.createdOn = this.objectFromApi.createdOn;
    this.status = this.objectFromApi.status;
    this.id = this.objectFromApi.id;
  }

  createNew(element: any): ReferralInfo {
    return new ReferralInfo(element);
  }
}

