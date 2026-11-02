import {BreadCrumb} from '@/core/models/bread-crumb';
import {AuthService} from '@/core/services/auth.service';
import {Component, OnInit} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import {SchoolDetailsService} from '../../services/school-details.service';
import {AcademicYearsService} from '../../services/academic-years.service';
import {LicenceService} from '@/core/services/licence.service';
import {AccessService} from '@/core/services/access.service';
import {cardPath} from '@/core/models/dashboard-cards';
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
    greetName: string = '';
    termLine: string = '';
    termTitle: string = '';

    quickLinks: any[] = [];

    constructor(
        private authService: AuthService,
        private schoolDetailsSvc: SchoolDetailsService,
        private academicYearSvc: AcademicYearsService,
        private toastr: ToastrService,
        private http: HttpClient,
        public licence: LicenceService,
        private access: AccessService
    ) {}

    // Class exam performance names top students: for roles given the card.
    showExamSummary = false;

    ngOnInit(): void {
        this.user = this.authService.getCurrentUser();
        this.licence.load().subscribe();
        this.access.canAny(cardPath('examPerformance'))
            .subscribe((ok) => (this.showExamSummary = ok));
        this.currentDate = DateTime.now().toFormat('EEEE, dd MMMM yyyy');
        this.setGreeting();
        this.buildQuickLinks();
        this.loadSchoolInfo();
        this.loadTerm();
    }

    setGreeting() {
        let hour = new Date().getHours();
        if (hour < 12) this.greeting = 'Good morning';
        else if (hour < 17) this.greeting = 'Good afternoon';
        else this.greeting = 'Good evening';
        this.greetName = DashboardComponent.addressAs(this.user);
    }

    // How staff address each other: "Madam Haji", "Mr. Arunga"; without a title, the
    // first name ("Fidel"). A login with no staff record keeps its last name ("Administrator").
    static addressAs(user: any): string {
        if (!user) return 'User';
        const proper = (s: string) => s.toLowerCase().replace(/(^|[\s'-])\S/g, (c) => c.toUpperCase());
        const titles: Record<string, string> = {md: 'Madam', madam: 'Madam', mr: 'Mr.', mrs: 'Mrs.', ms: 'Ms.', miss: 'Miss',
            dr: 'Dr.', tr: 'Tr.', prof: 'Prof.', rev: 'Rev.'};
        const words = String(user.firstName || '').trim().split(/\s+/).filter(Boolean);
        const last = String(user.lastName || '').trim();
        const title = words.length ? titles[words[0].toLowerCase().replace(/\.$/, '')] : undefined;
        if (title && last) return `${title} ${proper(last)}`;
        if (user.staffId == null && last) return proper(last);
        const given = title ? words[1] : words[0];
        return given ? proper(given) : last ? proper(last) : 'User';
    }

    // Where the school is in its calendar: "Term 3 · week 10 of 13 · 12 days left",
    // or between terms "Holiday · Term 1 2027 opens Mon 4 Jan".
    loadTerm() {
        this.http.get<any>('/dashboard/term').subscribe({
            next: (t) => {
                const day = (d: string) => DateTime.fromISO(d, {zone: 'utc'}).toFormat('ccc d LLL');
                if (t.inSession) {
                    const left = t.daysLeft === 0 ? 'ends today' : t.daysLeft === 1 ? '1 day left' : `${t.daysLeft} days left`;
                    this.termLine = `${t.term} · week ${Math.min(t.week, t.weeks)} of ${t.weeks} · ${left}`;
                    this.termTitle = `${t.term} ${t.year}: ${day(t.startDate)} to ${day(t.endDate)}`;
                } else if (t.term) {
                    this.termLine = `Holiday · ${t.term} ${t.year} opens ${day(t.startDate)}`;
                    this.termTitle = t.lastTerm ? `${t.lastTerm} ${t.lastYear} closed ${day(t.lastEnded)}` : '';
                } else if (t.lastTerm) {
                    this.termLine = `Holiday · ${t.lastTerm} ${t.lastYear} closed ${day(t.lastEnded)}`;
                }
            },
            error: () => {}
        });
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
