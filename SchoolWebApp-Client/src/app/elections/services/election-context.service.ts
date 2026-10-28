import {Injectable} from '@angular/core';
import {BehaviorSubject, Subject} from 'rxjs';
import {ElectionDetail, ElectionOptions} from '@/elections/models/election-models';

/**
 * The election open on the election page, shared by its tabs. Provided by the
 * page itself, so each election page gets its own.
 */
@Injectable()
export class ElectionContext {
    election$ = new BehaviorSubject<ElectionDetail | null>(null);
    options$ = new BehaviorSubject<ElectionOptions | null>(null);
    /** A tab changed something the page header shows (counts, stage): reload it. */
    reload$ = new Subject<void>();

    get election(): ElectionDetail | null {
        return this.election$.value;
    }

    get options(): ElectionOptions | null {
        return this.options$.value;
    }

    get canManage(): boolean {
        return !!this.options?.canManage;
    }

    reload() {
        this.reload$.next();
    }
}
