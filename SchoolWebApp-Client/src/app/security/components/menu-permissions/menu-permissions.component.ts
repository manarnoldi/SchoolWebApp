import {Component, OnInit} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import Swal from 'sweetalert2';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {RoleService} from '../../services/role.service';
import {MenuPermissionService} from '../../services/menu-permission.service';
import {AppRole} from '../../models/app-role';
import {MENU} from '@/core/modules/main/menu-sidebar/menu-sidebar.component';

interface MenuChild {
    path: string;
    name: string;
    subGroup?: string;
}

interface MenuGroup {
    name: string;
    iconClasses: string;
    children: MenuChild[];
    collapsed: boolean;
}

@Component({
    selector: 'app-menu-permissions',
    templateUrl: './menu-permissions.component.html',
    styleUrl: './menu-permissions.component.scss'
})
export class MenuPermissionsComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/security/menu-permissions'], title: 'Menu Permissions'}
    ];
    dashboardTitle = 'Menu Permissions';

    roles: AppRole[] = [];
    selectedRole: AppRole = null;
    menuGroups: MenuGroup[] = [];
    selectedPaths: Set<string> = new Set();
    isLoading = false;
    isSaving = false;

    constructor(
        private roleSvc: RoleService,
        private menuPermSvc: MenuPermissionService,
        private toastr: ToastrService
    ) {}

    ngOnInit(): void {
        this.buildMenuGroups();
        this.roleSvc.getAll().subscribe({
            next: (roles) => this.roles = roles
        });
    }

    buildMenuGroups() {
        for (const item of MENU) {
            if (item.path) {
                // Top-level item with direct path (e.g. Dashboard)
                const pathStr = Array.isArray(item.path) ? item.path.join('/') : item.path;
                this.menuGroups.push({
                    name: item.name,
                    iconClasses: item.iconClasses,
                    children: [{path: pathStr, name: item.name}],
                    collapsed: false
                });
            } else if (item.children) {
                const group: MenuGroup = {
                    name: item.name,
                    iconClasses: item.iconClasses,
                    children: [],
                    collapsed: false
                };
                this.extractChildren(item.children, group.children);
                this.menuGroups.push(group);
            }
        }
    }

    extractChildren(items: any[], target: MenuChild[], subGroup?: string) {
        for (const child of items) {
            if (child.path) {
                const pathStr = Array.isArray(child.path) ? child.path.join('/') : child.path;
                target.push({path: pathStr, name: child.name, subGroup});
            }
            if (child.children) {
                this.extractChildren(child.children, target, child.name);
            }
        }
    }

    get allPaths(): string[] {
        const paths: string[] = [];
        this.menuGroups.forEach(g => g.children.forEach(c => paths.push(c.path)));
        return paths;
    }

    selectRole(role: AppRole) {
        this.selectedRole = role;
        this.isLoading = true;
        this.selectedPaths.clear();

        this.menuPermSvc.getByRole(role.name).subscribe({
            next: (perms) => {
                perms.forEach(p => this.selectedPaths.add(p.menuPath));
                this.isLoading = false;
            },
            error: () => {
                this.isLoading = false;
                this.toastr.error('Error loading permissions');
            }
        });
    }

    isSelected(path: string): boolean {
        return this.selectedPaths.has(path);
    }

    toggleItem(path: string) {
        if (this.selectedPaths.has(path)) {
            this.selectedPaths.delete(path);
        } else {
            this.selectedPaths.add(path);
        }
    }

    getSelectedCount(group: MenuGroup): number {
        return group.children.filter(c => this.selectedPaths.has(c.path)).length;
    }

    isGroupAllSelected(group: MenuGroup): boolean {
        return group.children.every(c => this.selectedPaths.has(c.path));
    }

    isGroupPartial(group: MenuGroup): boolean {
        const count = group.children.filter(c => this.selectedPaths.has(c.path)).length;
        return count > 0 && count < group.children.length;
    }

    toggleGroup(group: MenuGroup) {
        if (this.isGroupAllSelected(group)) {
            group.children.forEach(c => this.selectedPaths.delete(c.path));
        } else {
            group.children.forEach(c => this.selectedPaths.add(c.path));
        }
    }

    selectAll() {
        this.allPaths.forEach(p => this.selectedPaths.add(p));
    }

    deselectAll() {
        this.selectedPaths.clear();
    }

    getSubGroups(group: MenuGroup): string[] {
        const subs = new Set<string>();
        group.children.forEach(c => { if (c.subGroup) subs.add(c.subGroup); });
        return Array.from(subs);
    }

    getChildrenBySubGroup(group: MenuGroup, subGroup: string | null): MenuChild[] {
        return group.children.filter(c => (c.subGroup || null) === subGroup);
    }

    hasSubGroups(group: MenuGroup): boolean {
        return group.children.some(c => !!c.subGroup);
    }

    save() {
        if (!this.selectedRole) return;

        Swal.fire({
            title: 'Save permissions?',
            text: `Save menu permissions for ${this.selectedRole.name}?`,
            width: 400, position: 'top', padding: '1em', icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Save',
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                this.isSaving = true;
                const menuPaths = Array.from(this.selectedPaths).map(path => {
                    let name = path;
                    for (const g of this.menuGroups) {
                        const c = g.children.find(ch => ch.path === path);
                        if (c) { name = g.name + ' > ' + c.name; break; }
                    }
                    return {path, name};
                });

                this.menuPermSvc.save(this.selectedRole.name, menuPaths).subscribe({
                    next: () => {
                        this.toastr.success('Permissions saved successfully');
                        this.isSaving = false;
                    },
                    error: (err) => {
                        this.toastr.error(err.error?.message || 'Error saving permissions');
                        this.isSaving = false;
                    }
                });
            }
        });
    }
}
