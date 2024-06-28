import {Pipe, PipeTransform} from '@angular/core';
const DAYS_IN_WEEK = 7;
@Pipe({
    name: 'daysToWeeksYears'
})
export class DaysToWeeksYearsPipe implements PipeTransform {
    // transform(value: unknown, ...args: unknown[]): unknown {
    //     return null;
    // }

    transform(numberOfDays: number, ...args: boolean[]): string {
        let years, months, weeks, days: number;
        let showNumOfDays: boolean = args[0];
        let returnString = '';
        if (numberOfDays != null) {
            years = Math.floor(numberOfDays / 365);
            months = Math.floor((numberOfDays % 365) / 30);
            weeks = Math.floor((numberOfDays % 30) / 7);
            days = Math.floor(numberOfDays % 7);

            returnString = showNumOfDays
                ? numberOfDays + '- '
                : '' +
                  Math.round(years) +
                  'Y, ' +
                  Math.round(months) +
                  'M, ' +
                  Math.round(weeks) +
                  'W, ' +
                  Math.round(days) +
                  'D';
        }
        return returnString;
    }
}
