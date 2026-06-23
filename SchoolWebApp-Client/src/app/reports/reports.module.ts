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
import { StaffSubjectDetailedReportComponent } from './components/staff-report/staff-subject-detailed-report/staff-subject-detailed-report.component';
import { StudentSubjectDetailedReportComponent } from './components/student-report/student-subject-detailed-report/student-subject-detailed-report.component';
import { StudentSubjectPageComponent } from './components/pages/student-subject-page.component';
import { ClassListsReportComponent } from './components/class-report/class-lists-report/class-lists-report.component';
import { ClassAttendanceReportComponent } from './components/class-report/class-attendance-report/class-attendance-report.component';
import { ClassAttendanceReportTableComponent } from './components/class-report/class-attendance-report/class-attendance-report-table/class-attendance-report-table.component';
import { ClassAttendanceDetailsReportComponent } from './components/class-report/class-attendance-details-report/class-attendance-details-report.component';
import { MissingMarksReportComponent } from './components/academics-report/missing-marks-report/missing-marks-report.component';
import { ResultsAnalysisComponent } from './components/academics-report/results-analysis/results-analysis.component';
import { BroadsheetComponent } from "./components/academics-report/results-analysis/broadsheet/broadsheet.component";
import { StaffAttendancePageComponent } from './components/pages/staff-attendance-page.component';
import { StaffAttendanceDetailsPageComponent } from './components/pages/staff-attendance-details-page.component';
import { StaffSubjectPageComponent } from './components/pages/staff-subject-page.component';
import { ClassListPageComponent } from './components/pages/class-list-page.component';
import { ClassAttendancePageComponent } from './components/pages/class-attendance-page.component';
import { ClassAttendanceDetailsPageComponent } from './components/pages/class-attendance-details-page.component';
import { MissingMarksPageComponent } from './components/pages/missing-marks-page.component';
import { ResultsAnalysisPageComponent } from './components/pages/results-analysis-page.component';
import { ReportFormsPageComponent } from './components/pages/report-forms-page.component';
import { ReportFormComponent } from './components/academics-report/report-form/report-form.component';
import { AssessmentReportComponent } from './components/academics-report/assessment-report/assessment-report.component';
import { SubjectPerformanceComponent } from './components/academics-report/subject-performance/subject-performance.component';
import { ClassPerformanceComponent } from './components/academics-report/class-performance/class-performance.component';

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
        StaffAttendanceDetailsReportComponent,
        StaffSubjectDetailedReportComponent,
        StudentSubjectDetailedReportComponent,
        StudentSubjectPageComponent,
        ClassListsReportComponent,
        ClassAttendanceReportComponent,
        ClassAttendanceReportTableComponent,
        ClassAttendanceDetailsReportComponent,
        MissingMarksReportComponent,
        ResultsAnalysisComponent,
        BroadsheetComponent,
        StaffAttendancePageComponent,
        StaffAttendanceDetailsPageComponent,
        StaffSubjectPageComponent,
        ClassListPageComponent,
        ClassAttendancePageComponent,
        ClassAttendanceDetailsPageComponent,
        MissingMarksPageComponent,
        ResultsAnalysisPageComponent,
        ReportFormsPageComponent,
        ReportFormComponent,
        AssessmentReportComponent,
        SubjectPerformanceComponent,
        ClassPerformanceComponent
    ],
    imports: [CoreModule, SharedModule]
})
export class ReportsModule {}
