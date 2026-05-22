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

    // How long a request must run before the spinner appears. Tuned so most
    // cached / fast requests stay invisible, while genuinely slow requests
    // still get a loading indicator after a brief moment.
    private readonly SHOW_DELAY_MS = 250;

    constructor(private spinner: NgxSpinnerService) {}

    intercept(
        req: HttpRequest<any>,
        next: HttpHandler
    ): Observable<HttpEvent<any>> {
        // Opt-out path: the caller marked this request as "don't show spinner".
        // Strip the header before forwarding so it doesn't leak to the server,
        // and pass through without touching the spinner state.
        if (req.headers.has(SKIP_LOADER_HEADER)) {
            const forwarded = req.clone({headers: req.headers.delete(SKIP_LOADER_HEADER)});
            return next.handle(forwarded).pipe(catchError(this.handleError));
        }

        this.startRequest();
        return next.handle(req).pipe(
            finalize(() => this.endRequest()),
            catchError(this.handleError)
        );
    }

    private startRequest() {
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
            // Hide only if the spinner was actually shown — avoids spurious
            // hide calls that could interfere with other UI overlays.
            if (this.spinnerVisible) {
                this.spinner.hide();
                this.spinnerVisible = false;
            }
        }
    }

    private handleError = (error: HttpErrorResponse) => throwError(() => error);
}
