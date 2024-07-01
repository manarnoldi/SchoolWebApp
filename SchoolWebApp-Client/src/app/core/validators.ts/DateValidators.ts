import { AbstractControl, ValidationErrors } from "@angular/forms";

export function DateLessThanOrEqualsValidator(dateCompareControlName: string, operation: string) {

    let thisDateControl: AbstractControl;
    let otherDateControl: AbstractControl;

    return function DateLessThanOrEqualsValidate(control: AbstractControl): ValidationErrors {
        if (!control.parent) {
            return null;
        }
        if (!thisDateControl) {
            thisDateControl = control;
            otherDateControl = control.parent.get(dateCompareControlName) as AbstractControl;
            if (!otherDateControl) {
                throw new Error('dateLessThanOrEqualsValidator(): other control is not found in parent group');
            }
            otherDateControl.valueChanges.subscribe(() => {
                thisDateControl.updateValueAndValidity();
            });
        }
        if (!otherDateControl || !otherDateControl.value) {
            return null;
        }
        const date1 = thisDateControl.value;
        const date2 = otherDateControl.value;
        if (date1 !== null && date2 !== null && date1 < date2 && operation=='less') {
            return {
                'date_less_than_or_equal': true
            };
        }
        if (date1 !== null && date2 !== null && date1 > date2 && operation=='greater') {
            return {
                'date_less_than_or_equal': true
            };
        }
        return null;
    };
}
