import { Injectable } from '@angular/core';

// RXJS
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

// PROVIDERS
import { HttpService } from './base/http.service';
import { UrlService } from './base/url.service';
import { Helper } from 'providers/helpers/helper.service';
import { Referral } from 'models/referral.model';
import { Dashboard } from 'models/dashboard.model';


@Injectable()
export class ReferralHttpService {

  constructor(private http: HttpService) {

  }

  public add(refferal: Referral): Observable<any> {
    return this.http.postObservable(UrlService.createReferral(), refferal).pipe(map(data => {
      return data
    }));
  }

  getDashboardInfo(): Observable<Dashboard> {
    return this.http.getObservable(UrlService.getReferralDashboard()).pipe(map(data => {
      return Helper.api.convertFromApiModel(new Dashboard(), data);
    }));  
  }

  UpdateStatus(id: number, status: number): Observable<Dashboard> {
    return this.http.putObservable(UrlService.updateStatus(id, status), null).pipe(map(data => {
      return Helper.api.convertFromApiModel(new Dashboard(), data);
    }));  
  }

}
