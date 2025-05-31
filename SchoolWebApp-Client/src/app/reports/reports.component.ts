import {BreadCrumb} from '@/core/models/bread-crumb';
import {Component, HostBinding, OnInit, Renderer2} from '@angular/core';

@Component({
    selector: 'app-reports',
    templateUrl: './reports.component.html',
    styleUrl: './reports.component.scss'
})
export class ReportsComponent implements OnInit {
    dashboardTitle = 'SchoolSoftWeb Reports';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/'], title: 'Reports'}
    ];

    @HostBinding('class') class = 'wrapper';
    public sidebarMenuOpened = true;

    constructor(private renderer: Renderer2) {}
    ngOnInit(): void {
        this.renderer.removeClass(
            document.querySelector('app-root'),
            'login-page'
        );
        this.renderer.removeClass(
            document.querySelector('app-root'),
            'register-page'
        );
    }

    toggleMenuSidebar() {
        if (this.sidebarMenuOpened) {
            this.renderer.removeClass(
                document.querySelector('app-root'),
                'sidebar-open'
            );
            this.renderer.addClass(
                document.querySelector('app-root'),
                'sidebar-collapse'
            );
            this.sidebarMenuOpened = false;
        } else {
            this.renderer.removeClass(
                document.querySelector('app-root'),
                'sidebar-collapse'
            );
            this.renderer.addClass(
                document.querySelector('app-root'),
                'sidebar-open'
            );
            this.sidebarMenuOpened = true;
        }
    }
}
