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
                path: ['/school']
            },
            {
                name: 'Departments',
                iconClasses: 'fas fa-bullseye',
                path: ['/school']
            },
            {
                name: 'Events',
                iconClasses: 'fas fa-bullseye',
                path: ['/school']
            },
            {
                name: 'Classes',
                iconClasses: 'fas fa-bullseye',
                path: ['/school']
            },
            {
                name: 'Sessions',
                iconClasses: 'fas fa-bullseye',
                path: ['/school']
            },
            {
                name: 'Academis years',
                iconClasses: 'fas fa-bullseye',
                path: ['/school']
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
                path: ['/academics']
            },
            {
                name: 'Subjects',
                iconClasses: 'fas fa-bullseye',
                path: ['/academics']
            },
            {
                name: 'Grading System',
                iconClasses: 'fas fa-bullseye',
                path: ['/academics']
            },
            {
                name: 'Exam Types',
                iconClasses: 'fas fa-bullseye',
                path: ['/academics']
            },
            {
                name: 'Exams',
                iconClasses: 'fas fa-bullseye',
                path: ['/academics']
            },
            {
                name: 'Exams Results',
                iconClasses: 'fas fa-bullseye',
                path: ['/academics']
            }
        ]
    },
    {
        name: 'Settings',
        iconClasses: 'fas fa-wrench',
        children: [
            {
                name: 'Education Level Types',
                iconClasses: 'fas fa-bullseye',
                path: ['/settings']
            },
            {
                name: 'Education Levels',
                iconClasses: 'fas fa-bullseye',
                path: ['/settings']
            },
            {
                name: 'Learning Modes',
                iconClasses: 'fas fa-bullseye',
                path: ['/settings']
            },
            {
                name: 'Learning Levels',
                iconClasses: 'fas fa-bullseye',
                path: ['/settings']
            },
            {
                name: 'Streams',
                iconClasses: 'fas fa-bullseye',
                path: ['/settings']
            },
            {
                name: 'Curricula',
                iconClasses: 'fas fa-bullseye',
                path: ['/settings']
            },
            {
                name: 'Designations',
                iconClasses: 'fas fa-bullseye',
                path: ['/settings']
            },
            {
                name: 'Employment types',
                iconClasses: 'fas fa-bullseye',
                path: ['/settings']
            },
            {
                name: 'Gender',
                iconClasses: 'fas fa-bullseye',
                path: ['/settings']
            },
            {
                name: 'Nationality',
                iconClasses: 'fas fa-bullseye',
                path: ['/settings']
            },
            {
                name: 'Occupations',
                iconClasses: 'fas fa-bullseye',
                path: ['/settings']
            },
            {
                name: 'Occurence types',
                iconClasses: 'fas fa-bullseye',
                path: ['/settings']
            },
            {
                name: 'Outcomes',
                iconClasses: 'fas fa-bullseye',
                path: ['/settings']
            },
            {
                name: 'Relationships',
                iconClasses: 'fas fa-bullseye',
                path: ['/settings']
            },
            {
                name: 'Religions',
                iconClasses: 'fas fa-bullseye',
                path: ['/settings']
            },
            {
                name: 'Session types',
                iconClasses: 'fas fa-bullseye',
                path: ['/settings']
            },
            {
                name: 'Staff categories',
                iconClasses: 'fas fa-bullseye',
                path: ['/settings']
            }
        ]
    }
];
