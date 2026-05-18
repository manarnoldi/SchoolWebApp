import {Component, HostBinding, OnDestroy, OnInit, Renderer2} from '@angular/core';
import {UntypedFormControl, UntypedFormGroup, Validators} from '@angular/forms';
import {Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import {AuthService} from '@/core/services/auth.service';

@Component({
    selector: 'app-change-password',
    templateUrl: './change-password.component.html',
    styleUrls: ['./change-password.component.scss']
})
export class ChangePasswordComponent implements OnInit, OnDestroy {
    @HostBinding('class') class = 'login-box';
    public changePasswordForm: UntypedFormGroup;
    public isSaving = false;
    public showCurrent = false;
    public showNew = false;
    public showConfirm = false;
    public mustChange = false;
    public userName = '';

    constructor(
        private renderer: Renderer2,
        private toastr: ToastrService,
        private authService: AuthService,
        private router: Router
    ) {}

    ngOnInit(): void {
        this.renderer.addClass(document.querySelector('app-root'), 'login-page');
        const u = this.authService.getCurrentUser();
        this.mustChange = !!u?.mustChangePassword;
        this.userName = u?.userName || u?.firstName || '';
        this.changePasswordForm = new UntypedFormGroup({
            currentPassword: new UntypedFormControl(null, Validators.required),
            newPassword: new UntypedFormControl(null, [Validators.required, Validators.minLength(6)]),
            confirmPassword: new UntypedFormControl(null, Validators.required)
        }, {validators: this.passwordsMatch});
    }

    // Validator: new + confirm must match. Attached to the form group so the
    // error surfaces only after the user has filled both fields.
    private passwordsMatch = (group: UntypedFormGroup) => {
        const np = group.get('newPassword')?.value;
        const cp = group.get('confirmPassword')?.value;
        if (np && cp && np !== cp) return {mismatch: true};
        return null;
    };

    submit() {
        if (this.changePasswordForm.invalid) {
            this.changePasswordForm.markAllAsTouched();
            return;
        }

        const {currentPassword, newPassword} = this.changePasswordForm.value;
        if (currentPassword === newPassword) {
            this.toastr.warning('The new password must differ from the current one.');
            return;
        }

        this.isSaving = true;
        this.authService.changePassword(currentPassword, newPassword).subscribe(
            (res: any) => {
                this.isSaving = false;
                this.authService.clearMustChangePassword();
                this.toastr.success(res?.message || 'Password changed successfully.');
                this.router.navigate(['/']);
            },
            (err: any) => {
                this.isSaving = false;
                const msg = typeof err === 'string'
                    ? err
                    : (err?.error?.message || err?.message || 'Failed to change password.');
                this.toastr.error(msg);
            }
        );
    }

    signOut() {
        this.authService.doLogout();
    }

    ngOnDestroy(): void {
        this.renderer.removeClass(document.querySelector('app-root'), 'login-page');
    }
}
