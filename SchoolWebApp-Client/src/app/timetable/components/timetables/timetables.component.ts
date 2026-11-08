import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import Swal from 'sweetalert2';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {STATUS_BADGES, STATUS_LABELS, TimetableLookups, TimetableSummary} from '@/timetable/models/timetable-models';
import {TimetableSetupService} from '@/timetable/services/timetable-setup.service';
import {TimetablesService} from '@/timetable/services/timetables.service';

/** A term's timetables: drafts being built, the one published, and earlier ones. */
@Component({
    selector: 'app-timetables',
    templateUrl: './timetables.component.html',
    styleUrl: '../timetable.scss'
})
export class TimetablesComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/timetable/timetables'], title: 'Timetables'}
    ];
    dashboardTitle = 'Timetable: Timetables';

    lookups: TimetableLookups | null = null;
    sessionId: number | null = null;
    rows: TimetableSummary[] = [];
    loading = false;
    newName = '';
    creating = false;
    statusLabels = STATUS_LABELS;
    statusBadges = STATUS_BADGES;

    constructor(private svc: TimetablesService, private setup: TimetableSetupService, private router: Router,
                private route: ActivatedRoute, private toastr: ToastrService) {}

    ngOnInit(): void {
        this.setup.lookups().subscribe({
            next: (l) => {
                this.lookups = l;
                let q = +(this.route.snapshot.queryParamMap.get('term') || 0);
                this.sessionId = l.sessions.some((s) => s.id === q) ? q : l.currentSessionId;
                this.load();
            },
            error: (err) => this.toastr.error(err.error?.message || 'Error loading terms.')
        });
    }

    get sessionName(): string {
        return this.lookups?.sessions.find((s) => s.id === this.sessionId)?.name ?? '';
    }

    load() {
        this.router.navigate([], {relativeTo: this.route, queryParams: {term: this.sessionId}, replaceUrl: true});
        this.loading = true;
        this.svc.list(this.sessionId).subscribe({
            next: (r) => {
                this.rows = r;
                this.loading = false;
                if (!this.newName) this.newName = `${this.sessionName} timetable`;
            },
            error: (err) => {
                this.loading = false;
                this.toastr.error(err.error?.message || 'Error loading timetables.');
            }
        });
    }

    create() {
        if (!this.sessionId || !this.newName.trim()) return;
        this.creating = true;
        this.svc.create(this.newName.trim(), this.sessionId).subscribe({
            next: (r) => {
                this.creating = false;
                this.router.navigate(['/timetable/timetables', r.id]);
            },
            error: (err) => {
                this.creating = false;
                this.toastr.error(err.error?.message || 'Error creating the timetable.');
            }
        });
    }

    copy(t: TimetableSummary) {
        this.svc.copy(t.id).subscribe({
            next: (r) => this.router.navigate(['/timetable/timetables', r.id]),
            error: (err) => this.toastr.error(err.error?.message || 'Error copying the timetable.')
        });
    }

    delete(t: TimetableSummary) {
        Swal.fire({title: `Delete ${t.name}?`, width: 420, position: 'top', icon: 'warning',
            showCancelButton: true, confirmButtonText: 'Delete', confirmButtonColor: '#d33'}).then((r) => {
            if (!r.value) return;
            this.svc.delete(t.id).subscribe({
                next: () => {
                    this.toastr.success('Timetable deleted.');
                    this.load();
                },
                error: (err) => this.toastr.error(err.error?.message || 'Error deleting the timetable.')
            });
        });
    }
}
