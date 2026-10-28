/**
 * The grade band a percentage falls in, from one grading category's bands: the
 * highest band whose minimum it reaches. Bands are set in whole numbers (ME 2
 * 50-62, ME 1 63-74), so a mark between two (62.6) takes the lower band instead
 * of getting no grade.
 */
export function gradeForPercent<T extends {minScore: any; maxScore: any}>(grades: T[] | null | undefined, percent: number): T | undefined {
    if (percent === null || percent === undefined || isNaN(percent)) return undefined;
    let best: T | undefined;
    for (const g of grades || []) {
        if (percent >= +g.minScore && (!best || +g.minScore > +best.minScore)) best = g;
    }
    return best;
}
