import {Injectable} from '@angular/core';
import {ToastrService} from 'ngx-toastr';

/**
 * Detects when a newer build of the app has been deployed while a tab is open,
 * and prompts the user to refresh.
 *
 * How: the app's content-hashed `main.<hash>.js` filename changes on every
 * build. We capture the hash loaded at startup, then periodically re-fetch
 * index.html (bypassing cache) and compare. On a change we show a persistent
 * "new version" toast; clicking it reloads. We never auto-reload, so unsaved
 * form input is never lost.
 *
 * Pairs with the server's no-cache header on index.html, which ensures a normal
 * navigation/refresh always pulls the latest shell.
 */
@Injectable({providedIn: 'root'})
export class VersionCheckService {
    private currentHash: string | null = null;
    private prompted = false;
    private timerId: any = null;

    constructor(private toastr: ToastrService) {}

    /** Begin polling. intervalMinutes defaults to 10. Safe to call once at startup. */
    init(intervalMinutes = 10): void {
        this.currentHash = this.readLoadedHash();
        // If we can't determine the loaded build (e.g. dev server), do nothing.
        if (!this.currentHash || this.timerId) return;
        const ms = intervalMinutes * 60 * 1000;
        this.timerId = setInterval(() => this.check(), ms);
    }

    private readLoadedHash(): string | null {
        const scripts = Array.from(document.getElementsByTagName('script'));
        for (const s of scripts) {
            const m = s.src.match(/main\.([\w]+)\.js/);
            if (m) return m[1];
        }
        return null;
    }

    private async check(): Promise<void> {
        if (this.prompted) return;
        try {
            const res = await fetch(`index.html?_=${Date.now()}`, {
                cache: 'no-store'
            });
            if (!res.ok) return;
            const html = await res.text();
            const m = html.match(/main\.([\w]+)\.js/);
            const latest = m ? m[1] : null;
            if (latest && this.currentHash && latest !== this.currentHash) {
                this.prompted = true;
                if (this.timerId) {
                    clearInterval(this.timerId);
                    this.timerId = null;
                }
                const t = this.toastr.info(
                    'A new version is available. Click here to refresh.',
                    'Update available',
                    {
                        disableTimeOut: true,
                        tapToDismiss: false,
                        closeButton: true,
                        positionClass: 'toast-bottom-right'
                    }
                );
                t?.onTap?.subscribe(() => window.location.reload());
            }
        } catch {
            // Network blip / offline - ignore and try again next tick.
        }
    }
}
