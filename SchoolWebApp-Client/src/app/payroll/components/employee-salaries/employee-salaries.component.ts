import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {EmployeeSalary, EmployeeSalaryItem, EarningType, DeductionType} from '@/payroll/models/payroll-models';
import {EmployeeSalaryService, EarningTypeService, DeductionTypeService} from '@/payroll/services/payroll-services';
import {StaffDetailsService} from '@/staff/services/staff-details.service';
import {Status} from '@/core/enums/status';

// One grid row per active staff member. Staff details is the single source of
// truth for who appears here: a staff member with no salary record yet still
// gets a row (salaryId null) so their pay can be entered in place, and the
// batch save inserts it. Rows are only sent to the server once edited.
interface SalaryRow {
    staffId: number;
    staffName: string;
    staffUpi: string;
    // Statutory numbers live on the staff record, not the salary. Carried here so
    // gaps can be spotted and fixed before payroll is processed.
    kraPinNo: string | null;
    idNumber: string | null;
    nssfNo: string | null;
    nhifNo: string | null;
    // Kept on the staff list but not paid through payroll. The row is shown
    // disabled so they can be put back on the payroll from here.
    excludeFromPayroll: boolean;
    salaryId: number | null;
    basicSalary: number;
    houseAllowance: number;
    transportAllowance: number;
    otherAllowances: number;
    effectiveDate: string;
    isActive: boolean;
    notes: string | null;
    itemCount: number;
    baseline: string;
}

