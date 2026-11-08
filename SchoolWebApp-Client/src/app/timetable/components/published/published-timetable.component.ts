import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {TimetableDetail} from '@/timetable/models/timetable-models';
import {TimetablesService} from '@/timetable/services/timetables.service';

/**
 * The published timetable, read-only: View Timetables (any class or teacher)
 * and My Timetable (the signed-in teacher's own week, route data mine: true).
 */
@Component({
    selector: 'app-published-timetable',
    templateUrl: './published-timetable.component.html',
    styleUrl: '../timetable.scss'
})
export class PublishedTimetableComponent implements OnInit {
    mine = false;
    breadcrumbs: BreadCrumb[] = [];
    dashboardTitle = '';

    timetable: TimetableDetail | null = null;
    sessions: {id: number; name: string}[] = [];
    sessionId: number | null = null;
    staffId: number | null = null;
    loading = true;

    constructor(private svc: TimetablesService, private route: ActivatedRoute, private toastr: ToastrService) {}

    ngOnInit(): void {
        this.mine = !!this.route.snapshot.data['mine'];
        let path = this.mine ? '/timetable/my' : '/timetable/view';
        this.dashboardTitle = this.mine ? 'Timetable: My Timetable' : 'Timetable: View Timetables';
        this.breadcrumbs = [{link: ['/'], title: 'Dashboard'}, {link: [path], title: this.mine ? 'My Timetable' : 'View Timetables'}];
        this.load();
    }

    load() {
        this.loading = true;
        let done = (r: {timetable: TimetableDetail | null; sessions: {id: number; name: string}[]}) => {
            this.timetable = r.timetable;
            this.sessions = r.sessions;
            this.sessionId = r.timetable?.sessionId ?? this.sessionId;
            this.loading = false;
        };
        let fail = (err: any) => {
            this.loading = false;
            this.toastr.error(err.error?.message || 'Error loading the timetable.');
        };
        if (this.mine) {
            this.svc.mine(this.sessionId).subscribe({next: (r) => {
                this.staffId = r.staffDetailsId;
                done(r);
            }, error: fail});
        } else {
            this.svc.published(this.sessionId).subscribe({next: done, error: fail});
        }
    }

    get myLessons(): number {
        return this.timetable?.cards.filter((c) => c.staffDetailsId === this.staffId && c.day).length ?? 0;
    }
}
