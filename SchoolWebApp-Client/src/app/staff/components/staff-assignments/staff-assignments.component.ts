import { Subject } from '@/academics/models/subject';
import {SubjectsService} from '@/academics/services/subjects.service';
import {Status} from '@/core/enums/status';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {OccurenceType} from '@/settings/models/occurence-type';
import {Outcome} from '@/settings/models/outcome';
import {OccurenceTypeService} from '@/settings/services/occurence-type.service';
import {OutcomesService} from '@/settings/services/outcomes.service';
import {StaffDetails} from '@/staff/models/staff-details';
import {StaffDetailsService} from '@/staff/services/staff-details.service';
import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {Location} from '@angular/common';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import {LicenceService} from '@/core/services/licence.service';
import {AccessService} from '@/core/services/access.service';

@Component({
    selector: 'app-staff-assignments',
    templateUrl: './staff-assignments.component.html',
    styleUrl: './staff-assignments.component.scss'
})
export class StaffAssignmentsComponent implements OnInit {
    staffId: number = 0;
    sourceLink: string = '';
    activeNav: string = 'attendance';
    // Tabs from other modules: shown when the school has the module and the user may see what they show.
    showTeaching = false;
    showPay = false;
    // Tabs opened so far; each loads the first time it is opened.
    opened = new Set<string>();
    staff: StaffDetails;
    outcomes: Outcome[] = [];
    occurenceTypes: OccurenceType[] = [];
    subjects: Subject[] = [];

    teachingStaff: boolean = false;

    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {
            link: ['/staff/' + this.sourceLink],
            title: 'Staff: ' + this.sourceLink
        }
    ];

    dashboardTitle = 'Staff ' + this.sourceLink;
    backLinkUrl: string;
    status = Status;
    statuses;

    constructor(
        private toastr: ToastrService,
        private staffsSvc: StaffDetailsService,
        private route: ActivatedRoute,
        private router: Router,
        private location: Location,
        private outcomesSvc: OutcomesService,
        private occurenceTypesSvc: OccurenceTypeService,
        private subjectsSvc: SubjectsService,
        private licence: LicenceService,
        private access: AccessService
    ) {
        this.statuses = Object.keys(this.status).filter((k) =>
            isNaN(Number(k))
        );
    }
    ngOnInit(): void {
        this.loadSelectedStaff();
        this.licence.load().subscribe(() => {
            this.showTeaching = this.licence.has('Academics');
            if (this.licence.has('Payroll')) {
                // Their own payslips, or anyone's for payroll officers (the API checks too).
                const me = JSON.parse(localStorage.getItem('current_user') || 'null');
                const self = me?.personId != null && +me.personId === +this.staffId;
                this.access.canAny('/payroll/periods').subscribe((ok) => (this.showPay = ok || self));
            }
        });
    }

    // Opening a tab records it in the address (?activeNav=), without reloading, so a
    // refresh or a shared link comes back to the same tab.
    selectTab(tab: string) {
        this.activeNav = tab;
        this.opened.add(tab);
        const url = this.router.createUrlTree([], {relativeTo: this.route, queryParams: {activeNav: tab}, queryParamsHandling: 'merge'});
        this.location.replaceState(url.toString());
    }

    loadSelectedStaff = () => {
        this.route.queryParams.subscribe((params) => {
            this.staffId = params['id'];
            this.sourceLink = params['action'];
            this.activeNav = params['activeNav'] || 'attendance';
            this.opened.add(this.activeNav);
            this.backLinkUrl = '/staff/' + this.sourceLink;
            let staffByIdReq = this.staffsSvc.getById(
                this.staffId,
                '/staffDetails'
            );
            let outcomesReq = this.outcomesSvc.get('/outcomes');
            let occurenceTypesReq =
                this.occurenceTypesSvc.get('/occurenceTypes');
            let subjectsReq =
                this.subjectsSvc.get('/subjects');

            forkJoin([staffByIdReq, outcomesReq, occurenceTypesReq,subjectsReq]).subscribe(
                ([staff, outcomes, occurenceTypes, subjects]) => {
                    this.staff = staff;
                    this.teachingStaff = this.staff.staffCategory?.forTeaching;
                    this.outcomes = outcomes;
                    this.occurenceTypes = occurenceTypes;
                    this.subjects = subjects;
                },
                (err) => {
                    this.toastr.error(err.error);
                }
            );
        });
    };
}
