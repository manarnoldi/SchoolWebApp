import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {DatePipe} from '@angular/common';
import {ToastrService} from 'ngx-toastr';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {escapeHtml, printInFrame} from '@/shared/utils/print-frame';
import {apiError, ElectionDetail, RegisterSummary, RegisterVoter, VoterRegisterMode} from '@/elections/models/election-models';
import {ElectionsService} from '@/elections/services/elections.service';

/**
 * Who votes in each race: the frozen register once nominations close, or (live
 * register, or before closing) the students where they are now.
 */
@Component({
    selector: 'app-voter-register',
    templateUrl: './voter-register.component.html',
    providers: [DatePipe]
})
export class VoterRegisterComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/elections/manage'], title: 'Manage Elections'}
    ];
    dashboardTitle = 'Elections: Voter register';

    election: ElectionDetail | null = null;
    summary: RegisterSummary | null = null;
    contestId: number | null = null;
    voters: RegisterVoter[] = [];
    loadingVoters = false;
    search = '';
    page = 1;
    pageSize = 15;
    Mode = VoterRegisterMode;

    constructor(private svc: ElectionsService, private toastr: ToastrService, private route: ActivatedRoute,
                public router: Router, private datePipe: DatePipe) {}

    ngOnInit(): void {
        this.route.paramMap.subscribe((p) => {
            const id = Number(p.get('id'));
            this.svc.get(id).subscribe({
                next: (e) => {
                    this.election = e;
                    this.breadcrumbs = [...this.breadcrumbs.slice(0, 2),
                        {link: ['/elections/manage', String(e.id)], title: e.name},
                        {link: ['/elections/manage', String(e.id), 'register'], title: 'Voter register'}];
                    this.dashboardTitle = `Elections: ${e.name} - Voter register`;
                },
                error: (err) => this.toastr.error(apiError(err, 'Error loading the election.'))
            });
            this.svc.register(id).subscribe({
                next: (s) => {
                    this.summary = s;
                    if (s.contests.length) this.pick(s.contests[0].contestId);
                },
                error: (err) => this.toastr.error(apiError(err, 'Error loading the voter register.'))
            });
        });
    }

    get contest() {
        return this.summary?.contests.find((c) => c.contestId === this.contestId);
    }

    pick(contestId: number) {
        this.contestId = contestId;
        this.page = 1;
        this.loadingVoters = true;
        this.svc.registerContest(Number(this.route.snapshot.paramMap.get('id')), contestId).subscribe({
            next: (v) => {
                this.voters = v;
                this.loadingVoters = false;
            },
            error: (err) => {
                this.loadingVoters = false;
                this.toastr.error(apiError(err, 'Error loading the voters.'));
            }
        });
    }

    get filtered(): RegisterVoter[] {
        const q = this.search.trim().toLowerCase();
        return this.voters.filter((v) => !q || v.name.toLowerCase().includes(q) || v.upi.toLowerCase().includes(q)
            || v.className.toLowerCase().includes(q));
    }

    get paged(): RegisterVoter[] {
        return this.filtered.slice((this.page - 1) * this.pageSize, this.page * this.pageSize);
    }

    get basis(): string {
        const s = this.summary;
        if (!s) return '';
        if (s.frozenAt) return `Frozen register, taken ${this.datePipe.transform(s.frozenAt, 'd MMM y, HH:mm')}.`;
        return s.mode === VoterRegisterMode.Frozen
            ? 'Not taken yet - it is taken when nominations close. Shown: the students where they are now.'
            : 'Live register - the students where they are now; it follows class changes until they vote.';
    }

    // The race's register with a signature column, for checking voters in on paper.
    print() {
        const c = this.contest!;
        const rows = this.voters.map((v, i) => `<tr><td>${i + 1}</td><td>${escapeHtml(v.name)}</td><td>${escapeHtml(v.upi)}</td>
            <td>${escapeHtml(v.className)}</td><td>${escapeHtml(v.genderName)}</td><td></td></tr>`).join('');
        const css = `@page { size: A4 portrait; margin: 12mm; }
            body { font-family: Arial, sans-serif; font-size: 10pt; } h1 { font-size: 14pt; margin: 0; }
            .sub { color: #555; margin: 1mm 0 4mm; } table { width: 100%; border-collapse: collapse; }
            th, td { border: 1px solid #999; padding: 1.2mm 2mm; text-align: left; } th { background: #eee; }
            td:first-child { width: 8mm; text-align: right; } td:last-child { width: 35mm; } tr { break-inside: avoid; }`;
        printInFrame(`${this.election?.name} - ${c.name} register`, css,
            `<h1>${escapeHtml(this.election?.name || '')} - ${escapeHtml(c.name)}</h1>
             <div class="sub">Voter register: ${this.voters.length} voters. ${escapeHtml(this.basis)}</div>
             <table><thead><tr><th>#</th><th>Name</th><th>UPI</th><th>Class</th><th>Gender</th><th>Signature</th></tr></thead>
             <tbody>${rows}</tbody></table>`);
    }
}
