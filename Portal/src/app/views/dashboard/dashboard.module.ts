import { NgModule } from '@angular/core';
import { ChartsModule } from 'ng2-charts/ng2-charts';
import { CommonModule } from '@angular/common';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { TimepickerModule } from 'ngx-bootstrap/timepicker';

import { TranslateModule } from '@ngx-translate/core';
import { NgxUploaderModule } from 'ngx-uploader';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ModalModule } from 'ngx-bootstrap/modal';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { NgSelectModule } from '@ng-select/ng-select';
import { DashboardRoutingModule } from './dashboard-routing.module';
import { DashboardComponent } from './dashboard.component';

@NgModule({
  imports: [
    CommonModule,
    NgxUploaderModule,
    FormsModule,
    ReactiveFormsModule,
    TranslateModule,
    ChartsModule,
    ModalModule.forRoot(),
    TabsModule,
    DashboardRoutingModule,
    NgSelectModule,
    BsDatepickerModule,
    TimepickerModule
  ],
  declarations: [
    DashboardComponent
  ]
})
export class DashboardModule { }
