import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {
    apiError,
    ElectionDetail,
    ElectionOptions,
    ElectionStage,
    SaveElection,
    VoterRegisterMode
} from '@/elections/models/election-models';
import {ElectionsService} from '@/elections/services/elections.service';

// datetime-local inputs hold local time without a zone; the API keeps UTC.
function toLocalInput(utc: string | null): string | null {
    if (!utc) return null;
    const d = new Date(utc);
    const pad = (n: number) => String(n).padStart(2, '0');
    return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
}

function todayAt(hours: number, minutes: number): string | null {
    const d = new Date();
    d.setHours(hours, minutes, 0, 0);
    return toLocalInput(d.toISOString());
}

function toUtc(local: string | null): string | null {
    return local ? new Date(local).toISOString() : null;
}

/** Creates an election, or edits one's name, year, register mode and polling window. */
@Component({
    selector: 'app-election-form',
    templateUrl: './election-form.component.html'
})
export class ElectionFormComponent implements OnInit {
    @Input() options!: ElectionOptions;
    @Input() election: ElectionDetail | null = null;
    @Output() saved = new EventEmitter<ElectionDetail>();
    @Output() closed = new EventEmitter<void>();

    model: SaveElection = {
        name: '',
        description: null,
        academicYearId: null,
        registerMode: VoterRegisterMode.Frozen,
        pollingStart: null,
        pollingEnd: null,
        dashboardRefreshSeconds: 10
    };
    saving = false;
    Mode = VoterRegisterMode;

    constructor(private svc: ElectionsService, private toastr: ToastrService) {}

    ngOnInit(): void {
        const e = this.election;
        this.model = e
            ? {
                  name: e.name,
                  description: e.description,
                  academicYearId: e.academicYearId,
                  registerMode: e.registerMode,
                  pollingStart: toLocalInput(e.pollingStart),
                  pollingEnd: toLocalInput(e.pollingEnd),
                  dashboardRefreshSeconds: e.dashboardRefreshSeconds
              }
            : {
                  ...this.model,
                  // A new election defaults to polling today, 08:00 to 14:00.
                  pollingStart: todayAt(8, 0),
                  pollingEnd: todayAt(14, 0),
                  academicYearId: this.options.currentAcademicYearId,
                  registerMode: this.options.defaultRegisterMode,
                  dashboardRefreshSeconds: this.options.defaultRefreshSeconds
              };
    }

    // The year picks the classes contests are drawn from; it is fixed once nominations open.
    get yearLocked(): boolean {
        return !!this.election && this.election.stage !== ElectionStage.Draft;
    }

    get modeLocked(): boolean {
        return !!this.election?.registerFrozenAt;
    }

    save() {
        if (!this.model.name.trim()) {
            this.toastr.warning('Enter the election name.');
            return;
        }
        if (!this.model.academicYearId) {
            this.toastr.warning('Choose the academic year.');
            return;
        }
        const dto: SaveElection = {
            ...this.model,
            pollingStart: toUtc(this.model.pollingStart),
            pollingEnd: toUtc(this.model.pollingEnd),
            dashboardRefreshSeconds: Number(this.model.dashboardRefreshSeconds) || 10
        };
        this.saving = true;
        const call = this.election ? this.svc.update(this.election.id, dto) : this.svc.create(dto);
        call.subscribe({
            next: (d) => {
                this.saving = false;
                this.toastr.success(this.election ? 'Election saved.' : 'Election created.');
                this.saved.emit(d);
            },
            error: (err) => {
                this.saving = false;
                this.toastr.error(apiError(err, 'Error saving the election.'));
            }
        });
    }
}
