import {Injectable} from '@angular/core';
import {
    HttpErrorResponse,
    HttpEvent,
    HttpHandler,
    HttpInterceptor,
    HttpRequest
} from '@angular/common/http';
import {Observable, throwError} from 'rxjs';
import {catchError, finalize} from 'rxjs/operators';
import {NgxSpinnerService} from 'ngx-spinner';
import {LoadingStateService} from '../services/loading-state.service';

/**
 * Header that callers can set to opt their request out of the global spinner.
 * Useful for background work like dashboard widgets or polling that should not
 * cause the full-screen loader to flash on/off and block user input.
 *
 * Usage:
 *   this.http.get(url, { headers: new HttpHeaders().set(SKIP_LOADER_HEADER, 'true') })
 */
export const SKIP_LOADER_HEADER = 'X-Skip-Loader';

@Injectable()
export class LoaderInterceptor implements HttpInterceptor {
    // Tracks how many requests are currently in flight (excluding skipped ones).
    // The spinner only hides when this returns to 0, so overlapping requests
    // never cause flicker.
    private activeRequests = 0;

    // Whether the spinner is actually visible on screen right now.
    private spinnerVisible = false;

    // Pending "show the spinner" timer. We delay showing the spinner by a short
    // window so requests that finish quickly never trigger it at all.
    private showTimer: ReturnType<typeof setTimeout> | null = null;

    // Pending "hide the spinner" timer. We keep the spinner up for a short
    // settling window after activeRequests drops to 0 so cascades of requests
    // (e.g. dashboard widgets where one finishing triggers the next) read as
    // one continuous load rather than several blinks.
    private hideTimer: ReturnType<typeof setTimeout> | null = null;

    // How long a request must run before the spinner appears. Tuned so most
    // cached / fast requests stay invisible, while genuinely slow requests
    // still get a loading indicator after a brief moment.
    private readonly SHOW_DELAY_MS = 250;

    // How long to wait after the last in-flight request before actually
    // hiding the spinner. If a new request arrives within this window, the
    // pending hide is cancelled and the spinner stays up smoothly.
    private readonly HIDE_DELAY_MS = 400;

    constructor(
        private spinner: NgxSpinnerService,
        private loadingState: LoadingStateService
    ) {}

    intercept(
        req: HttpRequest<any>,
        next: HttpHandler
    ): Observable<HttpEvent<any>> {
        // Per-request opt-out via header. Strip it before forwarding so it
        // doesn't leak to the server, then pass through without touching state.
        if (req.headers.has(SKIP_LOADER_HEADER)) {
            const forwarded = req.clone({headers: req.headers.delete(SKIP_LOADER_HEADER)});
            return next.handle(forwarded).pipe(catchError(this.handleError));
        }

        // Global opt-out: any component currently suspending the spinner
        // (e.g. dashboard widgets with their own inline loaders) bypasses
        // show/hide entirely.
        if (this.loadingState.isSuspended) {
            return next.handle(req).pipe(catchError(this.handleError));
        }

        this.startRequest();
        return next.handle(req).pipe(
            finalize(() => this.endRequest()),
            catchError(this.handleError)
        );
    }

    private startRequest() {
        // A new request cancels any pending "hide" — we're not idle anymore.
        if (this.hideTimer) {
            clearTimeout(this.hideTimer);
            this.hideTimer = null;
        }
        this.activeRequests++;
        // Only schedule a "show" if this is the first request and the spinner
        // isn't already visible. Subsequent overlapping requests just increment
        // the counter — no extra timers, no flicker.
        if (this.activeRequests === 1 && !this.spinnerVisible && !this.showTimer) {
            this.showTimer = setTimeout(() => {
                this.showTimer = null;
                if (this.activeRequests > 0) {
                    this.spinner.show();
                    this.spinnerVisible = true;
                }
            }, this.SHOW_DELAY_MS);
        }
    }

    private endRequest() {
        this.activeRequests = Math.max(0, this.activeRequests - 1);
        if (this.activeRequests === 0) {
            // Cancel a pending "show" if the request finished before the delay.
            if (this.showTimer) {
                clearTimeout(this.showTimer);
                this.showTimer = null;
            }
            // Defer the hide. If a cascade of requests is in progress (e.g.
            // dashboard widgets that fire sequentially), each new request will
            // cancel this timer in startRequest and the spinner stays up as
            // one continuous load. After HIDE_DELAY_MS of true idle, hide it.
            if (this.spinnerVisible && !this.hideTimer) {
                this.hideTimer = setTimeout(() => {
                    this.hideTimer = null;
                    if (this.activeRequests === 0 && this.spinnerVisible) {
                        this.spinner.hide();
                        this.spinnerVisible = false;
                    }
                }, this.HIDE_DELAY_MS);
            }
        }
    }

    private handleError = (error: HttpErrorResponse) => throwError(() => error);
}
