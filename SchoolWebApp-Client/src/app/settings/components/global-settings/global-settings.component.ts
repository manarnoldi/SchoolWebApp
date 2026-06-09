import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {GlobalSettingService} from '../../services/global-setting.service';
import {ExamTypeService} from '@/cbe/exams/services/exam-type.service';
import {AccountService} from '@/finance/services/finance-services';
import {CurriculumService} from '@/academics/services/curriculum.service';
import {EducationLevelService} from '@/school/services/education-level.service';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-global-settings',
    templateUrl: './global-settings.component.html',
    styleUrl: './global-settings.component.scss'
})
export class GlobalSettingsComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/settings/global-settings'], title: 'Global Settings'}
    ];
    dashboardTitle = 'Global Settings';

    collapsedModules: Set<string> = new Set();

    isModuleExpanded = (m: any): boolean => !this.collapsedModules.has(m.name);

    toggleModule = (m: any) => {
        if (this.collapsedModules.has(m.name)) {
            this.collapsedModules.delete(m.name);
        } else {
            this.collapsedModules.add(m.name);
        }
    };

    modules = [
        {
            name: 'General',
            title: 'General Dashboard Settings',
            color: 'primary',
            icon: 'fas fa-cog',
            settings: [
                {key: 'TodoListPageSize', label: 'Todo List Items on Dashboard', type: 'select', options: [
                    {value: '2', label: '2 items'},
                    {value: '3', label: '3 items'},
                    {value: '4', label: '4 items'},
                    {value: '5', label: '5 items'},
                    {value: '6', label: '6 items'},
                    {value: '8', label: '8 items'},
                    {value: '10', label: '10 items'}
                ], description: 'Number of todo list items displayed per page on the dashboard'},
                {key: 'UpcomingEventsCount', label: 'Upcoming Events on Dashboard', type: 'select', options: [
                    {value: '2', label: '2 events'},
                    {value: '3', label: '3 events'},
                    {value: '4', label: '4 events'},
                    {value: '5', label: '5 events'},
                    {value: '6', label: '6 events'},
                    {value: '8', label: '8 events'},
                    {value: '10', label: '10 events'}
                ], description: 'Number of upcoming events displayed on the dashboard'},
                {key: 'ShowTopStudent', label: 'Show Top Student on Dashboard', type: 'boolean', description: 'Show the name of the top student in the class exam performance summary on the dashboard'},
                {key: 'CurrentExamType', label: 'Current Exam Type for Dashboard', type: 'select', options: [], description: 'The exam type shown by default on the dashboard class exam performance summary'},
                {key: 'AverageCalculation', label: 'Average Calculation Method', type: 'select', options: [
                    {value: 'students_with_scores', label: 'Students who did the exam only'},
                    {value: 'all_allocated_students', label: 'All students allocated to the subject/class'}
                ], description: 'How class averages are computed: count only students with scores, or include all allocated students (absent students count as 0)'},
                {key: 'RequireAbsenceReason', label: 'Require Reason for Absence', type: 'boolean', description: 'When enabled, a remark/reason is required for every absent record in both student and staff attendance. When disabled, absences can be saved without a remark.'},
                {key: 'DepartmentCodePrefix', label: 'Department Code Prefix', type: 'text', description: 'Prefix used when auto-generating department codes (e.g. DEPT produces DEPT001, DEPT002). Default: DEPT.'},
                {key: 'BudgetMasterCodePrefix', label: 'Budget Plan Code Prefix', type: 'text', description: 'Prefix used when auto-generating budget plan codes (e.g. BUD produces BUD001). Default: BUD.'},
                {key: 'ApprovalEditLockPolicy', label: 'Approval Edit Lock Policy', type: 'select', options: [
                    {value: 'Strict', label: 'Strict — lock as soon as the request is submitted'},
                    {value: 'AfterFirstApproval', label: 'Lenient — allow edits until the first step is approved'}
                ], description: 'Strict locks the record as soon as it is submitted. Lenient keeps it editable until the first approval step is approved. Approved records are always locked.'}
            ]
        },
        {
            name: 'Finance',
            title: 'Finance Settings',
            color: 'info',
            icon: 'fas fa-coins',
            settings: [
                {key: 'PaymentApportionMode', label: 'Payment Apportioning Mode', type: 'select', options: [
                    {value: 'auto', label: 'Auto (by fee category priority/rank)'},
                    {value: 'manual', label: 'Manual (finance officer allocates)'}
                ], description: 'How payments are allocated to individual invoice items. Auto mode distributes by fee category rank (lowest rank = highest priority). Manual mode lets the finance officer enter amounts per item.'},
                {key: 'AutoPostInvoiceJournal', label: 'Auto-Post Invoice Journal', type: 'boolean', description: 'Automatically create a journal entry (Debit Debtors, Credit Income) when a student invoice is created.'},
                {key: 'AutoPostPaymentJournal', label: 'Auto-Post Payment Journal', type: 'boolean', description: 'Automatically create a journal entry (Debit Bank/Cash, Credit Debtors) when a payment is received.'},
                {key: 'DebtorsAccountId', label: 'Student Debtors Account', type: 'account', description: 'Asset account for student receivables (e.g. 1200 Student Debtors).'},
                {key: 'CashAccountId', label: 'Default Cash/Bank Account', type: 'account', description: 'Default asset account for receiving fee payments (e.g. 1100 Cash at Hand).'},
                {key: 'SalaryExpenseAccountId', label: 'Salary Expense Account', type: 'account', description: 'Expense account for salary costs (e.g. 5100 Salary Expense).'},
                {key: 'PayeAccountId', label: 'PAYE Payable Account', type: 'account', description: 'Liability account for PAYE tax (e.g. 2100 PAYE Payable).'},
                {key: 'NssfAccountId', label: 'NSSF Payable Account', type: 'account', description: 'Liability account for NSSF (e.g. 2110 NSSF Payable).'},
                {key: 'ShifAccountId', label: 'SHIF Payable Account', type: 'account', description: 'Liability account for SHIF (e.g. 2120 SHIF Payable).'},
                {key: 'AhlAccountId', label: 'Housing Levy Account', type: 'account', description: 'Liability account for AHL (e.g. 2130 Housing Levy Payable).'}
            ]
        },
        {
            name: 'Grading',
            title: 'Grading System Settings',
            color: 'success',
            icon: 'fas fa-graduation-cap',
            settings: [
                {key: 'ExamResults', label: 'Exam Results', type: 'select', options: [
                    {value: '4-Point', label: '4-Point (EE, ME, AE, BE)'},
                    {value: '8-Point', label: '8-Point (EE1, EE2, ME1, ME2, AE1, AE2, BE1, BE2)'}
                ], description: 'Default grading for exam results entry, report form and broadsheet. Can be overridden per education level below.'},
                {key: 'StudentAssessment', label: 'Student Assessment', type: 'select', options: [
                    {value: '4-Point', label: '4-Point (EE, ME, AE, BE)'},
                    {value: '8-Point', label: '8-Point (EE1, EE2, ME1, ME2, AE1, AE2, BE1, BE2)'}
                ], description: 'Grading for student assessments and assessment report'},
                {key: 'ValueScores', label: 'Value Scores', type: 'select', options: [
                    {value: '4-Point', label: '4-Point (EE, ME, AE, BE)'},
                    {value: '8-Point', label: '8-Point (EE1, EE2, ME1, ME2, AE1, AE2, BE1, BE2)'}
                ], description: 'Grading for values assessment'},
                {key: 'CoCurricular', label: 'Co-Curricular Scores', type: 'select', options: [
                    {value: '4-Point', label: '4-Point (EE, ME, AE, BE)'},
                    {value: '8-Point', label: '8-Point (EE1, EE2, ME1, ME2, AE1, AE2, BE1, BE2)'}
                ], description: 'Grading for co-curricular student scores'},
                {key: 'RankingMethod', label: 'Ranking Method', type: 'select', options: [
                    {value: 'mean_marks', label: 'Mean Marks (Average %)'},
                    {value: 'mean_points', label: 'Mean Points (Score)'}
                ], description: 'How students are ranked on broadsheet and report form'},
                {key: 'MeanBasis', label: 'Totals & Means Basis', type: 'select', options: [
                    {value: 'subjects_done', label: 'Subjects with results recorded'},
                    {value: 'subjects_expected', label: 'All subjects allocated to the student'}
                ], description: "How a student's totals, means and grade are divided: by subjects with marks recorded, or by all subjects they are allocated to (missing ones count as zero). Affects broadsheet and report forms."}
            ]
        },
        {
            name: 'ReportForm',
            title: 'Report Form Settings',
            color: 'warning',
            icon: 'fas fa-file-alt',
            settings: [
                {key: 'DisplayMode', label: 'Report Display Mode', type: 'select', options: [
                    {value: 'marks', label: 'Marks Only'},
                    {value: 'grades', label: 'Grades Only'},
                    {value: 'points_grades', label: 'Points & Grades'},
                    {value: 'marks_grades', label: 'Marks & Grades'}
                ], description: 'Controls what is shown in the subject columns on the report form'},
                {key: 'ShowValues', label: 'Show Values Section', type: 'boolean', description: 'Show the values section on the report form'},
                {key: 'ShowCoCurricular', label: 'Show Co-Curricular Section', type: 'boolean', description: 'Show co-curricular activities on the report form'},
                {key: 'ShowResponsibilities', label: 'Show Responsibilities Section', type: 'boolean', description: 'Show responsibilities & social skills on the report form'},
                {key: 'ShowCommunityService', label: 'Show Community Service Section', type: 'boolean', description: 'Show community service activities on the report form'},
                {key: 'ShowPosition', label: 'Show Position/Rank', type: 'boolean', description: 'Show student position and out of on the report form'}
            ]
        }
    ];

    settingValues: { [key: string]: string } = {};
    isSaving: boolean = false;
    accounts: any[] = [];

    constructor(
        private toastr: ToastrService,
        private globalSettingSvc: GlobalSettingService,
        private examTypeSvc: ExamTypeService,
        private accountSvc: AccountService,
        private curriculaSvc: CurriculumService,
        private educationLevelSvc: EducationLevelService
    ) {}

    ngOnInit(): void {
        // Collapse all modules except the first one
        this.modules.slice(1).forEach((m) => this.collapsedModules.add(m.name));
        this.loadExamTypes();
        this.loadAccounts();
        // Inject the per-education-level "Exam Results" rows, THEN load values -
        // loadSettings() must run after the dynamic settings exist so their
        // saved values/defaults are mapped.
        this.loadGradingPerLevel();
    }

    // Appends one "Exam Results" grading selector per education level to the
    // Grading module. Each defaults to "Use global default" (empty value), so a
    // level only overrides the global Exam Results setting when explicitly set.
    loadGradingPerLevel = () => {
        forkJoin([
            this.curriculaSvc.get('/curricula'),
            this.educationLevelSvc.get('/educationLevels')
        ]).subscribe({
            next: ([curricula, edLevels]: any[]) => {
                let currName = (id: any) => curricula.find((c: any) => +c.id === +id)?.name || '';
                let grading = this.modules.find((m) => m.name === 'Grading');
                let levels = (edLevels || []).slice().sort((a: any, b: any) =>
                    (+a.curriculumId - +b.curriculumId) || ((a.rank || 0) - (b.rank || 0)));
                levels.forEach((el: any) => {
                    let cn = currName(el.curriculumId);
                    grading?.settings.push({
                        key: `ExamResults:${el.id}`,
                        label: `Exam Results — ${cn ? cn + ': ' : ''}${el.name}`,
                        type: 'select',
                        options: [
                            {value: '', label: 'Use global default'},
                            {value: '4-Point', label: '4-Point (EE, ME, AE, BE)'},
                            {value: '8-Point', label: '8-Point (EE1, EE2, ME1, ME2, AE1, AE2, BE1, BE2)'}
                        ],
                        description: "Exam results grading for this education level. 'Use global default' inherits the global Exam Results setting above."
                    } as any);
                });
                this.loadSettings();
            },
            error: () => this.loadSettings()
        });
    };

    loadAccounts = () => {
        this.accountSvc.get('/accounts').subscribe({
            next: (accounts: any[]) => {
                let typeNames: any = {1: 'Asset', 2: 'Liability', 3: 'Equity', 4: 'Income', 5: 'Expense'};
                this.accounts = accounts
                    .map((a: any) => ({...a, accountTypeName: typeNames[a.accountType] || ''}))
                    .sort((a: any, b: any) => (a.code || '').localeCompare(b.code || ''));
            },
            error: () => {}
        });
    };

    loadExamTypes = () => {
        this.examTypeSvc.get('/examTypes').subscribe({
            next: (examTypes) => {
                let sorted = examTypes.sort((a, b) => (a.rank || 0) - (b.rank || 0));
                let options = sorted.map((et) => ({value: et.id.toString(), label: et.name}));
                let generalModule = this.modules.find((m) => m.name === 'General');
                if (generalModule) {
                    let examTypeSetting = generalModule.settings.find((s) => s.key === 'CurrentExamType');
                    if (examTypeSetting) {
                        examTypeSetting.options = options;
                    }
                }
            },
            error: () => {}
        });
    };

    loadSettings = () => {
        this.globalSettingSvc.get('/globalSettings').subscribe({
            next: (settings) => {
                settings.forEach((s) => {
                    this.settingValues[s.module + '.' + s.settingKey] = s.settingValue;
                });
                // Set defaults for missing settings
                this.modules.forEach((m) => {
                    m.settings.forEach((s: any) => {
                        let key = m.name + '.' + s.key;
                        if (!this.settingValues[key]) {
                            if (s.type === 'boolean') this.settingValues[key] = 'true';
                            else if (s.type === 'select' && s.options?.length > 0) this.settingValues[key] = s.options[0].value;
                            else if (s.type === 'text' && s.key === 'DepartmentCodePrefix') this.settingValues[key] = 'DEPT';
                            else if (s.type === 'text' && s.key === 'BudgetMasterCodePrefix') this.settingValues[key] = 'BUD';
                        }
                    });
                });
            },
            error: (err) => this.toastr.error(err.error)
        });
    };

    getValue = (module: string, key: string): string => {
        return this.settingValues[module + '.' + key] || '';
    };

    setValue = (module: string, key: string, value: string) => {
        this.settingValues[module + '.' + key] = value;
    };

    toggleBoolean = (module: string, key: string) => {
        let current = this.settingValues[module + '.' + key];
        this.settingValues[module + '.' + key] = current === 'true' ? 'false' : 'true';
    };

    saveAll = () => {
        Swal.fire({
            title: 'Save settings?',
            text: 'All settings will be updated.',
            width: 400, position: 'top', padding: '1em', icon: 'question',
            showCancelButton: true, confirmButtonText: 'Save', cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                this.isSaving = true;
                let requests: any[] = [];
                this.modules.forEach((m) => {
                    m.settings.forEach((s) => {
                        let value = this.settingValues[m.name + '.' + s.key] || '';
                        requests.push(
                            this.globalSettingSvc.upsert({
                                module: m.name,
                                settingKey: s.key,
                                settingValue: value,
                                description: s.description
                            })
                        );
                    });
                });

                import('rxjs').then(({forkJoin}) => {
                    forkJoin(requests).subscribe(
                        () => {
                            this.isSaving = false;
                            this.toastr.success('Settings saved!');
                            this.loadSettings();
                        },
                        (err) => {
                            this.isSaving = false;
                            this.toastr.error(err.error?.message || 'Error saving settings.');
                        }
                    );
                });
            }
        });
    };
}
