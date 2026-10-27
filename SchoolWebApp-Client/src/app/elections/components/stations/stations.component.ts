import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import Swal from 'sweetalert2';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {apiError, IdName, SaveStation, Station, Stations, STAGE_BADGES, STAGE_LABELS} from '@/elections/models/election-models';
import {ElectionsService} from '@/elections/services/elections.service';

/**
 * Polling stations: which classes vote where, and the returning officer who runs
 * each station's screen. One click makes a station per class with its class teacher.
 */
@Component({
    selector: 'app-stations',
    templateUrl: './stations.component.html'
})
export class StationsComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/elections/manage'], title: 'Manage Elections'}
    ];
    dashboardTitle = 'Elections: Polling stations';

    data: Stations | null = null;
    loading = false;
    busy = false;

    // The station editor
    editing: Station | null = null;
    showForm = false;
    form: SaveStation = {name: '', officerUserId: null, schoolClassIds: []};
    classSearch = '';

    stageLabels = STAGE_LABELS;
    stageBadges = STAGE_BADGES;
    private electionId = 0;

    constructor(private svc: ElectionsService, private toastr: ToastrService, private route: ActivatedRoute, public router: Router) {}

    ngOnInit(): void {
        this.route.paramMap.subscribe((p) => {
            this.electionId = Number(p.get('id'));
            this.loading = true;
            this.svc.stations(this.electionId).subscribe({
                next: (d) => {
                    this.loading = false;
                    this.show(d);
                },
                error: (err) => {
                    this.loading = false;
                    this.toastr.error(apiError(err, 'Error loading the polling stations.'));
                }
            });
        });
    }

    show(d: Stations) {
        this.data = d;
        this.breadcrumbs = [...this.breadcrumbs.slice(0, 2),
            {link: ['/elections/manage', String(d.electionId)], title: d.electionName},
            {link: ['/elections/manage', String(d.electionId), 'stations'], title: 'Polling stations'}];
        this.dashboardTitle = `Elections: ${d.electionName} - Polling stations`;
    }

    private run(call: ReturnType<ElectionsService['stations']>, done: string, after?: () => void) {
        this.busy = true;
        call.subscribe({
            next: (d) => {
                this.busy = false;
                this.toastr.success(done);
                this.show(d);
                after?.();
            },
            error: (err) => {
                this.busy = false;
                this.toastr.error(apiError(err, 'Error saving the station.'));
            }
        });
    }

    generate() {
        this.run(this.svc.generateStations(this.electionId), 'Stations added.');
    }

    add() {
        this.editing = null;
        this.form = {name: '', officerUserId: null, schoolClassIds: this.data!.uncoveredClasses.map((c) => c.id)};
        this.classSearch = '';
        this.showForm = true;
    }

    edit(s: Station) {
        this.editing = s;
        this.form = {name: s.name, officerUserId: s.officerUserId, schoolClassIds: s.classes.map((c) => c.id)};
        this.classSearch = '';
        this.showForm = true;
    }

    // Classes the station can take: its own and those no other station has.
    get availableClasses(): IdName[] {
        if (!this.data) return [];
        const elsewhere = new Set(this.data.stations.filter((s) => s.id !== this.editing?.id).flatMap((s) => s.classes.map((c) => c.id)));
        const q = this.classSearch.trim().toLowerCase();
        return this.data.classes.filter((c) => !elsewhere.has(c.id) && (!q || c.name.toLowerCase().includes(q)));
    }

    hasClass(id: number): boolean {
        return this.form.schoolClassIds.includes(id);
    }

    toggleClass(id: number) {
        this.form.schoolClassIds = this.hasClass(id) ? this.form.schoolClassIds.filter((x) => x !== id) : [...this.form.schoolClassIds, id];
    }

    save() {
        if (!this.form.name.trim()) {
            this.toastr.warning('Enter the station name.');
            return;
        }
        if (this.form.schoolClassIds.length === 0) {
            this.toastr.warning('Choose the classes that vote at this station.');
            return;
        }
        const call = this.editing ? this.svc.updateStation(this.editing.id, this.form) : this.svc.addStation(this.electionId, this.form);
        this.run(call, 'Station saved.', () => (this.showForm = false));
    }

    remove(s: Station) {
        Swal.fire({title: `Remove ${s.name}?`, width: 400, position: 'top', icon: 'warning', showCancelButton: true,
            confirmButtonText: 'Remove', confirmButtonColor: '#d33'}).then((r) => {
            if (r.isConfirmed) this.run(this.svc.deleteStation(s.id), 'Station removed.');
        });
    }

    percent(s: Station): number {
        return s.voters ? Math.round((100 * s.voted) / s.voters) : 0;
    }
}
