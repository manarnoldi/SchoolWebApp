import {Pipe, PipeTransform} from '@angular/core';

@Pipe({
    name: 'displayTimeTo'
})
export class DisplayTimeToPipe implements PipeTransform {
    transform(dateToCheck: string): string {
        let returnStr = '';
        let hoursVal = '';
        let minsVal = '';
        let dateProvided = new Date(dateToCheck.toString());
        let dateDiff = dateProvided.getTime() - new Date().getTime();
        let dateDiffInMin = Math.round(dateDiff / (1000 * 60));

        if (dateDiffInMin < 60 && dateDiffInMin > 0) {
            returnStr = dateDiffInMin + ' mins';
        } else if (dateDiffInMin >= 60 && dateDiffInMin < 1440) {
            hoursVal = Math.round((dateDiffInMin / 60)).toString();
            minsVal = (Math.round(dateDiffInMin % 60)).toString();
            returnStr = hoursVal + ' hrs, ' + minsVal + ' mins.';
        } else {
            returnStr = dateProvided.toLocaleString();
        }
        return returnStr;
    }
}
