import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {apiError, ElectionStage, ElectionSummary, STAGE_BADGES, STAGE_LABELS} from '@/elections/models/election-models';
import {ElectionsService} from '@/elections/services/elections.service';

/** Elections that have polled: open one's live or declared results. */
@Component({
    selector: 'app-results-home',
    template: `
        <app-dashboard-header [title]="'Elections: Results'" [breadcrumbs]="breadcrumbs"></app-dashboard-header>
        <hr class="mb-1">
        <section class="content">
            <div class="container-fluid px-1">
                <div *ngIf="loading" class="text-center text-muted py-4"><i class="fas fa-spinner fa-spin"></i> Loading...</div>
                <div *ngIf="!loading && rows.length === 0" class="card"><div class="card-body text-center text-muted py-4">
                    No election has opened polling yet.</div></div>
                <div class="row g-2">
                    <div class="col-xl-3 col-lg-4 col-md-6" *ngFor="let e of rows">
                        <div class="card h-100 mb-0 card-outline" [ngClass]="e.stage === Stage.Declared ? 'card-success' : 'card-info'">
                            <div class="card-body p-3 d-flex flex-column">
                                <h5 class="mb-1"><i class="fas fa-poll text-info me-1"></i>{{ e.name }}</h5>
                                <div class="small mb-3"><span class="badge" [ngClass]="stageBadges[e.stage]">{{ stageLabels[e.stage] }}</span>
                                    <span class="text-muted ms-1">{{ e.academicYearName }} - {{ e.contests }} races</span></div>
                                <button class="btn btn-primary mt-auto" (click)="router.navigate(['/elections/results', e.id])">
                                    <i class="fas fa-chart-bar"></i> {{ e.stage === Stage.Declared ? 'Results' : 'Live results' }}</button>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </section>`
})
export class ResultsHomeComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/elections/results'], title: 'Results'}
    ];
    rows: ElectionSummary[] = [];
    loading = false;
    stageLabels = STAGE_LABELS;
    stageBadges = STAGE_BADGES;
    Stage = ElectionStage;

    constructor(private svc: ElectionsService, private toastr: ToastrService, public router: Router) {}

    ngOnInit(): void {
        this.loading = true;
        this.svc.list().subscribe({
            next: (r) => {
                this.loading = false;
                this.rows = r.filter((e) => e.stage >= ElectionStage.Polling);
            },
            error: (err) => {
                this.loading = false;
                this.toastr.error(apiError(err, 'Error loading the elections.'));
            }
        });
    }
}
