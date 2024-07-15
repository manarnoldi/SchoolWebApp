import {NgModule} from '@angular/core';
import {Routes, RouterModule} from '@angular/router';
import {MainComponent} from './core/modules/main/main.component';
import {AuthGuard} from './core/guards/auth.guard';
import {ProfileComponent} from './core/pages/profile/profile.component';
import {BlankComponent} from './core/pages/blank/blank.component';
import {SubMenuComponent} from './core/pages/main-menu/sub-menu/sub-menu.component';
import {DashboardComponent} from './school/components/dashboard/dashboard.component';
import {LoginComponent} from './core/auth/login/login.component';
import {NonAuthGuard} from './core/guards/non-auth.guard';
import {RegisterComponent} from './core/auth/register/register.component';
import {ForgotPasswordComponent} from './core/auth/forgot-password/forgot-password.component';
import {RecoverPasswordComponent} from './core/auth/recover-password/recover-password.component';
import {DesignationsComponent} from './settings/components/designations/designations.component';
import {OccupationsComponent} from './settings/components/occupations/occupations.component';
import {SettingsComponent} from './settings/settings.component';
import {EmploymentTypesComponent} from './settings/components/employment-types/employment-types.component';
import {GenderComponent} from './settings/components/gender/gender.component';
import {NationalitiesComponent} from './settings/components/nationalities/nationalities.component';
import {OccurenceTypesComponent} from './settings/components/occurence-types/occurence-types.component';
import {OutcomesComponent} from './settings/components/outcomes/outcomes.component';
import {RelationshipsComponent} from './settings/components/relationships/relationships.component';
import {ReligionsComponent} from './settings/components/religions/religions.component';
import {SessionTypesComponent} from './settings/components/session-types/session-types.component';
import {StaffCategoriesComponent} from './settings/components/staff-categories/staff-categories.component';
import {LearningModesComponent} from './school/components/learning-modes/learning-modes.component';
import {SchoolComponent} from './school/school.component';
import {SchoolDetailsComponent} from './school/components/school-details/school-details.component';
import {EducationLevelTypesComponent} from './school/components/education-level-types/education-level-types.component';
import {AcademicYearsComponent} from './school/components/academic-years/academic-years.component';
import {AcademicsComponent} from './academics/academics.component';
import {CurriculumComponent} from './academics/components/curriculum/curriculum.component';
import {ExamTypesComponent} from './academics/components/exam-types/exam-types.component';
import {EducationLevelsComponent} from './school/components/education-levels/education-levels.component';
import {StaffComponent} from './staff/staff.component';
import {StaffDetailsComponent} from './staff/components/staff-details/staff-details.component';
import {StaffDetailsFormComponent} from './staff/components/staff-details/staff-details-form/staff-details-form.component';
import {StudentsComponent} from './students/students.component';
import {StudentsViewComponent} from './students/components/students-view/students-view.component';
import {StudentsAddFormComponent} from './students/components/students-add-form/students-add-form.component';
import {SubjectGroupsComponent} from './academics/components/subject-groups/subject-groups.component';
import {GradesComponent} from './academics/components/grades/grades.component';
import {DepartmentsComponent} from './school/components/departments/departments.component';
import {EventsComponent} from './school/components/events/events.component';
import {LearningLevelsComponent} from './class/components/learning-levels/learning-levels.component';
import {SchoolClassComponent} from './class/components/school-class/school-class.component';
import {SchoolStreamsComponent} from './class/components/school-streams/school-streams.component';
import {SessionsComponent} from './class/components/sessions/sessions.component';
import {ClassLeadershipRolesComponent} from './class/components/class-leadership-roles/class-leadership-roles.component';
import {ClassDetailsComponent} from './class/components/school-class/class-details/class-details.component';
import { StaffAssignmentsComponent } from './staff/components/staff-assignments/staff-assignments.component';

