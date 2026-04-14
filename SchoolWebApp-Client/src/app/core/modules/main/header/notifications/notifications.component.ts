import {AuthService} from '@/core/services/auth.service';
import {TodolistsService} from '@/school/services/todolists.service';
import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';

@Component({
    selector: 'app-notifications',
    templateUrl: './notifications.component.html',
    styleUrls: ['./notifications.component.scss']
})
export class NotificationsComponent implements OnInit {
    pendingTodos: any[] = [];
    overdueTodos: any[] = [];
    notificationCount: number = 0;

    constructor(
        private authService: AuthService,
        private todoListSvc: TodolistsService,
        private router: Router
    ) {}

    ngOnInit(): void {
        this.loadNotifications();
    }

    loadNotifications() {
        let curUser = this.authService.getCurrentUser();
        if (!curUser?.id) return;

        this.todoListSvc.get('/ToDoLists/byUserId/' + curUser.id).subscribe(
            (res) => {
                let now = new Date().getTime();
                let incomplete = res.filter((t) => !t.completed);
                this.overdueTodos = incomplete
                    .filter((t) => new Date(t.completeBy).getTime() < now)
                    .sort((a, b) => new Date(a.completeBy).getTime() - new Date(b.completeBy).getTime())
                    .slice(0, 3);
                this.pendingTodos = incomplete
                    .filter((t) => new Date(t.completeBy).getTime() >= now)
                    .sort((a, b) => new Date(a.completeBy).getTime() - new Date(b.completeBy).getTime())
                    .slice(0, 3);
                this.notificationCount = incomplete.length;
            },
            (err) => {}
        );
    }

    getTimeAgo(dateStr: string): string {
        let now = new Date().getTime();
        let target = new Date(dateStr).getTime();
        let diff = target - now;
        let absDiff = Math.abs(diff);
        let minutes = Math.floor(absDiff / 60000);
        let hours = Math.floor(absDiff / 3600000);
        let days = Math.floor(absDiff / 86400000);

        if (diff < 0) {
            if (days > 0) return days + 'd overdue';
            if (hours > 0) return hours + 'h overdue';
            return minutes + 'm overdue';
        } else {
            if (days > 0) return 'in ' + days + 'd';
            if (hours > 0) return 'in ' + hours + 'h';
            return 'in ' + minutes + 'm';
        }
    }

    goToDashboard() {
        this.router.navigate(['/']);
    }
}
