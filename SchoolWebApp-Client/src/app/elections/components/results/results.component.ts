import {Component, ElementRef, HostListener, Inject, OnDestroy, OnInit, ViewChild} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {DatePipe} from '@angular/common';
import {ToastrService} from 'ngx-toastr';
import Swal from 'sweetalert2';
import {escapeHtml, printInFrame} from '@/shared/utils/print-frame';
import {safeFileName, saveBlob} from '@/teaching/utils/download';
import {
    apiError,
    ElectionStage,
    ResultCandidate,
    ResultRace,
    Results,
    STAGE_BADGES,
    STAGE_LABELS
} from '@/elections/models/election-models';
import {ElectionsService} from '@/elections/services/elections.service';
import {ElectionContext} from '@/elections/services/election-context.service';

/**
 * Live results: turnout and tallies per race as votes come in, refreshing on the
 * election's interval, with a full-screen mode for display. Each race's ballots
 * are checked against the voters marked as voted. After polling closes: draw lots
 * for ties and declare; after declaring: print, CSV and the leadership records.
 */
@Component({
    selector: 'app-results',
    templateUrl: './results.component.html',
    providers: [DatePipe],
    styles: [`
        .full { position: fixed; inset: 0; z-index: 2000; background: #f4f6f9; overflow-y: auto; padding: 12px; }
        .bar { height: 10px; background: #e9ecef; border-radius: 5px; overflow: hidden; }
        .bar > div { height: 100%; background: #6c8ebf; transition: width .6s; }
        .bar.win > div { background: #198754; }
        .bar.tie > div { background: #fd7e14; }
        .face { width: 38px; height: 46px; object-fit: cover; border-radius: 4px; }
        .face.none { background: #e9ecef; color: #adb5bd; display: inline-flex; align-items: center; justify-content: center; }
        .full .face { width: 52px; height: 62px; }
        .full .cname { font-size: 1.15rem; }
    `]
})
export class ResultsComponent implements OnInit, OnDestroy {
    @ViewChild('board') board?: ElementRef<HTMLElement>;
    r: Results | null = null;
    canManage = false;
    loading = false;
    auto = true;
    full = false;
    positionId: number | null = null;
    busy = false;
    // Drawing lots: the tied race and the order drawn.
    drawRace: ResultRace | null = null;
    drawOrder: ResultCandidate[] = [];

    stageLabels = STAGE_LABELS;
    stageBadges = STAGE_BADGES;
    Stage = ElectionStage;
    private electionId = 0;
    private timer: any;

    constructor(private svc: ElectionsService, private toastr: ToastrService, private route: ActivatedRoute,
                public router: Router, private datePipe: DatePipe, private ctx: ElectionContext, @Inject('BASE_API_URL') private baseUrl: string) {}

    ngOnInit(): void {
        this.svc.options().subscribe({next: (o) => (this.canManage = o.canManage)});
        this.route.parent!.paramMap.subscribe((p) => {
            this.electionId = Number(p.get('id'));
            this.load();
        });
    }

    ngOnDestroy(): void {
        clearTimeout(this.timer);
        if (document.fullscreenElement) document.exitFullscreen().catch(() => {});
    }

    load(quiet = false) {
        clearTimeout(this.timer);
        if (!quiet) this.loading = true;
        this.svc.results(this.electionId).subscribe({
            next: (r) => {
                this.loading = false;
                this.show(r);
                this.schedule();
            },
            error: (err) => {
                this.loading = false;
                if (!quiet) this.toastr.error(err.status === 403 ? 'The results are shown to elections officers until they are declared.'
                    : apiError(err, 'Error loading the results.'));
                this.schedule();
            }
        });
    }

    // Refresh while votes can still change the count.
    private schedule() {
        clearTimeout(this.timer);
        if (!this.auto || !this.r || this.r.stage === ElectionStage.Declared) return;
        this.timer = setTimeout(() => this.load(true), Math.max(3, this.r.refreshSeconds) * 1000);
    }

    toggleAuto() {
        this.auto = !this.auto;
        this.schedule();
    }

    show(r: Results) {
        this.r = r;
    }

    get positions() {
        const seen = new Map<number, string>();
        (this.r?.races || []).forEach((x) => seen.set(x.positionId, x.positionName));
        return [...seen].map(([id, name]) => ({id, name}));
    }

    get races(): ResultRace[] {
        return (this.r?.races || []).filter((x) => this.positionId === null || x.positionId === this.positionId);
    }

    get turnout(): number {
        return this.r && this.r.voters ? Math.round((1000 * this.r.voted) / this.r.voters) / 10 : 0;
    }

    pct(n: number, of: number): number {
        return of ? Math.round((1000 * n) / of) / 10 : 0;
    }

    // Bars are scaled to the race's top count so small races stay readable.
    width(race: ResultRace, c: ResultCandidate): number {
        const top = Math.max(...race.candidates.map((x) => x.votes), 0);
        return top ? (100 * c.votes) / top : 0;
    }

    checks(race: ResultRace): boolean {
        return race.ballots === race.voted;
    }

    get allCheck(): boolean {
        return (this.r?.races || []).every((x) => this.checks(x));
    }

