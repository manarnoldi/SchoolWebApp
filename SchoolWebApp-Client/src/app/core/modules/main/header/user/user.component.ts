import {Component, OnInit} from '@angular/core';
import { AuthService } from '@/core/services/auth.service';
import {DateTime} from 'luxon';

@Component({
    selector: 'app-user',
    templateUrl: './user.component.html',
    styleUrls: ['./user.component.scss']
})
export class UserComponent implements OnInit {
    public user;

    constructor(private authService: AuthService) {}

    ngOnInit(): void {
        this.user  = JSON.parse(localStorage.getItem('current_user'));
        this.authService.currentUser = this.user;
    }

    logout() {
        this.authService.doLogout();
    }

    formatDate(date) {
        return DateTime.fromISO(date).toFormat('dd LLL yyyy');
    }
}
