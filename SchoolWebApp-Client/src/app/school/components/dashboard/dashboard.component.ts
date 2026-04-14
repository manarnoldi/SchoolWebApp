import {BreadCrumb} from '@/core/models/bread-crumb';
import {AuthService} from '@/core/services/auth.service';
import {Component, OnInit} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import {SchoolDetailsService} from '../../services/school-details.service';
import {AcademicYearsService} from '../../services/academic-years.service';
import {DateTime} from 'luxon';

@Component({
    selector: 'app-dashboard',
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
    dashboardTitle = 'Dashboard';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/'], title: 'Dashboard'}
    ];

    user: any;
    schoolName: string = '';
    schoolMotto: string = '';
    schoolLogo: string = '';
    activeAcademicYear: string = '';
    currentDate: string = '';
    greeting: string = '';

    quickLinks: any[] = [];

    constructor(
        private authService: AuthService,
        private schoolDetailsSvc: SchoolDetailsService,
        private academicYearSvc: AcademicYearsService,
        private toastr: ToastrService
    ) {}

    ngOnInit(): void {
        this.user = this.authService.getCurrentUser();
        this.currentDate = DateTime.now().toFormat('EEEE, dd MMMM yyyy');
        this.setGreeting();
        this.buildQuickLinks();
        this.loadSchoolInfo();
    }

    setGreeting() {
        let hour = new Date().getHours();
        if (hour < 12) this.greeting = 'Good morning';
        else if (hour < 17) this.greeting = 'Good afternoon';
        else this.greeting = 'Good evening';
    }

    buildQuickLinks() {
        if (!this.user) return;
        this.quickLinks = [];

        let src = {source: 'dashboard'};
        if (this.user.currentUserAdministrator || this.user.currentUserHeadTeacher) {
            this.quickLinks.push(
                {label: 'Students', icon: 'fas fa-user-graduate', link: '/students/details', color: 'bg-info', queryParams: src},
                {label: 'Staff', icon: 'fas fa-user-tie', link: '/staff/details', color: 'bg-warning', queryParams: src},
                {label: 'Subjects', icon: 'fas fa-book', link: '/academics/subjects', color: 'bg-success', queryParams: src},
                {label: 'Exams', icon: 'fas fa-file-alt', link: '/cbe/exams/exams', color: 'bg-danger', queryParams: src},
                {label: 'Reports', icon: 'fas fa-chart-bar', link: '/reports/academics', color: 'bg-purple', queryParams: src},
                {label: 'Settings', icon: 'fas fa-cog', link: '/settings/dropdowns', color: 'bg-secondary', queryParams: {}}
            );
        } else if (this.user.currentUserTeacher) {
            this.quickLinks.push(
                {label: 'Students', icon: 'fas fa-user-graduate', link: '/students/details', color: 'bg-info', queryParams: src},
                {label: 'Exams', icon: 'fas fa-file-alt', link: '/cbe/exams/exams', color: 'bg-danger', queryParams: src},
                {label: 'Assessments', icon: 'fas fa-clipboard-check', link: '/cbe/assessments/assessments', color: 'bg-success', queryParams: src},
                {label: 'Reports', icon: 'fas fa-chart-bar', link: '/reports/academics', color: 'bg-purple', queryParams: src}
            );
        } else if (this.user.currentUserParent || this.user.currentUserStudent) {
            this.quickLinks.push(
                {label: 'Results', icon: 'fas fa-file-alt', link: '/reports/academics', color: 'bg-info', queryParams: src},
                {label: 'Reports', icon: 'fas fa-chart-bar', link: '/reports/academics', color: 'bg-success', queryParams: src}
            );
        }
    }

    loadSchoolInfo() {
        let schoolReq = this.schoolDetailsSvc.get('/schoolDetails');
        let acadYearReq = this.academicYearSvc.get('/academicYears');

        forkJoin([schoolReq, acadYearReq]).subscribe({
            next: ([schools, academicYears]) => {
                if (schools && schools.length > 0) {
                    let school = schools[0];
                    this.schoolName = school.name || '';
                    this.schoolMotto = school.motto || '';
                    this.schoolLogo = school.logoAsBase64 || '';
                }
                if (academicYears && academicYears.length > 0) {
                    let active = academicYears.find((y) => y.status === true);
                    this.activeAcademicYear = active ? active.name : academicYears[0]?.name || '';
                }
            },
            error: (err) => {}
        });
    }
}
