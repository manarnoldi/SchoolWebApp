import {Component, OnInit} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {ActivatedRoute, Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {escapeHtml, printInFrame} from '@/shared/utils/print-frame';

export interface CurriculumStatement {
    name: string;
    description: string | null;
}

export interface CurriculumRubric {
    indicator: string;
    exceedsExpectations: string | null;
    meetsExpectations: string | null;
    approachesExpectations: string | null;
    belowExpectations: string | null;
}

export interface CurriculumSubStrandView {
    id: number;
    code: string | null;
    name: string;
    lessons: number;
    specificOutcomes: string[];
    learningExperiences: string[];
    keyQuestions: string[];
    coreCompetencies: CurriculumStatement[];
    values: CurriculumStatement[];
    pcis: CurriculumStatement[];
    learningAreaLinks: CurriculumStatement[];
    subjectOutcome: string | null;
    levelOutcome: string | null;
    suggestedResources: string | null;
    suggestedAssessmentMethods: string | null;
    suggestedNonFormalActivities: string | null;
    rubrics: CurriculumRubric[];
}

export interface CurriculumStrandView {
    id: number;
    code: string | null;
    name: string;
    subStrands: CurriculumSubStrandView[];
    rubrics: CurriculumRubric[];
}

export interface CurriculumView {
    subjectId: number;
    subjectName: string;
    learningLevelId: number;
    learningLevelName: string;
    title: string | null;
    source: string | null;
    essenceStatement: string | null;
    lessonsPerWeek: number | null;
    totalLessons: number;
    subjectOutcomes: string[];
    strands: CurriculumStrandView[];
    appendix: {label: string; items: string[]}[];
    sections: {title: string; content: string | null}[];
}

const PICK_KEY = 'teaching.curriculum';

/**
 * A subject's curriculum design for one grade, read-only, laid out like the
 * KICD document: each strand's sub-strands with their outcomes, learning
 * experiences, key questions, core competencies, values, PCIs and links to
 * other learning areas, then the strand's assessment rubric; the essence
 * statement and subject outcomes before, the appendices and other sections
 * after. Changes are made through CBE Settings > Curriculum Upload.
 */
@Component({
    selector: 'app-curriculum-view',
    templateUrl: './curriculum-view.component.html',
    styleUrl: '../teaching.scss'
})
export class CurriculumViewComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/teaching/curriculum'], title: 'Curriculum'}
    ];
    dashboardTitle = 'Teaching Documents: Curriculum';

    subjects: any[] = [];
    levels: any[] = [];
    subjectId: number | null = null;
    learningLevelId: number | null = null;
    view: CurriculumView | null = null;
    loading = false;
    letters = 'abcdefghijklmnopqrstuvwxyz';
    // Strands shown open; the first opens on load, the rest on click.
    openStrands = new Set<number>();

    constructor(private http: HttpClient, private route: ActivatedRoute, private router: Router,
                private toastr: ToastrService) {}

    ngOnInit(): void {
        // Subjects come from the API in their set order.
        this.http.get<any[]>('/subjects').subscribe({next: (s) => this.subjects = s || []});
        this.http.get<any[]>('/learningLevels').subscribe({next: (l) => this.levels = (l || []).sort((a, b) => a.rank - b.rank)});
        let q = this.route.snapshot.queryParamMap;
        let subjectId = +(q.get('subjectId') || 0), levelId = +(q.get('learningLevelId') || 0);
        if (!subjectId || !levelId) {
            try {
                let saved = JSON.parse(sessionStorage.getItem(PICK_KEY) || 'null');
                subjectId = saved?.subjectId || 0;
                levelId = saved?.learningLevelId || 0;
            } catch { /* storage blocked */ }
        }
        if (subjectId && levelId) {
            this.subjectId = subjectId;
            this.learningLevelId = levelId;
            this.load();
        }
    }

    load() {
        if (!this.subjectId || !this.learningLevelId) { this.view = null; return; }
        let pick = {subjectId: this.subjectId, learningLevelId: this.learningLevelId};
        try { sessionStorage.setItem(PICK_KEY, JSON.stringify(pick)); } catch { /* storage blocked */ }
        this.router.navigate([], {relativeTo: this.route, queryParams: pick, replaceUrl: true});
        this.loading = true;
        this.http.get<CurriculumView>('/curriculum', {params: pick}).subscribe({
            next: (v) => {
                this.view = v;
                this.loading = false;
                this.openStrands = new Set(v.strands.slice(0, 1).map((s) => s.id));
            },
            error: (err) => { this.loading = false; this.toastr.error(err.error?.message || 'Error loading the curriculum.'); }
        });
    }

    lessonsOf(s: CurriculumStrandView): number {
        return s.subStrands.reduce((n, ss) => n + (ss.lessons || 0), 0);
    }

    // One strand open at a time: opening one closes the other; clicking the open one closes it.
    toggleStrand(id: number) {
        this.openStrands = this.openStrands.has(id) ? new Set() : new Set([id]);
    }

    // The subject outcome's letter, as the design numbers them.
    outcomeLetter(outcome: string | null): string {
        let i = this.view?.subjectOutcomes.indexOf(outcome || '') ?? -1;
        return i >= 0 ? this.letters[i] : '';
    }

    get isEmpty(): boolean {
        return !!this.view && this.view.strands.length === 0;
    }

    print() {
        let v = this.view;
        if (!v) return;
        let t = (s: string | null | undefined) => escapeHtml(s || '').replace(/\n/g, '<br>');
        let list = (items: string[], ordered = false) => items.length
            ? `<${ordered ? 'ol type="a"' : 'ul'}>${items.map((i) => `<li>${t(i)}</li>`).join('')}</${ordered ? 'ol' : 'ul'}>` : '';
        let statements = (title: string, items: CurriculumStatement[]) => items.length
            ? `<tr><td colspan="4"><b>${title}</b>${items.map((s) => `<div>${t(s.name)}${s.description ? ': ' + t(s.description) : ''}</div>`).join('')}</td></tr>` : '';
        let rubric = (rows: CurriculumRubric[]) => rows.length ? `<h4>Assessment Rubric</h4><table>
            <tr><th>Indicator</th><th>Exceeds Expectations</th><th>Meets Expectations</th><th>Approaches Expectations</th><th>Below Expectations</th></tr>
            ${rows.map((r) => `<tr><td>${t(r.indicator)}</td><td>${t(r.exceedsExpectations)}</td><td>${t(r.meetsExpectations)}</td>
                <td>${t(r.approachesExpectations)}</td><td>${t(r.belowExpectations)}</td></tr>`).join('')}</table>` : '';
        let body = `<h1>${t(v.title || `${v.subjectName} - ${v.learningLevelName} curriculum design`)}</h1>
            ${v.source ? `<p class="muted">${t(v.source)}</p>` : ''}
            ${v.essenceStatement ? `<h3>Essence Statement</h3><p>${t(v.essenceStatement)}</p>` : ''}
            ${v.subjectOutcomes.length ? `<h3>Subject General Learning Outcomes</h3>${list(v.subjectOutcomes, true)}` : ''}
            ${v.strands.map((s) => `<h2>Strand ${t(s.code)}: ${t(s.name)}</h2>
                ${s.subStrands.map((ss) => `<table class="ss">
                    <tr><th style="width:16%">Sub-strand</th><th style="width:24%">Specific Learning Outcomes</th>
                        <th style="width:42%">Suggested Learning Experiences</th><th>Suggested Key Inquiry Question(s)</th></tr>
                    <tr><td><b>${t(ss.code)} ${t(ss.name)}</b><br>(${ss.lessons} lessons)</td>
                        <td>By the end of the sub-strand, the learner should be able to:${list(ss.specificOutcomes, true)}</td>
                        <td>The learner is guided to:${list(ss.learningExperiences)}</td>
                        <td>${list(ss.keyQuestions)}</td></tr>
                    ${statements('Core Competencies to be developed:', ss.coreCompetencies)}
                    ${statements('Values:', ss.values)}
                    ${statements('Link to Pertinent Contemporary Issues (PCIs):', ss.pcis)}
                    ${statements('Link to other Learning Areas:', ss.learningAreaLinks)}
                    </table>${rubric(ss.rubrics)}`).join('')}
                ${rubric(s.rubrics)}`).join('')}
            ${v.appendix.length ? `<h2>Appendix: Suggested Resources, Assessment Methods and Non-formal Programmes</h2>
                ${v.appendix.map((a) => `<h4>${t(a.label)}</h4>${list(a.items)}`).join('')}` : ''}
            ${v.sections.map((s) => `<h2>${t(s.title)}</h2><p>${t(s.content)}</p>`).join('')}`;
        let css = `@page { size: A4 landscape; margin: 10mm; }
            body { font-family: Arial, Helvetica, sans-serif; font-size: 10px; color: #111; }
            h1 { font-size: 15px; margin: 0 0 2px; } h2 { font-size: 13px; margin: 14px 0 4px; } h3 { font-size: 12px; margin: 10px 0 3px; }
            h4 { font-size: 11px; margin: 6px 0 3px; } .muted { color: #666; margin: 0; }
            table { border-collapse: collapse; width: 100%; margin-bottom: 6px; } th, td { border: 1px solid #555; padding: 3px 4px; vertical-align: top; text-align: left; }
            th { background: #e9e9e9; } ul, ol { margin: 2px 0 0 14px; padding: 0; } tr { page-break-inside: avoid; }`;
        printInFrame(`${v.subjectName} ${v.learningLevelName} curriculum design`, css, body, 'landscape');
    }
}
