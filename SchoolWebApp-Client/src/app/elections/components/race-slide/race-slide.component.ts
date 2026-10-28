import {Component, Input} from '@angular/core';
import {ResultCandidate, ResultRace} from '@/elections/models/election-models';

// Candidate colours, in a fixed order (see the dataviz palette); a candidate keeps
// its colour as the count changes because it follows the candidate, not their place.
const SLOTS = ['#2a78d6', '#eb6834', '#1baf7a', '#eda100', '#e87ba4', '#008300', '#6250d6', '#e34948'];
const OTHER = '#adb5bd';

const R = 80;
const CIRCUMFERENCE = 2 * Math.PI * R;
const GAP = 1.5; // the surface gap between slices, in viewBox units

interface Slice {
    candidate: ResultCandidate;
    color: string;
    dash: string;
    offset: number;
}

/** One race on the results slideshow: a donut of the votes beside the candidates, votes and shares, and the race's turnout. */
@Component({
    selector: 'app-race-slide',
    templateUrl: './race-slide.component.html',
    // Sized for a projector across a hall: type scales with the screen.
    styles: [`
        .slide { min-height: calc(100vh - 90px); display: flex; flex-direction: column; background: #fff; border-radius: 10px;
            box-shadow: 0 1px 4px rgba(0, 0, 0, .08); padding: clamp(16px, 2.2vw, 40px); font-variant-numeric: tabular-nums; }
        .slide-head { display: flex; flex-wrap: wrap; align-items: flex-end; gap: 24px; padding-bottom: clamp(12px, 1.6vw, 24px);
            border-bottom: 3px solid #e7f1ff; }
        .kicker { text-transform: uppercase; letter-spacing: .12em; font-weight: 700; color: #2a78d6; font-size: clamp(.8rem, 1vw, 1.1rem); }
        .race-name { font-weight: 800; color: #0b2a5b; line-height: 1.05; margin: 2px 0 6px; font-size: clamp(2rem, 3.8vw, 4.2rem); }
        .race-sub { color: #52514e; font-size: clamp(.95rem, 1.2vw, 1.35rem); }
        .turnout { flex: 0 1 440px; min-width: 260px; }
        .turnout-pct { font-weight: 800; color: #0b2a5b; line-height: 1; font-size: clamp(2.4rem, 4.4vw, 5rem); }
        .meter { height: clamp(12px, 1.2vw, 20px); background: #e9ecef; border-radius: 10px; overflow: hidden; margin-top: 8px; }
        .meter > div { height: 100%; background: #2a78d6; border-radius: 10px; transition: width .6s; }
        .slide-body { flex: 1; display: flex; flex-wrap: wrap; align-items: center; gap: clamp(16px, 3vw, 56px); padding-top: clamp(12px, 2vw, 32px); }
        .donut-wrap { flex: 0 0 auto; width: min(44vh, 40vw, 560px); min-width: 240px; text-align: center; margin: 0 auto; }
        .donut { width: 100%; }
        .donut circle.slice { transition: stroke-dasharray .6s, stroke-dashoffset .6s; }
        .donut-total { font-size: 46px; font-weight: 800; fill: #0b2a5b; }
        .donut-label { font-size: 12px; font-weight: 700; letter-spacing: 2px; fill: #52514e; }
        .cands { flex: 1 1 480px; min-width: 0; }
        .cand { display: flex; align-items: center; gap: clamp(10px, 1.2vw, 20px); padding: clamp(8px, 1vw, 16px) clamp(8px, 1vw, 16px);
            border-radius: 10px; border-bottom: 1px solid #e9ecef; }
        .cand:last-child { border-bottom: 0; }
        .cand.lead { background: #f3faf6; border-bottom-color: transparent; box-shadow: inset 6px 0 0 #198754; }
        .swatch { width: clamp(14px, 1.3vw, 24px); height: clamp(14px, 1.3vw, 24px); border-radius: 4px; flex: 0 0 auto; }
        .face { width: clamp(64px, 6vw, 112px); height: clamp(76px, 7.2vw, 134px); object-fit: cover; border-radius: 8px; flex: 0 0 auto; }
        .face.none { background: #e9ecef; color: #adb5bd; display: inline-flex; align-items: center; justify-content: center;
            font-size: clamp(1.4rem, 2.4vw, 3rem); }
        .cname { font-weight: 700; color: #0b0b0b; line-height: 1.15; font-size: clamp(1.4rem, 2.3vw, 2.7rem); }
        .cand.lead .cname { color: #0b2a5b; font-weight: 800; }
        .crown { color: #e0a800; margin-left: 8px; }
        .cmeta { color: #52514e; font-size: clamp(.95rem, 1.25vw, 1.4rem); }
        .pill { display: inline-block; margin-top: 6px; padding: 2px 12px; border-radius: 20px; font-weight: 700; letter-spacing: .06em;
            text-transform: uppercase; font-size: clamp(.75rem, .9vw, 1rem); }
        .pill-lead { background: #198754; color: #fff; }
        .pill-tie { background: #ffe08a; color: #6b4f00; }
        .score { text-align: right; min-width: clamp(90px, 9vw, 170px); }
        .votes { font-weight: 800; color: #0b0b0b; line-height: 1; font-size: clamp(2.4rem, 4.2vw, 5rem); }
        .share { color: #52514e; font-weight: 600; font-size: clamp(1.05rem, 1.6vw, 1.9rem); }
    `]
})
export class RaceSlideComponent {
    @Input() race!: ResultRace;
    @Input() photoBase = '';
    @Input() declared = false;

    r = R;

    pct(n: number, of: number): number {
        return of ? Math.round((1000 * n) / of) / 10 : 0;
    }

    photo(url: string | null): string | null {
        return url ? this.photoBase + url : null;
    }

    color(c: ResultCandidate): string {
        const i = [...this.race.candidates].sort((a, b) => a.candidateId - b.candidateId).findIndex((x) => x.candidateId === c.candidateId);
        return SLOTS[i] ?? OTHER;
    }

    get counted(): number {
        return this.race.candidates.reduce((n, c) => n + c.votes, 0);
    }

    // The slices in the list's order, clockwise from the top, a small gap between each.
    get slices(): Slice[] {
        const total = this.counted;
        if (!total) return [];
        const shown = this.race.candidates.filter((c) => c.votes > 0);
        const gap = shown.length > 1 ? GAP : 0;
        let at = 0;
        return shown.map((c) => {
            const len = (CIRCUMFERENCE * c.votes) / total;
            const slice = {
                candidate: c,
                color: this.color(c),
                dash: `${Math.max(len - gap, 0.5)} ${CIRCUMFERENCE}`,
                offset: -at
            };
            at += len;
            return slice;
        });
    }

    trackById(_: number, s: Slice) {
        return s.candidate.candidateId;
    }
}
