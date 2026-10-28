import {inject} from '@angular/core';
import {CanActivateFn, Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import {map} from 'rxjs';
import {AppModule, LicenceService, MODULE_LABELS} from '@/core/services/licence.service';

/**
 * Pages of a paid module open only when the school's licence includes it; give
 * the module in the route's data, e.g. {data: {module: 'Finance'}}. The API
 * refuses the module's calls anyway - this keeps people off an empty page.
 */
export const moduleGuard: CanActivateFn = (route) => {
    const module = route.data?.['module'] as AppModule | undefined;
    if (!module) return true;
    const licence = inject(LicenceService);
    const router = inject(Router);
    const toastr = inject(ToastrService);
    return licence.load().pipe(
        map((s) => {
            if (s.modules.includes(module)) return true;
            toastr.warning(`${MODULE_LABELS[module]} is not part of this school's licence.`);
            return router.createUrlTree(['/']);
        })
    );
};
