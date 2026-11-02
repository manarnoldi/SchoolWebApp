import {Component} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {BreadCrumb} from '@/core/models/bread-crumb';

/**
 * Hosts the two class-level performance reports under one menu item as tabs:
 * the single-exam Class Performance ranking, and the across-the-year Class
 * Performance Trend. Both stay mounted (hidden) so switching tabs preserves
 * each report's loaded state.
 */
@Component({
    selector: 'app-class-performance-tabs',
    templateUrl: './class-performance-tabs.component.html',
    styleUrl: './class-performance-tabs.component.scss'
})
export class ClassPerformanceTabsComponent {
    dashboardTitle = 'Academics: Class Performance';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/reports/academics/class-performance'], title: 'Academics: Class Performance'}
    ];
    activeNav: 'performance' | 'trend' = 'performance';

    // The open tab lives in the URL (?tab=trend), so a refresh keeps it.
    constructor(private route: ActivatedRoute, private router: Router) {
        if (route.snapshot.queryParamMap.get('tab') === 'trend') this.activeNav = 'trend';
    }

    setTab(tab: 'performance' | 'trend') {
        this.activeNav = tab;
        this.router.navigate([], {relativeTo: this.route, queryParams: {tab}, queryParamsHandling: 'merge', replaceUrl: true});
    }
}
