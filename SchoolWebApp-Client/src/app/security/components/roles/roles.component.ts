import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {ToastrService} from 'ngx-toastr';
import Swal from 'sweetalert2';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {RoleService} from '../../services/role.service';
import {AppRole} from '../../models/app-role';

@Component({
    selector: 'app-roles',
    templateUrl: './roles.component.html',
    styleUrl: './roles.component.scss'
})
export class RolesComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/security/roles'], title: 'Role Management'}
    ];
    dashboardTitle = 'Role Management';

    roles: AppRole[] = [];
    roleForm: FormGroup;
    editMode = false;
    editRoleId: number = null;
    showForm = false;
    isLoading = false;

    constructor(
        private roleSvc: RoleService,
        private toastr: ToastrService,
        private fb: FormBuilder
    ) {}

    ngOnInit(): void {
        this.roleForm = this.fb.group({
            name: ['', Validators.required]
        });
        this.loadRoles();
    }

    loadRoles() {
        this.isLoading = true;
        this.roleSvc.getAll().subscribe({
            next: (roles) => {
                this.roles = roles;
                this.isLoading = false;
            },
            error: () => {
                this.toastr.error('Error loading roles');
                this.isLoading = false;
            }
        });
    }

    toggleForm() {
        this.showForm = !this.showForm;
        if (!this.showForm) this.resetForm();
    }

    resetForm() {
        this.editMode = false;
        this.editRoleId = null;
        this.roleForm.reset();
    }

    editRole(role: AppRole) {
        this.editMode = true;
        this.editRoleId = role.id;
        this.showForm = true;
        this.roleForm.patchValue({name: role.name});
    }

    onSubmit() {
        if (this.roleForm.invalid) return;

        Swal.fire({
            title: `${this.editMode ? 'Update' : 'Create'} role?`,
            text: `Confirm to ${this.editMode ? 'update' : 'create'} role.`,
            width: 400, position: 'top', padding: '1em', icon: 'question',
            showCancelButton: true,
            confirmButtonText: this.editMode ? 'Update' : 'Create',
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                const request = this.editMode
                    ? this.roleSvc.update(this.editRoleId, this.roleForm.value)
                    : this.roleSvc.create(this.roleForm.value);

                request.subscribe({
                    next: () => {
                        this.toastr.success(`Role ${this.editMode ? 'updated' : 'created'} successfully`);
                        this.showForm = false;
                        this.resetForm();
                        this.loadRoles();
                    },
                    error: (err) => this.toastr.error(err.error?.error || err.error?.message || 'Error')
                });
            }
        });
    }

    deleteRole(role: AppRole) {
        Swal.fire({
            title: 'Delete role?',
            text: `Delete role "${role.name}"?`,
            width: 400, position: 'top', padding: '1em', icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Delete',
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                this.roleSvc.delete(role.id).subscribe({
                    next: () => {
                        this.toastr.success('Role deleted');
                        this.loadRoles();
                    },
                    error: (err) => this.toastr.error(err.error?.message || 'Error deleting role')
                });
            }
        });
    }
}
