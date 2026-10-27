import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {forkJoin, of} from 'rxjs';
import {
    apiError,
    Candidate,
    EligibleStudent,
    NominationContest,
    NominationPosition,
    Nominations
} from '@/elections/models/election-models';
import {ElectionsService} from '@/elections/services/elections.service';

/**
 * Nominates a student in a race (with their running mate on a joint ticket), or
 * edits a nomination's slogan and running mate. Only students who can stand in
 * the race are listed; ones already standing elsewhere are shown but cannot be picked.
 */
@Component({
    selector: 'app-nominate-form',
    templateUrl: './nominate-form.component.html'
})
export class NominateFormComponent implements OnInit {
    @Input() position!: NominationPosition;
    @Input() contest!: NominationContest;
    @Input() candidate: Candidate | null = null;
    @Output() saved = new EventEmitter<Nominations>();
    @Output() closed = new EventEmitter<void>();

    students: EligibleStudent[] = [];
    mates: EligibleStudent[] = [];
    studentId: number | null = null;
    mateId: number | null = null;
    slogan = '';
    studentSearch = '';
    mateSearch = '';
    loading = false;
    saving = false;

    constructor(private svc: ElectionsService, private toastr: ToastrService) {}

    ngOnInit(): void {
        const c = this.candidate;
        if (c) {
            this.studentId = c.studentId;
            this.mateId = c.runningMateStudentId;
            this.slogan = c.slogan || '';
        }
        this.loading = true;
        forkJoin({
            students: c ? of([]) : this.svc.eligible(this.contest.id, 'candidate'),
            mates: this.position.runningMateTitle ? this.svc.eligible(this.contest.id, 'mate') : of([])
        }).subscribe({
            next: (r) => {
                this.students = r.students;
                this.mates = r.mates;
                this.loading = false;
            },
            error: (err) => {
                this.loading = false;
                this.toastr.error(apiError(err, 'Error loading the students who can stand.'));
            }
        });
    }

    private match(list: EligibleStudent[], q: string): EligibleStudent[] {
        const s = q.trim().toLowerCase();
        return list.filter((x) => !s || x.name.toLowerCase().includes(s) || x.upi.toLowerCase().includes(s)
            || x.className.toLowerCase().includes(s));
    }

    get shownStudents(): EligibleStudent[] {
        return this.match(this.students, this.studentSearch).slice(0, 200);
    }

    // The candidate is never their own running mate; this nomination's own mate is free to keep.
    get shownMates(): EligibleStudent[] {
        return this.match(this.mates, this.mateSearch)
            .filter((m) => m.studentId !== this.studentId)
            .map((m) => (this.candidate && m.studentId === this.candidate.runningMateStudentId ? {...m, taken: null} : m))
            .slice(0, 200);
    }

    get pickedStudent(): EligibleStudent | undefined {
        return this.students.find((s) => s.studentId === this.studentId);
    }

    get pickedMate(): EligibleStudent | undefined {
        return this.mates.find((s) => s.studentId === this.mateId);
    }

    pickStudent(s: EligibleStudent) {
        if (s.taken) return;
        this.studentId = s.studentId;
        if (this.mateId === s.studentId) this.mateId = null;
    }

    pickMate(s: EligibleStudent) {
        if (s.taken) return;
        this.mateId = s.studentId;
    }

    save() {
        if (!this.studentId) {
            this.toastr.warning('Choose the student standing.');
            return;
        }
        if (this.position.runningMateTitle && !this.mateId) {
            this.toastr.warning(`Choose the ${this.position.runningMateTitle}.`);
            return;
        }
        const dto = {studentId: this.studentId, runningMateStudentId: this.mateId, slogan: this.slogan.trim() || null};
        this.saving = true;
        const call = this.candidate ? this.svc.updateCandidate(this.candidate.id, dto) : this.svc.nominate(this.contest.id, dto);
        call.subscribe({
            next: (n) => {
                this.saving = false;
                this.toastr.success(this.candidate ? 'Nomination saved.' : 'Nominated.');
                this.saved.emit(n);
            },
            error: (err) => {
                this.saving = false;
                this.toastr.error(apiError(err, 'Error saving the nomination.'));
            }
        });
    }
}
