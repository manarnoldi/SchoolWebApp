import {Status} from '@/core/enums/status';
import {StaffCategory} from '@/settings/models/staff-category';
import {StaffAttendance} from '@/staff/models/staff-attendance';
import {StaffDetails} from '@/staff/models/staff-details';
import {StaffAttendancesService} from '@/staff/services/staff-attendances.service';
import {StaffDetailsService} from '@/staff/services/staff-details.service';
import {Observable, of} from 'rxjs';
import {catchError, map, switchMap} from 'rxjs/operators';

/**
 * Defaults for the staff reports when opened without filters: the first staff
 * category that has active staff (else the first category), and - for the
 * attendance reports - the month and year of that category's latest day of
 * attendance up to today (read off its first staff member's register; this
 * month if nothing is found).
 */
export interface StaffReportDefaults {
    staffCategoryId: any;
    month: number;
    year: number;
}

export const staffReportDefaults = (
    staffDetailsSvc: StaffDetailsService,
    staffAttendancesSvc: StaffAttendancesService,
    staffCategories: StaffCategory[],
    withAttendance: boolean
): Observable<StaffReportDefaults> => {
    let now = new Date();
    let today = `${now.getFullYear()}-${String(now.getMonth() + 1).padStart(2, '0')}-${String(now.getDate()).padStart(2, '0')}`;
    let fallback: StaffReportDefaults = {
        staffCategoryId: staffCategories?.[0]?.id ?? null,
        month: now.getMonth() + 1,
        year: now.getFullYear()
    };
    return staffDetailsSvc.getBySearchDetails(Status.Active, null, null).pipe(
        switchMap((staff: StaffDetails[]) => {
            let category = (staffCategories || []).find((c) => (staff || []).some((s) => String(s.staffCategoryId) === String(c.id)));
            let first = (staff || []).find((s) => category && String(s.staffCategoryId) === String(category.id));
            let picked = {...fallback, staffCategoryId: category?.id ?? fallback.staffCategoryId};
            if (!withAttendance || !first) return of(picked);
            return staffAttendancesSvc.get('/staffAttendances/byStaffDetailsId/' + first.id).pipe(
                map((records: StaffAttendance[]) => {
                    let days = (records || [])
                        .map((r) => String(r.date ?? '').substring(0, 10))
                        .filter((d) => d && d <= today)
                        .sort();
                    let last = days[days.length - 1];
                    return last ? {...picked, year: +last.substring(0, 4), month: +last.substring(5, 7)} : picked;
                }),
                catchError(() => of(picked))
            );
        }),
        catchError(() => of(fallback))
    );
};
