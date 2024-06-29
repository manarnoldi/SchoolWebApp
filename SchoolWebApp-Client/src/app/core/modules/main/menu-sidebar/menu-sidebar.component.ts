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
        this.user = JSON.parse(sessionStorage.getItem('current_user'));
        console.log(this.user);
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
                name: 'Classes',
                iconClasses: 'fas fa-bullseye',
                path: ['/school/classes']
            },
            {
                name: 'Streams',
                iconClasses: 'fas fa-bullseye',
                path: ['/school/streams']
            },
            {
                name: 'Sessions',
                iconClasses: 'fas fa-bullseye',
                path: ['/school/sessions']
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
            },
            {
                name: 'Learning levels',
                iconClasses: 'fas fa-bullseye',
                path: ['/school/learningLevels']
            }
        ]
    },
    {
        name: 'Staff',
        iconClasses: 'fas fa-user-tie',
        children: [
            {
                name: 'Details',
                iconClasses: 'fas fa-bullseye',
                path: ['/staff']
            },
            {
                name: 'Attendance',
                iconClasses: 'fas fa-bullseye',
                path: ['/staff']
            },
            {
                name: 'Discipline',
                iconClasses: 'fas fa-bullseye',
                path: ['/staff']
            },
            {
                name: 'Subjects',
                iconClasses: 'fas fa-bullseye',
                path: ['/staff']
            }
        ]
    },
    {
        name: 'Students',
        iconClasses: 'fas fa-user-graduate',
        children: [
            {
                name: 'Details',
                iconClasses: 'fas fa-bullseye',
                path: ['/student']
            },
            {
                name: 'Former School',
                iconClasses: 'fas fa-bullseye',
                path: ['/student']
            },
            {
                name: 'Class',
                iconClasses: 'fas fa-bullseye',
                path: ['/student']
            },
            {
                name: 'Parents',
                iconClasses: 'fas fa-bullseye',
                path: ['/student']
            },
            {
                name: 'Discipline',
                iconClasses: 'fas fa-bullseye',
                path: ['/student']
            },
            {
                name: 'Subjects',
                iconClasses: 'fas fa-bullseye',
                path: ['/student']
            },
            {
                name: 'Attendance',
                iconClasses: 'fas fa-bullseye',
                path: ['/student']
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
                name: 'Curricula',
                iconClasses: 'fas fa-bullseye',
                path: ['/academics/curricula']
            },
            {
                name: 'Grading System',
                iconClasses: 'fas fa-bullseye',
                path: ['/academics/gradingSystem']
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
    }
];
