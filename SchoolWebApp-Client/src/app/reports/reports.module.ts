import {NgModule} from '@angular/core';
import {CoreModule} from '@/core/core.module';
import {SharedModule} from '@/shared/shared.module';
import {SchoolReportComponent} from './components/school-report/school-report.component';
import {StaffReportComponent} from './components/staff-report/staff-report.component';
import {ClassReportComponent} from './components/class-report/class-report.component';
import {AcademicsReportComponent} from './components/academics-report/academics-report.component';
import { ReportsComponent } from './reports.component';
import { ReportsLoaderComponent } from './components/shared/reports-loader/reports-loader.component';
import { StaffAttendanceReportComponent } from './components/staff-report/staff-attendance-report/staff-attendance-report.component';
import { StaffAttendanceReportTableComponent } from './components/staff-report/staff-attendance-report/staff-attendance-report-table/staff-attendance-report-table.component';
import { StaffAttendanceDetailsReportComponent } from './components/staff-report/staff-attendance-details-report/staff-attendance-details-report.component';

@NgModule({
    declarations: [
        ReportsComponent,
        SchoolReportComponent,
        StaffReportComponent,
        ClassReportComponent,
        AcademicsReportComponent,
        ReportsLoaderComponent,
        StaffAttendanceReportComponent,
        StaffAttendanceReportTableComponent,
        StaffAttendanceDetailsReportComponent
    ],
    imports: [CoreModule, SharedModule]
})
export class ReportsModule {}
