import {HttpClient, HttpErrorResponse, HttpHeaders} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {User} from '../models/User';
import {Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import {Observable, catchError, map, throwError} from 'rxjs';
import { AppService } from './app.service';

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    headers = new HttpHeaders().set('Content-Type', 'application/json');
    public currentUser: User;
    private loggingOut = false;

    constructor(
        private http: HttpClient,
        public router: Router,
        private toastr: ToastrService,
        private appService:AppService
    ) {}

    // Sign-up
    signUp(user: User): Observable<any> {
        let api = '/user/register';
        return this.http
            .post(api, user, {responseType: 'text'})
            .pipe(catchError(this.handleError));
    }

    // Sign-up
    updateUser(user: User): Observable<any> {
        let api = '/user/update-user';
        return this.http
            .put(api, user, {responseType: 'text'})
            .pipe(catchError(this.handleError));
    }

    // Sign-in
    signIn(user: User) {
        return this.http
            .post<any>('/auth/login', user)
            .pipe(catchError(this.handleError));
    }

    // Change own password (used by the forced-on-first-login flow)
    changePassword(currentPassword: string, newPassword: string) {
        return this.http
            .post<any>('/auth/change-password', {currentPassword, newPassword})
            .pipe(catchError(this.handleError));
    }

    clearMustChangePassword() {
        const u = this.getCurrentUser();
        if (u && u.mustChangePassword) {
            u.mustChangePassword = false;
            this.setCurrentUser(u);
        }
    }

    // Set current user
    setCurrentUser(user) {
        this.currentUser = user;
        localStorage.setItem('current_user', JSON.stringify(user));
        // A fresh sign-in re-arms logout so a later expiry can fire the audit.
        this.loggingOut = false;
    }

    getCurrentUser() {
        return JSON.parse(localStorage.getItem('current_user'));
    }

    getToken() {
        return localStorage.getItem('ssw_token');
    }

    get isLoggedIn(): boolean {
        let authToken = localStorage.getItem('ssw_token');
        return authToken !== null ? true : false;
    }

    doLogout() {
        // Collapse the burst of concurrent 401s (one per in-flight request)
        // into a single logout so we don't fire N audit POSTs and N navigations.
        if (this.loggingOut) {
            return;
        }
        this.loggingOut = true;

        // Fire the audit POST BEFORE we strip the token from
        // localStorage - the auth interceptor reads the token at
        // request time, so as long as the request leaves the queue
        // first it carries a valid JWT. Skip it entirely when there is
        // no token (already expired/cleared): the POST would just bounce
        // 401/403 and add noise. Errors are swallowed because an
        // audit-recording failure must not block sign-out.
        if (this.getToken()) {
            this.http
                .post('/auditevents/logout', {})
                .subscribe({error: () => {}});
        }

        localStorage.removeItem('ssw_token');
        localStorage.removeItem('current_user');
        this.appService.setUserLoggedIn(false);
        this.router.navigate(['/login']);
    }

    // autoLogout(expirationDate: number) {
    //     setTimeout(() => {
    //         this.doLogout();
    //     }, expirationDate);
    // }

    // User profile
    getUserProfile(id): Observable<any> {
        let api = `/users/${id}`;
        return this.http.get(api, {headers: this.headers}).pipe(
            map((res: Response) => {
                return res || {};
            }),
            catchError(this.handleError)
        );
    }

    //Get user by staffId
    getUserByStaffId(staffId): Observable<any> {
        let api = `/user/user/${staffId}`;
        return this.http.get(api, {headers: this.headers}).pipe(
            map((res: Response) => {
                return res || {};
            }),
            catchError(this.handleError)
        );
    }

    getUserRoles(id): Observable<any> {
        let api = `/user/user-roles/${id}`;
        return this.http.get(api, {headers: this.headers}).pipe(
            map((res: Response) => {
                return res || {};
            }),
            catchError(this.handleError)
        );
    }

    //Get user by staffId
    //  getUserByStaffId(staffId): Observable<any> {
    //     let api = `/user/user/${staffId}`;
    //     return this.http.get(api, {headers: this.headers}).pipe(
    //         map((res: Response) => {
    //             return res || {};
    //         }),
    //         catchError(this.handleError)
    //     );
    // }

    // Error
    handleError(error: HttpErrorResponse) {
        let msg = '';
        if (error.error instanceof ErrorEvent) {
            // client-side error
            msg = error.error.message;
        } else {
            // server-side error
            msg = `Error Code: ${error.status}\nMessage: ${error.message}`;
        }
        return throwError(msg);
    }
}
