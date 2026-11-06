import {Component, OnInit} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {TimetableLookups} from '@/timetable/models/timetable-models';
import {TimetableSetupService} from '@/timetable/services/timetable-setup.service';

type SetupTab = 'schedules' | 'subjects' | 'groups' | 'teachers' | 'readiness';

/**
 * Everything the generator needs before it runs: the shape of the school day
 * per group of grades, how subjects are placed, which subjects run side by
 * side, teachers' limits, and a readiness check. The tab is kept in the URL.
 */
@Component({
    selector: 'app-timetable-setup',
    templateUrl: './timetable-setup.component.html',
    styleUrl: '../timetable.scss'
})
export class TimetableSetupComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/timetable/setup'], title: 'Timetable Setup'}
    ];
    dashboardTitle = 'Timetable: Setup';

    tabs: {key: SetupTab; label: string; icon: string}[] = [
        {key: 'schedules', label: 'Bell Schedules', icon: 'fa-bell'},
        {key: 'subjects', label: 'Subject Rules', icon: 'fa-book'},
        {key: 'groups', label: 'Option Groups', icon: 'fa-layer-group'},
        {key: 'teachers', label: 'Teachers', icon: 'fa-user-clock'},
        {key: 'readiness', label: 'Readiness Check', icon: 'fa-clipboard-check'}
    ];
    tab: SetupTab = 'schedules';
    lookups: TimetableLookups | null = null;

    constructor(private svc: TimetableSetupService, private route: ActivatedRoute, private router: Router,
                private toastr: ToastrService) {}

    ngOnInit(): void {
        this.route.queryParamMap.subscribe((q) => {
            let t = q.get('tab') as SetupTab;
            this.tab = this.tabs.some((x) => x.key === t) ? t : 'schedules';
        });
        this.svc.lookups().subscribe({
            next: (l) => (this.lookups = l),
            error: (err) => this.toastr.error(err.error?.message || 'Error loading the timetable set-up.')
        });
    }

    open(tab: SetupTab) {
        this.router.navigate([], {relativeTo: this.route, queryParams: {tab}, queryParamsHandling: 'merge'});
    }
}
