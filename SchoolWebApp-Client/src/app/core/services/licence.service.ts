import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {BehaviorSubject, catchError, Observable, of, shareReplay, tap} from 'rxjs';

/** The paid modules; core (dashboard, school, students, staff, settings, reports, security) is always on. */
export type AppModule = 'Academics' | 'Finance' | 'Payroll' | 'Elections' | 'Communications';

export const MODULE_LABELS: {[m in AppModule]: string} = {
    Academics: 'Academics & Teaching Management',
    Finance: 'Finance',
    Payroll: 'Payroll',
    Elections: 'Elections',
    Communications: 'Communications (SMS)'
};
export const ALL_MODULES = Object.keys(MODULE_LABELS) as AppModule[];

export interface LicenceStatus {
    /** None, Invalid, WrongSite, Active, Expiring, Grace or Expired. */
    state: string;
    message: string | null;
    licenceId: string | null;
    school: string | null;
    hosts: string[];
    licensedModules: AppModule[];
    /** The modules usable now. */
    modules: AppModule[];
    issued: string | null;
    expires: string | null;
    graceEnds: string | null;
    daysLeft: number | null;
}

const NONE: LicenceStatus = {
    state: 'None', message: null, licenceId: null, school: null, hosts: [], licensedModules: [], modules: [],
    issued: null, expires: null, graceEnds: null, daysLeft: null
};

/**
 * The school's licence, as the API reads it: which paid modules are switched on.
 * Menus, pages, settings sections and dashboard cards follow it; the API enforces it.
 */
@Injectable({providedIn: 'root'})
export class LicenceService {
    private readonly status$ = new BehaviorSubject<LicenceStatus | null>(null);
    private request$: Observable<LicenceStatus> | null = null;

    constructor(private http: HttpClient) {}

    get changes(): Observable<LicenceStatus | null> {
        return this.status$.asObservable();
    }

    get status(): LicenceStatus | null {
        return this.status$.value;
    }

    /** The licence, fetched once per sign-in (refresh() fetches it again). */
    load(): Observable<LicenceStatus> {
        if (this.status$.value) return of(this.status$.value);
        if (!this.request$) {
            this.request$ = this.http.get<LicenceStatus>('/licence').pipe(
                // If it cannot be read, show only the core features; the API decides anyway.
                catchError(() => of(NONE)),
                tap((s) => {
                    this.status$.next(s);
                    this.request$ = null;
                }),
                shareReplay(1)
            );
        }
        return this.request$;
    }

    refresh(): Observable<LicenceStatus> {
        this.status$.next(null);
        this.request$ = null;
        return this.load();
    }

    install(key: string): Observable<LicenceStatus> {
        return this.http.post<LicenceStatus>('/licence', {key}).pipe(tap((s) => this.status$.next(s)));
    }

    /** Whether a module is on; undefined/null means core, which always is. */
    has(module: AppModule | null | undefined): boolean {
        if (!module) return true;
        return !!this.status$.value?.modules.includes(module);
    }

    /** Forget the licence (on sign-out), so the next user's sign-in reads it afresh. */
    clear() {
        this.status$.next(null);
        this.request$ = null;
    }
}
