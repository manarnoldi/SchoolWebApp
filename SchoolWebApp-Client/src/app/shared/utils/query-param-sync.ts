import {ActivatedRoute, Router} from '@angular/router';

// Tiny helper used by pages that want to persist their filter selections in
// the URL so refresh / bookmark / share restores the same view. Each call
// merges the supplied keys into the existing query string (so callers can
// update only the field that changed), and clears empty/falsy values so the
// URL stays tidy. `replaceUrl: true` keeps the back-button reserved for
// real page navigation rather than every filter tweak.
export function pushQueryParams(
    router: Router,
    route: ActivatedRoute,
    params: Record<string, unknown>
): void {
    const cleaned: Record<string, unknown> = {};
    for (const k of Object.keys(params)) {
        const v = params[k];
        cleaned[k] = v == null || v === '' || (typeof v === 'number' && Number.isNaN(v)) ? null : v;
    }
    router.navigate([], {
        relativeTo: route,
        queryParams: cleaned,
        queryParamsHandling: 'merge',
        replaceUrl: true
    });
}

// Convenience reader (snapshot — fine for component init where we only need
// the value once). Returns null if absent.
export function readQueryParam(route: ActivatedRoute, key: string): string | null {
    return route.snapshot.queryParamMap.get(key);
}

// Resolves a URL-string id to the canonical id from a list of options so
// [ngValue]="item.id" bindings highlight correctly after restore. The URL
// always serialises ids as strings, but the API may return them as numbers,
// and Angular's strict-equality value comparison then misses the match: the
// dropdown looks empty even though the model is "set". Returning the id
// straight off the matching item ensures both sides have the same type.
// Falls back to the raw URL value if no option matches yet (lets downstream
// cascade logic still see "user intended this id").
export function matchOptionId<T extends {id?: any}>(
    items: T[] | null | undefined,
    urlValue: string | null | undefined
): any {
    if (urlValue == null || urlValue === '') return null;
    if (!items || !items.length) return urlValue;
    const match = items.find((i) => String(i.id) === String(urlValue));
    return match ? match.id : urlValue;
}
