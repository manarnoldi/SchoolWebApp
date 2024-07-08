import {BreadCrumb} from '@/core/models/bread-crumb';
import {Component, HostBinding, OnInit, Renderer2} from '@angular/core';

@Component({
    selector: 'app-students',
    templateUrl: './students.component.html',
    styleUrl: './students.component.scss'
})
export class StudentsComponent implements OnInit {
    dashboardTitle = 'SchoolSoftWeb Students';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/'], title: 'Students'}
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
