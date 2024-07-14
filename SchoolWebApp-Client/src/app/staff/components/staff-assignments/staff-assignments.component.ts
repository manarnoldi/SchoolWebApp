import {Status} from '@/core/enums/status';
import {BreadCrumb} from '@/core/models/bread-crumb';
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

    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {
            link: ['/staff/' + this.sourceLink],
            title: 'Staff: ' + this.sourceLink
        }
    ];

    dashboardTitle = 'Staff ' + this.sourceLink;
    backLinkUrl: string = '/staff/attendance';
    status = Status;
    statuses;

    constructor(
        private toastr: ToastrService,
        private staffsSvc: StaffDetailsService,
        private route: ActivatedRoute
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
          this.sourceLink = params['attendance'];

            let staffByIdReq = this.staffsSvc.getById(
                this.staffId,
                '/staffDetails'
            );

            forkJoin([staffByIdReq]).subscribe(
                ([staff]) => {
                    this.staff = staff;
                },
                (err) => {
                    this.toastr.error(err.error);
                }
            );
        });
    };
}
