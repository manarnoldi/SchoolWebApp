import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import Swal from 'sweetalert2';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {STATUS_BADGES, STATUS_LABELS, TimetableDetail, TimetableStatus} from '@/timetable/models/timetable-models';
import {TimetablesService} from '@/timetable/services/timetables.service';

/**
 * One timetable: generate it (locked cards stay), read the generator's report,
 * look at it by class, teacher or the master view, print it, and publish it.
 */
@Component({
    selector: 'app-timetable-detail',
    templateUrl: './timetable-detail.component.html',
    styleUrl: '../timetable.scss'
})
export class TimetableDetailComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/timetable/timetables'], title: 'Timetables'}
    ];
    dashboardTitle = 'Timetable';

    t: TimetableDetail | null = null;
    generating = false;
    showWarnings = false;
    statusLabels = STATUS_LABELS;
    statusBadges = STATUS_BADGES;
    Status = TimetableStatus;

    constructor(private svc: TimetablesService, private route: ActivatedRoute, private router: Router,
                private toastr: ToastrService) {}

    ngOnInit(): void {
        this.route.paramMap.subscribe((p) => this.load(+(p.get('id') || 0)));
    }

    load(id: number) {
        this.svc.get(id).subscribe({
            next: (t) => this.show(t),
            error: (err) => this.toastr.error(err.error?.message || 'Error loading the timetable.')
        });
    }

    private show(t: TimetableDetail) {
        this.t = t;
        this.dashboardTitle = `Timetable: ${t.name}`;
        this.breadcrumbs = [
            {link: ['/'], title: 'Dashboard'},
            {link: ['/timetable/timetables'], title: 'Timetables'},
            {link: ['/timetable/timetables', String(t.id)], title: t.name}
        ];
    }

    get placed(): number {
        return this.t ? this.t.cards.filter((c) => c.day).length : 0;
    }

    get hasCards(): boolean {
        return !!this.t?.cards.length;
    }

    generate() {
        let t = this.t!;
        let run = () => {
            this.generating = true;
            this.svc.generate(t.id, 20).subscribe({
                next: (r) => {
                    this.generating = false;
                    this.show(r);
                    let left = r.report?.unplaced.length ?? 0;
                    if (left) this.toastr.warning(`Generated. ${left} lesson(s) could not be placed - see the report.`);
                    else this.toastr.success('Generated. Every lesson is placed.');
                },
                error: (err) => {
                    this.generating = false;
                    this.toastr.error(err.error?.message || 'Error generating the timetable.');
                }
            });
        };
        if (!this.hasCards) return run();
        let locked = t.cards.filter((c) => c.isLocked).length;
        Swal.fire({title: 'Generate again?', width: 460, position: 'top', icon: 'question',
            text: `The week is laid out afresh${locked ? `; the ${locked} locked lesson(s) stay where they are` : ''}.`,
            showCancelButton: true, confirmButtonText: 'Generate'}).then((r) => {
            if (r.value) run();
        });
    }

    rename() {
        let t = this.t!;
        Swal.fire({title: 'Timetable name', input: 'text', inputValue: t.name, width: 460, position: 'top',
            showCancelButton: true, confirmButtonText: 'Save'}).then((r) => {
            if (!r.value || !r.value.trim()) return;
            this.svc.update(t.id, r.value.trim(), t.notes).subscribe({
                next: () => this.load(t.id),
                error: (err) => this.toastr.error(err.error?.message || 'Error renaming.')
            });
        });
    }

    publish() {
        let t = this.t!;
        let unplaced = t.cards.filter((c) => !c.day).length;
        let today = new Date().toISOString().substring(0, 10);
        Swal.fire({title: 'Publish this timetable?', width: 480, position: 'top', icon: 'question',
            html: `Teachers will see it under View Timetables and My Timetable.` +
                (unplaced ? `<br><b class="text-danger">${unplaced} lesson(s) are not placed.</b>` : '') +
                `<br><br>In use from:`,
            input: 'date', inputValue: today, showCancelButton: true, confirmButtonText: 'Publish'}).then((r) => {
            if (!r.value) return;
            this.svc.publish(t.id, r.value).subscribe({
                next: () => {
                    this.toastr.success('Timetable published.');
                    this.load(t.id);
                },
                error: (err) => this.toastr.error(err.error?.message || 'Error publishing.')
            });
        });
    }

    copy() {
        this.svc.copy(this.t!.id).subscribe({
            next: (r) => {
                this.toastr.success('A draft copy was made.');
                this.router.navigate(['/timetable/timetables', r.id]);
            },
            error: (err) => this.toastr.error(err.error?.message || 'Error copying.')
        });
    }
}
