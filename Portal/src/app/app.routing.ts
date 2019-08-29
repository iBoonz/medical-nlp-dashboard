import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

// Import Containers
import {
  FullLayoutComponent,
  SimpleLayoutComponent
} from './containers';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'referral',
    pathMatch: 'full', 
  },
  {
    path: '',
    component: FullLayoutComponent,
    data: {
      title: 'New Referal'
    },
    children: [
      {
        path: 'referral',
        loadChildren: './views/referral/referral.module#ReferralModule'
      },
      {
        path: 'dashboard',
        loadChildren: './views/dashboard/dashboard.module#DashboardModule'
      }
    ],
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
