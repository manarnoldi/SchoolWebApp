import {Status} from '@/core/enums/status';
import {HttpClient} from '@angular/common/http';
import {Component, OnInit} from '@angular/core';

interface Split {
    total: number;
    male: number;
    female: number;
}

interface Summary {
    students: Split;
    parents: Split;
    studentsWithoutParent: number;
    teachingStaff: Split;
    nonTeachingStaff: Split;
    teachingCategoryId: number | null;
    nonTeachingCategoryId: number | null;
}

/**
 * The four summary cards: active learners, parents and staff, each split by gender,
 * from one request (/dashboard/summary). Each card opens its list.
 */
@Component({
    selector: 'app-dashboard-summary',
    templateUrl: './dashboard-summary.component.html',
    styleUrl: './dashboard-summary.component.scss'
})
export class DashboardSummaryComponent implements OnInit {
    summary: Summary | null = null;
    status = Status;

    constructor(private http: HttpClient) {}

    ngOnInit(): void {
        this.http.get<Summary>('/dashboard/summary').subscribe({
            next: (s) => (this.summary = s),
            error: () => (this.summary = null)
        });
    }
}
