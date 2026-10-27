import {Component, Inject, OnDestroy, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import Swal from 'sweetalert2';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {
    apiError,
    Ballot,
    BallotCandidate,
    BallotRace,
    ElectionStage,
    STAGE_BADGES,
    STAGE_LABELS,
    StationRegister,
    StationVoter
} from '@/elections/models/election-models';
import {ElectionsService} from '@/elections/services/elections.service';

type Mode = 'register' | 'ballot' | 'review' | 'done';

/**
 * A polling station's screen. The returning officer finds the voter on the
 * station's register and issues the ballot; the screen then becomes the ballot
 * (full screen) for the voter, who chooses, reviews and casts. After casting it
 * goes back to the register for the next voter.
 */
@Component({
    selector: 'app-station-screen',
    templateUrl: './station-screen.component.html',
    styles: [`
        .ballot { position: fixed; inset: 0; z-index: 2000; background: #f4f6f9; overflow-y: auto; }
        .ballot-head { position: sticky; top: 0; z-index: 1; background: #1f2d3d; color: #fff; }
        .tile { cursor: pointer; border: 3px solid #dee2e6; border-radius: 10px; background: #fff; transition: border-color .1s, box-shadow .1s; }
        .tile:hover { border-color: #86b7fe; }
        .tile.picked { border-color: #198754; box-shadow: 0 0 0 4px rgba(25,135,84,.25); }
        .tile .pick { width: 34px; height: 34px; border-radius: 50%; border: 3px solid #adb5bd; display: flex; align-items: center; justify-content: center; }
        .tile.picked .pick { background: #198754; border-color: #198754; color: #fff; }
        .tile img, .tile .nophoto { width: 110px; height: 132px; object-fit: cover; border-radius: 6px; }
        .tile .mate img, .tile .mate .nophoto { width: 64px; height: 76px; }
        .nophoto { background: #e9ecef; color: #adb5bd; display: flex; align-items: center; justify-content: center; }
        .no { font-size: 1.6rem; font-weight: 700; color: #6c757d; min-width: 2rem; text-align: center; }
    `]
})
export class StationScreenComponent implements OnInit, OnDestroy {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/elections/polling'], title: 'Polling Station'}
    ];
    dashboardTitle = 'Elections: Polling Station';

    station: StationRegister | null = null;
    loading = false;
    search = '';
    show: 'waiting' | 'voted' | 'all' = 'waiting';
    page = 1;
    pageSize = 15;

    mode: Mode = 'register';
    ballot: Ballot | null = null;
    // contestId -> chosen candidate ids
    picks: {[contestId: number]: number[]} = {};
    casting = false;
    issuing: number | null = null;
    private doneTimer: any;

    stageLabels = STAGE_LABELS;
    stageBadges = STAGE_BADGES;
    Stage = ElectionStage;
    private stationId = 0;

    constructor(private svc: ElectionsService, private toastr: ToastrService, private route: ActivatedRoute,
                public router: Router, @Inject('BASE_API_URL') private baseUrl: string) {}

    ngOnInit(): void {
        this.route.paramMap.subscribe((p) => {
            this.stationId = Number(p.get('stationId'));
            this.load();
        });
    }

    ngOnDestroy(): void {
        clearTimeout(this.doneTimer);
    }

    load() {
        this.loading = true;
        this.svc.stationRegister(this.stationId).subscribe({
            next: (s) => {
                this.loading = false;
                this.station = s;
                this.breadcrumbs = [...this.breadcrumbs.slice(0, 2),
                    {link: ['/elections/polling', String(s.stationId)], title: s.stationName}];
                this.dashboardTitle = `${s.electionName}: ${s.stationName}`;
            },
            error: (err) => {
                this.loading = false;
                this.toastr.error(err.status === 403 ? 'You are not the returning officer of this station.' : apiError(err, 'Error loading the station.'));
            }
        });
    }

    get counts() {
        const v = this.station?.voters || [];
        return {all: v.length, voted: v.filter((x) => x.voted).length};
    }

    get filtered(): StationVoter[] {
        const q = this.search.trim().toLowerCase();
        return (this.station?.voters || []).filter((v) =>
            (this.show === 'all' || (this.show === 'voted') === v.voted) &&
            (!q || v.name.toLowerCase().includes(q) || v.upi.toLowerCase().includes(q) || v.className.toLowerCase().includes(q)));
    }

    get paged(): StationVoter[] {
        return this.filtered.slice((this.page - 1) * this.pageSize, this.page * this.pageSize);
    }

    photo(url: string | null): string | null {
        return url ? this.baseUrl + url : null;
    }

    // The officer checks the student's identity, then hands over the screen.
    issue(v: StationVoter) {
        Swal.fire({
            title: 'Issue the ballot?',
            html: `<b>${v.name}</b><br>${v.className} - UPI ${v.upi}<br><small class="text-muted">Check this is the student before handing over the screen.</small>`,
            width: 460, position: 'top', icon: 'question', showCancelButton: true, confirmButtonText: 'Issue ballot', confirmButtonColor: '#198754'
        }).then((r) => {
            if (!r.isConfirmed) return;
            this.issuing = v.studentId;
            this.svc.ballot(this.stationId, v.studentId).subscribe({
                next: (b) => {
                    this.issuing = null;
                    this.ballot = b;
                    this.picks = {};
                    b.races.forEach((race) => (this.picks[race.contestId] = []));
                    this.mode = 'ballot';
                    window.scrollTo(0, 0);
                },
                error: (err) => {
                    this.issuing = null;
                    this.toastr.error(apiError(err, 'Error issuing the ballot.'));
                    this.load();
                }
            });
        });
    }

    // Officer-only: take the ballot back without recording anything.
    cancelBallot() {
        Swal.fire({title: 'Cancel this ballot?', text: 'Nothing is recorded; the student can be issued a ballot again.',
            width: 420, position: 'top', icon: 'warning', showCancelButton: true, confirmButtonText: 'Cancel ballot',
            cancelButtonText: 'Keep voting'}).then((r) => {
            if (!r.isConfirmed) return;
            this.ballot = null;
            this.mode = 'register';
        });
    }

    isPicked(race: BallotRace, c: BallotCandidate): boolean {
        return this.picks[race.contestId].includes(c.candidateId);
    }

    genders(race: BallotRace): string[] {
        return [...new Set(race.candidates.map((c) => c.genderName))];
    }

    candidatesOf(race: BallotRace, gender: string | null): BallotCandidate[] {
        return gender === null ? race.candidates : race.candidates.filter((c) => c.genderName === gender);
    }

    rule(race: BallotRace): string {
        if (race.seatsPerGender) return race.seats === 1 ? 'Choose one of each' : `Choose up to ${race.seats} of each`;
        return race.seats === 1 ? 'Choose one' : `Choose up to ${race.seats}`;
    }

    // Tapping picks or unpicks; with one seat a new pick replaces the old one.
    toggle(race: BallotRace, c: BallotCandidate) {
        const chosen = this.picks[race.contestId];
        if (chosen.includes(c.candidateId)) {
            this.picks[race.contestId] = chosen.filter((id) => id !== c.candidateId);
            return;
        }
        const pool = race.seatsPerGender ? race.candidates.filter((x) => x.genderName === c.genderName) : race.candidates;
        const inPool = chosen.filter((id) => pool.some((x) => x.candidateId === id));
        if (inPool.length < race.seats) {
            this.picks[race.contestId] = [...chosen, c.candidateId];
        } else if (race.seats === 1) {
            this.picks[race.contestId] = [...chosen.filter((id) => !inPool.includes(id)), c.candidateId];
        } else {
            this.toastr.info(`You can choose up to ${race.seats}${race.seatsPerGender ? ' of each' : ''} - tap a choice to remove it first.`);
        }
    }

    chosenNames(race: BallotRace): string[] {
        return race.candidates.filter((c) => this.isPicked(race, c)).map((c) => c.name);
    }

    review() {
        this.mode = 'review';
        window.scrollTo(0, 0);
    }

    cast() {
        const b = this.ballot!;
        this.casting = true;
        this.svc.cast(this.stationId, b.studentId, b.races.map((r) => ({contestId: r.contestId, candidateIds: this.picks[r.contestId]})))
            .subscribe({
                next: () => {
                    this.casting = false;
                    this.mode = 'done';
                    const v = this.station?.voters.find((x) => x.studentId === b.studentId);
                    if (v) {
                        v.voted = true;
                        v.votedAt = new Date().toISOString();
                    }
                    this.doneTimer = setTimeout(() => this.next(), 5000);
                },
                error: (err) => {
                    this.casting = false;
                    this.toastr.error(apiError(err, 'The vote could not be cast.'));
                }
            });
    }

    next() {
        clearTimeout(this.doneTimer);
        this.ballot = null;
        this.mode = 'register';
        this.search = '';
        this.load();
    }
}
