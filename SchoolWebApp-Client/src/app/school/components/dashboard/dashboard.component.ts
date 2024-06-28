import { BreadCrumb } from '@/core/models/bread-crumb';
import {Component} from '@angular/core';

@Component({
    selector: 'app-dashboard',
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent {
    dashboardTitle = "School Web App Dashboard";
    breadcrumbs: BreadCrumb[] = [
        { link: ['/'], title: 'Home' },
        { link: ['/'], title: 'Dashboard' }
      ];
}
