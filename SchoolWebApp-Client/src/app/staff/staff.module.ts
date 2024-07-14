import {NgModule} from '@angular/core';
import {StaffComponent} from './staff.component';
import {SharedModule} from '@/shared/shared.module';
import {CoreModule} from '@/core/core.module';
import {StaffDetailsComponent} from './components/staff-details/staff-details.component';
import {StaffDetailsFormComponent} from './components/staff-details/staff-details-form/staff-details-form.component';
import {StaffDetailsTableComponent} from './components/staff-details/staff-details-table/staff-details-table.component';
import {DataTablesModule} from 'angular-datatables';
import {StaffDetailsViewComponent} from './components/staff-details/staff-details-view/staff-details-view.component';
import {StaffAssignmentsComponent} from './components/staff-assignments/staff-assignments.component';
import { StaffAttendanceComponent } from './components/staff-assignments/staff-attendance/staff-attendance.component';
import { StaffAttendanceFormComponent } from './components/staff-assignments/staff-attendance/staff-attendance-form/staff-attendance-form.component';
import { StaffAttendanceTableComponent } from './components/staff-assignments/staff-attendance/staff-attendance-table/staff-attendance-table.component';
import { StaffDisciplineComponent } from './components/staff-assignments/staff-discipline/staff-discipline.component';
import { StaffDisciplineFormComponent } from './components/staff-assignments/staff-discipline/staff-discipline-form/staff-discipline-form.component';
import { StaffDisciplineTableComponent } from './components/staff-assignments/staff-discipline/staff-discipline-table/staff-discipline-table.component';

@NgModule({
    declarations: [
        StaffComponent,
        StaffDetailsComponent,
        StaffDetailsFormComponent,
        StaffDetailsTableComponent,
        StaffDetailsViewComponent,
        StaffAssignmentsComponent,
        StaffAttendanceComponent,
        StaffAttendanceFormComponent,
        StaffAttendanceTableComponent,
        StaffDisciplineComponent,
        StaffDisciplineFormComponent,
        StaffDisciplineTableComponent
    ],
    imports: [DataTablesModule, CoreModule, SharedModule]
})
export class StaffModule {}
