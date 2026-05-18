import {Injectable} from '@angular/core';
import {
    CanActivate,
    CanActivateChild,
    ActivatedRouteSnapshot,
    RouterStateSnapshot,
    UrlTree,
    Router
} from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable({
    providedIn: 'root'
})
export class AuthGuard implements CanActivate, CanActivateChild {
    constructor(private router: Router, private authService: AuthService) {}

    canActivate(
        next: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ):
        | Observable<boolean | UrlTree>
        | Promise<boolean | UrlTree>
        | boolean
        | UrlTree {
        if (!this.authService.isLoggedIn) {
            return this.router.parseUrl('/login');
        }

        // If a forced password change is pending, the only route allowed is
        // /change-password — everything else bounces back there.
        const user = this.authService.getCurrentUser();
        if (user?.mustChangePassword && !state.url.startsWith('/change-password')) {
            return this.router.parseUrl('/change-password');
        }

        return true;
    }

    canActivateChild(
        next: ActivatedRouteSnapshot,
        state: RouterStateSnapshot
    ):
        | Observable<boolean | UrlTree>
        | Promise<boolean | UrlTree>
        | boolean
        | UrlTree {
        return this.canActivate(next, state);
    }
}
