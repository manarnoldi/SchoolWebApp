import {ActivatedRoute, Router} from '@angular/router';

/**
 * "Back" between teaching documents: a page opened from another one (lesson plans
 * opening the record of work, the tracker opening a scheme) gets the opener's URL
 * in a `from` query parameter, and its Back button returns there instead of to
 * its own list. Each page keeps `from` when it rewrites its own query parameters.
 */

const PAGE_NAMES: Record<string, string> = {
    '/teaching/schemes': 'scheme of work',
    '/teaching/lesson-plans': 'lesson plans',
    '/teaching/record-of-work': 'record of work',
    '/teaching/tracker': 'tracker'
};

// Only a page inside Teaching Documents is a valid place to go back to.
export function fromParam(route: ActivatedRoute): string | null {
    let from = route.snapshot.queryParamMap.get('from');
    return from && from.startsWith('/teaching/') ? from : null;
}

// Query parameters that open another teaching page and remember this one.
export function openedFrom(router: Router, params: Record<string, any>): Record<string, any> {
    return {...params, from: router.url};
}

// This page's own query parameters, keeping where it was opened from.
export function withFrom(from: string | null, params: Record<string, any>): Record<string, any> {
    return from ? {...params, from} : params;
}

export function backLabel(from: string | null): string {
    if (!from) return 'Back';
    let name = PAGE_NAMES[from.split('?')[0]];
    return name ? `Back to ${name}` : 'Back';
}
