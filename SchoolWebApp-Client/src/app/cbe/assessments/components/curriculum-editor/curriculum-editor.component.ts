import {Component, OnInit} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {ActivatedRoute, Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import Swal from 'sweetalert2';
import {BreadCrumb} from '@/core/models/bread-crumb';

interface Statement {
    name: string;
    description: string | null;
    linkId: number | null;
}

interface Rubric {
    subStrandId: number | null;
    indicator: string;
    exceedsExpectations: string | null;
    meetsExpectations: string | null;
    approachesExpectations: string | null;
    belowExpectations: string | null;
}

interface SubStrand {
    id: number;
    code: string | null;
    name: string;
    coreCompetencies: Statement[];
    values: Statement[];
    learningAreaLinks: Statement[];
    suggestedResources: string | null;
    suggestedAssessmentMethods: string | null;
    suggestedNonFormalActivities: string | null;
}

interface Strand {
    id: number;
    code: string | null;
    name: string;
    subStrands: SubStrand[];
    rubrics: Rubric[];
}

interface Lookup {
    id: number;
    name: string;
}

interface DesignEdit {
    subjectId: number;
    learningLevelId: number;
    title: string | null;
    source: string | null;
    essenceStatement: string | null;
    lessonsPerWeek: number | null;
    items: {category: string; name: string}[];
    sections: {title: string; content: string | null}[];
    strands: Strand[];
    competencies: Lookup[];
    values: Lookup[];
    subjects: Lookup[];
}

type Tab = 'design' | 'substrands' | 'rubrics' | 'appendix' | 'sections';

/**
 * Editing what a curriculum design holds beyond strands and outcomes - the
 * design text, each sub-strand's core competency statements, values, links
 * to other learning areas and suggestions, the assessment rubrics, the
 * appendix lists and the other sections. Each part saves on its own.
 */
@Component({
    selector: 'app-curriculum-editor',
    templateUrl: './curriculum-editor.component.html'
})
export class CurriculumEditorComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/settings/dropdowns'], title: 'Dropdowns'},
        {link: ['/cbe/assessments/curriculum-editor'], title: 'Curriculum Design Details'}
    ];
    dashboardTitle = 'CBE Settings: Curriculum Design Details';

    subjects: any[] = [];
    levels: any[] = [];
    subjectId: number | null = null;
    learningLevelId: number | null = null;
    data: DesignEdit | null = null;
    tab: Tab = 'design';
    subStrandId: number | null = null;
    strandId: number | null = null;
    dirty = false;
    saving = false;

    categories = [
        {value: 'Resource', label: 'Suggested Resources'},
        {value: 'AssessmentMethod', label: 'Suggested Assessment Methods'},
        {value: 'AssessmentTool', label: 'Suggested Assessment Tools'},
        {value: 'NonFormalProgramme', label: 'Non-formal Programmes'}
    ];
    tabs: {id: Tab; label: string}[] = [
        {id: 'design', label: 'Design'},
        {id: 'substrands', label: 'Sub-strand details'},
        {id: 'rubrics', label: 'Rubrics'},
        {id: 'appendix', label: 'Appendix'},
        {id: 'sections', label: 'Other sections'}
    ];

    constructor(private http: HttpClient, private route: ActivatedRoute, private router: Router,
                private toastr: ToastrService) {}

    ngOnInit(): void {
        this.http.get<any[]>('/learningLevels').subscribe({next: (l) => this.levels = (l || []).sort((a, b) => a.rank - b.rank)});
        let q = this.route.snapshot.queryParamMap;
        let subjectId = +(q.get('subjectId') || 0), levelId = +(q.get('learningLevelId') || 0);
        if (levelId) {
            this.learningLevelId = levelId;
            this.subjectId = subjectId || null;
            this.loadSubjects();
        }
    }

    // Grade first: only the subjects taught at the chosen grade are offered.
    pickLevel() {
        if (this.dirty && !window.confirm('Leave without saving your changes?')) return;
        this.subjectId = null;
        this.data = null;
        this.loadSubjects();
    }

    private loadSubjects() {
        this.subjects = [];
        if (!this.learningLevelId) return;
        this.http.get<any[]>('/curriculum/subjects', {params: {learningLevelId: this.learningLevelId}}).subscribe({
            next: (s) => {
                this.subjects = s || [];
                if (this.subjectId && !this.subjects.some((x) => x.id === this.subjectId)) this.subjectId = null;
                this.load();
            },
            error: (err) => this.toastr.error(err.error?.message || 'Error loading the grade\'s subjects.')
        });
    }

    pick() {
        if (this.dirty && !window.confirm('Leave without saving your changes?')) return;
        this.load();
    }

    load() {
        if (!this.subjectId || !this.learningLevelId) { this.data = null; return; }
        let pick = {subjectId: this.subjectId, learningLevelId: this.learningLevelId};
        this.router.navigate([], {relativeTo: this.route, queryParams: pick, replaceUrl: true});
        this.http.get<DesignEdit>('/curriculumDesign', {params: pick}).subscribe({
            next: (d) => {
                this.data = d;
                this.dirty = false;
                let subs = d.strands.flatMap((s) => s.subStrands);
                if (!subs.some((s) => s.id === this.subStrandId)) this.subStrandId = subs[0]?.id ?? null;
                if (!d.strands.some((s) => s.id === this.strandId)) this.strandId = d.strands[0]?.id ?? null;
            },
            error: (err) => this.toastr.error(err.error?.message || 'Error loading the curriculum design.')
        });
    }

    setTab(t: Tab) {
        if (this.dirty && !window.confirm('Leave this tab without saving your changes?')) return;
        if (this.dirty) this.load();
        this.tab = t;
    }

    touch() { this.dirty = true; }

    get subStrand(): SubStrand | undefined {
        return this.data?.strands.flatMap((s) => s.subStrands).find((s) => s.id === this.subStrandId);
    }

    get strand(): Strand | undefined {
        return this.data?.strands.find((s) => s.id === this.strandId);
    }

    selectSubStrand(id: number | null) {
        if (this.dirty && !window.confirm('Leave this sub-strand without saving your changes?')) return;
        if (this.dirty) this.load();
        this.subStrandId = id;
    }

    selectStrand(id: number | null) {
        if (this.dirty && !window.confirm('Leave this strand without saving your changes?')) return;
        if (this.dirty) this.load();
        this.strandId = id;
    }

    // ---------- Row lists ----------

    add<T>(list: T[], row: T) { list.push(row); this.touch(); }

    remove<T>(list: T[], i: number) { list.splice(i, 1); this.touch(); }

    move<T>(list: T[], i: number, by: number) {
        let j = i + by;
        if (j < 0 || j >= list.length) return;
        [list[i], list[j]] = [list[j], list[i]];
        this.touch();
    }

    blankStatement(): Statement { return {name: '', description: null, linkId: null}; }

    blankRubric(): Rubric {
        return {subStrandId: null, indicator: '', exceedsExpectations: null, meetsExpectations: null,
            approachesExpectations: null, belowExpectations: null};
    }

    itemsOf(category: string) {
        return this.data!.items.filter((i) => i.category === category);
    }

    addItem(category: string) { this.data!.items.push({category, name: ''}); this.touch(); }

    removeItem(item: {category: string; name: string}) {
        this.data!.items.splice(this.data!.items.indexOf(item), 1);
        this.touch();
    }

    // ---------- Saving ----------

    private done(what: string) {
        this.saving = false;
        this.dirty = false;
        this.toastr.success(`${what} saved.`);
        this.load();
    }

    private failed(err: any) {
        this.saving = false;
        this.toastr.error(err.error?.message || 'Error saving.');
    }

    saveDesign() {
        let d = this.data!;
        this.saving = true;
        this.http.put('/curriculumDesign/design', {
            subjectId: d.subjectId, learningLevelId: d.learningLevelId, title: d.title, source: d.source,
            essenceStatement: d.essenceStatement, lessonsPerWeek: d.lessonsPerWeek ? +d.lessonsPerWeek : null,
            items: d.items, sections: d.sections
        }).subscribe({next: () => this.done('Design'), error: (err) => this.failed(err)});
    }

    saveSubStrand() {
        let s = this.subStrand!;
        this.saving = true;
        this.http.put(`/curriculumDesign/subStrands/${s.id}`, s)
            .subscribe({next: () => this.done(`${s.code || ''} ${s.name}`.trim()), error: (err) => this.failed(err)});
    }

    saveRubrics() {
        let s = this.strand!;
        this.saving = true;
        this.http.put(`/curriculumDesign/strands/${s.id}/rubrics`, s.rubrics)
            .subscribe({next: () => this.done(`Strand ${s.code || ''} rubrics`), error: (err) => this.failed(err)});
    }

    confirmRemove<T>(list: T[], i: number, what: string) {
        Swal.fire({title: `Remove this ${what}?`, width: 380, position: 'top', icon: 'warning',
            showCancelButton: true, confirmButtonText: 'Remove'}).then((r) => { if (r.value) this.remove(list, i); });
    }
}
