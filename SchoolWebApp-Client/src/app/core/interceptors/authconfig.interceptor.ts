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
                if (error.status === 401 || error.status === 0) {
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
