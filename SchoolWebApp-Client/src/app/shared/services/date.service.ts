import {Injectable} from '@angular/core';

@Injectable({
    providedIn: 'root'
})
export class DateService {
    constructor() {}

    public toTimeString(hours: number, minutes: number): string {
        return `${String(hours).padStart(2, '0')}:${String(minutes).padStart(2, '0')}:00`;
    }
}
