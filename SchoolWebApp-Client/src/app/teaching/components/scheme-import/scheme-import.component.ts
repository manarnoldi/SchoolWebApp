import {Component, EventEmitter, Output} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {SchemeImportResult, SchemeImportSheet} from '@/teaching/models/teaching-models';
import {SchemesOfWorkService} from '@/teaching/services/teaching-services';

/**
 * Uploading a schemes-of-work workbook: the file is checked first and every
 * sheet's problems shown by row; nothing is saved until the user confirms, and
 * then only the sheets without errors are saved. Sheets left as downloaded
 * are passed over.
 */
@Component({
    selector: 'app-scheme-import',
    templateUrl: './scheme-import.component.html'
})
export class SchemeImportComponent {
    // true when something was saved, so the page reloads.
    @Output() closed = new EventEmitter<boolean>();

    file: File | null = null;
    preview: SchemeImportResult | null = null;
    result: SchemeImportResult | null = null;
    busy = false;
    expanded = new Set<string>();

    constructor(private svc: SchemesOfWorkService, private toastr: ToastrService) {}

    onFile(e: Event) {
        let input = e.target as HTMLInputElement;
        this.file = input.files?.[0] || null;
        this.preview = null;
        this.result = null;
        if (this.file) this.check();
    }

    check() {
        if (!this.file) return;
        this.busy = true;
        this.svc.upload(this.file, false).subscribe({
            next: (r) => {
                this.busy = false;
                this.preview = r;
                // Open the sheets with problems.
                this.expanded = new Set(r.sheets.filter((s) => s.errors.length).map((s) => s.sheetName));
            },
            error: (err) => { this.busy = false; this.toastr.error(err.error?.message || 'Error checking the workbook.'); }
        });
    }

    get filledSheets(): SchemeImportSheet[] {
        return (this.result || this.preview)?.sheets.filter((s) => s.action !== 'Empty') || [];
    }

    get emptySheets(): number {
        return (this.result || this.preview)?.sheets.filter((s) => s.action === 'Empty').length || 0;
    }

    get toSave(): number {
        return this.preview?.sheets.filter((s) => s.action === 'Create' || s.action === 'Update').length || 0;
    }

    get withErrors(): number {
        return this.preview?.sheets.filter((s) => s.action === 'Skip').length || 0;
    }

    save() {
        if (!this.file || !this.toSave) return;
        this.busy = true;
        this.svc.upload(this.file, true).subscribe({
            next: (r) => {
                this.busy = false;
                this.result = r;
                let saved = r.sheets.filter((s) => s.saved).length;
                if (saved) this.toastr.success(`${saved} scheme(s) of work saved.`);
                else this.toastr.warning('Nothing was saved - see the problems listed.');
            },
            error: (err) => { this.busy = false; this.toastr.error(err.error?.message || 'Error saving the workbook.'); }
        });
    }

    toggle(s: SchemeImportSheet) {
        this.expanded.has(s.sheetName) ? this.expanded.delete(s.sheetName) : this.expanded.add(s.sheetName);
    }

    actionBadge(s: SchemeImportSheet): string {
        if (s.saved) return 'bg-success';
        switch (s.action) {
            case 'Create': return 'bg-primary';
            case 'Update': return 'bg-info';
            case 'Skip': return 'bg-danger';
            default: return 'bg-secondary';
        }
    }

    actionLabel(s: SchemeImportSheet): string {
        if (s.saved) return s.action === 'Create' ? 'Created' : 'Updated';
        switch (s.action) {
            case 'Create': return 'Will create';
            case 'Update': return 'Will replace lessons';
            case 'Skip': return 'Not saved - fix errors';
            default: return 'Empty';
        }
    }

    close() {
        this.closed.emit(!!this.result?.sheets.some((s) => s.saved));
    }
}
