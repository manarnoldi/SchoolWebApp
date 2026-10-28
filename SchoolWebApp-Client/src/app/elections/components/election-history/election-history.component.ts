import {Component} from '@angular/core';
import {STAGE_LABELS} from '@/elections/models/election-models';
import {ElectionContext} from '@/elections/services/election-context.service';

/** Election page, History tab: every stage move, who made it and their note. */
@Component({
    selector: 'app-election-history',
    template: `
        <div class="card mb-2" *ngIf="ctx.election$ | async as e">
            <div class="card-header py-2"><h6 class="mb-0"><i class="fas fa-history text-secondary me-1"></i>Stage history</h6></div>
            <div class="card-body p-0">
                <table class="table table-sm mb-0 small">
                    <tbody>
                        <tr *ngIf="e.history.length === 0"><td class="text-muted text-center py-3">Still in draft - no stage moves yet.</td></tr>
                        <tr *ngFor="let h of e.history">
                            <td class="text-nowrap" style="width: 1%;">{{ h.at | date:'d MMM y, HH:mm' }}</td>
                            <td class="text-nowrap" style="width: 1%;">{{ h.by }}</td>
                            <td><span class="text-muted">{{ stageLabels[h.fromStage] }}</span> <i class="fas fa-arrow-right mx-1 text-muted"></i>
                                <b>{{ stageLabels[h.toStage] }}</b><span *ngIf="h.note" class="text-muted"> - {{ h.note }}</span></td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>`
})
export class ElectionHistoryComponent {
    stageLabels = STAGE_LABELS;

    constructor(public ctx: ElectionContext) {}
}
