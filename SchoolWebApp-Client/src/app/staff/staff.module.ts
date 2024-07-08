import { NgModule } from '@angular/core';
import { StaffComponent } from './staff.component';
import { SharedModule } from '@/shared/shared.module';
import { CoreModule } from '@/core/core.module';
import { StaffDetailsComponent } from './components/staff-details/staff-details.component';
import { StaffAttendanceComponent } from './components/staff-attendance/staff-attendance.component';
import { StaffDetailsFormComponent } from './components/staff-details/staff-details-form/staff-details-form.component';
import { StaffDetailsTableComponent } from './components/staff-details/staff-details-table/staff-details-table.component';
import { DataTablesModule } from 'angular-datatables';

@NgModule({
  declarations: [StaffComponent, StaffDetailsComponent, StaffAttendanceComponent, StaffDetailsFormComponent, StaffDetailsTableComponent],
  imports: [
    DataTablesModule,
    CoreModule,
    SharedModule
  ]
})
export class StaffModule { }