    photo(url: string | null): string | null {
        return url ? this.baseUrl + url : null;
    }

    get declared(): boolean {
        return this.r?.stage === ElectionStage.Declared;
    }

    get tiedRaces(): ResultRace[] {
        return (this.r?.races || []).filter((x) => x.tied);
    }

    @HostListener('document:fullscreenchange')
    onFullscreenChange() {
        if (!document.fullscreenElement) this.full = false;
    }

    toggleFull() {
        this.full = !this.full;
        if (this.full) document.documentElement.requestFullscreen?.().catch(() => {});
        else if (document.fullscreenElement) document.exitFullscreen().catch(() => {});
    }

    // Lots
    startDraw(race: ResultRace) {
        this.drawRace = race;
        this.drawOrder = race.candidates.filter((c) => c.inTie);
    }

    moveDraw(i: number, by: number) {
        const j = i + by;
        if (j < 0 || j >= this.drawOrder.length) return;
        [this.drawOrder[i], this.drawOrder[j]] = [this.drawOrder[j], this.drawOrder[i]];
    }

    saveDraw() {
        this.busy = true;
        this.svc.tieBreak(this.drawRace!.contestId, this.drawOrder.map((c) => c.candidateId)).subscribe({
            next: (r) => {
                this.busy = false;
                this.drawRace = null;
                this.toastr.success('The draw is recorded.');
                this.show(r);
            },
            error: (err) => {
                this.busy = false;
                this.toastr.error(apiError(err, 'Error recording the draw.'));
            }
        });
    }

    declare() {
        Swal.fire({title: 'Declare the results?', text: 'The leading candidates are declared elected and the results become final and visible to all staff.',
            width: 460, position: 'top', icon: 'question', showCancelButton: true, confirmButtonText: 'Declare', confirmButtonColor: '#198754'}).then((res) => {
            if (!res.isConfirmed) return;
            this.busy = true;
            this.svc.changeStage(this.electionId, ElectionStage.Declared, null).subscribe({
                next: () => {
                    this.busy = false;
                    this.toastr.success('Results declared.');
                    this.ctx.reload();
                    this.load();
                },
                error: (err) => {
                    this.busy = false;
                    this.toastr.error(apiError(err, 'Error declaring the results.'));
                }
            });
        });
    }

    csv() {
        this.svc.resultsCsv(this.electionId).subscribe({
            next: (b) => saveBlob(b, safeFileName(`${this.r!.electionName} results`) + '.csv'),
            error: () => this.toastr.error('Error downloading the results.')
        });
    }

    print() {
        const r = this.r!;
        const word = this.declared ? 'Elected' : 'Leading';
        const body = r.races.map((race) => `
            <section><h2>${escapeHtml(race.name)}</h2>
            <div class="sub">${race.voted} of ${race.voters} voted (${this.pct(race.voted, race.voters)}%) - ${race.ballots} ballots, ${race.blank} blank
                - ${race.seats} seat(s)${race.seatsPerGender ? ' per gender' : ''}</div>
            <table><thead><tr><th>Candidate</th><th>Class</th>${race.runningMateTitle ? `<th>${escapeHtml(race.runningMateTitle)}</th>` : ''}
                <th class="n">Votes</th><th class="n">%</th><th></th></tr></thead><tbody>
            ${race.candidates.map((c) => `<tr class="${c.winning ? 'win' : ''}"><td>${escapeHtml(c.name)}</td><td>${escapeHtml(c.className)}</td>
                ${race.runningMateTitle ? `<td>${escapeHtml(c.runningMateName || '')}</td>` : ''}
                <td class="n">${c.votes}</td><td class="n">${c.percent}</td><td>${c.winning ? word : c.inTie ? 'Tied' : ''}</td></tr>`).join('')}
            </tbody></table></section>`).join('');
        const css = `@page { size: A4 portrait; margin: 12mm; } body { font-family: Arial, sans-serif; font-size: 10pt; }
            h1 { font-size: 15pt; margin: 0; } .head { color: #555; margin: 1mm 0 5mm; }
            section { break-inside: avoid; margin-bottom: 5mm; } h2 { font-size: 11.5pt; margin: 0; } .sub { color: #555; font-size: 9pt; margin: 1mm 0 2mm; }
            table { width: 100%; border-collapse: collapse; } th, td { border: 1px solid #aaa; padding: 1mm 2mm; text-align: left; }
            th { background: #eee; } .n { text-align: right; width: 16mm; } tr.win td { font-weight: bold; background: #eef7ee; }`;
        const status = this.declared
            ? `Declared ${this.datePipe.transform(r.declaredAt, 'd MMM y, HH:mm')} by ${escapeHtml(r.declaredBy || '')}`
            : `Provisional - ${escapeHtml(this.stageLabels[r.stage])}, as at ${this.datePipe.transform(r.asAt, 'd MMM y, HH:mm')}`;
        printInFrame(`${r.electionName} - Results`, css,
            `<h1>${escapeHtml(r.electionName)} - Results</h1><div class="head">${status}. Turnout ${r.voted} of ${r.voters} (${this.turnout}%).</div>${body}`);
    }
}
