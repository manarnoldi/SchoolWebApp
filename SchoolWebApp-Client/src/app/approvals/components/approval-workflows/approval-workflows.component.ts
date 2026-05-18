import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {ApprovalService} from '../../services/approval.service';
import {RoleService} from '@/security/services/role.service';
import {ApprovalWorkflow, ApprovalWorkflowStep} from '../../models/approval.models';
import Swal from 'sweetalert2';

@Component({
    selector: 'app-approval-workflows',
    templateUrl: './approval-workflows.component.html',
    styleUrl: './approval-workflows.component.scss'
})
export class ApprovalWorkflowsComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/settings/approval-workflows'], title: 'Approval Workflows'}
    ];
    dashboardTitle = 'Approval Workflows';

    workflows: ApprovalWorkflow[] = [];
    roles: any[] = [];

    form: ApprovalWorkflow = this.blankForm();
    editMode: boolean = false;

    formKeys = [
        {value: 'BudgetAmendment', label: 'Budget Amendment'},
        {value: 'Expense', label: 'Expense Request'},
        {value: 'JournalEntry', label: 'Journal Entry'},
        {value: 'CreditDebitNote', label: 'Credit / Debit Note'}
    ];

    constructor(
        private toastr: ToastrService,
        private approvalSvc: ApprovalService,
        private roleSvc: RoleService
    ) {}

    ngOnInit(): void {
        this.load();
        this.roleSvc.getAll().subscribe({
            next: (r) => { this.roles = r || []; },
            error: () => {}
        });
    }

    blankForm(): ApprovalWorkflow {
        return {
            name: '',
            formKey: '',
            description: '',
            isMakerChecker: true,
            isActive: true,
            steps: [
                {rank: 1, name: 'Reviewer', roleId: 0, isFinal: false, notifyNextApprover: true, notifyPreviousApprover: false, notifyApplicant: true},
                {rank: 2, name: 'Approver', roleId: 0, isFinal: true, notifyNextApprover: false, notifyPreviousApprover: true, notifyApplicant: true}
            ]
        };
    }

    load() {
        this.approvalSvc.getAllWorkflows().subscribe({
            next: (w) => { this.workflows = w || []; },
            error: (err) => this.toastr.error(err.error?.message || 'Error loading workflows.')
        });
    }

    addStep() {
        let rank = (this.form.steps.length > 0)
            ? Math.max(...this.form.steps.map(s => s.rank)) + 1
            : 1;
        this.form.steps.push({
            rank,
            name: '',
            roleId: 0,
            isFinal: false,
            notifyNextApprover: true,
            notifyPreviousApprover: false,
            notifyApplicant: true
        });
        this.reassignFinalFlag();
    }

    removeStep(idx: number) {
        this.form.steps.splice(idx, 1);
        this.reassignRanks();
        this.reassignFinalFlag();
    }

    moveStep(idx: number, dir: -1 | 1) {
        let target = idx + dir;
        if (target < 0 || target >= this.form.steps.length) return;
        [this.form.steps[idx], this.form.steps[target]] = [this.form.steps[target], this.form.steps[idx]];
        this.reassignRanks();
        this.reassignFinalFlag();
    }

    reassignRanks() {
        this.form.steps.forEach((s, i) => s.rank = i + 1);
    }

    reassignFinalFlag() {
        if (this.form.steps.length === 0) return;
        this.form.steps.forEach((s, i) => s.isFinal = (i === this.form.steps.length - 1));
    }

    startEdit(w: ApprovalWorkflow) {
        this.editMode = true;
        this.form = {
            id: w.id,
            name: w.name,
            formKey: w.formKey,
            description: w.description,
            isMakerChecker: w.isMakerChecker,
            isActive: w.isActive,
            steps: (w.steps || []).map(s => ({...s}))
        };
    }

    cancel() {
        this.editMode = false;
        this.form = this.blankForm();
    }

    save() {
        if (!this.form.name) { this.toastr.info('Name is required.'); return; }
        if (!this.form.formKey) { this.toastr.info('Form key is required.'); return; }
        if (this.form.steps.length === 0) { this.toastr.info('At least one step is required.'); return; }
        for (let s of this.form.steps) {
            if (!s.name) { this.toastr.info('All steps need a name.'); return; }
            if (!s.roleId) { this.toastr.info(`Select a role for step '${s.name || s.rank}'.`); return; }
        }
        this.reassignRanks();
        this.reassignFinalFlag();

        let payload = {
            name: this.form.name,
            formKey: this.form.formKey,
            description: this.form.description,
            isMakerChecker: this.form.isMakerChecker,
            isActive: this.form.isActive,
            steps: this.form.steps.map(s => ({
                rank: s.rank, name: s.name, roleId: s.roleId, isFinal: s.isFinal,
                notifyNextApprover: s.notifyNextApprover,
                notifyPreviousApprover: s.notifyPreviousApprover,
                notifyApplicant: s.notifyApplicant
            }))
        };

        Swal.fire({
            title: `${this.editMode ? 'Update' : 'Create'} workflow?`,
            width: 400, position: 'top', padding: '1em', icon: 'question',
            showCancelButton: true, confirmButtonText: this.editMode ? 'Update' : 'Create'
        }).then(r => {
            if (!r.value) return;
            let obs = this.editMode
                ? this.approvalSvc.updateWorkflow(this.form.id!, payload)
                : this.approvalSvc.createWorkflow(payload);
            obs.subscribe({
                next: () => {
                    this.toastr.success(this.editMode ? 'Workflow updated.' : 'Workflow created.');
                    this.cancel();
                    this.load();
                },
                error: (err) => this.toastr.error(err.error?.message || 'Error saving workflow.')
            });
        });
    }

    deleteWorkflow(w: ApprovalWorkflow) {
        Swal.fire({
            title: `Delete workflow '${w.name}'?`, icon: 'warning',
            width: 400, position: 'top', padding: '1em',
            showCancelButton: true, confirmButtonColor: '#d33', confirmButtonText: 'Delete'
        }).then(r => {
            if (!r.value) return;
            this.approvalSvc.deleteWorkflow(w.id!).subscribe({
                next: () => { this.toastr.success('Deleted.'); this.load(); },
                error: (err) => this.toastr.error(err.error?.message || 'Error deleting.')
            });
        });
    }

    formKeyLabel(key: string): string {
        return this.formKeys.find(f => f.value === key)?.label || key;
    }
}
