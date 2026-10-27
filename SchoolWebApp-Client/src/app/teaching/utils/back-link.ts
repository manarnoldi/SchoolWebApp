import {ActivatedRoute, Router} from '@angular/router';

/**
 * "Back" between teaching documents: a page opened from another one (lesson plans
 * opening the record of work, the tracker opening a scheme) gets the opener's URL
 * in a `from` query parameter, and its Back button returns there instead of to
 * its own list. Each page keeps `from` when it rewrites its own query parameters.
 * A document opened from its own page's list gets the list's URL (with its
 * filters), so Back returns to the same filtered list.
 */

const PAGE_NAMES: Record<string, string> = {
    '/teaching/schemes': 'scheme of work',
    '/teaching/lesson-plans': 'lesson plans',
    '/teaching/record-of-work': 'record of work',
    '/teaching/tracker': 'tracker'
};

const path = (url: string) => url.split('?')[0];

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

// Whether `from` is this page itself - its list, before a document was opened.
export function isOwnList(router: Router, from: string | null): boolean {
    return !!from && path(from) === path(router.url);
}

export function backLabel(from: string | null, router: Router): string {
    if (!from || isOwnList(router, from)) return 'Back';
    let name = PAGE_NAMES[path(from)];
    return name ? `Back to ${name}` : 'Back';
}
