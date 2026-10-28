import {Component, Input, OnInit} from '@angular/core';
import {ProfilesService, StaffDuties} from '@/core/services/profiles.service';
import {LicenceService} from '@/core/services/licence.service';

/** Staff page, Duties tab: class roles held, and polling stations run as returning officer. */
@Component({
    selector: 'app-staff-duties',
    template: `
        <div *ngIf="loading" class="text-muted p-2"><i class="fas fa-spinner fa-spin"></i> Loading...</div>
        <div class="row g-2" *ngIf="d">
            <div class="col-lg-6">
                <div class="card mb-0 h-100">
                    <div class="card-header py-1"><b><i class="fas fa-chalkboard text-primary me-1"></i>Class roles</b></div>
                    <ul class="list-group list-group-flush small">
                        <li class="list-group-item text-muted" *ngIf="d.classRoles.length === 0">No class role.</li>
                        <li class="list-group-item d-flex" *ngFor="let r of d.classRoles">
                            <span class="me-auto"><b>{{ r.role }}</b> - {{ r.className }}
                                <span class="text-muted" *ngIf="r.description"> ({{ r.description }})</span></span>
                            <span class="text-muted">{{ r.year }}</span></li>
                    </ul>
                </div>
            </div>
            <div class="col-lg-6" *ngIf="licence.has('Elections')">
                <div class="card mb-0 h-100">
                    <div class="card-header py-1"><b><i class="fas fa-person-booth text-success me-1"></i>Returning officer</b></div>
                    <ul class="list-group list-group-flush small">
                        <li class="list-group-item text-muted" *ngIf="d.stations.length === 0">Has not run a polling station.</li>
                        <li class="list-group-item d-flex" *ngFor="let s of d.stations">
                            <span class="me-auto"><b>{{ s.station }}</b> - {{ s.election }}</span>
                            <span class="text-muted">{{ s.year }}</span></li>
                    </ul>
                </div>
            </div>
        </div>`
})
export class StaffDutiesComponent implements OnInit {
    @Input() staffId!: number;
    d: StaffDuties | null = null;
    loading = false;

    constructor(private svc: ProfilesService, public licence: LicenceService) {}

    ngOnInit(): void {
        this.loading = true;
        this.svc.staffDuties(+this.staffId).subscribe({
            next: (d) => {
                this.loading = false;
                this.d = d;
            },
            error: () => (this.loading = false)
        });
    }
}
