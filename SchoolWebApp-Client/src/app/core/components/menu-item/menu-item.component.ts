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

        if (this.isExpandable) {
            if (this.hasActiveChild(this.menuItem, url)) {
                this.isOneOfChildrenActive = true;
                this.isMenuExtended = true;
            }
        } else if (this.menuItem.path && this.menuItem.path[0] === url) {
            this.isMainActive = true;
        }
    }

    private hasActiveChild(item: any, url: string): boolean {
        if (!item.children) {
            return false;
        }
        return item.children.some(
            (child: any) =>
                (child.path && url.startsWith(child.path[0])) ||
                this.hasActiveChild(child, url)
        );
    }
}
