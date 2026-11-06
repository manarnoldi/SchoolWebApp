import {Component, Input, OnInit} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {Readiness, TimetableLookups} from '@/timetable/models/timetable-models';
import {TimetableSetupService} from '@/timetable/services/timetable-setup.service';

/**
 * Before generating: does every class fit its week, does every teacher fit
 * theirs, and is every subject taught by someone? Problems block generation;
 * warnings are worth a look but a timetable can still be made.
 */
@Component({
    selector: 'app-tt-readiness',
    templateUrl: './readiness.component.html',
    styleUrl: '../timetable.scss'
})
export class ReadinessComponent implements OnInit {
    @Input() lookups!: TimetableLookups;

    sessionId: number | null = null;
    result: Readiness | null = null;
    loading = false;
    openClass: number | null = null;

    constructor(private svc: TimetableSetupService, private toastr: ToastrService) {}

    ngOnInit(): void {
        this.sessionId = this.lookups.currentSessionId;
        this.check();
    }

    check() {
        if (!this.sessionId) return;
        this.loading = true;
        this.svc.readiness(this.sessionId).subscribe({
            next: (r) => {
                this.result = r;
                this.loading = false;
            },
            error: (err) => {
                this.loading = false;
                this.toastr.error(err.error?.message || 'Error running the readiness check.');
            }
        });
    }

    pct(used: number, of: number): number {
        return of > 0 ? Math.min(100, Math.round((used / of) * 100)) : 100;
    }

    barClass(used: number, of: number): string {
        if (used > of) return 'bg-danger';
        return used === of ? 'bg-warning' : 'bg-success';
    }
}
