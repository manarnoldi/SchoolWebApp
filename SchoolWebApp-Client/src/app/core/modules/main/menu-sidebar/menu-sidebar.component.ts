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

        // Administrators get full access
        const isAdmin = this.user.roles?.some((r: string) =>
            r.toLowerCase() === 'administrator'
        );
        if (isAdmin) {
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
            {name: 'Education Levels', iconClasses: 'fas fa-bullseye', path: ['/school/educationLevels']},
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
            {name: 'Promotion', iconClasses: 'fas fa-bullseye', path: ['/students/promotion']}
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
            {name: 'User Roles', iconClasses: 'fas fa-user-tag text-warning', path: ['/security/user-roles']},
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
                    }
                ]
            }
        ]
    }
];
