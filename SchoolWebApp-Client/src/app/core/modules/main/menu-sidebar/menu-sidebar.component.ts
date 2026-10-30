import {AppState} from '@/core/store/state';
import {UiState} from '@/core/store/ui/state';
import {Component, HostBinding, OnInit} from '@angular/core';
import {Store} from '@ngrx/store';
import {AuthService} from '@/core/services/auth.service';
import {MenuPermissionService} from '../../../../security/services/menu-permission.service';
import {Observable} from 'rxjs';
import {LicenceService} from '@/core/services/licence.service';

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
        private menuPermSvc: MenuPermissionService,
        private licence: LicenceService
    ) {}

    ngOnInit() {
        this.ui = this.store.select('ui');
        this.ui.subscribe((state: UiState) => {
            this.classes = `${BASE_CLASSES} ${state.sidebarSkin}`;
        });
        this.user = JSON.parse(localStorage.getItem('current_user'));
        this.loadMenuPermissions();
    }

    // What the school has bought decides the menus first; then the user's permissions.
    loadMenuPermissions() {
        if (!this.user) return;
        this.menu = licensedMenu(MENU, this.licence);
        this.licence.load().subscribe(() => {
            const licensed = licensedMenu(MENU, this.licence);
            this.menu = licensed;

            // Super Administrators get full access
            const isSuperAdmin = this.user.roles?.some((r: string) =>
                r.toLowerCase() === 'superadministrator'
            );
            if (isSuperAdmin) return;

            this.menuPermSvc.getMyPermissions().subscribe({
                next: (result) => {
                    this.menu = result.allAccess ? licensed : this.filterMenu(licensed, result.paths);
                },
                // On error, show the licensed menu (fallback)
                error: () => (this.menu = licensed)
            });
        });
    }

    filterMenu(items: any[], allowedPaths: string[]): any[] {
        // A role with no menu permissions sees only the dashboard (the API refuses the rest).
        if (!allowedPaths || allowedPaths.length === 0) {
            return items.filter((item) => item.path && item.path.join('/') === '/');
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

/**
 * The menu without the modules the school's licence does not include. An item
 * with a `module` (and everything under it) shows only when that module is on;
 * a group left empty is dropped.
 */
export function licensedMenu(items: any[], licence: LicenceService): any[] {
    return items
        .filter((item) => licence.has(item.module))
        .map((item) => (item.children ? {...item, children: licensedMenu(item.children, licence)} : item))
        .filter((item) => !item.children || item.children.length > 0);
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
            {name: 'Events', iconClasses: 'fas fa-bullseye', path: ['/school/events']},
            {
                name: 'Reports',
                iconClasses: 'fas fa-chart-bar text-success',
                children: [
                    {name: 'Class List', iconClasses: 'fas fa-bullseye text-success', path: ['/reports/class/class-list']},
                    {name: 'Attendance Report', iconClasses: 'fas fa-bullseye text-success', path: ['/reports/class/attendance']},
                    {name: 'Attendance Detailed', iconClasses: 'fas fa-bullseye text-success', path: ['/reports/class/attendance-details']}
                ]
            }
        ]
    },
    {
        name: 'Staff',
        iconClasses: 'fas fa-user-tie',
        children: [
            {name: 'Staff Details', iconClasses: 'fas fa-bullseye', path: ['/staff/details'],
                // A staff member's own page, and adding one.
                extraActivePaths: ['/staff/manage', '/staff/add']},
            {name: 'Attendance', iconClasses: 'fas fa-bullseye', path: ['/staff/staff-attendances']},
            {name: 'Subject Allocation', iconClasses: 'fas fa-bullseye', path: ['/school/bulk-staff-subjects'], module: 'Academics'},
            {
                name: 'Reports',
                iconClasses: 'fas fa-chart-bar text-info',
                children: [
                    {name: 'Attendance Report', iconClasses: 'fas fa-bullseye text-info', path: ['/reports/staff/attendance']},
                    {name: 'Attendance Detailed', iconClasses: 'fas fa-bullseye text-info', path: ['/reports/staff/attendance-details']},
                    {name: 'Subject Allocation', iconClasses: 'fas fa-bullseye text-info', path: ['/reports/staff/subject-allocation'], module: 'Academics'}
                ]
            }
        ]
    },
    {
        name: 'Students',
        iconClasses: 'fas fa-user-graduate',
        children: [
            {name: 'Basic Details', iconClasses: 'fas fa-bullseye', path: ['/students/details'],
                // A student's own page (tabs: class, parents, academics, fees...).
                extraActivePaths: ['/students/manage']},
            {name: 'Parents', iconClasses: 'fas fa-bullseye', path: ['/students/parents']},
            {name: 'Subject Allocation', iconClasses: 'fas fa-bullseye', path: ['/students/students-subjects'], module: 'Academics'},
            {name: 'Attendance', iconClasses: 'fas fa-bullseye', path: ['/students/students-attendances']},
            {name: 'Class Assignment', iconClasses: 'fas fa-bullseye', path: ['/students/promotion']}
        ]
    },
    {
        name: 'Academics',
        iconClasses: 'fas fa-pen',
        module: 'Academics',
        children: [
            {
                name: 'Assessments',
                iconClasses: 'fas fa-tasks text-info',
                children: [
                    {name: 'Student Assessments', iconClasses: 'fas fa-bullseye text-info', path: ['/cbe/assessments/assessments']},
                    // Permission flag (not a navigable feature). When assigned to a role,
                    // members of that role can enter Student Assessment marks for any
                    // teacher / class / subject. Without it (and without Administrator
                    // or SuperAdministrator role), users are restricted to their own
                    // allocations on /cbe/assessments/assessments.
                    {name: 'Student Assessments (Admin)', iconClasses: 'fas fa-user-shield text-warning', path: ['/cbe/assessments/admin']}
                ]
            },
            {
                name: 'Exams',
                iconClasses: 'fas fa-file-signature text-warning',
                children: [
                    {name: 'School Exams', iconClasses: 'fas fa-calendar-check text-info', path: ['/cbe/exams/school-exams']},
                    {name: 'Register Exams', iconClasses: 'fas fa-bullseye text-warning', path: ['/cbe/exams/exams']},
                    {name: 'Results Entry', iconClasses: 'fas fa-bullseye text-warning', path: ['/cbe/exams/exam-results-bulk'],
                        extraActivePaths: ['/cbe/exams/exam-results', '/cbe/exams/exam-results-classwise']}
                ]
            },
            {
                name: 'Learner Development',
                iconClasses: 'fas fa-seedling text-success',
                children: [
                    {name: 'Co-curricular', iconClasses: 'fas fa-bullseye text-success', path: ['/cbe/cocurriculum/student-scores'],
                        extraActivePaths: ['/cbe/cocurriculum/student-assignments', '/cbe/cocurriculum/scores-setup']},
                    {name: 'Responsibilities', iconClasses: 'fas fa-bullseye text-primary', path: ['/cbe/responsibilities/student-assignments']},
                    {name: 'Community Service', iconClasses: 'fas fa-bullseye text-secondary', path: ['/cbe/community-service/student-assignments']},
                    {name: 'Values', iconClasses: 'fas fa-bullseye text-danger', path: ['/cbe/values/student-assignments']}
                ]
            },
            {
                name: 'Reports',
                iconClasses: 'fas fa-chart-bar text-primary',
                children: [
                    {name: 'Exam Results', iconClasses: 'fas fa-bullseye text-warning', path: ['/reports/academics/exam-results'],
                        extraActivePaths: ['/reports/academics/report-forms', '/reports/academics/missing-marks', '/reports/academics/results-analysis']},
                    {name: 'Assessment Report', iconClasses: 'fas fa-bullseye text-warning', path: ['/reports/academics/assessment-report']},
                    {name: 'Subject Performance', iconClasses: 'fas fa-bullseye text-warning', path: ['/reports/academics/subject-performance']},
                    {name: 'Class Performance', iconClasses: 'fas fa-bullseye text-warning', path: ['/reports/academics/class-performance']},
                    {name: 'Student Subject Allocation', iconClasses: 'fas fa-bullseye text-warning', path: ['/reports/academics/student-subject-allocation']}
                ]
            }
        ]
    },
    {
        name: 'Teaching Management',
        iconClasses: 'fas fa-chalkboard-teacher',
        // Sold as part of Academics.
        module: 'Academics',
        children: [
            {name: 'Schemes of Work', iconClasses: 'fas fa-table text-primary', path: ['/teaching/schemes']},
            {name: 'Lesson Plans', iconClasses: 'fas fa-book-open text-info', path: ['/teaching/lesson-plans']},
            {name: 'Record of Work', iconClasses: 'fas fa-clipboard-check text-success', path: ['/teaching/record-of-work']},
            {name: 'Tracker', iconClasses: 'fas fa-th text-warning', path: ['/teaching/tracker']},
            // Permission flag (not a navigable feature). A role holding it sees
            // and opens every teacher's documents, as Administrators do. Heads of
            // department see their own departments without it.
            {name: 'Whole-school Access (Admin)', iconClasses: 'fas fa-user-shield text-warning', path: ['/teaching/admin']},
            // Read-only: the full curriculum design of a subject and grade.
            {name: 'Curriculum', iconClasses: 'fas fa-book text-info', path: ['/teaching/curriculum']}
        ]
    },
    {
        name: 'Finance',
        iconClasses: 'fas fa-coins',
        module: 'Finance',
        children: [
            {
                name: 'Budgets',
                iconClasses: 'fas fa-piggy-bank text-warning',
                children: [
                    {name: 'Budget Register', iconClasses: 'fas fa-clipboard-list text-info', path: ['/finance/budget-register']},
                    {name: 'Budgets', iconClasses: 'fas fa-piggy-bank text-warning', path: ['/finance/budgets']},
                    {name: 'Budget Amendments', iconClasses: 'fas fa-edit text-warning', path: ['/finance/budget-amendments']}
                ]
            },
            {
                name: 'Fees',
                iconClasses: 'fas fa-file-invoice-dollar text-success',
                children: [
                    {name: 'Fee Structures', iconClasses: 'fas fa-file-invoice-dollar text-success', path: ['/finance/fee-structures']},
                    {name: 'Student Invoices', iconClasses: 'fas fa-file-invoice text-warning', path: ['/finance/invoices']},
                    {name: 'Fee Payments', iconClasses: 'fas fa-cash-register text-success', path: ['/finance/payments']},
                    {name: 'Fee Reports', iconClasses: 'fas fa-chart-bar text-info', path: ['/finance/reports/fees']}
                ]
            },
            {
                name: 'Sponsors',
                iconClasses: 'fas fa-hands-helping text-primary',
                children: [
                    {name: 'Sponsors', iconClasses: 'fas fa-hands-helping text-primary', path: ['/finance/sponsors']},
                    {name: 'Sponsorships', iconClasses: 'fas fa-award text-primary', path: ['/finance/sponsorships']},
                    {name: 'Sponsor Payments', iconClasses: 'fas fa-hand-holding-usd text-success', path: ['/finance/sponsor-payments']}
                ]
            },
            {
                name: 'Expenses & Accounts',
                iconClasses: 'fas fa-money-bill-wave text-danger',
                children: [
                    {name: 'Expenses', iconClasses: 'fas fa-money-bill-wave text-danger', path: ['/finance/expenses']},
                    {name: 'Journal Entries', iconClasses: 'fas fa-pen text-primary', path: ['/finance/journal-entries']}
                ]
            },
            {
                name: 'Reports',
                iconClasses: 'fas fa-chart-bar text-info',
                children: [
                    {name: 'Statements', iconClasses: 'fas fa-file-invoice-dollar text-primary', path: ['/finance/reports/statements']},
                    {name: 'Expenses Report', iconClasses: 'fas fa-money-bill-wave text-danger', path: ['/finance/reports/expenses']}
                ]
            }
        ]
    },
    {
        name: 'Payroll',
        iconClasses: 'fas fa-money-check-alt',
        module: 'Payroll',
        children: [
            // Day-to-day payroll work, in the order it is done: who is paid and
            // what, then process (which also shows, checks and prints the payslips),
            // then report. The one-off set-up (earning and deduction types, tax and
            // NSSF bands, reliefs, rates) lives under Settings > Dropdowns > Payroll
            // Settings.
            {name: 'Employee Salaries', iconClasses: 'fas fa-user-tie text-success', path: ['/payroll/employee-salaries']},
            {name: 'Loans & Advances', iconClasses: 'fas fa-hand-holding-usd text-warning', path: ['/payroll/loan-advances']},
            {name: 'Processing', iconClasses: 'fas fa-cogs text-primary', path: ['/payroll/periods']},
            {name: 'Reports', iconClasses: 'fas fa-chart-pie text-info', path: ['/payroll/reports']}
        ]
    },
    {
        name: 'Elections',
        iconClasses: 'fas fa-vote-yea',
        module: 'Elections',
        children: [
            {name: 'Manage Elections', iconClasses: 'fas fa-cogs text-info', path: ['/elections/manage']},
            // Returning officers: the stations they run (elections officers see all).
            {name: 'Polling Station', iconClasses: 'fas fa-person-booth text-success', path: ['/elections/polling']},
            {name: 'Results', iconClasses: 'fas fa-poll text-warning', path: ['/elections/results']}
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
                    '/cbe/assessments/curriculum-upload',
                    '/cbe/assessments/curriculum-editor',
                    '/cbe/assessments/sub-strands',
                    '/cbe/assessments/specific-outcomes',
                    '/cbe/assessments/general-outcomes',
                    '/cbe/assessments/broad-outcomes',
                    '/academics/subjects',
                    '/academics/educationLevelSubjects',
                    '/academics/grades',
                    '/class/streams',
                    '/school/learning-levels',
                    // Payroll set-up pages, opened from the Payroll Settings section.
                    '/payroll/earning-types',
                    '/payroll/deduction-types',
                    '/payroll/tax-bands',
                    '/payroll/nssf-bands',
                    '/payroll/reliefs',
                    '/payroll/settings',
                    // Set-up screens opened from the Dropdowns sections.
                    '/school/learningModes', '/school/curricula', '/school/departments',
                    '/class/classNames', '/class/leadership-roles',
                    '/academics/examTypes', '/academics/examNames', '/academics/subjectGroups', '/academics/examResults',
                    '/cbe/assessments/assessment-types', '/cbe/assessments/competencies', '/cbe/assessments/key-questions',
                    '/cbe/assessments/learning-experiences', '/cbe/assessments/lesson-allocations', '/cbe/assessments/pcis',
                    '/cbe/values/values-register', '/cbe/values/values-scores', '/cbe/responsibilities/responsibilities',
                    '/cbe/cocurriculum/activities', '/cbe/cocurriculum/score-types', '/cbe/community-service/activities',
                    '/cbe/exams/exam-types',
                    '/settings/occupations', '/settings/designations', '/settings/employmentTypes', '/settings/genders',
                    '/settings/nationalities', '/settings/occurenceTypes', '/settings/outcomes', '/settings/relationships',
                    '/settings/religions', '/settings/sessionTypes', '/settings/staffCategories',
                    '/finance/accounts', '/finance/fee-categories', '/finance/expense-categories'
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
            },
            {
                name: 'Licence',
                iconClasses: 'fas fa-key text-warning',
                path: ['/settings/licence']
            }
        ]
    },
    {
        name: 'Security',
        iconClasses: 'fas fa-lock',
        children: [
            {name: 'Users', iconClasses: 'fas fa-users text-primary', path: ['/security/users']},
            {name: 'Roles', iconClasses: 'fas fa-user-shield text-success', path: ['/security/roles']},
            {name: 'Menu Permissions', iconClasses: 'fas fa-bars text-danger', path: ['/security/menu-permissions']},
            {name: 'Error Logs', iconClasses: 'fas fa-bug text-warning', path: ['/security/logs']},
            {name: 'Audit Logs', iconClasses: 'fas fa-clipboard-list text-info', path: ['/security/audit-logs']},
            {name: 'Active Users', iconClasses: 'fas fa-user-clock text-info', path: ['/security/active-users']}
        ]
    },
    {
        name: 'Communications',
        iconClasses: 'fas fa-comment-dots',
        // Add-on.
        module: 'Communications',
        children: [
            {name: 'Send Message', iconClasses: 'fas fa-paper-plane text-success', path: ['/communications/compose']},
            {name: 'Message Queue', iconClasses: 'fas fa-stream text-primary', path: ['/communications/queue']},
            {name: 'Templates', iconClasses: 'fas fa-file-alt text-info', path: ['/communications/templates']},
            {name: 'SMS Report', iconClasses: 'fas fa-chart-bar text-warning', path: ['/communications/sms-report']},
            // Gateway credentials - the API serves them to SuperAdministrators only.
            {name: 'Gateway Settings', iconClasses: 'fas fa-plug text-danger', path: ['/communications/settings']}
        ]
    }
];
