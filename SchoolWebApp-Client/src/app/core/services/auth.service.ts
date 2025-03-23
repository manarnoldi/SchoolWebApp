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

    // Set current user
    setCurrentUser(user) {
        this.currentUser = user;
        localStorage.setItem('current_user', JSON.stringify(user));
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
        let removeToken = localStorage.removeItem('ssw_token');
        localStorage.removeItem('current_user');
        this.appService.setUserLoggedIn(false);
        if (removeToken == null) {
            this.router.navigate(['/login']);
        }

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
