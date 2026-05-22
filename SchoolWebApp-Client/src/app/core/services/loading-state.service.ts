import {Injectable} from '@angular/core';

/**
 * Global toggle that lets components suspend the page-level ngx-spinner while
 * they handle their own loading indicators. Use this when a screen has multiple
 * inline loaders (e.g. dashboard widgets) and the full-page spinner would just
 * be noise.
 *
 * Refcounted so multiple components can independently suspend / resume without
 * stepping on each other. The interceptor checks `isSuspended` and skips its
 * normal show/hide flow while suspension is active.
 *
 * Usage:
 *   ngOnInit() { this.loadingState.suspend(); ...load... }
 *   ngOnDestroy() { this.loadingState.resume(); }
 *
 * Pair with `finalize` to make sure resume() runs even on error:
 *   this.svc.get(...).pipe(finalize(() => this.loadingState.resume()))
 */
@Injectable({providedIn: 'root'})
export class LoadingStateService {
    private suspendCount = 0;

    suspend(): void {
        this.suspendCount++;
    }

    resume(): void {
        this.suspendCount = Math.max(0, this.suspendCount - 1);
    }

    get isSuspended(): boolean {
        return this.suspendCount > 0;
    }
}