@Component({
    selector: 'app-payroll-employee-salaries',
    templateUrl: './employee-salaries.component.html',
    styleUrls: ['./employee-salaries.component.scss']
})
export class PayrollEmployeeSalariesComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/payroll/employee-salaries'], title: 'Employee Salaries'}
    ];
    dashboardTitle = 'Payroll: Employee Salaries';

    rows: SalaryRow[] = [];
    staffList: any[] = [];
    earningTypes: EarningType[] = [];
    deductionTypes: DeductionType[] = [];

    // Salary records belonging to staff who are no longer active. Those staff are
    // not in this grid at all, so surface the count rather than let the totals
    // silently disagree with the staff list. (Staff excluded from payroll ARE
    // listed, just disabled, so they need no such warning.)
    orphanedCount = 0;

    item: EmployeeSalary = new EmployeeSalary({isActive: true, items: []});
    editMode = false;
    showForm = false;
    saving = false;
    loading = false;

    filterStaffId: any = null;
    filterHasSalary: any = null;
    bulkEffectiveDate: string = new Date().toISOString().substring(0, 10);

    page = 1;
    pageSize = 10;
    pageChanged = (p: number) => { this.page = p; };
    pageSizeChanged = (s: number) => { this.pageSize = s; };

    constructor(
        private toastr: ToastrService,
        private svc: EmployeeSalaryService,
        private staffSvc: StaffDetailsService,
        private earningTypeSvc: EarningTypeService,
        private deductionTypeSvc: DeductionTypeService
    ) {}

    ngOnInit(): void {
        this.loading = true;
        forkJoin([
            this.svc.get('/employeeSalaries'),
            this.staffSvc.getBySearchDetails(Status.Active, null as any, null as any),
            this.earningTypeSvc.get('/earningTypes'),
            this.deductionTypeSvc.get('/deductionTypes')
        ]).subscribe({
            next: ([salaries, staff, earnings, deductions]) => {
                // Staff excluded from payroll are listed too, but disabled. Hiding
                // them left no way to put someone back on the payroll from here.
                this.staffList = (staff || []).sort((a: any, b: any) =>
                    (a.fullName || '').localeCompare(b.fullName || ''));
                // System-computed types (basic, house, transport; PAYE, NSSF, SHIF,
                // Housing Levy) are worked out by payroll itself. Offering them as extra
                // lines would let them be counted twice, so they are left out.
                this.earningTypes = (earnings || []).filter((e: EarningType) => e.isActive && !e.isSystemComputed);
                this.deductionTypes = (deductions || []).filter((d: DeductionType) => d.isActive && !d.isSystemComputed);
                this.buildRows(salaries || []);
                this.loading = false;
            },
            error: (err) => { this.loading = false; this.toastr.error(err.error); }
        });
    }

    // Reloads salaries only - the staff list is the stable side of the join.
    load(): void {
        this.svc.get('/employeeSalaries').subscribe({
            next: (r) => this.buildRows(r || []),
            error: (err) => this.toastr.error(err.error)
        });
    }

    private buildRows(salaries: EmployeeSalary[]): void {
        // Most recent salary wins where a staff member somehow has more than one,
        // matching what the batch endpoint updates.
        let byStaff = new Map<number, any>();
        for (let s of salaries as any[]) {
            let existing = byStaff.get(+s.staffDetailsId);
            if (!existing || (s.effectiveDate || '') > (existing.effectiveDate || ''))
                byStaff.set(+s.staffDetailsId, s);
        }

        let today = new Date().toISOString().substring(0, 10);
        this.rows = this.staffList.map((st: any) => {
            let sal = byStaff.get(+st.id);
            let row: SalaryRow = {
                staffId: +st.id,
                staffName: st.fullName || `${st.firstName || ''} ${st.lastName || ''}`.trim(),
                staffUpi: st.upi || '',
                kraPinNo: st.kraPinNo ?? null,
                idNumber: st.idNumber ?? null,
                nssfNo: st.nssfNo ?? null,
                nhifNo: st.nhifNo ?? null,
                excludeFromPayroll: !!st.excludeFromPayroll,
                salaryId: sal ? +sal.id : null,
                basicSalary: sal ? +sal.basicSalary || 0 : 0,
                houseAllowance: sal ? +sal.houseAllowance || 0 : 0,
                transportAllowance: sal ? +sal.transportAllowance || 0 : 0,
                otherAllowances: sal ? +sal.otherAllowances || 0 : 0,
                effectiveDate: sal?.effectiveDate ? sal.effectiveDate.substring(0, 10) : today,
                isActive: sal ? !!sal.isActive : true,
                notes: sal ? sal.notes ?? null : null,
                itemCount: sal ? (sal.items || []).length : 0,
                baseline: ''
            };
            row.baseline = this.snapshot(row);
            return row;
        });

        let listedIds = new Set(this.staffList.map((s: any) => +s.id));
        this.orphanedCount = (salaries as any[]).filter((s) => !listedIds.has(+s.staffDetailsId)).length;
        this.page = 1;
    }

    // --- Change tracking ---
    private snapshot(r: SalaryRow): string {
        return JSON.stringify([
            +r.basicSalary || 0, +r.houseAllowance || 0, +r.transportAllowance || 0,
            +r.otherAllowances || 0, r.effectiveDate || '', !!r.isActive, r.notes || ''
        ]);
    }

    isDirty(r: SalaryRow): boolean { return this.snapshot(r) !== r.baseline; }

    // Excluded rows are never saved, even if something managed to change one -
    // a salary must not be created for someone who is off the payroll.
    dirtyRows(): SalaryRow[] {
        return this.rows.filter((r) => !r.excludeFromPayroll && this.isDirty(r));
    }

    // --- Derived values ---
    grossFor(r: SalaryRow): number {
        return (+r.basicSalary || 0) + (+r.houseAllowance || 0) +
            (+r.transportAllowance || 0) + (+r.otherAllowances || 0);
    }

    // Counts describe the people payroll will actually pay, so excluded staff are
    // reported separately rather than folded into "with"/"without salary".
    payrollRows(): SalaryRow[] { return this.rows.filter((r) => !r.excludeFromPayroll); }
    withSalaryCount(): number { return this.payrollRows().filter((r) => r.salaryId != null).length; }
    withoutSalaryCount(): number { return this.payrollRows().filter((r) => r.salaryId == null).length; }
    excludedCount(): number { return this.rows.filter((r) => r.excludeFromPayroll).length; }

    filtered(): SalaryRow[] {
        return this.rows.filter((r) => {
            if (this.filterStaffId != null && r.staffId != this.filterStaffId) return false;
            if (this.filterHasSalary === 'excluded') return r.excludeFromPayroll;
            if (this.filterHasSalary === true) return !r.excludeFromPayroll && r.salaryId != null;
            if (this.filterHasSalary === false) return !r.excludeFromPayroll && r.salaryId == null;
            return true;
        });
    }

    clearFilters(): void { this.filterStaffId = null; this.filterHasSalary = null; this.page = 1; }

    applyDateToAll(): void {
        if (!this.bulkEffectiveDate) { this.toastr.warning('Pick an effective date first.'); return; }
        let targets = this.filtered().filter((r) => !r.excludeFromPayroll);
        for (let r of targets) r.effectiveDate = this.bulkEffectiveDate;
        this.toastr.info(`Effective date applied to ${targets.length} row(s). Save to persist.`);
    }

    resetChanges(): void {
        if (this.dirtyRows().length === 0) return;
        this.load();
    }

    // --- Batch save ---
    saveAll(): void {
        let dirty = this.dirtyRows();
        if (dirty.length === 0) { this.toastr.info('No changes to save.'); return; }

        let invalid = dirty.find((r) => !r.effectiveDate);
        if (invalid) { this.toastr.warning(`Effective date is required (${invalid.staffName}).`); return; }
        invalid = dirty.find((r) => this.grossFor(r) < 0 || +r.basicSalary < 0 || +r.houseAllowance < 0 ||
            +r.transportAllowance < 0 || +r.otherAllowances < 0);
        if (invalid) { this.toastr.warning(`Amounts cannot be negative (${invalid.staffName}).`); return; }

        let payload = dirty.map((r) => ({
            staffDetailsId: r.staffId,
            basicSalary: +r.basicSalary || 0,
            houseAllowance: +r.houseAllowance || 0,
            transportAllowance: +r.transportAllowance || 0,
            otherAllowances: +r.otherAllowances || 0,
            effectiveDate: r.effectiveDate,
            isActive: !!r.isActive,
            notes: r.notes || null,
            items: []
        }));

        this.saving = true;
        this.svc.saveBatch(payload).subscribe({
            next: (res: any) => {
                this.saving = false;
                let ins = res?.inserted ?? 0;
                let upd = res?.updated ?? 0;
                this.toastr.success(`Saved - ${ins} added, ${upd} updated.`);
                this.load();
            },
            error: (err) => {
                this.saving = false;
                this.toastr.error(err.error?.message || err.error || 'Error saving salaries.');
            }
        });
    }

    // --- Payroll details cleanup (statutory numbers, exclude from payroll) ---
    // This belongs before processing: a missing KRA PIN or a supplier left on the
    // payroll has to be fixed here, not discovered on the payslips afterwards.
    showPayrollDetails = false;
    savingPayrollDetails = false;
    detailsFor: SalaryRow | null = null;
    details = {kraPinNo: '', idNumber: '', nssfNo: '', nhifNo: '', excludeFromPayroll: false};

    // Someone off the payroll is never paid or filed for, so blank statutory
    // numbers are not a problem for them and are not reported as missing.
    missingIdentifiers(r: SalaryRow): string[] {
        if (r.excludeFromPayroll) return [];
        let missing: string[] = [];
        if (!r.kraPinNo) missing.push('KRA PIN');
        if (!r.idNumber) missing.push('ID No');
        if (!r.nssfNo) missing.push('NSSF No');
        if (!r.nhifNo) missing.push('SHA No');
        return missing;
    }

    incompleteCount(): number {
        return this.rows.filter((r) => this.missingIdentifiers(r).length > 0).length;
    }

    editPayrollDetails(r: SalaryRow): void {
        this.detailsFor = r;
        this.details = {
            kraPinNo: r.kraPinNo || '',
            idNumber: r.idNumber || '',
            nssfNo: r.nssfNo || '',
            nhifNo: r.nhifNo || '',
            excludeFromPayroll: r.excludeFromPayroll
        };
        this.showPayrollDetails = true;
    }

    cancelPayrollDetails(): void {
        this.showPayrollDetails = false;
        this.detailsFor = null;
    }

    savePayrollDetails(): void {
        let target = this.detailsFor;
        if (!target) return;
        this.savingPayrollDetails = true;
        this.staffSvc.updatePayrollDetails(target.staffId, {
            kraPinNo: this.details.kraPinNo?.trim() || null,
            idNumber: this.details.idNumber?.trim() || null,
            nssfNo: this.details.nssfNo?.trim() || null,
            nhifNo: this.details.nhifNo?.trim() || null,
            excludeFromPayroll: !!this.details.excludeFromPayroll
        }).subscribe({
            next: () => {
                this.savingPayrollDetails = false;
                // Patch in place rather than reloading: a reload rebuilds every row
                // from the server and would throw away salary amounts typed into the
                // grid but not yet saved.
                target!.kraPinNo = this.details.kraPinNo?.trim() || null;
                target!.idNumber = this.details.idNumber?.trim() || null;
                target!.nssfNo = this.details.nssfNo?.trim() || null;
                target!.nhifNo = this.details.nhifNo?.trim() || null;

                // The row stays put either way - excluding just disables it, so the
                // person can be put back on the payroll from the same place.
                let wasExcluded = target!.excludeFromPayroll;
                target!.excludeFromPayroll = this.details.excludeFromPayroll;
                let staff = this.staffList.find((s: any) => +s.id === target!.staffId);
                if (staff) staff.excludeFromPayroll = this.details.excludeFromPayroll;

                if (this.details.excludeFromPayroll && !wasExcluded)
                    this.toastr.success(`${target!.staffName} taken off the payroll.`);
                else if (!this.details.excludeFromPayroll && wasExcluded)
                    this.toastr.success(`${target!.staffName} put back on the payroll.`);
                else
                    this.toastr.success(`Payroll details updated for ${target!.staffName}.`);

                this.showPayrollDetails = false;
                this.detailsFor = null;
            },
            error: (err) => {
                this.savingPayrollDetails = false;
                this.toastr.error(err.error?.message || err.error || 'Error saving payroll details.');
            }
        });
    }

    // --- Per-employee details modal (earnings / deductions line items) ---
    openDetails(r: SalaryRow): void {
        if (r.salaryId == null) {
            // No record yet: open the modal pre-filled from the grid row so the
            // header can be saved first, which unlocks the line-item sections.
            this.item = new EmployeeSalary({
                staffDetailsId: r.staffId,
                basicSalary: +r.basicSalary || 0,
                houseAllowance: +r.houseAllowance || 0,
                transportAllowance: +r.transportAllowance || 0,
                otherAllowances: +r.otherAllowances || 0,
                effectiveDate: r.effectiveDate,
                isActive: r.isActive,
                notes: r.notes ?? undefined,
                items: []
            });
            this.editMode = false;
            this.showForm = true;
            return;
        }
        this.svc.getById(r.salaryId, '/employeeSalaries').subscribe({
            next: (raw: any) => {
                let full: any = Array.isArray(raw) ? raw[0] : raw;
                this.item = new EmployeeSalary(full);
                this.item.items = this.hydrateItems(full?.items);
                this.item.effectiveDate = full.effectiveDate ? full.effectiveDate.substring(0, 10) : '';
                this.editMode = true;
                this.showForm = true;
            },
            error: (err) => this.toastr.error(err.error)
        });
    }

    cancel(): void { this.showForm = false; }

    // Lines are grouped by `kind`, not by which type id is set: a line that has
    // just been added has neither id yet, and filtering on the ids made it vanish
    // the moment it was created - which looked like the Add buttons doing nothing.
    getEarningItems(): EmployeeSalaryItem[] {
        return (this.item.items || []).filter((i) => i.kind === 'earning');
    }

    getDeductionItems(): EmployeeSalaryItem[] {
        return (this.item.items || []).filter((i) => i.kind === 'deduction');
    }

    // Rebuilds line items coming back from the API, tagging each with the kind it
    // belongs to so the two tables can render it before a type is picked.
    private hydrateItems(raw: any[]): EmployeeSalaryItem[] {
        return (raw || []).map((i: any) => Object.assign(new EmployeeSalaryItem(), i, {
            kind: i.deductionTypeId != null ? 'deduction' : 'earning'
        }));
    }

    addEarningItem(): void {
        this.item.items.push(Object.assign(new EmployeeSalaryItem(),
            {kind: 'earning', earningTypeId: null, deductionTypeId: null, amount: 0}));
    }

    addDeductionItem(): void {
        this.item.items.push(Object.assign(new EmployeeSalaryItem(),
            {kind: 'deduction', deductionTypeId: null, earningTypeId: null, amount: 0}));
    }

    removeItem(line: EmployeeSalaryItem): void {
        let i = this.item.items.indexOf(line);
        if (i >= 0) this.item.items.splice(i, 1);
    }

    onEarningTypeChange(line: EmployeeSalaryItem): void { line.deductionTypeId = null; }

    onDeductionTypeChange(line: EmployeeSalaryItem): void { line.earningTypeId = null; }

    getTotalEarnings(): number {
        return this.getEarningItems().reduce((s, i) => s + (+i.amount || 0), 0);
    }

    getTotalDeductions(): number {
        return this.getDeductionItems().reduce((s, i) => s + (+i.amount || 0), 0);
    }

    getGrossTotal(): number {
        return (+this.item.basicSalary || 0) + (+this.item.houseAllowance || 0) +
            (+this.item.transportAllowance || 0) + (+this.item.otherAllowances || 0) +
            this.getTotalEarnings();
    }

    save(): void {
        if (!this.item.staffDetailsId) { this.toastr.warning('Staff member is required.'); return; }
        if (!this.item.effectiveDate) { this.toastr.warning('Effective date is required.'); return; }

        let lines = this.item.items || [];
        if (lines.some((i) => i.earningTypeId == null && i.deductionTypeId == null)) {
            this.toastr.warning('Choose a type for every earning and deduction line, or remove the blank ones.');
            return;
        }
        if (lines.some((i) => (+i.amount! || 0) <= 0)) {
            this.toastr.warning('Every earning and deduction line needs an amount greater than zero.');
            return;
        }

        // `kind` is a client-side grouping marker; the API has no such field.
        let items = lines.map(({kind, ...rest}) => rest);
        let payload = new EmployeeSalary({...this.item, items: items as any});
        let req = this.editMode
            ? this.svc.updateById(parseInt(this.item.id), payload)
            : this.svc.create('/employeeSalaries', payload);
        req.subscribe({
            next: (saved: any) => {
                this.toastr.success('Saved.');
                if (!this.editMode && saved?.id) {
                    // Re-open in edit mode so the line-item sections become available.
                    this.svc.getById(+saved.id, '/employeeSalaries').subscribe({
                        next: (raw: any) => {
                            let full: any = Array.isArray(raw) ? raw[0] : raw;
                            this.item = new EmployeeSalary(full);
                            this.item.items = this.hydrateItems(full?.items);
                            this.item.effectiveDate = full.effectiveDate ? full.effectiveDate.substring(0, 10) : '';
                            this.editMode = true;
                            this.load();
                        }
                    });
                } else {
                    this.showForm = false;
                    this.load();
                }
            },
            error: (err) => this.toastr.error(err.error?.message || 'Error.')
        });
    }

    delete(r: SalaryRow): void {
        if (r.salaryId == null) return;
        Swal.fire({
            title: 'Delete salary?',
            text: `This removes the salary record for ${r.staffName}. The staff member stays on the list.`,
            icon: 'warning', showCancelButton: true, confirmButtonText: 'Delete', confirmButtonColor: '#d33'
        }).then((res) => {
            if (res.value) {
                this.svc.delete('/employeeSalaries', r.salaryId as number).subscribe({
                    next: () => { this.toastr.success('Deleted.'); this.load(); },
                    error: (err) => this.toastr.error(err.error?.message || 'Error.')
                });
            }
        });
    }
}
