import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {AppRole} from '../models/app-role';

@Injectable({providedIn: 'root'})
export class RoleService {
    constructor(private http: HttpClient) {}

    getAll(): Observable<AppRole[]> {
        return this.http.get<AppRole[]>('/roles');
    }

    getById(id: number): Observable<AppRole> {
        return this.http.get<AppRole>(`/roles/${id}`);
    }

    create(role: any): Observable<any> {
        return this.http.post('/roles', role);
    }

    update(id: number, role: any): Observable<any> {
        return this.http.put(`/roles/${id}`, role);
    }

    delete(id: number): Observable<any> {
        return this.http.delete(`/roles/${id}`);
    }
}
