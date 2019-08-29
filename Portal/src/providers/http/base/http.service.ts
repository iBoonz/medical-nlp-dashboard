import { Injectable } from '@angular/core';
import { Http, Response, Headers, RequestOptions, ResponseContentType } from '@angular/http';
// RXJS
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import 'rxjs/add/observable/throw';

import { HttpHeaders, HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

// SERVICES

@Injectable()
export class HttpService {
  constructor(private http: HttpClient) {

  }

  getBlobObservable(url: string): Observable<any> {
    return this.getBlob(url);
  }

  getObservable(url: string): Observable<any> {
    return this.get(url);
  }

  postObservable(url: string, data: any): Observable<any> {
    return this.post(url, data, null);
  }

  postFormObservable(url: string, data: any): Observable<any> {
    let headers = new HttpHeaders();
    headers = headers
      .set('enctype', 'multipart/form-data')
      // .set('Authorization', `Bearer ${this.adalSvc.accessToken}`);
    let formHeader = { headers };
    return this.post(url, data, formHeader);
  }

  putObservable(url: string, data: any): Observable<any> {
    return this.put(url, data);
  }

  deleteObservable(url: string): Observable<any> {
    return this.delete(url);
  }

  private getRequestOptions(): any {
    let headers = new HttpHeaders();
    headers = headers
      .set('Content-Type', 'application/json')
      // .set('Authorization', `Bearer ${this.adalSvc.accessToken}`);
    return { headers };
  }

  private getBlob(url: string): Observable<any> {
    let requestOptions: any = this.getRequestOptions();
    requestOptions.responseType = 'Blob';
    return this.http.get(url, requestOptions);
  }


  private get(url: string): Observable<any> {
    return this.http.get(url, this.getRequestOptions())
      .catch(this.handleErrorObservable);
  }

  private post(url: string, data: any, requestOptions: any): Observable<any> {
    if (requestOptions) {
      return this.http.post(url, data, requestOptions)
        .catch(this.handleErrorObservable);
    }
    return this.http.post(url, data, this.getRequestOptions())
      .catch(this.handleErrorObservable);
  }

  private put(url: string, data: any): Observable<any> {
    return this.http.put(url, data, this.getRequestOptions())
      .catch(this.handleErrorObservable);
  }

  private delete(url: string): Observable<any> {
    return this.http.delete(url, this.getRequestOptions());
  }

  private handleErrorObservable(response: Response | any): any {
    // In a real world app, you might use a remote logging infrastructure
    let errMsg: string;
    if (response instanceof Response) {
      const body: any = response.json() || '';
      const err: string = body.error || JSON.stringify(body);
      errMsg = `${response.status} - ${response.statusText || ''} ${err}`;
    } else {
      errMsg = response.error ? response.error : response.message;
    }

    return Observable.throw(errMsg);
  }

}
