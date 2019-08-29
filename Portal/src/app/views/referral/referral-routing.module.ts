import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ReferralComponent } from './referral.component';

const routes: Routes = [
  {
    path: '',
    component: ReferralComponent,
    data: {
      title: 'Referral'
    },
    // canActivate: [AuthenticationGuard]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ReferralRoutingModule { }
