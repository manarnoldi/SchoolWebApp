import {Component, OnInit} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {AuditLogsService} from '../../services/audit-logs.service';
import {ActiveUser} from '../../models/audit-log';

@Component({
    selector: 'app-active-users',
    templateUrl: './active-users.component.html'
})
export class ActiveUsersComponent implements OnInit {
    dashboardTitle = 'Security: Active Users';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/security/active-users'], title: 'Security: Active Users'}
    ];

    users: ActiveUser[] = [];
    loading = false;

    constructor(private svc: AuditLogsService, private toastr: ToastrService) {}

    ngOnInit(): void {
        this.load();
    }

    load = () => {
        this.loading = true;
        this.svc.activeUsers().subscribe({
            next: (users) => {
                this.users = users || [];
                this.loading = false;
            },
            error: (err) => {
                this.toastr.error(err?.error?.message || err?.error || 'Failed to load active users');
                this.loading = false;
            }
        });
    };
}
