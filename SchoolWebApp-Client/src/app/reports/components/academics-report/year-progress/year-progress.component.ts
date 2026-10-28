import {Component, OnDestroy, OnInit} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import {ProfileExam} from '@/core/services/profiles.service';
import {printInFrame, removePrintFrame} from '@/shared/utils/print-frame';
import {PROGRESS_PRINT_CSS, progressReportPage} from '@/shared/utils/progress-report';

interface Learner {
    studentId: number;
    name: string;
    upi: string;
    exams: ProfileExam[];
}

/**
 * Exam Results > Year Progress: a class's learners through a year, one at a
 * time as on the student's own page (chart against the class mean, grades, each
 * exam's marks), and printed - one learner, or the whole class a page each.
 */
@Component({
    selector: 'app-year-progress',
    template: `
        <div class="card card-outline card-primary mb-1">
            <div class="card-body p-2">
                <div class="row g-2 align-items-end">
                    <div class="col-md-3">
                        <label class="form-label mb-0 small fw-bold">Year</label>
                        <select class="form-select form-select-sm" [(ngModel)]="yearId" (ngModelChange)="loadClasses()">
                            <option *ngFor="let y of years" [ngValue]="y.id">{{ y.name }}</option>
                        </select>
                    </div>
                    <div class="col-md-4">
                        <label class="form-label mb-0 small fw-bold">Class</label>
                        <select class="form-select form-select-sm" [(ngModel)]="classId" (ngModelChange)="learners = []; learner = null">
                            <option [ngValue]="null">- choose -</option>
                            <option *ngFor="let c of classes" [ngValue]="c.id">{{ c.name }}</option>
                        </select>
                    </div>
                    <div class="col-md-2">
                        <button class="btn btn-sm btn-primary" [disabled]="!classId || loading" (click)="load()">
                            <i class="fas" [ngClass]="loading ? 'fa-spinner fa-spin' : 'fa-search'"></i> Load</button>
                    </div>
                </div>
                <small class="text-muted d-block mt-1"><i class="fas fa-info-circle"></i> Every exam of the year, each learner's mean against
                    the class mean, with grades. Print one learner, or the whole class a page each.</small>
            </div>
        </div>

        <div class="card card-outline card-primary mb-1" *ngIf="learners.length">
            <div class="card-body p-2 d-flex flex-wrap align-items-end gap-2">
                <div style="min-width: 320px;">
                    <label class="form-label mb-0 small fw-bold">Learner</label>
                    <select class="form-select form-select-sm" [(ngModel)]="learner">
                        <option *ngFor="let l of learners" [ngValue]="l">{{ l.upi }} - {{ l.name }}{{ l.exams.length ? '' : ' (no results)' }}</option>
                    </select>
                </div>
                <button class="btn btn-sm btn-outline-primary ms-auto" [disabled]="!learner" (click)="print([learner!])">
                    <i class="fas fa-print"></i> Print learner</button>
                <button class="btn btn-sm btn-primary" (click)="print(learners)">
                    <i class="fas fa-print"></i> Print all {{ learners.length }} learners</button>
            </div>
        </div>

        <div class="card mb-1" *ngIf="learner">
            <div class="card-header py-2"><b>{{ learner.name }}</b> <span class="text-muted ms-1">{{ learner.upi }} - {{ className }}</span></div>
            <div class="card-body p-2">
                <app-student-academics [studentId]="learner.studentId" [studentName]="learner.name" [preloaded]="learner.exams"></app-student-academics>
            </div>
        </div>`
})
export class YearProgressComponent implements OnInit, OnDestroy {
    years: any[] = [];
    classes: any[] = [];
    yearId: number | null = null;
    classId: number | null = null;
    learners: Learner[] = [];
    learner: Learner | null = null;
    loading = false;
    private schoolName = '';

    constructor(private http: HttpClient, private toastr: ToastrService) {}

    ngOnInit(): void {
        forkJoin([this.http.get<any[]>('/academicYears'), this.http.get<any[]>('/schoolDetails')]).subscribe({
            next: ([years, school]) => {
                this.years = (years || []).sort((a, b) => (b.rank || 0) - (a.rank || 0));
                this.schoolName = (school || [])[0]?.name || '';
                // The current year first.
                this.yearId = (this.years.find((y) => y.status === true) || this.years[0])?.id ?? null;
                this.loadClasses();
            },
            error: () => this.toastr.error('Error loading the years.')
        });
    }

    ngOnDestroy(): void {
        removePrintFrame();
    }

    get className(): string {
        return this.classes.find((c) => c.id === this.classId)?.name || '';
    }

    get yearName(): string {
        return this.years.find((y) => y.id === this.yearId)?.name || '';
    }

    loadClasses() {
        this.classId = null;
        this.classes = [];
        this.learners = [];
        this.learner = null;
        if (!this.yearId) return;
        this.http.get<any[]>(`/schoolClasses/byAcademicYearId/${this.yearId}`).subscribe({
            next: (c) => (this.classes = (c || []).sort((a, b) => (a.rank || 0) - (b.rank || 0) || (a.name || '').localeCompare(b.name || ''))),
            error: () => this.toastr.error('Error loading the classes.')
        });
    }

    load() {
        if (!this.classId) return;
        this.loading = true;
        this.http.get<Learner[]>(`/profiles/classes/${this.classId}/academics`).subscribe({
            next: (l) => {
                this.loading = false;
                this.learners = l;
                this.learner = l.find((x) => x.exams.length) || l[0] || null;
                if (!l.length) this.toastr.info('No learners in this class.');
            },
            error: (err) => {
                this.loading = false;
                this.toastr.error(typeof err?.error === 'string' ? err.error : 'Error loading the results.');
            }
        });
    }

    // One learner, or the whole class: a page each, the same report as on a student's page.
    print(list: Learner[]) {
        const pages = list.map((l) => progressReportPage({
            school: this.schoolName, name: l.name, className: this.className, yearName: this.yearName,
            exams: l.exams.filter((e) => e.academicYearId === this.yearId)
        })).join('');
        printInFrame(list.length === 1 ? `Progress report - ${list[0].name}` : `Progress reports - ${this.className}`, PROGRESS_PRINT_CSS, pages);
    }
}
