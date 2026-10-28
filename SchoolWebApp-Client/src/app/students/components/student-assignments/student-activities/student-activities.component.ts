import {Component, Input, OnInit} from '@angular/core';
import {ProfilesService, StudentActivities} from '@/core/services/profiles.service';
import {LicenceService} from '@/core/services/licence.service';

/**
 * Student page, Leadership & Activities tab: class leadership roles,
 * responsibilities and co-curricular activities (Academics), and the
 * elections they stood in (Elections).
 */
@Component({
    selector: 'app-student-activities',
    template: `
        <div *ngIf="loading" class="text-muted p-2"><i class="fas fa-spinner fa-spin"></i> Loading...</div>
        <div class="row g-2" *ngIf="a">
            <div class="col-lg-6">
                <div class="card mb-0 h-100">
                    <div class="card-header py-1"><b><i class="fas fa-user-shield text-primary me-1"></i>Class leadership</b></div>
                    <ul class="list-group list-group-flush small">
                        <li class="list-group-item text-muted" *ngIf="a.classRoles.length === 0">No class leadership role.</li>
                        <li class="list-group-item d-flex" *ngFor="let r of a.classRoles">
                            <span class="me-auto"><b>{{ r.role }}</b> - {{ r.className }}
                                <span class="text-muted" *ngIf="r.description"> ({{ r.description }})</span></span>
                            <span class="text-muted">{{ r.year }}</span></li>
                    </ul>
                </div>
            </div>
            <div class="col-lg-6" *ngIf="licence.has('Elections')">
                <div class="card mb-0 h-100">
                    <div class="card-header py-1"><b><i class="fas fa-vote-yea text-info me-1"></i>Elections</b></div>
                    <ul class="list-group list-group-flush small">
                        <li class="list-group-item text-muted" *ngIf="a.elections.length === 0">Has not stood in an election.</li>
                        <li class="list-group-item" *ngFor="let e of a.elections">
                            <div class="d-flex">
                                <span class="me-auto"><b>{{ e.position }}</b><span class="text-muted ms-1" *ngIf="e.race !== e.position">- {{ e.race }}</span></span>
                                <span class="badge" [ngClass]="e.elected ? 'bg-success' : 'bg-light text-dark border'">
                                    <i class="fas fa-crown me-1" *ngIf="e.elected"></i>{{ e.outcome }}</span>
                            </div>
                            <div class="text-muted">{{ e.election }}<span *ngIf="e.votes !== null"> - {{ e.votes }} votes</span>
                                <span *ngIf="e.partner"> - {{ e.asRunningMate ? 'running mate of' : 'with' }} {{ e.partner }}</span></div>
                        </li>
                    </ul>
                </div>
            </div>
            <div class="col-lg-6" *ngIf="licence.has('Academics')">
                <div class="card mb-0 h-100">
                    <div class="card-header py-1"><b><i class="fas fa-hands-helping text-success me-1"></i>Responsibilities</b></div>
                    <ul class="list-group list-group-flush small">
                        <li class="list-group-item text-muted" *ngIf="a.responsibilities.length === 0">No responsibility recorded.</li>
                        <li class="list-group-item d-flex" *ngFor="let r of a.responsibilities">
                            <span class="me-auto"><b>{{ r.name }}</b><span class="text-muted" *ngIf="r.description"> - {{ r.description }}</span></span>
                            <span class="text-muted">{{ r.year }}</span></li>
                    </ul>
                </div>
            </div>
            <div class="col-lg-6" *ngIf="licence.has('Academics')">
                <div class="card mb-0 h-100">
                    <div class="card-header py-1"><b><i class="fas fa-futbol text-warning me-1"></i>Co-curricular activities</b></div>
                    <ul class="list-group list-group-flush small">
                        <li class="list-group-item text-muted" *ngIf="a.coCurricular.length === 0">No co-curricular activity recorded.</li>
                        <li class="list-group-item" *ngFor="let c of a.coCurricular"><b>{{ c.name }}</b>
                            <span class="text-muted" *ngIf="c.description"> - {{ c.description }}</span></li>
                    </ul>
                </div>
            </div>
        </div>`
})
export class StudentActivitiesComponent implements OnInit {
    @Input() studentId!: number;
    a: StudentActivities | null = null;
    loading = false;

    constructor(private svc: ProfilesService, public licence: LicenceService) {}

    ngOnInit(): void {
        this.loading = true;
        this.svc.studentActivities(this.studentId).subscribe({
            next: (a) => {
                this.loading = false;
                this.a = a;
            },
            error: () => (this.loading = false)
        });
    }
}
