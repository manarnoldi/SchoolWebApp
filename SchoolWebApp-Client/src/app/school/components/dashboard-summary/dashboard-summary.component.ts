import {StaffCategoriesService} from '@/settings/services/staff-categories.service';
import {StaffDetailsService} from '@/staff/services/staff-details.service';
import {ParentsService} from '@/students/services/parents.service';
import {StudentDetailsService} from '@/students/services/student-details.service';
import {Component, OnInit} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';

@Component({
    selector: 'app-dashboard-summary',
    templateUrl: './dashboard-summary.component.html',
    styleUrl: './dashboard-summary.component.scss'
})
export class DashboardSummaryComponent implements OnInit {
    studentCount: number = 0;
    parentsCount: number = 0;
    teachingStaffCount: number = 0;
    nonTeachingStaffCount: number = 0;
    teachingStaffCategoryId: number = 0;
    nonTeachingStaffCategoryId: number = 0;

    constructor(
        private studentsSvc: StudentDetailsService,
        private toastr: ToastrService,
        private parentsSvc: ParentsService,
        private staffsSvc: StaffDetailsService,
        private StaffCaegoriesSvc: StaffCategoriesService
    ) {}

    ngOnInit(): void {
        let studentsCountReq = this.studentsSvc.getCount(
            '/students/GetCount?active=true'
        );
        let parentsCountReq = this.studentsSvc.getCount(
            '/parents/GetCount?active=true'
        );

        forkJoin([studentsCountReq, parentsCountReq]).subscribe({
            next: ([studentsCount, parentsCount]) => {
                this.studentCount = studentsCount;
                this.parentsCount = parentsCount;
            },
            error: (err) => {
                this.toastr.error(err.error?.message);
            }
        });
    }
}