const routes: Routes = [
    {
        path: '',
        component: MainComponent,
        canActivate: [AuthGuard],
        canActivateChild: [AuthGuard],
        children: [
            {path: 'profile', component: ProfileComponent},
            {path: 'blank', component: BlankComponent},
            {path: 'sub-menu-1', component: SubMenuComponent},
            {path: 'sub-menu-2', component: BlankComponent},
            {path: '', component: DashboardComponent}
        ]
    },
    {
        path: 'school',
        component: SchoolComponent,
        canActivate: [AuthGuard],
        canActivateChild: [AuthGuard],
        children: [
            {path: 'learningModes', component: LearningModesComponent},
            {path: 'details', component: SchoolDetailsComponent},
            {
                path: 'educationLevelTypes',
                component: EducationLevelTypesComponent
            },
            {path: 'academicYears', component: AcademicYearsComponent},
            {path: 'educationLevels', component: EducationLevelsComponent},
            {path: 'departments', component: DepartmentsComponent},
            {path: 'events', component: EventsComponent}
        ]
    },
    {
        path: 'class',
        component: SchoolComponent,
        canActivate: [AuthGuard],
        canActivateChild: [AuthGuard],
        children: [
            {path: 'classNames', component: LearningLevelsComponent},
            {path: 'streams', component: SchoolStreamsComponent},
            {path: 'classDetails', component: ClassDetailsComponent},
            {path: 'classes', component: SchoolClassComponent},
            {path: 'sessions', component: SessionsComponent},
            {path: 'leadership-roles', component: ClassLeadershipRolesComponent}
        ]
    },
    {
        path: 'staff',
        component: StaffComponent,
        canActivate: [AuthGuard],
        canActivateChild: [AuthGuard],
        children: [
            {path: 'details', component: StaffDetailsComponent},
            {path: 'add', component: StaffDetailsFormComponent},
            {path: 'attendance', component: StaffDetailsComponent},
            { path: 'attendance/add', component: StaffAssignmentsComponent },
            {path: 'discipline', component: StaffDetailsComponent},
            {path: 'discipline/add', component: StaffAssignmentsComponent},
        ]
    },
    {
        path: 'students',
        component: StudentsComponent,
        canActivate: [AuthGuard],
        canActivateChild: [AuthGuard],
        children: [
            {path: 'details', component: StudentsViewComponent},
            {path: 'add', component: StudentsAddFormComponent}
        ]
    },
    {
        path: 'academics',
        component: AcademicsComponent,
        canActivate: [AuthGuard],
        canActivateChild: [AuthGuard],
        children: [
            {path: 'curricula', component: CurriculumComponent},
            {path: 'examTypes', component: ExamTypesComponent},
            {path: 'subjectGroups', component: SubjectGroupsComponent},
            {path: 'grades', component: GradesComponent}
        ]
    },
    {
        path: 'settings',
        component: SettingsComponent,
        canActivate: [AuthGuard],
        canActivateChild: [AuthGuard],
        children: [
            {path: 'occupations', component: OccupationsComponent},
            {path: 'designations', component: DesignationsComponent},
            {path: 'employmentTypes', component: EmploymentTypesComponent},
            {path: 'genders', component: GenderComponent},
            {path: 'nationalities', component: NationalitiesComponent},
            {path: 'occurenceTypes', component: OccurenceTypesComponent},
            {path: 'outcomes', component: OutcomesComponent},
            {path: 'relationships', component: RelationshipsComponent},
            {path: 'religions', component: ReligionsComponent},
            {path: 'sessionTypes', component: SessionTypesComponent},
            {path: 'staffCategories', component: StaffCategoriesComponent}
        ]
    },
    {
        path: 'login',
        component: LoginComponent,
        canActivate: [NonAuthGuard]
    },
    {
        path: 'register',
        component: RegisterComponent,
        canActivate: [NonAuthGuard]
    },
    {
        path: 'forgot-password',
        component: ForgotPasswordComponent,
        canActivate: [NonAuthGuard]
    },
    {
        path: 'recover-password',
        component: RecoverPasswordComponent,
        canActivate: [NonAuthGuard]
    },
    {path: '**', redirectTo: ''}
];

@NgModule({
    imports: [RouterModule.forRoot(routes, {})],
    exports: [RouterModule]
})
export class AppRoutingModule {}
