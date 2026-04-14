import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ToastrService} from 'ngx-toastr';
import {HttpClient} from '@angular/common/http';
import {AuthService} from '@/core/services/auth.service';

@Component({
    selector: 'app-profile',
    templateUrl: './profile.component.html',
    styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
    user: any = {};
    passwordForm: FormGroup;
    isSaving = false;

    constructor(
        private fb: FormBuilder,
        private toastr: ToastrService,
        private http: HttpClient,
        private authService: AuthService
    ) {}

    ngOnInit(): void {
        this.user = this.authService.getCurrentUser() || {};
        this.passwordForm = this.fb.group({
            currentPassword: ['', Validators.required],
            newPassword: ['', [Validators.required, Validators.minLength(6)]],
            confirmPassword: ['', Validators.required]
        });
    }

    get f() { return this.passwordForm.controls; }

    get passwordsMatch(): boolean {
        return this.passwordForm.get('newPassword').value === this.passwordForm.get('confirmPassword').value;
    }

    changePassword() {
        if (this.passwordForm.invalid || !this.passwordsMatch) return;

        this.isSaving = true;
        this.http.post('/auth/change-password', {
            currentPassword: this.passwordForm.get('currentPassword').value,
            newPassword: this.passwordForm.get('newPassword').value
        }).subscribe({
            next: (res: any) => {
                this.toastr.success(res.message || 'Password changed successfully');
                this.passwordForm.reset();
                this.isSaving = false;
            },
            error: (err) => {
                this.toastr.error(err.error?.message || 'Error changing password');
                this.isSaving = false;
            }
        });
    }
}
