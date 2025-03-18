import {NgModule} from '@angular/core';
import {CoreModule} from '@/core/core.module';
import {SharedModule} from '@/shared/shared.module';
import {StudentsComponent} from './students.component';
import {StudentsTableComponent} from './components/students-details/students-table/students-table.component';
import {DataTablesModule} from 'angular-datatables';
import {StudentsAddFormComponent} from './components/students-details/students-add-form/students-add-form.component';
import {StudentsDetailsComponent} from './components/students-details/students-details.component';
import {StudentAssignmentsComponent} from './components/student-assignments/student-assignments.component';
import {StudentClassComponent} from './components/student-assignments/student-class/student-class.component';
import {StudentParentsComponent} from './components/student-assignments/student-parents/student-parents.component';
import {StudentDisciplineComponent} from './components/student-assignments/student-discipline/student-discipline.component';
import {StudentAttendanceComponent} from './components/student-assignments/student-attendance/student-attendance.component';
import {StudentSubjectsComponent} from './components/student-assignments/student-subjects/student-subjects.component';
import {StudentDetailsViewComponent} from './components/students-details/student-details-view/student-details-view.component';
import {StudentFormerSchoolTableComponent} from './components/student-assignments/student-former-school/student-former-school-table/student-former-school-table.component';
import {StudentFormerSchoolComponent} from './components/student-assignments/student-former-school/student-former-school.component';
import {StudentFormerSchoolFormComponent} from './components/student-assignments/student-former-school/student-former-school-form/student-former-school-form.component';
import {StudentClassFormComponent} from './components/student-assignments/student-class/student-class-form/student-class-form.component';
import {StudentClassTableComponent} from './components/student-assignments/student-class/student-class-table/student-class-table.component';
import {StudentsMinTableComponent} from './components/students-details/students-min-table/students-min-table.component';
import {StudentParentsExistingFormComponent} from './components/student-assignments/student-parents/student-parents-existing-form/student-parents-existing-form.component';
import {StudentParentsTableComponent} from './components/student-assignments/student-parents/student-parents-table/student-parents-table.component';
import {ParentsListComponent} from './components/parents/parents-list/parents-list.component';
import {ParentAddFormComponent} from './components/parents/parent-add-form/parent-add-form.component';
import {ParentsTableComponent} from './components/parents/parents-table/parents-table.component';
import {ParentViewComponent} from './components/parents/parent-view/parent-view.component';
import {StudentDisciplineFormComponent} from './components/student-assignments/student-discipline/student-discipline-form/student-discipline-form.component';
import {StudentDisciplineTableComponent} from './components/student-assignments/student-discipline/student-discipline-table/student-discipline-table.component';
import {StudentSubjectsFormComponent} from './components/student-assignments/student-subjects/student-subjects-form/student-subjects-form.component';
import {StudentSubjectsTableComponent} from './components/student-assignments/student-subjects/student-subjects-table/student-subjects-table.component';
import {StudentSubjectsLoadFormComponent} from './components/student-assignments/student-subjects/student-subjects-load-form/student-subjects-load-form.component';
import {AcademicsModule} from '@/academics/academics.module';
import { StudentAttendanceFormComponent } from './components/student-assignments/student-attendance/student-attendance-form/student-attendance-form.component';
import { StudentAttendanceTableComponent } from './components/student-assignments/student-attendance/student-attendance-table/student-attendance-table.component';
import { StudentClassesComponent } from '../shared/components/student-classes/student-classes.component';

@NgModule({
    declarations: [
        StudentsComponent,
        StudentsTableComponent,
        StudentsAddFormComponent,
        StudentsDetailsComponent,
        StudentAssignmentsComponent,
        StudentFormerSchoolComponent,
        StudentClassComponent,
        StudentParentsComponent,
        StudentDisciplineComponent,
        StudentAttendanceComponent,
        StudentSubjectsComponent,
        StudentDetailsViewComponent,
        StudentFormerSchoolFormComponent,
        StudentFormerSchoolTableComponent,
        StudentClassFormComponent,
        StudentClassTableComponent,
        StudentsMinTableComponent,
        StudentParentsExistingFormComponent,
        StudentParentsTableComponent,
        ParentsListComponent,
        ParentsTableComponent,
        ParentAddFormComponent,
        ParentViewComponent,
        StudentDisciplineFormComponent,
        StudentDisciplineTableComponent,
        StudentSubjectsFormComponent,
        StudentSubjectsTableComponent,
        StudentSubjectsLoadFormComponent,
        StudentAttendanceFormComponent,
        StudentAttendanceTableComponent
    ],
    imports: [AcademicsModule, DataTablesModule, CoreModule, SharedModule]
})
export class StudentsModule {}
