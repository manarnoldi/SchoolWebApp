import {Component, Inject, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import Swal from 'sweetalert2';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {escapeHtml, printInFrame} from '@/shared/utils/print-frame';
import {
    apiError,
    Candidate,
    CANDIDATE_STATUS_BADGES,
    CANDIDATE_STATUS_LABELS,
    CandidateStatus,
    ElectionStage,
    NominationContest,
    NominationPosition,
    Nominations,
    PositionGenderRule,
    STAGE_BADGES,
    STAGE_LABELS
} from '@/elections/models/election-models';
import {ElectionsService} from '@/elections/services/elections.service';

/**
 * Who stands in each race: nominate students (with running mates), approve or
 * reject them, and add their photos. Prints the candidate list.
 */
@Component({
    selector: 'app-nominations',
    templateUrl: './nominations.component.html'
})
export class NominationsComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/elections/manage'], title: 'Manage Elections'}
    ];
    dashboardTitle = 'Elections: Nominations';

    data: Nominations | null = null;
    loading = false;
    busy = false;
    positionId: number | null = null;
    status: CandidateStatus | null = null;
    search = '';

    // The modal
    formPosition: NominationPosition | null = null;
    formContest: NominationContest | null = null;
    formCandidate: Candidate | null = null;

    statusLabels = CANDIDATE_STATUS_LABELS;
    statusBadges = CANDIDATE_STATUS_BADGES;
    stageLabels = STAGE_LABELS;
    stageBadges = STAGE_BADGES;
    Status = CandidateStatus;
    Stage = ElectionStage;
    statuses = [CandidateStatus.Nominated, CandidateStatus.Approved, CandidateStatus.Rejected, CandidateStatus.Withdrawn];

    private electionId = 0;

    constructor(private svc: ElectionsService, private toastr: ToastrService, private route: ActivatedRoute,
                public router: Router, @Inject('BASE_API_URL') private baseUrl: string) {}

    ngOnInit(): void {
        this.route.paramMap.subscribe((p) => {
            this.electionId = Number(p.get('id'));
            this.load();
        });
    }

    load() {
        this.loading = true;
        this.svc.nominations(this.electionId).subscribe({
            next: (n) => {
                this.loading = false;
                this.show(n);
            },
            error: (err) => {
                this.loading = false;
                this.toastr.error(apiError(err, 'Error loading the nominations.'));
            }
        });
    }

    show(n: Nominations) {
        this.data = n;
        this.breadcrumbs = [
            ...this.breadcrumbs.slice(0, 2),
            {link: ['/elections/manage', String(n.electionId)], title: n.electionName},
            {link: ['/elections/manage', String(n.electionId), 'nominations'], title: 'Nominations'}
        ];
        this.dashboardTitle = `Elections: ${n.electionName} - Nominations`;
    }

    photo(url: string | null): string | null {
        return url ? this.baseUrl + url : null;
    }

    // Positions and races narrowed by the filters; a race shows even when empty
    // unless a status or search filter is on.
    get positions(): NominationPosition[] {
        if (!this.data) return [];
        const q = this.search.trim().toLowerCase();
        const filtering = this.status !== null || !!q;
        return this.data.positions
            .filter((p) => this.positionId === null || p.id === this.positionId)
            .map((p) => ({
                ...p,
                contests: p.contests
                    .map((c) => ({
                        ...c,
                        candidates: c.candidates.filter((x) =>
                            (this.status === null || x.status === this.status) &&
                            (!q || x.studentName.toLowerCase().includes(q) || (x.runningMateName || '').toLowerCase().includes(q)
                                || x.className.toLowerCase().includes(q) || c.name.toLowerCase().includes(q)))
                    }))
                    .filter((c) => !filtering || c.candidates.length > 0)
            }))
            .filter((p) => p.contests.length > 0);
    }

    get counts() {
        const all = this.data?.positions.flatMap((p) => p.contests.flatMap((c) => c.candidates)) || [];
        const races = this.data?.positions.flatMap((p) => p.contests) || [];
        return {
            pending: all.filter((x) => x.status === CandidateStatus.Nominated).length,
            approved: all.filter((x) => x.status === CandidateStatus.Approved).length,
            racesWithout: races.filter((c) => !c.candidates.some((x) => x.status === CandidateStatus.Approved)).length,
            races: races.length
        };
    }

    approvedIn(c: NominationContest): number {
        return c.candidates.filter((x) => x.status === CandidateStatus.Approved).length;
    }

    positionHint(p: NominationPosition): string {
        const parts: string[] = [];
        if (p.seats > 1 || p.genderRule === PositionGenderRule.SeatsPerGender)
            parts.push(p.genderRule === PositionGenderRule.SeatsPerGender ? `${p.seats} seat(s) per gender` : `${p.seats} seats`);
        if (p.genderRule === PositionGenderRule.SingleGender && p.genderName) parts.push(`${p.genderName} candidates`);
        if (p.runningMateTitle) parts.push(`with ${p.runningMateTitle}`);
        return parts.join(' - ');
    }

    nominate(p: NominationPosition, c: NominationContest) {
        this.formPosition = this.data!.positions.find((x) => x.id === p.id)!;
        this.formContest = c;
        this.formCandidate = null;
    }

    edit(p: NominationPosition, c: NominationContest, x: Candidate) {
        this.formPosition = this.data!.positions.find((y) => y.id === p.id)!;
        this.formContest = c;
        this.formCandidate = x;
    }

    closeForm() {
        this.formPosition = this.formContest = this.formCandidate = null;
    }

    saved(n: Nominations) {
        this.closeForm();
        this.show(n);
    }

    private run(call: ReturnType<ElectionsService['nominations']>, done?: string) {
        this.busy = true;
        call.subscribe({
            next: (n) => {
                this.busy = false;
                if (done) this.toastr.success(done);
                this.show(n);
            },
            error: (err) => {
                this.busy = false;
                this.toastr.error(apiError(err, 'Error saving the change.'));
            }
        });
    }

    approve(x: Candidate) {
        this.run(this.svc.setCandidateStatus(x.id, CandidateStatus.Approved, null), `${x.studentName} approved.`);
    }

    reject(x: Candidate) {
        Swal.fire({title: `Reject ${x.studentName}?`, input: 'text', inputPlaceholder: 'Reason (required)', width: 460,
            position: 'top', icon: 'warning', showCancelButton: true, confirmButtonText: 'Reject', confirmButtonColor: '#d33',
            inputValidator: (v) => (v && v.trim() ? null : 'Give the reason for rejecting.')}).then((r) => {
            if (r.isConfirmed) this.run(this.svc.setCandidateStatus(x.id, CandidateStatus.Rejected, r.value), 'Nomination rejected.');
        });
    }

    withdraw(x: Candidate) {
        Swal.fire({title: `Withdraw ${x.studentName}?`, input: 'text', inputPlaceholder: 'Note (optional)', width: 460,
            position: 'top', icon: 'question', showCancelButton: true, confirmButtonText: 'Withdraw'}).then((r) => {
            if (r.isConfirmed) this.run(this.svc.setCandidateStatus(x.id, CandidateStatus.Withdrawn, r.value || null), 'Nomination withdrawn.');
        });
    }

    restore(x: Candidate) {
        this.run(this.svc.setCandidateStatus(x.id, CandidateStatus.Nominated, null), 'Nomination restored.');
    }

    remove(x: Candidate) {
        Swal.fire({title: `Delete ${x.studentName}'s nomination?`, text: 'Use this for a mistaken entry; reject or withdraw to keep a record.',
            width: 460, position: 'top', icon: 'warning', showCancelButton: true, confirmButtonText: 'Delete', confirmButtonColor: '#d33'}).then((r) => {
            if (r.isConfirmed) this.run(this.svc.deleteCandidate(x.id), 'Nomination deleted.');
        });
    }

    upload(x: Candidate, mate: boolean, event: Event) {
        const input = event.target as HTMLInputElement;
        const file = input.files?.[0];
        input.value = '';
        if (!file) return;
        if (file.size > 3 * 1024 * 1024) {
            this.toastr.warning('The photo is larger than 3 MB.');
            return;
        }
        this.run(this.svc.uploadPhoto(x.id, file, mate), 'Photo saved.');
    }

    removePhoto(x: Candidate, mate: boolean) {
        this.run(this.svc.deletePhoto(x.id, mate), 'Photo removed.');
    }

    // The approved candidates of every race, in ballot order, with photos.
    print() {
        const d = this.data!;
        const img = (url: string | null) => url
            ? `<img src="${escapeHtml(new URL(this.photo(url)!, location.href).href)}">`
            : '<div class="nophoto">No photo</div>';
        const body = d.positions.map((p) => p.contests.map((c) => {
            const list = c.candidates.filter((x) => x.status === CandidateStatus.Approved);
            const rows = list.length === 0 ? '<p class="none">No approved candidate.</p>' : list.map((x, i) => `
                <div class="cand">
                    <div class="no">${x.ballotOrder || i + 1}</div>
                    ${img(x.photoUrl)}
                    <div class="who"><b>${escapeHtml(x.studentName)}</b><br><span>${escapeHtml(x.className)}</span>
                        ${x.slogan ? `<div class="slogan">"${escapeHtml(x.slogan)}"</div>` : ''}</div>
                    ${p.runningMateTitle ? `${img(x.runningMatePhotoUrl)}
                    <div class="who"><small>${escapeHtml(p.runningMateTitle)}</small><br><b>${escapeHtml(x.runningMateName || '')}</b>
                        <br><span>${escapeHtml(x.runningMateClassName || '')}</span></div>` : ''}
                </div>`).join('');
            return `<section><h2>${escapeHtml(c.name)}</h2>${rows}</section>`;
        }).join('')).join('');
        const css = `@page { size: A4 portrait; margin: 12mm; }
            body { font-family: Arial, sans-serif; font-size: 11pt; color: #000; }
            h1 { font-size: 16pt; margin: 0 0 2mm; } .sub { color: #555; margin-bottom: 5mm; }
            section { break-inside: avoid; margin-bottom: 5mm; }
            h2 { font-size: 12pt; background: #eee; padding: 1.5mm 2mm; margin: 0 0 2mm; }
            .cand { display: flex; align-items: center; gap: 4mm; padding: 1.5mm 0; border-bottom: 1px solid #ddd; }
            .no { font-size: 16pt; font-weight: bold; width: 8mm; text-align: center; }
            img, .nophoto { width: 18mm; height: 22mm; object-fit: cover; border: 1px solid #999; }
            .nophoto { display: flex; align-items: center; justify-content: center; font-size: 7pt; color: #888; }
            .who { min-width: 45mm; } .who span, .who small { color: #555; font-size: 9pt; }
            .slogan { font-style: italic; font-size: 9pt; } .none { color: #888; font-style: italic; }`;
        printInFrame(`${d.electionName} - Candidates`, css,
            `<h1>${escapeHtml(d.electionName)}</h1><div class="sub">Candidates - ${escapeHtml(this.stageLabels[d.stage])}</div>${body}`);
    }
}
