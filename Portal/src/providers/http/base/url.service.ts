import { environment } from '../../../environments/environment'
export class UrlService {

  public static createReferral(): string {
    return UrlService.getFullUrl(`/Referral`);
  }

  public static getReferralDashboard(): string {
    return UrlService.getFullUrl(`/Referral/dashboard`);
  }
  
  public static updateStatus(id:number, status: number): string {
    return UrlService.getFullUrl(`/Referral/${id}/status/${status}`);
  }

  // Helpers:
  public static getFullUrl(url: string): string {
    return environment.baseUrl + url;
  }

  public static getPagedUrl(url: string, skip: number, take: number) {
    return `${url}?skip=${skip}&take=${take}`;

  }

}

