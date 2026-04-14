import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ToastrService} from 'ngx-toastr';
import {HttpClient} from '@angular/common/http';
import Swal from 'sweetalert2';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {UserService} from '../../services/user.service';
import {RoleService} from '../../services/role.service';
import {AppUser} from '../../models/app-user';
import {AppRole} from '../../models/app-role';

@Component({
    selector: 'app-users',
    templateUrl: './users.component.html',
    styleUrl: './users.component.scss'
})
export class UsersComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/security/users'], title: 'User Management'}
    ];
    dashboardTitle = 'User Management';

    users: AppUser[] = [];
    roles: AppRole[] = [];
    userForm: FormGroup;
    editMode = false;
    editUserId: number = null;
    showForm = false;
    isLoading = false;

    constructor(
        private userSvc: UserService,
        private roleSvc: RoleService,
        private toastr: ToastrService,
        private fb: FormBuilder,
        private http: HttpClient
    ) {}

    ngOnInit(): void {
        this.initForm();
        this.loadData();
    }

    initForm() {
        this.userForm = this.fb.group({
            userName: ['', Validators.required],
            email: ['', [Validators.required, Validators.email]],
            firstName: ['', Validators.required],
            lastName: ['', Validators.required],
            password: ['', this.editMode ? [] : [Validators.required, Validators.minLength(6)]]
        });
    }

    loadData() {
        this.isLoading = true;
        this.userSvc.getAll().subscribe({
            next: (users) => {
                this.users = users;
                this.isLoading = false;
            },
            error: (err) => {
                this.toastr.error('Error loading users');
                this.isLoading = false;
            }
        });
        this.roleSvc.getAll().subscribe({
            next: (roles) => this.roles = roles,
            error: () => {}
        });
    }

    toggleForm() {
        this.showForm = !this.showForm;
        if (!this.showForm) {
            this.resetForm();
        }
    }

    resetForm() {
        this.editMode = false;
        this.editUserId = null;
        this.userForm.reset();
        this.initForm();
    }

    editUser(user: AppUser) {
        this.editMode = true;
        this.editUserId = user.id;
        this.showForm = true;
        this.userForm.patchValue({
            userName: user.userName,
            email: user.email,
            firstName: user.firstName,
            lastName: user.lastName,
            password: ''
        });
        this.userForm.get('password').clearValidators();
        this.userForm.get('password').updateValueAndValidity();
    }

    onSubmit() {
        if (this.userForm.invalid) return;

        const formVal = this.userForm.value;

        Swal.fire({
            title: `${this.editMode ? 'Update' : 'Create'} user?`,
            text: `Confirm to ${this.editMode ? 'update' : 'create'} user.`,
            width: 400, position: 'top', padding: '1em', icon: 'question',
            showCancelButton: true,
            confirmButtonText: this.editMode ? 'Update' : 'Create',
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                if (this.editMode) {
                    const updateData: any = {
                        userName: formVal.userName,
                        email: formVal.email,
                        firstName: formVal.firstName,
                        lastName: formVal.lastName
                    };
                    if (formVal.password) updateData.password = formVal.password;

                    this.userSvc.update(this.editUserId, updateData).subscribe({
                        next: () => {
                            this.toastr.success('User updated successfully');
                            this.showForm = false;
                            this.resetForm();
                            this.loadData();
                        },
                        error: (err) => this.toastr.error(err.error?.message || 'Error updating user')
                    });
                } else {
                    this.userSvc.create(formVal).subscribe({
                        next: () => {
                            this.toastr.success('User created successfully');
                            this.showForm = false;
                            this.resetForm();
                            this.loadData();
                        },
                        error: (err) => {
                            const errors = err.error;
                            if (Array.isArray(errors)) {
                                errors.forEach((e: any) => this.toastr.error(e.description || e.code));
                            } else {
                                this.toastr.error(err.error?.message || 'Error creating user');
                            }
                        }
                    });
                }
            }
        });
    }

    deleteUser(user: AppUser) {
        Swal.fire({
            title: 'Delete user?',
            text: `Delete ${user.userName}?`,
            width: 400, position: 'top', padding: '1em', icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Delete',
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                this.userSvc.delete(user.id).subscribe({
                    next: () => {
                        this.toastr.success('User deleted');
                        this.loadData();
                    },
                    error: (err) => this.toastr.error(err.error?.message || 'Error deleting user')
                });
            }
        });
    }

    resetPassword(user: AppUser) {
        Swal.fire({
            title: `Reset password for ${user.userName}?`,
            html: '<input id="swal-new-password" type="password" class="swal2-input" placeholder="New password" style="font-size:14px;">',
            width: 400, position: 'top', padding: '1em', icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Reset Password',
            cancelButtonText: 'Cancel',
            preConfirm: () => {
                const pw = (document.getElementById('swal-new-password') as HTMLInputElement).value;
                if (!pw || pw.length < 6) {
                    Swal.showValidationMessage('Password must be at least 6 characters');
                    return false;
                }
                return pw;
            }
        }).then((result) => {
            if (result.value) {
                this.http.post('/users/reset-password', {
                    userId: user.id,
                    newPassword: result.value
                }).subscribe({
                    next: (res: any) => this.toastr.success(res.message || 'Password reset successfully'),
                    error: (err) => this.toastr.error(err.error?.message || 'Error resetting password')
                });
            }
        });
    }
}
