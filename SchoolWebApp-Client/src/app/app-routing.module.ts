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
import {ChangePasswordComponent} from './core/auth/change-password/change-password.component';
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
import {GlobalSettingsComponent} from './settings/components/global-settings/global-settings.component';
import {DropdownManagementComponent} from './settings/components/dropdown-management/dropdown-management.component';
import {ApprovalWorkflowsComponent} from './approvals/components/approval-workflows/approval-workflows.component';
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
import {StudentsAddFormComponent} from './students/components/students-details/students-add-form/students-add-form.component';
import {SubjectGroupsComponent} from './academics/components/subject-groups/subject-groups.component';
import {GradesComponent} from './academics/components/grades/grades.component';
import {DepartmentsComponent} from './school/components/departments/departments.component';
import {EventsComponent} from './school/components/events/events.component';
import {BulkStaffSubjectsComponent} from './school/components/bulk-staff-subjects/bulk-staff-subjects.component';
import {LearningLevelsComponent} from './class/components/learning-levels/learning-levels.component';
import {SchoolClassComponent} from './class/components/school-class/school-class.component';
import {SchoolStreamsComponent} from './class/components/school-streams/school-streams.component';
import {SessionsComponent} from './class/components/sessions/sessions.component';
import {ClassLeadershipRolesComponent} from './class/components/class-leadership-roles/class-leadership-roles.component';
import {ClassDetailsComponent} from './class/components/school-class/class-details/class-details.component';
import {StaffAssignmentsComponent} from './staff/components/staff-assignments/staff-assignments.component';
import {StudentsDetailsComponent} from './students/components/students-details/students-details.component';
import {StudentAssignmentsComponent} from './students/components/student-assignments/student-assignments.component';
import {ParentsListComponent} from './students/components/parents/parents-list/parents-list.component';
import {ParentAddFormComponent} from './students/components/parents/parent-add-form/parent-add-form.component';
import {SubjectsComponent} from './academics/components/subjects/subjects.component';
import {ExamsComponent} from './academics/components/exams/exams.component';
import {ExamAddFormComponent} from './academics/components/exams/exam-add-form/exam-add-form.component';
// import {EducationLevelSubject} from './academics/models/education-level-subject';
import {EducationLevelSubjectsComponent} from './academics/components/subjects/education-level-subjects/education-level-subjects.component';
import {Exam} from './academics/models/exam';
import {ExamResult} from './academics/models/exam-result';
import {ExamResultsComponent} from './academics/components/exam-results/exam-results.component';
import {StudentsSubjectsComponent} from './students/components/students-subjects/students-subjects.component';
import {StudentsSubjectsAddFormComponent} from './students/components/students-subjects/students-subjects-add-form/students-subjects-add-form.component';
import {StudentsAttendancesComponent} from './students/components/students-attendances/students-attendances.component';
import {StudentPromotionComponent} from './students/components/student-promotion/student-promotion.component';
import {StaffsAttendancesComponent} from './staff/components/staffs-attendances/staffs-attendances.component';
import { ReportsComponent } from './reports/reports.component';
import { SchoolReportComponent } from './reports/components/school-report/school-report.component';
import { ClassReportComponent } from './reports/components/class-report/class-report.component';
import { StaffReportComponent } from './reports/components/staff-report/staff-report.component';
import { AcademicsReportComponent } from './reports/components/academics-report/academics-report.component';
import { StaffAttendancePageComponent } from './reports/components/pages/staff-attendance-page.component';
import { StaffAttendanceDetailsPageComponent } from './reports/components/pages/staff-attendance-details-page.component';
import { StaffSubjectPageComponent } from './reports/components/pages/staff-subject-page.component';
import { StudentSubjectPageComponent } from './reports/components/pages/student-subject-page.component';
import { ClassListPageComponent } from './reports/components/pages/class-list-page.component';
import { ClassAttendancePageComponent } from './reports/components/pages/class-attendance-page.component';
import { ClassAttendanceDetailsPageComponent } from './reports/components/pages/class-attendance-details-page.component';
import { MissingMarksPageComponent } from './reports/components/pages/missing-marks-page.component';
import { ResultsAnalysisPageComponent } from './reports/components/pages/results-analysis-page.component';
import { ReportFormsPageComponent } from './reports/components/pages/report-forms-page.component';
import { AssessmentReportComponent } from './reports/components/academics-report/assessment-report/assessment-report.component';
import { ExamNamesComponent } from './academics/components/exam-names/exam-names.component';
import { CbeComponent } from './cbe/cbe.component';
import { AssessmentTypesComponent } from './cbe/assessments/components/assessment-types/assessment-types.component';
import { CompetenciesComponent } from './cbe/assessments/components/competencies/competencies.component';
import { GeneralOutcomesComponent } from './cbe/assessments/components/general-outcomes/general-outcomes.component';
import { StrandsComponent } from './cbe/assessments/components/strands/strands.component';
import { ThemesComponent as CbeThemesComponent } from './cbe/assessments/components/themes/themes.component';
import { ValuesComponent } from './cbe/values/components/values/values.component';
import { ValueScoresComponent } from './cbe/values/components/value-scores/value-scores.component';
import { ResponsibilitiesComponent } from './cbe/responsibilities/components/responsibilities/responsibilities.component';
// SocialSkill merged into Responsibility
import { ActivitiesComponent } from './cbe/cocurriculum/components/activities/activities.component';
import { ScoreTypesComponent } from './cbe/cocurriculum/components/score-types/score-types.component';
import { CommunityServiceActivitiesComponent } from './cbe/community-service/components/activities/activities.component';
import { SubStrandsComponent } from './cbe/assessments/components/sub-strands/sub-strands.component';
import { BroadOutcomesComponent } from './cbe/assessments/components/broad-outcomes/broad-outcomes.component';
import { SpecificOutcomesComponent } from './cbe/assessments/components/specific-outcomes/specific-outcomes.component';
import { KeyQuestionsComponent } from './cbe/assessments/components/key-questions/key-questions.component';
import { LearningExperiencesComponent } from './cbe/assessments/components/learning-experiences/learning-experiences.component';
import { LessonAllocationsComponent } from './cbe/assessments/components/lesson-allocations/lesson-allocations.component';
import { PCIsComponent } from './cbe/assessments/components/pcis/pcis.component';
import { StudentAssessmentsComponent } from './cbe/assessments/components/student-assessments/student-assessments.component';
import { ExamTypesComponent as CbeExamTypesComponent } from './cbe/exams/components/exam-types/exam-types.component';
import { ExamsComponent as CbeExamsComponent } from './cbe/exams/components/exams/exams.component';
import { ExamRegisterListComponent } from './cbe/exams/components/exam-register-list/exam-register-list.component';
import { ExamResultsComponent as CbeExamResultsComponent } from './cbe/exams/components/exam-results/exam-results.component';
import { ExamResultsClasswiseComponent as CbeExamResultsClasswiseComponent } from './cbe/exams/components/exam-results-classwise/exam-results-classwise.component';
import { ExamResultsBulkComponent } from './cbe/exams/components/exam-results-bulk/exam-results-bulk.component';
import { StudentResponsibilitiesComponent } from './cbe/responsibilities/components/student-responsibilities/student-responsibilities.component';
import { StudentValueScoresComponent } from './cbe/values/components/student-value-scores/student-value-scores.component';
import { CommunityServiceStudentAssignmentsComponent } from './cbe/community-service/components/student-assignments/student-assignments.component';
import { ScoresSetupComponent } from './cbe/cocurriculum/components/scores-setup/scores-setup.component';
import { StudentCoCurriculumAssignmentsComponent } from './cbe/cocurriculum/components/student-assignments/student-assignments.component';
import { StudentCoCurriculumScoresComponent } from './cbe/cocurriculum/components/student-scores/student-scores.component';
import { SecurityComponent } from './security/security.component';
import { UsersComponent } from './security/components/users/users.component';
import { RolesComponent as SecurityRolesComponent } from './security/components/roles/roles.component';
import { MenuPermissionsComponent } from './security/components/menu-permissions/menu-permissions.component';
import { LogsComponent } from './security/components/logs/logs.component';
import { FinanceComponent } from './finance/finance.component';
import { FinanceAccountsComponent } from './finance/components/accounts/accounts.component';
import { FeeCategoriesComponent } from './finance/components/fee-categories/fee-categories.component';
import { ExpenseCategoriesComponent } from './finance/components/expense-categories/expense-categories.component';
import { ExpensesComponent as FinanceExpensesComponent } from './finance/components/expenses/expenses.component';
import { JournalEntriesComponent } from './finance/components/journal-entries/journal-entries.component';
import { PaymentsComponent } from './finance/components/payments/payments.component';
import { FinanceInvoicesComponent } from './finance/components/invoices/invoices.component';
import { FinanceReportsComponent } from './finance/components/reports/finance-reports.component';
import { FinanceBudgetsComponent } from './finance/components/budgets/budgets.component';
import { FinanceBudgetAmendmentsComponent } from './finance/components/budget-amendments/budget-amendments.component';
import { SponsorsComponent } from './finance/components/sponsors/sponsors.component';
import { SponsorshipsComponent } from './finance/components/sponsorships/sponsorships.component';
import { SponsorPaymentsComponent } from './finance/components/sponsor-payments/sponsor-payments.component';
import { FinanceBudgetMastersComponent } from './finance/components/budget-masters/budget-masters.component';
import { FeeStructuresComponent } from './finance/components/fee-structures/fee-structures.component';
import { PayrollComponent } from './payroll/payroll.component';
import { PayrollEarningTypesComponent } from './payroll/components/earning-types/earning-types.component';
import { PayrollDeductionTypesComponent } from './payroll/components/deduction-types/deduction-types.component';
import { PayrollTaxBandsComponent } from './payroll/components/tax-bands/tax-bands.component';
import { PayrollSettingsComponent } from './payroll/components/payroll-settings/payroll-settings.component';
import { PayrollEmployeeSalariesComponent } from './payroll/components/employee-salaries/employee-salaries.component';
import { PayrollLoanAdvancesComponent } from './payroll/components/loan-advances/loan-advances.component';
import { PayrollPeriodsComponent } from './payroll/components/payroll-periods/payroll-periods.component';
import { PayrollReportsComponent } from './payroll/components/payroll-reports/payroll-reports.component';

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
            { path: 'details', component: SchoolDetailsComponent },
            {path: 'curricula', component: CurriculumComponent},
            { path: 'academicYears', component: AcademicYearsComponent },
            {path: 'sessions', component: SessionsComponent},
            {
                path: 'educationLevelTypes',
                component: EducationLevelTypesComponent
            },
            
            {path: 'learning-levels', component: LearningLevelsComponent},
            {path: 'departments', component: DepartmentsComponent},
            {path: 'events', component: EventsComponent},
            {path: 'bulk-staff-subjects', component: BulkStaffSubjectsComponent}
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
            {path: 'classes/manage', component: ClassDetailsComponent},
            {path: 'classes', component: SchoolClassComponent},
            
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
            {path: 'details/add', component: StaffDetailsFormComponent},
            {path: 'manage', redirectTo: 'details', pathMatch: 'full'},
            {path: 'manage/add', component: StaffAssignmentsComponent},
            {path: 'staff-attendances', component: StaffsAttendancesComponent}
        ]
    },
    // {
    //     path: 'parents',
    //     component: ParentsComponent,
    //     canActivate: [AuthGuard],
    //     canActivateChild: [AuthGuard],
    //     children: [
    //         {path: 'details', component: ParentsListComponent},
    //         {path: 'add', component: ParentAddFormComponent},
    //     ]
    // },
    {
        path: 'students',
        component: StudentsComponent,
        canActivate: [AuthGuard],
        canActivateChild: [AuthGuard],
        children: [
            {path: 'details', component: StudentsDetailsComponent},
            {path: 'details/add', component: StudentsAddFormComponent},
            {path: 'manage', redirectTo: 'details', pathMatch: 'full'},
            {path: 'manage/add', component: StudentAssignmentsComponent},
            {path: 'parents', component: ParentsListComponent},
            {path: 'parents/add', component: ParentAddFormComponent},
            {path: 'students-subjects', component: StudentsSubjectsComponent},
            {
                path: 'students-subjects/add',
                component: StudentsSubjectsAddFormComponent
            },
            {
                path: 'students-attendances',
                component: StudentsAttendancesComponent
            },
            {path: 'promotion', component: StudentPromotionComponent}
        ]
    },
    {
        path: 'academics',
        component: AcademicsComponent,
        canActivate: [AuthGuard],
        canActivateChild: [AuthGuard],
        children: [
            
            {path: 'examTypes', component: ExamTypesComponent},
            {path: 'examNames', component: ExamNamesComponent},
            {path: 'subjectGroups', component: SubjectGroupsComponent},
            {path: 'subjects', component: SubjectsComponent},
            {
                path: 'educationLevelSubjects',
                component: EducationLevelSubjectsComponent
            },
            {path: 'grades', component: GradesComponent},
            {path: 'exams', redirectTo: '/cbe/exams/exams', pathMatch: 'full'},
            {path: 'exams/add', redirectTo: '/cbe/exams/exams', pathMatch: 'full'},
            {path: 'examResults', component: ExamResultsComponent}
        ]
    },
     {
        path: 'cbe',
        component: CbeComponent,
        canActivate: [AuthGuard],
        canActivateChild: [AuthGuard],
        children: [
            
            {path: 'assessments/assessment-types', component: AssessmentTypesComponent},
            {path: 'assessments/competencies', component: CompetenciesComponent},
            {path: 'assessments/general-outcomes', component: GeneralOutcomesComponent},
            {path: 'assessments/strands', component: StrandsComponent},
            {path: 'assessments/themes', component: CbeThemesComponent},
            {path: 'values/values-register', component: ValuesComponent},
            {path: 'values/values-scores', component: ValueScoresComponent},
            {path: 'responsibilities/responsibilities', component: ResponsibilitiesComponent},
            {path: 'cocurriculum/activities', component: ActivitiesComponent},
            {path: 'cocurriculum/score-types', component: ScoreTypesComponent},
            {path: 'community-service/activities', component: CommunityServiceActivitiesComponent},
            {path: 'assessments/sub-strands', component: SubStrandsComponent},
            {path: 'assessments/broad-outcomes', component: BroadOutcomesComponent},
            {path: 'assessments/specific-outcomes', component: SpecificOutcomesComponent},
            {path: 'assessments/key-questions', component: KeyQuestionsComponent},
            {path: 'assessments/learning-experiences', component: LearningExperiencesComponent},
            {path: 'assessments/lesson-allocations', component: LessonAllocationsComponent},
            {path: 'assessments/pcis', component: PCIsComponent},
            {path: 'assessments/assessments', component: StudentAssessmentsComponent},
            // Permission sentinel — see MENU note in menu-sidebar.component.ts.
            // Reuses the same component; the component reads its mode from the
            // user's MenuPermissions list.
            {path: 'assessments/admin', redirectTo: 'assessments/assessments', pathMatch: 'full'},
            {path: 'exams/exam-types', component: CbeExamTypesComponent},
            {path: 'exams/exams', component: ExamRegisterListComponent},
            {path: 'exams/exams/register', component: CbeExamsComponent},
            {path: 'exams/exam-results', component: CbeExamResultsComponent},
            {path: 'exams/exam-results-classwise', component: CbeExamResultsClasswiseComponent},
            {path: 'exams/exam-results-bulk', component: ExamResultsBulkComponent},
            {path: 'values/student-assignments', component: StudentValueScoresComponent},
            {path: 'responsibilities/student-assignments', component: StudentResponsibilitiesComponent},
            {path: 'community-service/student-assignments', component: CommunityServiceStudentAssignmentsComponent},
            {path: 'cocurriculum/scores-setup', component: ScoresSetupComponent},
            {path: 'cocurriculum/student-assignments', component: StudentCoCurriculumAssignmentsComponent},
            {path: 'cocurriculum/student-scores', component: StudentCoCurriculumScoresComponent}
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
            {path: 'staffCategories', component: StaffCategoriesComponent},
            {path: 'global-settings', component: GlobalSettingsComponent},
            {path: 'dropdowns', component: DropdownManagementComponent},
            {path: 'approval-workflows', component: ApprovalWorkflowsComponent}
        ]
    },
    {
        path: 'reports',
        component: ReportsComponent,
        canActivate: [AuthGuard],
        canActivateChild: [AuthGuard],
        children: [
            {path: 'school', component: SchoolReportComponent},
            {path: 'class', component: ClassReportComponent},
            {path: 'staff', component: StaffReportComponent},
            {path: 'academics', component: AcademicsReportComponent},
            {path: 'staff/attendance', component: StaffAttendancePageComponent},
            {path: 'staff/attendance-details', component: StaffAttendanceDetailsPageComponent},
            {path: 'staff/subject-allocation', component: StaffSubjectPageComponent},
            {path: 'class/class-list', component: ClassListPageComponent},
            {path: 'class/attendance', component: ClassAttendancePageComponent},
            {path: 'class/attendance-details', component: ClassAttendanceDetailsPageComponent},
            {path: 'academics/missing-marks', component: MissingMarksPageComponent},
            {path: 'academics/results-analysis', component: ResultsAnalysisPageComponent},
            {path: 'academics/exam-results', component: ResultsAnalysisPageComponent},
            {path: 'academics/report-forms', component: ReportFormsPageComponent},
            {path: 'academics/assessment-report', component: AssessmentReportComponent},
            {path: 'academics/student-subject-allocation', component: StudentSubjectPageComponent}
        ]
    },
    {
        path: 'security',
        component: SecurityComponent,
        canActivate: [AuthGuard],
        canActivateChild: [AuthGuard],
        children: [
            {path: 'users', component: UsersComponent},
            {path: 'roles', component: SecurityRolesComponent},
            {path: 'user-roles', redirectTo: 'users', pathMatch: 'full'},
            {path: 'menu-permissions', component: MenuPermissionsComponent},
            {path: 'logs', component: LogsComponent}
        ]
    },
    {
        path: 'finance',
        component: FinanceComponent,
        canActivate: [AuthGuard],
        canActivateChild: [AuthGuard],
        children: [
            {path: 'accounts', component: FinanceAccountsComponent},
            {path: 'fee-categories', component: FeeCategoriesComponent},
            {path: 'fee-structures', component: FeeStructuresComponent},
            {path: 'expense-categories', component: ExpenseCategoriesComponent},
            {path: 'expenses', component: FinanceExpensesComponent},
            {path: 'journal-entries', component: JournalEntriesComponent},
            {path: 'payments', component: PaymentsComponent},
            {path: 'invoices', component: FinanceInvoicesComponent},
            {path: 'budget-register', component: FinanceBudgetMastersComponent},
            {path: 'budgets', component: FinanceBudgetsComponent},
            {path: 'budget-amendments', component: FinanceBudgetAmendmentsComponent},
            {path: 'sponsors', component: SponsorsComponent},
            {path: 'sponsorships', component: SponsorshipsComponent},
            {path: 'sponsor-payments', component: SponsorPaymentsComponent},
            {path: 'reports', redirectTo: 'reports/fees', pathMatch: 'full'},
            {path: 'reports/fees', component: FinanceReportsComponent, data: {reportGroup: 'fees'}},
            {path: 'reports/expenses', component: FinanceReportsComponent, data: {reportGroup: 'expenses'}},
            {path: 'reports/statements', component: FinanceReportsComponent, data: {reportGroup: 'statements'}}
        ]
    },
    {
        path: 'payroll',
        component: PayrollComponent,
        children: [
            {path: 'earning-types', component: PayrollEarningTypesComponent},
            {path: 'deduction-types', component: PayrollDeductionTypesComponent},
            {path: 'tax-bands', component: PayrollTaxBandsComponent},
            {path: 'settings', component: PayrollSettingsComponent},
            {path: 'employee-salaries', component: PayrollEmployeeSalariesComponent},
            {path: 'loan-advances', component: PayrollLoanAdvancesComponent},
            {path: 'periods', component: PayrollPeriodsComponent},
            {path: 'reports', component: PayrollReportsComponent}
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
    {
        path: 'change-password',
        component: ChangePasswordComponent,
        canActivate: [AuthGuard]
    },
    {path: '**', redirectTo: ''}
];

@NgModule({
    imports: [RouterModule.forRoot(routes, {})],
    exports: [RouterModule]
})
export class AppRoutingModule {}
