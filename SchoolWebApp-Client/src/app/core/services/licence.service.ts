import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {BehaviorSubject, catchError, finalize, Observable, of, retry, shareReplay, tap, throwError, timer} from 'rxjs';

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
    // Bumped by clear() and refresh(), so an answer to a request made before them
    // (e.g. the 401 that signed an expired session out) is not kept.
    private generation = 0;

    constructor(private http: HttpClient) {}

    get changes(): Observable<LicenceStatus | null> {
        return this.status$.asObservable();
    }

    get status(): LicenceStatus | null {
        return this.status$.value;
    }

    /**
     * The licence, fetched once per sign-in (refresh() fetches it again). A read
     * that fails shows only the core features for now but is not kept, so the
     * next page asks again; a dropped connection is retried first.
     */
    load(): Observable<LicenceStatus> {
        if (this.status$.value) return of(this.status$.value);
        if (!this.request$) {
            const generation = this.generation;
            this.request$ = this.http.get<LicenceStatus>('/licence').pipe(
                // Status 0 (a cold start or network blip) is worth another try; a 401 or 403 is not.
                retry({count: 2, delay: (err) => (err?.status === 0 || err?.status >= 500 ? timer(1500) : throwError(() => err))}),
                tap((s) => {
                    if (generation === this.generation) this.status$.next(s);
                }),
                // If it cannot be read, show only the core features; the API decides anyway.
                catchError(() => of(NONE)),
                finalize(() => {
                    if (generation === this.generation) this.request$ = null;
                }),
                shareReplay(1)
            );
        }
        return this.request$;
    }

    refresh(): Observable<LicenceStatus> {
        this.clear();
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
        this.generation++;
        this.status$.next(null);
        this.request$ = null;
    }
}
