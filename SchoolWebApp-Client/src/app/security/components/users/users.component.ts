import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ToastrService} from 'ngx-toastr';
import {HttpClient} from '@angular/common/http';
import {forkJoin, of} from 'rxjs';
import {catchError} from 'rxjs/operators';
import Swal from 'sweetalert2';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {AuthService} from '@/core/services/auth.service';
import {UserService} from '../../services/user.service';
import {RoleService} from '../../services/role.service';
import {AppUser, AvailablePerson} from '../../models/app-user';
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
    availablePersons: AvailablePerson[] = [];
    // Cached, stable-reference view of availablePersons filtered by personTypeFilter.
    // Must not be recomputed inside the template — ng-select resets its selection
    // every time [items] sees a new array reference.
    availablePersonsView: AvailablePerson[] = [];
    userForm: FormGroup;
    editMode = false;
    editUserId: number = null;
    editUserEmail: string = null;
    showForm = false;
    isLoading = false;
    isLoadingPersons = false;
    isSavingRoles = false;
    isSuperAdmin = false;
    showPassword = false;

    // Role names currently ticked in the form, and the snapshot taken when
    // the form opened. On save we diff (toAdd / toRemove) and sync via the
    // existing add/removeRole endpoints.
    selectedRoleNames: string[] = [];
    private originalRoleNames: string[] = [];

    // Admin picks Staff / Student / Parent first; the picker then shows only
    // persons of that discriminator. Server values are "Student", "StaffDetails", "Parent".
    personTypeFilter: 'Student' | 'StaffDetails' | 'Parent' | null = null;
    readonly personTypeOptions: {value: 'Student' | 'StaffDetails' | 'Parent'; label: string; icon: string}[] = [
        {value: 'StaffDetails', label: 'Staff', icon: 'fa-user-tie'},
        {value: 'Student', label: 'Student', icon: 'fa-user-graduate'},
        {value: 'Parent', label: 'Parent', icon: 'fa-users'}
    ];

    searchTerm: string = '';
    page = 1;
    pageSize = 10;
    pageChanged = (p: number) => { this.page = p; };
    pageSizeChanged = (s: number) => { this.pageSize = s; };

    filtered = (): AppUser[] => {
        let q = (this.searchTerm || '').trim().toLowerCase();
        if (!q) return this.users;
        return this.users.filter(u =>
            (u.userName || '').toLowerCase().includes(q) ||
            (u.email || '').toLowerCase().includes(q) ||
            ((u.firstName || '') + ' ' + (u.lastName || '')).toLowerCase().includes(q) ||
            (u.personName || '').toLowerCase().includes(q)
        );
    };

    addNew() {
        this.resetForm();
        this.showForm = true;
        this.personTypeFilter = null;
        this.selectedRoleNames = [];
        this.originalRoleNames = [];
        this.showPassword = false;
        this.loadAvailablePersons(null);
    }

    private rebuildAvailablePersonsView() {
        this.availablePersonsView = this.personTypeFilter
            ? this.availablePersons.filter(p => p.personType === this.personTypeFilter)
            : [];
    }

    onPersonTypeChange(type: 'Student' | 'StaffDetails' | 'Parent') {
        this.personTypeFilter = type;
        this.rebuildAvailablePersonsView();
        // Drop the current pick if it no longer matches the chosen type.
        const currentId = this.userForm.value.personId;
        if (currentId) {
            const current = this.availablePersons.find(p => p.id === +currentId);
            if (!current || current.personType !== type) {
                this.onPersonSelected(null);
            }
        }
    }

    cancel() {
        this.showForm = false;
        this.resetForm();
    }

    constructor(
        private userSvc: UserService,
        private roleSvc: RoleService,
        private toastr: ToastrService,
        private fb: FormBuilder,
        private http: HttpClient,
        private authSvc: AuthService
    ) {
        const u = this.authSvc.getCurrentUser();
        this.isSuperAdmin = !!(u?.roles?.some((r: any) =>
            (typeof r === 'string' ? r : r?.toString() || '').toLowerCase() === 'superadministrator'));
    }

    // SuperAdministrator role can only be toggled by another SuperAdministrator.
    isRoleLocked(role: AppRole): boolean {
        return (role?.name || '').toLowerCase() === 'superadministrator' && !this.isSuperAdmin;
    }

    hasRole(roleName: string): boolean {
        return this.selectedRoleNames.includes(roleName);
    }

    toggleRoleSelection(role: AppRole, checked: boolean) {
        if (this.isRoleLocked(role)) return;
        const name = role.name;
        if (checked) {
            if (!this.selectedRoleNames.includes(name)) this.selectedRoleNames = [...this.selectedRoleNames, name];
        } else {
            this.selectedRoleNames = this.selectedRoleNames.filter(r => r !== name);
        }
    }

    ngOnInit(): void {
        this.initForm();
        this.loadData();
    }

    initForm() {
        this.userForm = this.fb.group({
            personId: [null],
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

    loadAvailablePersons(includePersonId: number | null) {
        this.isLoadingPersons = true;
        this.userSvc.getAvailablePersons(includePersonId).subscribe({
            next: (persons) => {
                this.availablePersons = persons;
                this.rebuildAvailablePersonsView();
                this.isLoadingPersons = false;
            },
            error: () => {
                this.availablePersons = [];
                this.rebuildAvailablePersonsView();
                this.isLoadingPersons = false;
                this.toastr.error('Error loading persons');
            }
        });
    }

    // Splits "John Mary Doe" into firstName "John Mary" and lastName "Doe".
    // Used to seed the form fields from a picked Person; the admin can still edit either.
    // Accepts either an id (from ng-select with bindValue) or a Person object,
    // so the same handler works regardless of how the event was wired.
    onPersonSelected(personOrId: number | string | AvailablePerson | null) {
        let id: number | null;
        if (personOrId == null || personOrId === '') {
            id = null;
        } else if (typeof personOrId === 'object' && (personOrId as AvailablePerson).id != null) {
            id = +(personOrId as AvailablePerson).id;
        } else {
            id = +(personOrId as any);
        }

        this.userForm.patchValue({personId: id});
        if (id == null) return;

        const person = this.availablePersons.find(p => p.id === id);
        if (!person) return;

        const fullName = (person.fullName || '').trim();
        const parts = fullName.split(/\s+/);
        const lastName = parts.length > 1 ? parts.pop() : '';
        const firstName = parts.join(' ');

        this.userForm.patchValue({
            firstName: firstName || fullName,
            lastName: lastName || '',
            email: person.email || this.userForm.value.email
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
        this.editUserEmail = null;
        this.userForm.reset();
        this.initForm();
        this.selectedRoleNames = [];
        this.originalRoleNames = [];
    }

    editUser(user: AppUser) {
        this.editMode = true;
        this.editUserId = user.id;
        this.editUserEmail = user.email;
        this.showForm = true;
        this.showPassword = false;
        this.personTypeFilter = (user.personType as any) ?? null;
        this.loadAvailablePersons(user.personId ?? null);
        this.userForm.patchValue({
            personId: user.personId ?? null,
            userName: user.userName,
            email: user.email,
            firstName: user.firstName,
            lastName: user.lastName,
            password: ''
        });
        this.userForm.get('password').clearValidators();
        this.userForm.get('password').updateValueAndValidity();
        this.selectedRoleNames = [...(user.roles || [])];
        this.originalRoleNames = [...this.selectedRoleNames];
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
                        lastName: formVal.lastName,
                        personId: formVal.personId ?? null
                    };
                    if (formVal.password) updateData.password = formVal.password;

                    this.userSvc.update(this.editUserId, updateData).subscribe({
                        next: () => {
                            this.syncRoles(formVal.email, () => {
                                this.toastr.success('User updated successfully');
                                this.showForm = false;
                                this.resetForm();
                                this.loadData();
                            });
                        },
                        error: (err) => this.toastr.error(err.error?.error || err.error?.message || 'Error updating user')
                    });
                } else {
                    const createData: any = {
                        userName: formVal.userName,
                        email: formVal.email,
                        firstName: formVal.firstName,
                        lastName: formVal.lastName,
                        password: formVal.password,
                        personId: formVal.personId ?? null
                    };
                    this.userSvc.create(createData).subscribe({
                        next: () => {
                            this.syncRoles(formVal.email, () => {
                                this.toastr.success('User created successfully');
                                this.showForm = false;
                                this.resetForm();
                                this.loadData();
                            });
                        },
                        error: (err) => {
                            const errors = err.error;
                            if (Array.isArray(errors)) {
                                errors.forEach((e: any) => this.toastr.error(e.description || e.code));
                            } else {
                                this.toastr.error(err.error?.error || err.error?.message || 'Error creating user');
                            }
                        }
                    });
                }
            }
        });
    }

    // Diff selected vs original role lists, then fire add/removeRole for each
    // delta in parallel. Email is the join key the API uses.
    // Errors on individual role calls are surfaced as toasts but do not abort
    // the rest — the user is already saved at this point.
    private syncRoles(email: string, done: () => void) {
        const toAdd = this.selectedRoleNames.filter(r => !this.originalRoleNames.includes(r));
        const toRemove = this.originalRoleNames.filter(r => !this.selectedRoleNames.includes(r));

        if (toAdd.length === 0 && toRemove.length === 0) {
            done();
            return;
        }

        this.isSavingRoles = true;
        const calls = [
            ...toAdd.map(r => this.userSvc.addRole(email, r).pipe(
                catchError(err => {
                    this.toastr.error(err.error?.error || err.error?.message || `Error adding role ${r}`);
                    return of(null);
                })
            )),
            ...toRemove.map(r => this.userSvc.removeRole(email, r).pipe(
                catchError(err => {
                    this.toastr.error(err.error?.error || err.error?.message || `Error removing role ${r}`);
                    return of(null);
                })
            ))
        ];

        forkJoin(calls).subscribe({
            next: () => { this.isSavingRoles = false; done(); },
            error: () => { this.isSavingRoles = false; done(); }
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
                    error: (err) => this.toastr.error(err.error?.message || err.error?.error || err.message || 'Error resetting password')
                });
            }
        });
    }

    // ng-select search: match on fullName, UPI, email, and person type
    // ("Staff", "Student", "Parent") so the admin can narrow by role.
    personSearchFn = (term: string, item: AvailablePerson): boolean => {
        const t = (term || '').trim().toLowerCase();
        if (!t) return true;
        const type = this.personTypeLabel(item.personType).toLowerCase();
        return (item.fullName || '').toLowerCase().includes(t)
            || (item.upi || '').toLowerCase().includes(t)
            || (item.email || '').toLowerCase().includes(t)
            || type.includes(t);
    };

    personTypeLabel(t?: string | null): string {
        if (!t) return '';
        if (t === 'StaffDetails') return 'Staff';
        return t;
    }

    personTypeBadgeClass(t?: string | null): string {
        switch (t) {
            case 'Student': return 'bg-success';
            case 'StaffDetails': return 'bg-primary';
            case 'Parent': return 'bg-warning text-dark';
            default: return 'bg-secondary';
        }
    }
}
