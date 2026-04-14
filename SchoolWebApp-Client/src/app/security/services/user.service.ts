import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {AppUser} from '../models/app-user';

@Injectable({providedIn: 'root'})
export class UserService {
    constructor(private http: HttpClient) {}

    getAll(): Observable<AppUser[]> {
        return this.http.get<AppUser[]>('/users');
    }

    getById(id: number): Observable<AppUser> {
        return this.http.get<AppUser>(`/users/${id}`);
    }

    create(user: any): Observable<any> {
        return this.http.post('/users', user);
    }

    update(id: number, user: any): Observable<any> {
        return this.http.put(`/users/${id}`, user);
    }

    delete(id: number): Observable<any> {
        return this.http.delete(`/users/${id}`);
    }

    addRole(email: string, role: string): Observable<any> {
        return this.http.post('/users/userRole', {email, role});
    }

    removeRole(email: string, role: string): Observable<any> {
        return this.http.post('/users/removefromrole', {email, role});
    }
}
