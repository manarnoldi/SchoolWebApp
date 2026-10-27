import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {
    apiError,
    ElectionDetail,
    ElectionOptions,
    ElectionStage,
    ElectionSummary,
    STAGE_BADGES,
    STAGE_LABELS
} from '@/elections/models/election-models';
import {ElectionsService} from '@/elections/services/elections.service';

/** Every election, newest first; open one to set up its positions and move its stage. */
@Component({
    selector: 'app-election-list',
    templateUrl: './election-list.component.html'
})
export class ElectionListComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/elections/manage'], title: 'Manage Elections'}
    ];
    dashboardTitle = 'Elections: Manage Elections';

    options: ElectionOptions | null = null;
    rows: ElectionSummary[] = [];
    loading = false;
    search = '';
    stage: ElectionStage | null = null;
    page = 1;
    pageSize = 15;
    showForm = false;

    stages = [ElectionStage.Draft, ElectionStage.Nominations, ElectionStage.CandidatesFinal,
        ElectionStage.Polling, ElectionStage.PollingClosed, ElectionStage.Declared];
    stageLabels = STAGE_LABELS;
    stageBadges = STAGE_BADGES;

    constructor(private svc: ElectionsService, private toastr: ToastrService, private router: Router) {}

    ngOnInit(): void {
        this.svc.options().subscribe({
            next: (o) => (this.options = o),
            error: (err) => this.toastr.error(apiError(err, 'Error loading the election options.'))
        });
        this.load();
    }

    load() {
        this.loading = true;
        this.svc.list().subscribe({
            next: (r) => {
                this.rows = r;
                this.loading = false;
            },
            error: (err) => {
                this.loading = false;
                this.toastr.error(apiError(err, 'Error loading the elections.'));
            }
        });
    }

    get filtered(): ElectionSummary[] {
        const q = this.search.trim().toLowerCase();
        return this.rows.filter((r) =>
            (this.stage === null || r.stage === this.stage) &&
            (!q || r.name.toLowerCase().includes(q) || r.academicYearName.toLowerCase().includes(q)));
    }

    get paged(): ElectionSummary[] {
        return this.filtered.slice((this.page - 1) * this.pageSize, this.page * this.pageSize);
    }

    open(r: ElectionSummary) {
        this.router.navigate(['/elections/manage', r.id]);
    }

    created(d: ElectionDetail) {
        this.showForm = false;
        this.router.navigate(['/elections/manage', d.id]);
    }
}
