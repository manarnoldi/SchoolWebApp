import {Component} from '@angular/core';
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
}
