import {Component} from '@angular/core';
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
}
