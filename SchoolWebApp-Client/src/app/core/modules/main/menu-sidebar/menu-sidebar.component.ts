import {AppState} from '@/core/store/state';
import {UiState} from '@/core/store/ui/state';
import {Component, HostBinding, OnInit} from '@angular/core';
import {Store} from '@ngrx/store';
import {AuthService} from '@/core/services/auth.service';
import {MenuPermissionService} from '../../../../security/services/menu-permission.service';
import {Observable} from 'rxjs';

const BASE_CLASSES = 'main-sidebar elevation-4';
@Component({
    selector: 'app-menu-sidebar',
    templateUrl: './menu-sidebar.component.html',
    styleUrls: ['./menu-sidebar.component.scss']
})
export class MenuSidebarComponent implements OnInit {
    @HostBinding('class') classes: string = BASE_CLASSES;
    public ui: Observable<UiState>;
    public user;
    public menu = MENU;

    constructor(
        public authService: AuthService,
        private store: Store<AppState>,
        private menuPermSvc: MenuPermissionService
    ) {}

    ngOnInit() {
        this.ui = this.store.select('ui');
        this.ui.subscribe((state: UiState) => {
            this.classes = `${BASE_CLASSES} ${state.sidebarSkin}`;
        });
        this.user = JSON.parse(localStorage.getItem('current_user'));
        this.loadMenuPermissions();
    }

    loadMenuPermissions() {
        if (!this.user) return;

        // Super Administrators get full access
        const isSuperAdmin = this.user.roles?.some((r: string) =>
            r.toLowerCase() === 'superadministrator'
        );
        if (isSuperAdmin) {
            this.menu = MENU;
            return;
        }

        this.menuPermSvc.getMyPermissions().subscribe({
            next: (result) => {
                if (result.allAccess) {
                    this.menu = MENU;
                } else {
                    this.menu = this.filterMenu(MENU, result.paths);
                }
            },
            error: () => {
                // On error, show full menu (fallback)
                this.menu = MENU;
            }
        });
    }

    filterMenu(items: any[], allowedPaths: string[]): any[] {
        if (!allowedPaths || allowedPaths.length === 0) {
            // If no permissions configured at all, show full menu
            return MENU;
        }

        return items
            .map(item => {
                if (item.children) {
                    const filteredChildren = this.filterMenu(item.children, allowedPaths);
                    if (filteredChildren.length > 0) {
                        return {...item, children: filteredChildren};
                    }
                    return null;
                }
                if (item.path) {
                    const pathStr = Array.isArray(item.path) ? item.path.join('/') : item.path;
                    return allowedPaths.includes(pathStr) ? item : null;
                }
                return item;
            })
            .filter(item => item !== null);
    }
}

