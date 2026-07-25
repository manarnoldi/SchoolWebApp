import {Component} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';

/**
 * Combines the three exam-output reports under one menu item as tabs: Exam
 * Results (broadsheet), Report Forms and Missing Marks.
 *
 * Each tab's component is heavy (large per-class data loads), so a tab is only
 * mounted the first time it is opened (`visited`), then kept alive with
 * `[hidden]` so its loaded state survives further tab switches.
 */
@Component({
    selector: 'app-exam-results-tabs',
    templateUrl: './exam-results-tabs.component.html',
    styleUrl: './exam-results-tabs.component.scss'
})
export class ExamResultsTabsComponent {
    dashboardTitle = 'Academics: Exam Results';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/reports/academics/exam-results'], title: 'Academics: Exam Results'}
    ];

    activeNav: 'exam' | 'reportForm' | 'missing' = 'exam';
    visited: {[k: string]: boolean} = {exam: true, reportForm: false, missing: false};

    setTab(tab: 'exam' | 'reportForm' | 'missing') {
        this.activeNav = tab;
        this.visited[tab] = true;
    }
}
