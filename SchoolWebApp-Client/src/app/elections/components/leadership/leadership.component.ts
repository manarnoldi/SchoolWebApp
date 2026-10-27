import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import Swal from 'sweetalert2';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {apiError, ElectionStage, Leadership, LeadershipPosition} from '@/elections/models/election-models';
import {ElectionsService} from '@/elections/services/elections.service';

/**
 * After the results are declared: where each position's winners are recorded - a
 * class leadership role for class races, and/or a student responsibility for the
 * year - and recording them. Running it again only adds what is missing.
 */
@Component({
    selector: 'app-leadership',
    templateUrl: './leadership.component.html'
})
export class LeadershipComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/elections/manage'], title: 'Manage Elections'}
    ];
    dashboardTitle = 'Elections: Leadership records';

    data: Leadership | null = null;
    loading = false;
    busy = false;
    Stage = ElectionStage;
    private electionId = 0;

    constructor(private svc: ElectionsService, private toastr: ToastrService, private route: ActivatedRoute, public router: Router) {}

    ngOnInit(): void {
        this.route.paramMap.subscribe((p) => {
            this.electionId = Number(p.get('id'));
            this.loading = true;
            this.svc.leadership(this.electionId).subscribe({
                next: (d) => {
                    this.loading = false;
                    this.show(d);
                },
                error: (err) => {
                    this.loading = false;
                    this.toastr.error(apiError(err, 'Error loading the winners.'));
                }
            });
        });
    }

    show(d: Leadership) {
        this.data = d;
        this.breadcrumbs = [...this.breadcrumbs.slice(0, 2),
            {link: ['/elections/manage', String(d.electionId)], title: d.electionName},
            {link: ['/elections/manage', String(d.electionId), 'leadership'], title: 'Leadership records'}];
        this.dashboardTitle = `Elections: ${d.electionName} - Leadership records`;
    }

    mapped(p: LeadershipPosition): boolean {
        return p.classLeadershipRoleId !== null || p.responsibilityId !== null;
    }

    get toRecord(): number {
        return (this.data?.positions || []).filter((p) => this.mapped(p)).flatMap((p) => p.winners).filter((w) => !w.recordedAt).length;
    }

    record() {
        const d = this.data!;
        const unmapped = d.positions.filter((p) => !this.mapped(p) && p.winners.length).map((p) => p.name);
        Swal.fire({
            title: 'Record the winners?',
            html: 'Each winner is added to the chosen class leadership role and/or responsibility for the year.'
                + (unmapped.length ? `<br><br><small class="text-muted">Not recorded (nothing chosen): ${unmapped.join(', ')}</small>` : ''),
            width: 480, position: 'top', icon: 'question', showCancelButton: true, confirmButtonText: 'Record'
        }).then((r) => {
            if (!r.isConfirmed) return;
            this.busy = true;
            this.svc.recordLeadership(this.electionId, d.positions.map((p) => ({
                positionId: p.positionId,
                classLeadershipRoleId: p.classLeadershipRoleId,
                responsibilityId: p.responsibilityId
            }))).subscribe({
                next: (res) => {
                    this.busy = false;
                    this.toastr.success(res.added ? `${res.added} leadership record(s) added.` : 'Everything was already recorded.');
                    this.show(res.leadership);
                },
                error: (err) => {
                    this.busy = false;
                    this.toastr.error(apiError(err, 'Error recording the winners.'));
                }
            });
        });
    }
}
