import {AppState} from '@/core/store/state';
import {ToggleControlSidebar, ToggleSidebarMenu} from '@/core/store/ui/actions';
import {UiState} from '@/core/store/ui/state';
import {Component, HostBinding, OnInit} from '@angular/core';
import {UntypedFormGroup, UntypedFormControl} from '@angular/forms';
import {Store} from '@ngrx/store';
import { AuthService } from '@/core/services/auth.service';
import {AuditLogsService} from '@/security/services/audit-logs.service';
import {Observable} from 'rxjs';

const BASE_CLASSES = 'main-header navbar navbar-expand';
@Component({
    selector: 'app-header',
    templateUrl: './header.component.html',
    styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit {
    @HostBinding('class') classes: string = BASE_CLASSES;
    public ui: Observable<UiState>;
    public searchForm: UntypedFormGroup;

    // Super administrators see a live count of who is currently signed in.
    isSuperAdmin = false;
    activeUsersCount = 0;

    constructor(
        private authService: AuthService,
        private auditLogsSvc: AuditLogsService,
        private store: Store<AppState>
    ) {}

    ngOnInit() {
        this.ui = this.store.select('ui');
        this.ui.subscribe((state: UiState) => {
            this.classes = `${BASE_CLASSES} ${state.navbarVariant}`;
        });
        this.searchForm = new UntypedFormGroup({
            search: new UntypedFormControl(null)
        });
        this.loadActiveUsers();
    }

    private loadActiveUsers() {
        let user = this.authService.getCurrentUser();
        this.isSuperAdmin = (user?.roles || [])
            .some((r: any) => String(r).toLowerCase() === 'superadministrator');
        if (!this.isSuperAdmin) return;
        this.auditLogsSvc.activeUsers().subscribe({
            next: (users) => { this.activeUsersCount = (users || []).length; },
            error: () => {}
        });
    }

    logout() {
        this.authService.doLogout();
    }

    onToggleMenuSidebar() {
        this.store.dispatch(new ToggleSidebarMenu());
    }

    onToggleControlSidebar() {
        this.store.dispatch(new ToggleControlSidebar());
    }
}
