// Baseline-snapshot change tracker for the batch-save grid screens (values,
// community service, exam results, responsibilities, co-curriculum, student
// assessments).
//
// These grids used to re-submit every cell that had a value on each save, which
// on slow school links produced large uploads (and the Kestrel slow-upload
// timeouts). With this tracker each screen snapshots the editable values of the
// already-saved cells at load time (keyed by the cell's DB id, `existingId`).
// On save it asks the tracker which cells changed and submits only those, plus
// any brand-new cells (which have no `existingId` yet). After a successful save
// the screen reloads and re-snapshots so a second save in the same session
// doesn't resubmit what was just persisted.
//
// The comparable "value" is whatever object of editable fields a screen passes
// in (e.g. {score, description} or {gradeId, description}); it's serialised for
// a stable, order-independent-enough comparison. Keep the field order in the
// snapshot and the check identical for a given screen.
export class DirtyTracker {
    private baseline = new Map<string | number, string>();

    private norm(value: unknown): string {
        return JSON.stringify(value ?? null);
    }

    /** Record the loaded/saved state of a persisted cell. No-op for new cells. */
    snapshot(existingId: string | number | null | undefined, value: unknown): void {
        if (existingId == null) return;
        this.baseline.set(existingId, this.norm(value));
    }

    /**
     * True if a persisted cell's current value differs from its snapshot.
     * A cell with no recorded baseline (unknown id) is treated as changed so
     * it is never silently dropped.
     */
    hasChanged(existingId: string | number | null | undefined, value: unknown): boolean {
        if (existingId == null) return true; // new cell - always submit
        let base = this.baseline.get(existingId);
        return base === undefined || base !== this.norm(value);
    }

    /** Forget all snapshots (call before re-snapshotting a fresh load). */
    reset(): void {
        this.baseline.clear();
    }
}
