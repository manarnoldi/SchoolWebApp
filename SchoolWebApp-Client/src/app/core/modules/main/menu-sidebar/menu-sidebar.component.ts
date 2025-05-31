import {AppState} from '@/core/store/state';
import {UiState} from '@/core/store/ui/state';
import {Component, HostBinding, OnInit} from '@angular/core';
import {Store} from '@ngrx/store';
import {AuthService} from '@/core/services/auth.service';
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
        private store: Store<AppState>
    ) {}

    ngOnInit() {
        this.ui = this.store.select('ui');
        this.ui.subscribe((state: UiState) => {
            this.classes = `${BASE_CLASSES} ${state.sidebarSkin}`;
        });
        this.user = JSON.parse(localStorage.getItem('current_user'));
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
            {
                name: 'Details',
                iconClasses: 'fas fa-bullseye',
                path: ['/school/details']
            },
            {
                name: 'Departments',
                iconClasses: 'fas fa-bullseye',
                path: ['/school/departments']
            },
            {
                name: 'Events',
                iconClasses: 'fas fa-bullseye',
                path: ['/school/events']
            },
            {
                name: 'Education level types',
                iconClasses: 'fas fa-bullseye',
                path: ['/school/educationLevelTypes']
            },
            {
                name: 'Education levels',
                iconClasses: 'fas fa-bullseye',
                path: ['/school/educationLevels']
            },
            {
                name: 'Academic years',
                iconClasses: 'fas fa-bullseye',
                path: ['/school/academicYears']
            },
            {
                name: 'Learning modes',
                iconClasses: 'fas fa-bullseye',
                path: ['/school/learningModes']
            }
        ]
    },
    {
        name: 'Class',
        iconClasses: 'fas fa-book-reader',
        children: [
            {
                name: 'Learning levels',
                iconClasses: 'fas fa-bullseye',
                path: ['/class/classNames']
            },
            {
                name: 'Streams',
                iconClasses: 'fas fa-bullseye',
                path: ['/class/streams']
            },
            {
                name: 'Classes',
                iconClasses: 'fas fa-bullseye',
                path: ['/class/classes']
            },
            {
                name: 'Sessions',
                iconClasses: 'fas fa-bullseye',
                path: ['/class/sessions']
            },
            {
                name: 'Leaderships Roles',
                iconClasses: 'fas fa-bullseye',
                path: ['/class/leadership-roles']
            }
        ]
    },
    {
        name: 'Staff',
        iconClasses: 'fas fa-user-tie',
        children: [
            {
                name: 'Basic details',
                iconClasses: 'fas fa-bullseye',
                path: ['/staff/details']
            },
            {
                name: 'Manage staff',
                iconClasses: 'fas fa-bullseye',
                path: ['/staff/manage']
            },
            {
                name: 'Attendance',
                iconClasses: 'fas fa-bullseye',
                path: ['/staff/staff-attendances']
            }
        ]
    },
    {
        name: 'Students',
        iconClasses: 'fas fa-user-graduate',
        children: [
            {
                name: 'Basic details',
                iconClasses: 'fas fa-bullseye',
                path: ['/students/details']
            },
            {
                name: 'Manage students',
                iconClasses: 'fas fa-bullseye',
                path: ['/students/manage']
            },
            {
                name: 'Parents',
                iconClasses: 'fas fa-bullseye',
                path: ['/students/parents']
            },
            {
                name: 'Subject Allocation',
                iconClasses: 'fas fa-bullseye',
                path: ['/students/students-subjects']
            },
            {
                name: 'Attendance',
                iconClasses: 'fas fa-bullseye',
                path: ['/students/students-attendances']
            }
        ]
    },
    {
        name: 'Academics',
        iconClasses: 'fas fa-book-reader',
        children: [
            {
                name: 'Subject Groups',
                iconClasses: 'fas fa-bullseye',
                path: ['/academics/subjectGroups']
            },
            {
                name: 'Subjects',
                iconClasses: 'fas fa-bullseye',
                path: ['/academics/subjects']
            },
            {
                name: 'Edu-Level Subjects',
                iconClasses: 'fas fa-bullseye',
                path: ['/academics/educationLevelSubjects']
            },
            {
                name: 'Curricula',
                iconClasses: 'fas fa-bullseye',
                path: ['/academics/curricula']
            },
            {
                name: 'Grading System',
                iconClasses: 'fas fa-bullseye',
                path: ['/academics/grades']
            },
            {
                name: 'Exam Types',
                iconClasses: 'fas fa-bullseye',
                path: ['/academics/examTypes']
            },
            {
                name: 'Exams',
                iconClasses: 'fas fa-bullseye',
                path: ['/academics/exams']
            },
            {
                name: 'Exam Results',
                iconClasses: 'fas fa-bullseye',
                path: ['/academics/examResults']
            }
        ]
    },
    {
        name: 'Settings',
        iconClasses: 'fas fa-wrench',
        children: [
            {
                name: 'Designations',
                iconClasses: 'fas fa-bullseye',
                path: ['/settings/designations']
            },
            {
                name: 'Occupations',
                iconClasses: 'fas fa-bullseye',
                path: ['/settings/occupations']
            },
            {
                name: 'Employment types',
                iconClasses: 'fas fa-bullseye',
                path: ['/settings/employmentTypes']
            },
            {
                name: 'Gender',
                iconClasses: 'fas fa-bullseye',
                path: ['/settings/genders']
            },
            {
                name: 'Nationality',
                iconClasses: 'fas fa-bullseye',
                path: ['/settings/nationalities']
            },

            {
                name: 'Occurence types',
                iconClasses: 'fas fa-bullseye',
                path: ['/settings/occurenceTypes']
            },
            {
                name: 'Outcomes',
                iconClasses: 'fas fa-bullseye',
                path: ['/settings/outcomes']
            },
            {
                name: 'Relationships',
                iconClasses: 'fas fa-bullseye',
                path: ['/settings/relationships']
            },
            {
                name: 'Religions',
                iconClasses: 'fas fa-bullseye',
                path: ['/settings/religions']
            },
            {
                name: 'Session types',
                iconClasses: 'fas fa-bullseye',
                path: ['/settings/sessionTypes']
            },
            {
                name: 'Staff categories',
                iconClasses: 'fas fa-bullseye',
                path: ['/settings/staffCategories']
            }
        ]
    },
    {
        name: 'Reports',
        iconClasses: 'fas fa-chart-bar',
        children: [
            // {
            //     name: 'School',
            //     iconClasses: 'fas fa-bullseye',
            //     path: ['/reports/school']
            // },
            {
                name: 'Staff',
                iconClasses: 'fas fa-bullseye',
                path: ['/reports/staff']
            },
            {
                name: 'Class',
                iconClasses: 'fas fa-bullseye',
                path: ['/reports/class']
            },
            {
                name: 'Academics',
                iconClasses: 'fas fa-bullseye',
                path: ['/reports/academics']
            }
        ]
    }
];
