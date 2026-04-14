import {Component, OnInit} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {UserService} from '../../services/user.service';
import {RoleService} from '../../services/role.service';
import {AppUser} from '../../models/app-user';
import {AppRole} from '../../models/app-role';

@Component({
    selector: 'app-user-roles',
    templateUrl: './user-roles.component.html',
    styleUrl: './user-roles.component.scss'
})
export class UserRolesComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/security/user-roles'], title: 'User Role Assignment'}
    ];
    dashboardTitle = 'User Role Assignment';

    users: AppUser[] = [];
    roles: AppRole[] = [];
    selectedUser: AppUser = null;
    isLoading = false;
    isSaving = false;

    constructor(
        private userSvc: UserService,
        private roleSvc: RoleService,
        private toastr: ToastrService
    ) {}

    ngOnInit(): void {
        this.loadData();
    }

    loadData() {
        this.isLoading = true;
        this.userSvc.getAll().subscribe({
            next: (users) => {
                this.users = users;
                this.isLoading = false;
            },
            error: () => {
                this.toastr.error('Error loading users');
                this.isLoading = false;
            }
        });
        this.roleSvc.getAll().subscribe({
            next: (roles) => this.roles = roles
        });
    }

    selectUser(user: AppUser) {
        this.selectedUser = user;
    }

    hasRole(roleName: string): boolean {
        return this.selectedUser?.roles?.includes(roleName) || false;
    }

    toggleRole(role: AppRole) {
        if (!this.selectedUser) return;
        this.isSaving = true;

        if (this.hasRole(role.name)) {
            this.userSvc.removeRole(this.selectedUser.email, role.name).subscribe({
                next: () => {
                    this.selectedUser.roles = this.selectedUser.roles.filter(r => r !== role.name);
                    // Also update the users array
                    const u = this.users.find(x => x.id === this.selectedUser.id);
                    if (u) u.roles = [...this.selectedUser.roles];
                    this.toastr.success(`Removed ${role.name} from ${this.selectedUser.userName}`);
                    this.isSaving = false;
                },
                error: (err) => {
                    this.toastr.error(err.error?.error || 'Error removing role');
                    this.isSaving = false;
                }
            });
        } else {
            this.userSvc.addRole(this.selectedUser.email, role.name).subscribe({
                next: () => {
                    this.selectedUser.roles = [...(this.selectedUser.roles || []), role.name];
                    const u = this.users.find(x => x.id === this.selectedUser.id);
                    if (u) u.roles = [...this.selectedUser.roles];
                    this.toastr.success(`Added ${role.name} to ${this.selectedUser.userName}`);
                    this.isSaving = false;
                },
                error: (err) => {
                    this.toastr.error(err.error?.error || 'Error adding role');
                    this.isSaving = false;
                }
            });
        }
    }
}
