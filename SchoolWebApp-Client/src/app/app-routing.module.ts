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
import {SchoolStreamsComponent} from './school/components/school-streams/school-streams.component';
import {EducationLevelTypesComponent} from './school/components/education-level-types/education-level-types.component';
import {AcademicYearsComponent} from './school/components/academic-years/academic-years.component';
import {AcademicsComponent} from './academics/academics.component';
import {CurriculumComponent} from './academics/components/curriculum/curriculum.component';
import {ExamTypesComponent} from './academics/components/exam-types/exam-types.component';
import {SessionsComponent} from './school/components/sessions/sessions.component';
import { EducationLevelsComponent } from './school/components/education-levels/education-levels.component';

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
            {path: 'streams', component: SchoolStreamsComponent},
            {
                path: 'educationLevelTypes',
                component: EducationLevelTypesComponent
            },
            {path: 'academicYears', component: AcademicYearsComponent},
            {path: 'sessions', component: SessionsComponent},
            {path: 'educationLevels', component: EducationLevelsComponent}
            // {path: 'outcomes', component: OutcomesComponent},
            // {path: 'relationships', component: RelationshipsComponent},
            // {path: 'religions', component: ReligionsComponent},
            // {path: 'sessionTypes', component: SessionTypesComponent},
            // {path: 'staffCategories', component: StaffCategoriesComponent}
        ]
    },
    {
        path: 'academics',
        component: AcademicsComponent,
        canActivate: [AuthGuard],
        canActivateChild: [AuthGuard],
        children: [
            {path: 'curricula', component: CurriculumComponent},
            {path: 'examTypes', component: ExamTypesComponent}
            // {path: 'occurenceTypes', component: OccurenceTypesComponent},
            // {path: 'outcomes', component: OutcomesComponent},
            // {path: 'relationships', component: RelationshipsComponent},
            // {path: 'religions', component: ReligionsComponent},
            // {path: 'sessionTypes', component: SessionTypesComponent},
            // {path: 'staffCategories', component: StaffCategoriesComponent}
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
