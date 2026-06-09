import {Inject, Injectable} from '@angular/core';
import {HttpInterceptor, HttpRequest, HttpHandler} from '@angular/common/http';
import {AuthService} from '../services/auth.service';
import {catchError, throwError} from 'rxjs';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
    constructor(
        private authService: AuthService,
        @Inject('BASE_API_URL') private baseUrl: string
    ) {}

    intercept(req: HttpRequest<any>, next: HttpHandler) {
        const authToken = this.authService.getToken();
        if (req.url.startsWith('/assets/') || req.url.startsWith('assets/')) {
            // Do not modify
            return next.handle(req);
        }
        req = req.clone({
            setHeaders: {
                Authorization: 'Bearer ' + authToken
            },
            url: `${this.baseUrl}${req.url}`
        });

        return next.handle(req).pipe(
            catchError((error) => {
                // Only force-logout on a real "your token is no longer valid"
                // signal from the API. Status 0 means the request didn't
                // complete (network blip, hard refresh cancelling in-flight
                // XHRs, ERR_NETWORK_CHANGED, brief server unreachability) and
                // previously caused users to be bounced to /login mid-session.
                // Never force-logout off the audit-logout request itself -
                // it rides the same (now-expired) token, comes back 401, and
                // would re-enter doLogout() forever, flooding the console with
                // 401/403 POSTs to /auditevents/logout.
                if (
                    error.status === 401 &&
                    !req.url.endsWith('/auditevents/logout')
                ) {
                    this.authService.doLogout();
                } else if (error.status === 409) {
                    // console.log(error);
                    //error.message =
                }
                return throwError(error);
            })
        );
    }
}
