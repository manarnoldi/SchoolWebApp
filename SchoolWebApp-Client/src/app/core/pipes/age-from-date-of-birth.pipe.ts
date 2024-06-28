import {Pipe, PipeTransform} from '@angular/core';

@Pipe({
    name: 'ageFromDateOfBirth'
})
export class AgeFromDateOfBirthPipe implements PipeTransform {
    transform(dateString: string): number {
        var today = new Date();
        var birthDate = new Date(dateString);
        var dateDiff = new Date().getTime() - new Date(dateString).getTime();

        return Math.round(dateDiff / (1000 * 60 * 60 * 24));
    }
}
