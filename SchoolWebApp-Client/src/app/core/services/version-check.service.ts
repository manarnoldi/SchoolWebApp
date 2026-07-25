import {Injectable, NgZone} from '@angular/core';
import {ToastrService} from 'ngx-toastr';

/**
 * Detects when a newer build of the app has been deployed while a tab is open,
 * and prompts the user to refresh.
 *
 * How: the production build mints fresh content-hashed bundle filenames
 * (main.<hash>.js, runtime.<hash>.js, polyfills.<hash>.js, scripts.<hash>.js)
 * on every deploy. We fingerprint the bundles referenced by index.html at
 * startup, then periodically re-fetch index.html (bypassing cache) and compare.
 * On a change we show a persistent, clickable "new version" toast; clicking it
 * reloads. We never auto-reload, so unsaved form input is never lost.
 *
 * We fingerprint ALL hashed bundles, not just main - runtime.<hash>.js in
 * particular changes whenever any lazy-loaded chunk does, so route-only changes
 * are caught too.
 *
 * Pairs with the server's no-cache header on index.html, which ensures a normal
 * navigation/refresh always pulls the latest shell.
 */
@Injectable({providedIn: 'root'})
export class VersionCheckService {
    private baseline: string | null = null;
    private timerId: any = null;
    private promptShowing = false;

    constructor(private toastr: ToastrService, private zone: NgZone) {}

    /** Begin polling. intervalMinutes defaults to 10. Safe to call once at startup. */
    async init(intervalMinutes = 10): Promise<void> {
        if (this.timerId) return;
        // Baseline = the bundles the freshly-loaded shell references. If we
        // can't read a hash (dev server emits unhashed bundles), polling stays
        // off - ng serve hot-reloads anyway.
        this.baseline = await this.fetchFingerprint();
        if (!this.baseline) return;

        const ms = intervalMinutes * 60 * 1000;
        // Run the timer outside Angular so an idle user's change detection
        // isn't woken every interval just to poll.
        this.zone.runOutsideAngular(() => {
            this.timerId = setInterval(() => this.check(), ms);
        });
    }

    /** Pull index.html (cache-bypassed) and reduce it to a bundle fingerprint. */
    private async fetchFingerprint(): Promise<string | null> {
        try {
            const res = await fetch(`index.html?_=${Date.now()}`, {
                cache: 'no-store',
                credentials: 'omit'
            });
            if (!res.ok) return null;
            return this.fingerprintFrom(await res.text());
        } catch {
            // Offline / transient failure.
            return null;
        }
    }

    // Collect every hashed bundle reference (e.g. main.e0a3f324b3a4146d.js) and
    // join them into one stable string, so any bundle hash change flips it.
    private fingerprintFrom(text: string): string | null {
        const matches = text.match(/[a-z0-9]+\.[0-9a-f]{8,}\.js/gi);
        if (!matches || matches.length === 0) return null;
        return Array.from(new Set(matches)).sort().join('|');
    }

    private async check(): Promise<void> {
        if (this.promptShowing) return;
        const latest = await this.fetchFingerprint();
        if (latest && this.baseline && latest !== this.baseline) {
            // Re-enter Angular to render the toast (the timer runs outside it).
            this.zone.run(() => this.promptForReload(latest));
        }
    }

    private promptForReload(latest: string): void {
        if (this.promptShowing) return;
        this.promptShowing = true;

        // Sticky toast (timeOut: 0 = never auto-dismisses). Clicking the body
        // reloads; the close X dismisses and lets the user save their work.
        const toast = this.toastr.info(
            'A new version of ShuleNova is available. Click here to refresh.',
            'Update available',
            {
                timeOut: 0,
                extendedTimeOut: 0,
                tapToDismiss: false,
                closeButton: true,
                positionClass: 'toast-top-center'
            }
        );
        toast?.onTap?.subscribe(() => window.location.reload());

        // Adopt the new fingerprint as the baseline so a dismissed prompt only
        // reappears if YET ANOTHER deploy ships while the user keeps deferring.
        this.baseline = latest;
        toast?.onHidden?.subscribe(() => {
            this.promptShowing = false;
        });
    }
}
