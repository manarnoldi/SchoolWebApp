import {AppState} from '@/core/store/state';
import {ToggleSidebarMenu} from '@/core/store/ui/actions';
import {UiState} from '@/core/store/ui/state';
import {Component, HostBinding, OnInit, Renderer2} from '@angular/core';
import {NavigationEnd, Router} from '@angular/router';
import {Store} from '@ngrx/store';
import {filter, Observable} from 'rxjs';

// Below this width (AdminLTE's) the menu sits over the page instead of beside it.
const SMALL_SCREEN = 992;

@Component({
    selector: 'app-main',
    templateUrl: './main.component.html',
    styleUrls: ['./main.component.scss']
})
export class MainComponent implements OnInit {
    @HostBinding('class') class = 'wrapper';
    public ui: Observable<UiState>;

    constructor(
        private renderer: Renderer2,
        private store: Store<AppState>,
        private router: Router
    ) {
    }

    private menuOpen = false;

    ngOnInit() {
        this.ui = this.store.select('ui');
        this.renderer.removeClass(
            document.querySelector('app-root'),
            'login-page'
        );
        this.renderer.removeClass(
            document.querySelector('app-root'),
            'register-page'
        );
        this.renderer.addClass(
            document.querySelector('app-root'),
            'layout-fixed'
        );
        this.renderer.addClass(
            document.querySelector('app-root'),
            'layout-footer-fixed'
        );

        this.ui.subscribe(
            ({menuSidebarCollapsed, controlSidebarCollapsed, darkMode}) => {
                this.menuOpen = !menuSidebarCollapsed;
                if (menuSidebarCollapsed) {
                    this.renderer.removeClass(
                        document.querySelector('app-root'),
                        'sidebar-open'
                    );
                    this.renderer.addClass(
                        document.querySelector('app-root'),
                        'sidebar-collapse'
                    );
                } else {
                    this.renderer.removeClass(
                        document.querySelector('app-root'),
                        'sidebar-collapse'
                    );
                    this.renderer.addClass(
                        document.querySelector('app-root'),
                        'sidebar-open'
                    );
                }

                if (controlSidebarCollapsed) {
                    this.renderer.removeClass(
                        document.querySelector('app-root'),
                        'control-sidebar-slide-open'
                    );
                } else {
                    this.renderer.addClass(
                        document.querySelector('app-root'),
                        'control-sidebar-slide-open'
                    );
                }

                if (darkMode) {
                    this.renderer.addClass(
                        document.querySelector('app-root'),
                        'dark-mode'
                    );
                } else {
                    this.renderer.removeClass(
                        document.querySelector('app-root'),
                        'dark-mode'
                    );
                }
            }
        );

        this.closeMenuOnSmallScreens();
        this.router.events.pipe(filter((e) => e instanceof NavigationEnd)).subscribe(() => this.closeMenuOnSmallScreens());
    }

    // Phones and tablets: the menu starts closed, and closes again once a page is picked.
    private closeMenuOnSmallScreens() {
        if (window.innerWidth < SMALL_SCREEN && this.menuOpen) this.store.dispatch(new ToggleSidebarMenu());
    }

    onToggleMenuSidebar() {
        this.store.dispatch(new ToggleSidebarMenu());
    }
}
