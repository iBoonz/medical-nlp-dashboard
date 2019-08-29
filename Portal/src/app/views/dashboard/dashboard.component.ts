import { Component, OnInit, TemplateRef, ViewEncapsulation, EventEmitter } from '@angular/core';
import { ToasterService } from 'angular2-toaster';
import { ReferralHttpService } from 'providers';
import { Dashboard } from 'models/dashboard.model';
import { Router } from '@angular/router';


@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class DashboardComponent implements OnInit {
  formData: FormData;
  dashboardModel: Dashboard;
  constructor(
    private referralHttpService: ReferralHttpService,
    private router: Router) {
  }

  ngOnInit(): void {
    this.referralHttpService.getDashboardInfo().subscribe(data => {
      this.dashboardModel = data;
    })
  }

  onChange(id: number, value: number) {
    this.referralHttpService.UpdateStatus(id, value).subscribe(data => {
      this.dashboardModel = data;
    })
  }

  newReferral():void{
    this.router.navigate(['/referral' ])

  }

  applyStyles(width):any{
    const styles = {'width' : width + '%'};
    return styles;
  }

}