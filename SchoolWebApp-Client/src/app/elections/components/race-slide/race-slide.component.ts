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
    styles: [`
        .slide { min-height: calc(100vh - 150px); }
        .meter { height: 14px; background: #e9ecef; border-radius: 7px; overflow: hidden; }
        .meter > div { height: 100%; background: #2a78d6; border-radius: 7px; transition: width .6s; }
        .donut { width: 100%; max-width: 380px; }
        .donut circle.slice { transition: stroke-dasharray .6s, stroke-dashoffset .6s; }
        .swatch { width: 14px; height: 14px; border-radius: 3px; flex: 0 0 14px; }
        .face { width: 64px; height: 76px; object-fit: cover; border-radius: 6px; flex: 0 0 64px; }
        .face.none { background: #e9ecef; color: #adb5bd; display: inline-flex; align-items: center; justify-content: center; }
        .cand { border-bottom: 1px solid #e9ecef; }
        .cand:last-child { border-bottom: 0; }
        .votes { font-size: 1.9rem; font-weight: 700; line-height: 1; color: #212529; }
        .cname { font-size: 1.35rem; }
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
