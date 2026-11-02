/**
 * Default picks for filter dropdowns, so a page opens on the current year and
 * term instead of empty "Select ..." boxes. The rule follows the server's
 * (TeachingDocumentsController.Context): the one whose dates hold today, else
 * the one marked current (status), else the latest that has started, else the
 * first.
 */

const day = (d: any): string => (d ? String(d).substring(0, 10) : '');
const today = (): string => {
    let d = new Date();
    return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
};

function pickCurrent<T extends {startDate?: any; endDate?: any; status?: boolean}>(items: T[]): T | null {
    if (!items?.length) return null;
    let t = today();
    return (
        items.find((i) => day(i.startDate) <= t && t <= day(i.endDate) && !!i.startDate && !!i.endDate) ??
        items.find((i) => i.status === true) ??
        [...items].filter((i) => i.startDate && day(i.startDate) <= t).sort((a, b) => day(b.startDate).localeCompare(day(a.startDate)))[0] ??
        items[0]
    );
}

/** The current academic year (by dates, then status). */
export const currentYear = <T extends {startDate?: any; endDate?: any; status?: boolean}>(years: T[]): T | null => pickCurrent(years);

/** The current term among a year's sessions (by dates, then status). */
export const currentSession = <T extends {startDate?: any; endDate?: any; status?: boolean}>(sessions: T[]): T | null => pickCurrent(sessions);

/** The top curriculum by rank. */
export const topCurriculum = <T extends {rank?: number}>(curricula: T[]): T | null =>
    curricula?.length ? [...curricula].sort((a, b) => (a.rank ?? 0) - (b.rank ?? 0))[0] : null;

/** The first item of a list, or null - for dropdowns that need some selection. */
export const firstOf = <T>(items: T[]): T | null => (items?.length ? items[0] : null);

/**
 * The most recent school exam that has started (else the earliest coming up),
 * preferring the main exams - those on the report forms, which every class
 * sits - over extras such as the candidates' weekly tests.
 */
export const latestExam = <T extends {examStartDate?: any; examType?: {showOnReportForm?: boolean}}>(schoolExams: T[]): T | null => {
    if (!schoolExams?.length) return null;
    let main = schoolExams.filter((e) => e.examType?.showOnReportForm !== false);
    let pool = main.length ? main : schoolExams;
    let t = today();
    let byStart = [...pool].sort((a, b) => day(a.examStartDate).localeCompare(day(b.examStartDate)));
    return [...byStart].reverse().find((e) => e.examStartDate && day(e.examStartDate) <= t) ?? byStart[0];
};

/** An option's id as a string (as a URL query param holds it), or null. */
export const idOf = (item: {id?: any} | null | undefined): string | null => (item?.id != null ? String(item.id) : null);
