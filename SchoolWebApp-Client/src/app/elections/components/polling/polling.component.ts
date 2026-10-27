import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {apiError, ElectionStage, MyStation, STAGE_BADGES, STAGE_LABELS} from '@/elections/models/election-models';
import {ElectionsService} from '@/elections/services/elections.service';

/** The polling stations the signed-in returning officer runs (every station, for elections officers). */
@Component({
    selector: 'app-polling',
    templateUrl: './polling.component.html'
})
export class PollingComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/elections/polling'], title: 'Polling Station'}
    ];
    dashboardTitle = 'Elections: Polling Station';

    stations: MyStation[] = [];
    loading = false;
    stageLabels = STAGE_LABELS;
    stageBadges = STAGE_BADGES;
    Stage = ElectionStage;

    constructor(private svc: ElectionsService, private toastr: ToastrService, public router: Router) {}

    ngOnInit(): void {
        this.loading = true;
        this.svc.myStations().subscribe({
            next: (s) => {
                this.loading = false;
                this.stations = s;
            },
            error: (err) => {
                this.loading = false;
                this.toastr.error(apiError(err, 'Error loading your polling stations.'));
            }
        });
    }

    percent(s: MyStation): number {
        return s.voters ? Math.round((100 * s.voted) / s.voters) : 0;
    }
}
