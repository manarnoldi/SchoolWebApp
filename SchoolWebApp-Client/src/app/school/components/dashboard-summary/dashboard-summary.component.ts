import {Status} from '@/core/enums/status';
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

    status = Status;

    constructor(
        private studentsSvc: StudentDetailsService,
        private toastr: ToastrService,
        private parentsSvc: ParentsService,
        private staffsSvc: StaffDetailsService,
        private StaffCategoriesSvc: StaffCategoriesService
    ) {}

    onStaffLinkClick(event: MouseEvent, teaching: boolean): void {
        if (teaching) {
            if (this.teachingStaffCategoryId > 0) {
                event.preventDefault();
            }
        } else {
            if (this.nonTeachingStaffCategoryId > 0) {
                event.preventDefault();
            }
        }
    }

    ngOnInit(): void {
        this.StaffCategoriesSvc.get('/staffCategories').subscribe({
            next: (staffCats) => {
                this.teachingStaffCategoryId = staffCats.find(
                    (s) => s.forTeaching == true
                )
                    ? parseInt(staffCats.find((s) => s.forTeaching).id)
                    : 0;
                this.nonTeachingStaffCategoryId = staffCats.find(
                    (s) => s.forTeaching == false
                )
                    ? parseInt(staffCats.find((s) => !s.forTeaching).id)
                    : 0;

                let requests = [];
                requests.push(
                    this.studentsSvc.getCount('/students/GetCount?active=true')
                );

                requests.push(
                    this.studentsSvc.getCount('/parents/GetCount?active=true')
                );
                requests.push(
                    this.staffsSvc.getCount(
                        '/staffDetails/GetCount?active=true&staffCategoryId=' +
                            this.teachingStaffCategoryId
                    )
                );
                requests.push(
                    this.staffsSvc.getCount(
                        '/staffDetails/GetCount?active=true&staffCategoryId=' +
                            this.nonTeachingStaffCategoryId
                    )
                );

                forkJoin(...requests).subscribe(
                    (res) => {
                        this.studentCount = Number(res[0]);
                        this.parentsCount = Number(res[1]);
                        this.teachingStaffCount = Number(res[2]);
                        this.nonTeachingStaffCount = Number(res[3]);
                    },
                    (err) => {
                        console.error(err);
                        this.toastr.error(err.error);
                    }
                );
            },
            error: (err) => {
                this.toastr.error(err.error?.message);
            }
        });
    }
}
