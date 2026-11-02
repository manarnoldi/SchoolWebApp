import {Component} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {BreadCrumb} from '@/core/models/bread-crumb';

/**
 * Hosts the two subject-level performance reports under one menu item as tabs:
 * the single-exam Subject Performance snapshot, and the across-the-year Subject
 * Performance Trend. Both child components stay mounted (hidden) so switching
 * tabs preserves each one's loaded report.
 */
@Component({
    selector: 'app-subject-performance-tabs',
    templateUrl: './subject-performance-tabs.component.html',
    styleUrl: './subject-performance-tabs.component.scss'
})
export class SubjectPerformanceTabsComponent {
    dashboardTitle = 'Academics: Subject Performance';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/reports/academics/subject-performance'], title: 'Academics: Subject Performance'}
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
