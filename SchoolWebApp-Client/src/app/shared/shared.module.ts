import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {ButtonComponent} from './directives/button/button.component';
import {DashboardHeaderComponent} from './directives/dashboard-header/dashboard-header.component';
import {DropdownComponent} from './directives/dropdown/dropdown.component';
import {DropdownMenuComponent} from './directives/dropdown-menu/dropdown-menu.component';
import {ItemLinkComponent} from './directives/item-link/item-link.component';
import {SettingsControlsComponent} from './directives/settings-controls/settings-controls.component';
import {SettingsTableComponent} from './directives/settings-table/settings-table.component';
import {TableActionComponent} from './directives/table-action/table-action.component';
import {TableHeadingComponent} from './directives/table-heading/table-heading.component';
import {TablePagingComponent} from './directives/table-paging/table-paging.component';
import {CoreModule} from '@/core/core.module';
import {TableButtonComponent} from './directives/table-button/table-button.component';
import {PersonSelectComponent} from './directives/person-select/person-select.component';
import {YearClassStreamComponent} from './directives/year-class-stream/year-class-stream.component';
import {NgbModule} from '@ng-bootstrap/ng-bootstrap';
import {SchoolClassMinimumTableComponent} from './components/school-class-minimum-table/school-class-minimum-table.component';
import {SubjectsMinTableComponent} from './components/subjects-min-table/subjects-min-table.component';
import {SubjectsTableComponent} from './components/subjects-table/subjects-table.component';
import {StudentClassesComponent} from './components/student-classes/student-classes.component';
import {AcademicYearsSelectorFormComponent} from './components/academic-years-selector-form/academic-years-selector-form.component';
import {StaffsAttendancesSearchFormComponent} from './components/staffs-attendances-search-form/staffs-attendances-search-form.component';
import {StaffsAttendancesTableComponent} from './components/staffs-attendances-table/staffs-attendances-table.component';
import {StaffAttendanceTableComponent} from './components/staff-attendance-table/staff-attendance-table.component';
import {SchoolSoftFilterFormComponent} from './components/school-soft-filter-form/school-soft-filter-form.component';
import {StaffMinTableComponent} from './components/staff-min-table/staff-min-table.component';
import {StaffSubjectTableComponent} from './components/staff-subject-table/staff-subject-table.component';
import {StudentsMinTableComponent} from '@/shared/components/students-min-table/students-min-table.component';
import {StudentClassMinTableComponent} from './components/student-class-min-table/student-class-min-table.component';
import {SchoolClassMinTableComponent} from './components/school-class-min-table/school-class-min-table.component';
import {StudentAttendanceTableComponent} from '../students/components/student-assignments/student-attendance/student-attendance-table/student-attendance-table.component';
import {ExamResultsComponent} from '@/academics/components/exam-results/exam-results.component';
import {ExamResultsTableComponent} from '@/academics/components/exam-results/exam-results-table/exam-results-table.component';
import { DataTablesModule } from 'angular-datatables';

@NgModule({
    declarations: [
        ButtonComponent,
        DashboardHeaderComponent,
        DropdownComponent,
        DropdownMenuComponent,
        ItemLinkComponent,
        SettingsControlsComponent,
        SettingsTableComponent,
        TableActionComponent,
        TableHeadingComponent,
        TablePagingComponent,
        TableButtonComponent,
        PersonSelectComponent,
        StudentClassesComponent,
        YearClassStreamComponent,
        SubjectsTableComponent,
        SchoolClassMinimumTableComponent,
        SubjectsMinTableComponent,
        AcademicYearsSelectorFormComponent,
        StaffsAttendancesSearchFormComponent,
        StaffsAttendancesTableComponent,
        StaffAttendanceTableComponent,
        StaffSubjectTableComponent,
        SchoolSoftFilterFormComponent,
        StaffMinTableComponent,
        StudentClassMinTableComponent,
        SchoolClassMinTableComponent,
        StudentsMinTableComponent,
        StudentAttendanceTableComponent,
        ExamResultsComponent,
        ExamResultsTableComponent
    ],

    imports: [CommonModule, NgbModule, CoreModule,DataTablesModule],
    exports: [
        ButtonComponent,
        DashboardHeaderComponent,
        DropdownComponent,
        DropdownMenuComponent,
        ItemLinkComponent,
        SettingsControlsComponent,
        SettingsTableComponent,
        TableActionComponent,
        TableHeadingComponent,
        TablePagingComponent,
        TableButtonComponent,
        PersonSelectComponent,
        YearClassStreamComponent,
        SubjectsTableComponent,
        SchoolClassMinimumTableComponent,
        SubjectsMinTableComponent,
        StudentClassesComponent,
        AcademicYearsSelectorFormComponent,
        SchoolSoftFilterFormComponent,
        StaffsAttendancesSearchFormComponent,
        StaffsAttendancesTableComponent,
        StaffAttendanceTableComponent,
        StaffSubjectTableComponent,
        StaffMinTableComponent,
        StudentClassMinTableComponent,
        StudentsMinTableComponent,
        StudentAttendanceTableComponent,
        ExamResultsComponent,
        ExamResultsTableComponent,
        DataTablesModule,
        CommonModule,
        NgbModule,
        CoreModule
    ]
})
export class SharedModule {}