export const MENU = [
    {
        name: 'Dashboard',
        iconClasses: 'fas fa-tachometer-alt',
        path: ['/']
    },
    {
        name: 'School',
        iconClasses: 'fas fa-book',
        children: [
            {name: 'Details', iconClasses: 'fas fa-bullseye', path: ['/school/details']},
            {name: 'Academic Years', iconClasses: 'fas fa-bullseye', path: ['/school/academicYears']},
            {name: 'Sessions', iconClasses: 'fas fa-bullseye', path: ['/school/sessions']},
            {name: 'Classes', iconClasses: 'fas fa-bullseye', path: ['/class/classes']},
            {name: 'Staff Details', iconClasses: 'fas fa-bullseye', path: ['/staff/details']},
            {name: 'Staff Attendance', iconClasses: 'fas fa-bullseye', path: ['/staff/staff-attendances']},
            {name: 'Staff Subjects', iconClasses: 'fas fa-bullseye', path: ['/school/bulk-staff-subjects']},
            {name: 'Events', iconClasses: 'fas fa-bullseye', path: ['/school/events']}
        ]
    },
    {
        name: 'Students',
        iconClasses: 'fas fa-user-graduate',
        children: [
            {name: 'Basic Details', iconClasses: 'fas fa-bullseye', path: ['/students/details']},
            {name: 'Parents', iconClasses: 'fas fa-bullseye', path: ['/students/parents']},
            {name: 'Subject Allocation', iconClasses: 'fas fa-bullseye', path: ['/students/students-subjects']},
            {name: 'Attendance', iconClasses: 'fas fa-bullseye', path: ['/students/students-attendances']},
            {name: 'Class Assignment', iconClasses: 'fas fa-bullseye', path: ['/students/promotion']}
        ]
    },
    {
        name: 'CBE Curriculum',
        iconClasses: 'fas fa-pen',
        children: [
            {name: 'Student Assessments', iconClasses: 'fas fa-bullseye text-info', path: ['/cbe/assessments/assessments']},
            {name: 'Co-curricular', iconClasses: 'fas fa-bullseye text-success', path: ['/cbe/cocurriculum/student-scores']},
            {name: 'Responsibilities', iconClasses: 'fas fa-bullseye text-primary', path: ['/cbe/responsibilities/student-assignments']},
            {name: 'Community Service', iconClasses: 'fas fa-bullseye text-secondary', path: ['/cbe/community-service/student-assignments']},
            {name: 'Values', iconClasses: 'fas fa-bullseye text-danger', path: ['/cbe/values/student-assignments']},
            {name: 'Register Exams', iconClasses: 'fas fa-bullseye text-warning', path: ['/cbe/exams/exams']},
            {name: 'Results (Subject)', iconClasses: 'fas fa-bullseye text-warning', path: ['/cbe/exams/exam-results']},
            {name: 'Results (Bulk)', iconClasses: 'fas fa-bullseye text-warning', path: ['/cbe/exams/exam-results-bulk']}
        ]
    },
    {
        name: 'Finance',
        iconClasses: 'fas fa-coins',
        children: [
            {name: 'Budget Register', iconClasses: 'fas fa-clipboard-list text-info', path: ['/finance/budget-register']},
            {name: 'Budgets', iconClasses: 'fas fa-piggy-bank text-warning', path: ['/finance/budgets']},
            {name: 'Budget Amendments', iconClasses: 'fas fa-edit text-warning', path: ['/finance/budget-amendments']},
            {name: 'Fee Structures', iconClasses: 'fas fa-file-invoice-dollar text-success', path: ['/finance/fee-structures']},
            {name: 'Student Invoices', iconClasses: 'fas fa-file-invoice text-warning', path: ['/finance/invoices']},
            {name: 'Fee Payments', iconClasses: 'fas fa-cash-register text-success', path: ['/finance/payments']},
            {name: 'Sponsors', iconClasses: 'fas fa-hands-helping text-primary', path: ['/finance/sponsors']},
            {name: 'Sponsorships', iconClasses: 'fas fa-award text-primary', path: ['/finance/sponsorships']},
            {name: 'Sponsor Payments', iconClasses: 'fas fa-hand-holding-usd text-success', path: ['/finance/sponsor-payments']},
            {name: 'Expenses', iconClasses: 'fas fa-money-bill-wave text-danger', path: ['/finance/expenses']},
            {name: 'Journal Entries', iconClasses: 'fas fa-pen text-primary', path: ['/finance/journal-entries']},
            {name: 'Fee Reports', iconClasses: 'fas fa-chart-bar text-info', path: ['/finance/reports/fees']},
            {name: 'Expenses Report', iconClasses: 'fas fa-money-bill-wave text-danger', path: ['/finance/reports/expenses']},
            {name: 'Statements', iconClasses: 'fas fa-file-invoice-dollar text-primary', path: ['/finance/reports/statements']}
        ]
    },
    {
        name: 'Payroll',
        iconClasses: 'fas fa-money-check-alt',
        children: [
            {name: 'Processing', iconClasses: 'fas fa-cogs text-primary', path: ['/payroll/periods']},
            {name: 'Employee Salaries', iconClasses: 'fas fa-user-tie text-success', path: ['/payroll/employee-salaries']},
            {name: 'Loans & Advances', iconClasses: 'fas fa-hand-holding-usd text-warning', path: ['/payroll/loan-advances']},
            {name: 'Earning Types', iconClasses: 'fas fa-plus-circle text-info', path: ['/payroll/earning-types']},
            {name: 'Deduction Types', iconClasses: 'fas fa-minus-circle text-danger', path: ['/payroll/deduction-types']},
            {name: 'Tax Bands', iconClasses: 'fas fa-percentage text-secondary', path: ['/payroll/tax-bands']},
            {name: 'Payroll Settings', iconClasses: 'fas fa-sliders-h text-primary', path: ['/payroll/settings']},
            {name: 'Reports', iconClasses: 'fas fa-chart-pie text-info', path: ['/payroll/reports']}
        ]
    },
    {
        name: 'Settings',
        iconClasses: 'fas fa-wrench',
        children: [
            {
                name: 'Dropdowns',
                iconClasses: 'fas fa-list text-info',
                path: ['/settings/dropdowns'],
                extraActivePaths: [
                    '/cbe/assessments/themes',
                    '/cbe/assessments/strands',
                    '/cbe/assessments/sub-strands',
                    '/cbe/assessments/specific-outcomes',
                    '/cbe/assessments/general-outcomes',
                    '/cbe/assessments/broad-outcomes',
                    '/academics/subjects',
                    '/academics/educationLevelSubjects',
                    '/academics/grades'
                ]
            },
            {
                name: 'Approval Workflows',
                iconClasses: 'fas fa-sitemap text-info',
                path: ['/settings/approval-workflows']
            },
            {
                name: 'Global Settings',
                iconClasses: 'fas fa-cogs text-primary',
                path: ['/settings/global-settings']
            }
        ]
    },
    {
        name: 'Security',
        iconClasses: 'fas fa-lock',
        children: [
            {name: 'Users', iconClasses: 'fas fa-users text-primary', path: ['/security/users']},
            {name: 'Roles', iconClasses: 'fas fa-user-shield text-success', path: ['/security/roles']},
            {name: 'Menu Permissions', iconClasses: 'fas fa-bars text-danger', path: ['/security/menu-permissions']}
        ]
    },
    {
        name: 'Reports',
        iconClasses: 'fas fa-chart-bar',
        children: [
            {
                name: 'Staff Reports',
                iconClasses: 'fas fa-circle',
                children: [
                    {
                        name: 'Attendance Report',
                        iconClasses: 'fas fa-bullseye text-info',
                        path: ['/reports/staff/attendance']
                    },
                    {
                        name: 'Attendance Detailed',
                        iconClasses: 'fas fa-bullseye text-info',
                        path: ['/reports/staff/attendance-details']
                    },
                    {
                        name: 'Subject Allocation',
                        iconClasses: 'fas fa-bullseye text-info',
                        path: ['/reports/staff/subject-allocation']
                    }
                ]
            },
            {
                name: 'Class Reports',
                iconClasses: 'fas fa-circle',
                children: [
                    {
                        name: 'Class List',
                        iconClasses: 'fas fa-bullseye text-success',
                        path: ['/reports/class/class-list']
                    },
                    {
                        name: 'Attendance Report',
                        iconClasses: 'fas fa-bullseye text-success',
                        path: ['/reports/class/attendance']
                    },
                    {
                        name: 'Attendance Detailed',
                        iconClasses: 'fas fa-bullseye text-success',
                        path: ['/reports/class/attendance-details']
                    }
                ]
            },
            {
                name: 'Academics Reports',
                iconClasses: 'fas fa-circle',
                children: [
                    {
                        name: 'Missing Marks',
                        iconClasses: 'fas fa-bullseye text-warning',
                        path: ['/reports/academics/missing-marks']
                    },
                    {
                        name: 'Exam Results',
                        iconClasses: 'fas fa-bullseye text-warning',
                        path: ['/reports/academics/exam-results']
                    },
                    {
                        name: 'Report Forms',
                        iconClasses: 'fas fa-bullseye text-warning',
                        path: ['/reports/academics/report-forms']
                    },
                    {
                        name: 'Assessment Report',
                        iconClasses: 'fas fa-bullseye text-warning',
                        path: ['/reports/academics/assessment-report']
                    },
                    {
                        name: 'Student Subject Allocation',
                        iconClasses: 'fas fa-bullseye text-warning',
                        path: ['/reports/academics/student-subject-allocation']
                    }
                ]
            }
        ]
    }
];
