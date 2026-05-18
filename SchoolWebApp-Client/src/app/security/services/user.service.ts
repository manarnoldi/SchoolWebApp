import {Injectable} from '@angular/core';
import {HttpClient, HttpParams} from '@angular/common/http';
import {Observable} from 'rxjs';
import {AppUser, AvailablePerson} from '../models/app-user';

@Injectable({providedIn: 'root'})
export class UserService {
    constructor(private http: HttpClient) {}

    getAll(): Observable<AppUser[]> {
        return this.http.get<AppUser[]>('/users');
    }

    // Persons not yet linked to any user. Pass includePersonId so the
    // currently-linked person stays in the dropdown when editing.
    getAvailablePersons(includePersonId?: number | null): Observable<AvailablePerson[]> {
        let params = new HttpParams();
        if (includePersonId) params = params.set('includePersonId', includePersonId);
        return this.http.get<AvailablePerson[]>('/users/availablePersons', {params});
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
