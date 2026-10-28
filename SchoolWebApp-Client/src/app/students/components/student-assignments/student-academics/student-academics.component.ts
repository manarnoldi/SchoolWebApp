import {Component, Input, OnInit} from '@angular/core';
import {ProfileExam, ProfilesService} from '@/core/services/profiles.service';

/** Student page, Academics tab: every exam sat, newest first, each subject's mark. */
@Component({
    selector: 'app-student-academics',
    styles: [`
        .exam-head { cursor: pointer; background: #e7f1ff; color: #084298; }
        .exam-head:hover { background: #dbe9ff; }
        .exam.open > .exam-head { background: #0d6efd; color: #fff; }
        .exam.open > .exam-head .text-muted { color: #e7f1ff !important; }
        .bar { height: 8px; background: #e9ecef; border-radius: 4px; overflow: hidden; min-width: 80px; }
        .bar > div { height: 100%; background: #0d6efd; }
    `],
    template: `
        <div *ngIf="loading" class="text-muted p-2"><i class="fas fa-spinner fa-spin"></i> Loading the exam results...</div>
        <div *ngIf="!loading && exams.length === 0" class="text-muted p-2">No exam results recorded for this student yet.</div>
        <div class="card mb-1 exam" *ngFor="let e of exams; let i = index" [class.open]="open === i" style="border-left: 4px solid #0d6efd;">
            <div class="card-header p-2 d-flex align-items-center exam-head" (click)="open = open === i ? -1 : i">
                <i class="fas fa-fw me-1" [ngClass]="open === i ? 'fa-chevron-down' : 'fa-chevron-right'"></i>
                <b class="me-2">{{ e.name }}</b>
                <small class="text-muted me-auto">{{ e.session }} {{ e.year }} - {{ e.className }}</small>
                <span class="badge bg-warning text-dark me-2" *ngIf="!e.released" title="Not yet released to parents">Provisional</span>
                <b>{{ e.mean }}%</b><small class="text-muted ms-1">mean</small>
            </div>
            <div class="card-body p-0" *ngIf="open === i">
                <table class="table table-sm mb-0 align-middle">
                    <thead class="table-light"><tr><th>Subject</th><th class="text-end">Mark</th><th class="text-end">Out of</th><th style="width: 40%;">%</th></tr></thead>
                    <tbody>
                        <tr *ngFor="let s of e.subjects">
                            <td>{{ s.subject }}</td>
                            <td class="text-end">{{ s.score }}</td>
                            <td class="text-end text-muted">{{ s.outOf }}</td>
                            <td><div class="d-flex align-items-center gap-2"><div class="bar flex-grow-1"><div [style.width.%]="s.percent"></div></div>
                                <span style="width: 3.5rem;" class="text-end">{{ s.percent }}%</span></div></td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>`
})
export class StudentAcademicsComponent implements OnInit {
    @Input() studentId!: number;
    exams: ProfileExam[] = [];
    loading = false;
    open = 0;

    constructor(private svc: ProfilesService) {}

    ngOnInit(): void {
        this.loading = true;
        this.svc.studentAcademics(this.studentId).subscribe({
            next: (e) => {
                this.loading = false;
                this.exams = e;
            },
            error: () => (this.loading = false)
        });
    }
}
