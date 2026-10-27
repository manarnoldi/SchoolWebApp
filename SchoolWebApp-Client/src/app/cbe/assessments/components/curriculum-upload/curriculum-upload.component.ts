import {Component, OnInit} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {ToastrService} from 'ngx-toastr';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {blobError, safeFileName, saveBlob} from '@/teaching/utils/download';

interface ImportIssue {
    sheet: string | null;
    row: number | null;
    message: string;
}

interface CurriculumImportResult {
    committed: boolean;
    saved: boolean;
    subjectName: string | null;
    learningLevelName: string | null;
    subjectOutcomes: number;
    strands: number;
    subStrands: number;
    lessons: number;
    specificOutcomes: number;
    learningExperiences: number;
    keyQuestions: number;
    pcis: number;
    coreCompetencies: number;
    values: number;
    learningAreaLinks: number;
    rubrics: number;
    appendixItems: number;
    sections: number;
    changes: {[what: string]: number};
    errors: ImportIssue[];
    warnings: ImportIssue[];
}

/**
 * A subject's curriculum for one grade in Excel, filled from the KICD
 * curriculum design: download the template (pre-filled with what is saved),
 * fill it in, upload it - checked first, saved on confirm. Strands, sub-strands
 * and outcomes already saved are matched by name and updated, so uploading
 * again after a correction is safe.
 */
@Component({
    selector: 'app-curriculum-upload',
    templateUrl: './curriculum-upload.component.html'
})
export class CurriculumUploadComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/settings/dropdowns'], title: 'Dropdowns'},
        {link: ['/cbe/assessments/curriculum-upload'], title: 'Curriculum Upload'}
    ];
    dashboardTitle = 'CBE Settings: Curriculum Upload (Excel)';

    subjects: any[] = [];
    levels: any[] = [];
    subjectId: number | null = null;
    learningLevelId: number | null = null;

    file: File | null = null;
    preview: CurriculumImportResult | null = null;
    result: CurriculumImportResult | null = null;
    busy = false;

    constructor(private http: HttpClient, private toastr: ToastrService) {}

    ngOnInit(): void {
        this.http.get<any[]>('/learningLevels').subscribe({next: (l) => this.levels = (l || []).sort((a, b) => a.rank - b.rank)});
    }

    // Grade first: only the subjects taught at the chosen grade are offered.
    loadSubjects() {
        this.subjectId = null;
        this.subjects = [];
        if (!this.learningLevelId) return;
        this.http.get<any[]>('/curriculum/subjects', {params: {learningLevelId: this.learningLevelId}}).subscribe({
            next: (s) => this.subjects = s || [],
            error: (err) => this.toastr.error(err.error?.message || 'Error loading the grade\'s subjects.')
        });
    }

    download() {
        if (!this.subjectId || !this.learningLevelId) return;
        let subject = this.subjects.find((s) => s.id === this.subjectId)?.name || 'subject';
        let level = this.levels.find((l) => l.id === this.learningLevelId)?.name || 'grade';
        this.busy = true;
        this.http.get('/curriculumImports/template', {
            params: {subjectId: this.subjectId, learningLevelId: this.learningLevelId}, responseType: 'blob'
        }).subscribe({
            next: (blob) => { this.busy = false; saveBlob(blob, safeFileName(`Curriculum - ${subject} - ${level}`) + '.xlsx'); },
            error: async (err) => { this.busy = false; this.toastr.error(await blobError(err, 'Error preparing the template.')); }
        });
    }

    onFile(e: Event) {
        this.file = (e.target as HTMLInputElement).files?.[0] || null;
        this.preview = null;
        this.result = null;
        if (this.file) this.send(false);
    }

    save() {
        this.send(true);
    }

    private send(commit: boolean) {
        if (!this.file) return;
        let form = new FormData();
        form.append('file', this.file, this.file.name);
        this.busy = true;
        this.http.post<CurriculumImportResult>('/curriculumImports', form, {params: {commit}}).subscribe({
            next: (r) => {
                this.busy = false;
                if (commit) {
                    this.result = r;
                    if (r.saved) this.toastr.success(`${r.subjectName} ${r.learningLevelName} curriculum saved.`);
                } else this.preview = r;
            },
            error: (err) => { this.busy = false; this.toastr.error(err.error?.message || 'Error reading the workbook.'); }
        });
    }

    get shown(): CurriculumImportResult | null {
        return this.result || this.preview;
    }

    changeList(r: CurriculumImportResult): {what: string; n: number}[] {
        return Object.keys(r.changes || {}).map((what) => ({what, n: r.changes[what]}));
    }

    reset() {
        this.file = null;
        this.preview = null;
        this.result = null;
    }
}
