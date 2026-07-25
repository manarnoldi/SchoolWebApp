import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';
import {MenuPermission} from '../models/menu-permission';

@Injectable({providedIn: 'root'})
export class MenuPermissionService {
    constructor(private http: HttpClient) {}

    getAll(): Observable<MenuPermission[]> {
        return this.http.get<MenuPermission[]>('/menuPermissions');
    }

    getByRole(roleId: string): Observable<MenuPermission[]> {
        return this.http.get<MenuPermission[]>(`/menuPermissions/byRole/${roleId}`);
    }

    getMyPermissions(): Observable<{allAccess: boolean; paths: string[]}> {
        return this.http.get<{allAccess: boolean; paths: string[]}>('/menuPermissions/myPermissions');
    }

    // Applies only the delta: `added` paths are created, `removed` paths are
    // deleted; unchanged permissions for the role are left untouched.
    saveChanges(
        roleId: string,
        added: {path: string; name: string}[],
        removed: string[]
    ): Observable<any> {
        return this.http.post('/menuPermissions/save', {roleId, added, removed});
    }
}
