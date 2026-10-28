import {Injectable} from '@angular/core';
import {catchError, map, Observable, of, shareReplay} from 'rxjs';
import {MenuPermissionService} from '@/security/services/menu-permission.service';

interface MyPermissions {
    allAccess: boolean;
    paths: string[];
}

/**
 * Whether the signed-in user may open a page, by the same rule as the side menu
 * and the API: super administrators see everything, everyone else only the menu
 * paths their roles hold. Used for profile tabs and dashboard cards that show
 * what those pages show.
 */
@Injectable({providedIn: 'root'})
export class AccessService {
    private mine$: Observable<MyPermissions> | null = null;

    constructor(private menuPermSvc: MenuPermissionService) {}

    private load(): Observable<MyPermissions> {
        if (!this.mine$) {
            const user = JSON.parse(localStorage.getItem('current_user') || 'null');
            const superAdmin = !!user?.roles?.some((r: string) => r.toLowerCase() === 'superadministrator');
            this.mine$ = (superAdmin ? of({allAccess: true, paths: []}) : this.menuPermSvc.getMyPermissions()).pipe(
                catchError(() => of({allAccess: false, paths: [] as string[]})),
                shareReplay(1)
            );
        }
        return this.mine$;
    }

    /** True when the user may open any one of the menu paths given. */
    canAny(...paths: string[]): Observable<boolean> {
        return this.load().pipe(map((m) => m.allAccess || paths.some((p) => m.paths.includes(p))));
    }

    /** Forget the permissions (on sign-out). */
    clear() {
        this.mine$ = null;
    }
}
