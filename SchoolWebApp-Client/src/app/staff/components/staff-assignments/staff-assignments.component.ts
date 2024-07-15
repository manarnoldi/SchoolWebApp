import {Status} from '@/core/enums/status';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {OccurenceType} from '@/settings/models/occurence-type';
import {Outcome} from '@/settings/models/outcome';
import {OccurenceTypeService} from '@/settings/services/occurence-type.service';
import {OutcomesService} from '@/settings/services/outcomes.service';
import {StaffDetails} from '@/staff/models/staff-details';
import {StaffDetailsService} from '@/staff/services/staff-details.service';
import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';

@Component({
    selector: 'app-staff-assignments',
    templateUrl: './staff-assignments.component.html',
    styleUrl: './staff-assignments.component.scss'
})
export class StaffAssignmentsComponent implements OnInit {
    staffId: number = 0;
    sourceLink: string = '';
    staff: StaffDetails;
    outcomes: Outcome[] = [];
    occurenceTypes: OccurenceType[] = [];

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
        private outcomesSvc: OutcomesService,
        private occurenceTypesSvc: OccurenceTypeService
    ) {
        this.statuses = Object.keys(this.status).filter((k) =>
            isNaN(Number(k))
        );
    }
    ngOnInit(): void {
        this.loadSelectedStaff();
    }

    loadSelectedStaff = () => {
        this.route.queryParams.subscribe((params) => {
            this.staffId = params['id'];
            this.sourceLink = params['action'];
            this.backLinkUrl = '/staff/' + this.sourceLink;
            let staffByIdReq = this.staffsSvc.getById(
                this.staffId,
                '/staffDetails'
            );
            let outcomesReq = this.outcomesSvc.get('/outcomes');
            let occurenceTypesReq =
                this.occurenceTypesSvc.get('/occurenceTypes');

            forkJoin([staffByIdReq, outcomesReq, occurenceTypesReq]).subscribe(
                ([staff, outcomes, occurenceTypes]) => {
                    this.staff = staff;
                    this.outcomes = outcomes;
                    this.occurenceTypes = occurenceTypes;
                },
                (err) => {
                    this.toastr.error(err.error);
                }
            );
        });
    };
}
