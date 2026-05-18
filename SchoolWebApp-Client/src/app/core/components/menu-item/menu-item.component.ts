import {Component, HostBinding, Input, OnInit} from '@angular/core';
import {NavigationEnd, Router} from '@angular/router';
import {filter} from 'rxjs/operators';
import {openCloseAnimation, rotateAnimation} from './menu-item.animations';

@Component({
    selector: 'app-menu-item',
    templateUrl: './menu-item.component.html',
    styleUrls: ['./menu-item.component.scss'],
    animations: [openCloseAnimation, rotateAnimation]
})
export class MenuItemComponent implements OnInit {
    @Input() menuItem: any = null;
    @Input() depth: number = 0;
    public isExpandable: boolean = false;
    @HostBinding('class.nav-item') isNavItem: boolean = true;
    @HostBinding('class.menu-open') isMenuExtended: boolean = false;
    public isMainActive: boolean = false;
    public isOneOfChildrenActive: boolean = false;

    constructor(private router: Router) {}

    ngOnInit(): void {
        if (this.menuItem?.children?.length > 0) {
            this.isExpandable = true;
        }

        // initial active check
        this.calculateIsActive(this.router.url);

        // recalc on every navigation
        this.router.events
            .pipe(filter((event) => event instanceof NavigationEnd))
            .subscribe((event: NavigationEnd) => {
                this.calculateIsActive(
                    (event as NavigationEnd).urlAfterRedirects
                );
            });
    }

    public handleMainMenuAction() {
        if (this.isExpandable) {
            this.toggleMenu();
            return;
        }
        if (this.menuItem.path) {
            this.router.navigate(this.menuItem.path);
        }
    }

    public toggleMenu() {
        this.isMenuExtended = !this.isMenuExtended;
    }

    private calculateIsActive(url: string) {
        this.isMainActive = false;
        this.isOneOfChildrenActive = false;

        // Strip query params so pages that persist filter state in the URL
        // (e.g. /cbe/responsibilities/student-assignments?curriculumId=1) still
        // match their menu entry.
        const pathOnly = (url || '').split('?')[0];

        if (this.isExpandable) {
            if (this.hasActiveChild(this.menuItem, pathOnly)) {
                this.isOneOfChildrenActive = true;
                this.isMenuExtended = true;
            }
        } else if (this.menuItem.path && this.isSamePathOrChild(pathOnly, this.menuItem.path[0])) {
            this.isMainActive = true;
        } else if (this.menuItem.extraActivePaths?.some((p: string) => this.isSamePathOrChild(pathOnly, p))) {
            this.isMainActive = true;
        }
    }

    private hasActiveChild(item: any, url: string): boolean {
        if (!item.children) {
            return false;
        }
        return item.children.some(
            (child: any) =>
                (child.path && this.isSamePathOrChild(url, child.path[0])) ||
                (child.extraActivePaths?.some((p: string) => this.isSamePathOrChild(url, p))) ||
                this.hasActiveChild(child, url)
        );
    }

    // Matches when the current url is exactly target OR a sub-route of target
    // (target + '/'). The trailing-slash check is the segment-boundary guard:
    // '/cbe/responsibilities-other' will NOT match '/cbe/responsibilities'
    // even though the second is a prefix of the first.
    private isSamePathOrChild(url: string, target: string): boolean {
        if (!url || !target) return false;
        return url === target || url.startsWith(target + '/');
    }
}
