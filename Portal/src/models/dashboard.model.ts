import { ConvertableFromApi } from './api/convertable-from-api.model';
import { ReferralInfo } from './referralinfo.model';

export class Dashboard extends ConvertableFromApi<Dashboard> {
  totalReferrals: number;
  resolvedReferrals: number;
  openReferrals: number;
  deniedReferrals: number;
  referralInfo: ReferralInfo[];
  femaleRatio: any;
  maleRatio: any;
  constructor(data?: any) {
    super(data);
  }

  readFromObject(): void {
    this.totalReferrals = this.objectFromApi.totalReferrals;
    this.resolvedReferrals = this.objectFromApi.resolvedReferrals;
    this.openReferrals = this.objectFromApi.openReferrals;
    this.deniedReferrals = this.objectFromApi.deniedReferrals;
    this.femaleRatio = Math.round(this.objectFromApi.femaleRatio / this.objectFromApi.referralInfo.length * 100);
    this.maleRatio = Math.round(this.objectFromApi.maleRatio  / this.objectFromApi.referralInfo.length * 100);

    this.referralInfo = [];
    for (let index = 0; index <  this.objectFromApi.referralInfo.length; index++) {
      const element =  this.objectFromApi.referralInfo[index];
      this.referralInfo.push(new ReferralInfo(element));
    }
    
  }

  createNew(element: any): Dashboard {
    return new Dashboard(element);
  }
}

