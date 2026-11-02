import {Component} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {BreadCrumb} from '@/core/models/bread-crumb';

/**
 * Combines the exam-output reports under one menu item as tabs: Exam Results
 * (broadsheet), Report Forms, Annual Report Forms (all the year's terms on one
 * form), Missing Marks, and progress across repeated
 * exams of one type (weekly marathons) by class and by learner.
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

    activeNav: 'exam' | 'reportForm' | 'annualReportForm' | 'missing' | 'classProgress' | 'studentProgress' | 'yearProgress' = 'exam';
    visited: {[k: string]: boolean} = {
        exam: false, reportForm: false, annualReportForm: false, missing: false, classProgress: false, studentProgress: false, yearProgress: false
    };

    // The open tab lives in the URL (?tab=reportForm), so a refresh or a link -
    // e.g. from a student's page - opens that tab rather than the first.
    constructor(private route: ActivatedRoute, private router: Router) {
        const tab = route.snapshot.queryParamMap.get('tab') as any;
        this.setTab(tab && tab in this.visited ? tab : 'exam', false);
    }

    setTab(tab: 'exam' | 'reportForm' | 'annualReportForm' | 'missing' | 'classProgress' | 'studentProgress' | 'yearProgress', syncUrl = true) {
        this.activeNav = tab;
        this.visited[tab] = true;
        if (syncUrl)
            this.router.navigate([], {relativeTo: this.route, queryParams: {tab}, queryParamsHandling: 'merge', replaceUrl: true});
    }
}
