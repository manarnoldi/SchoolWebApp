import { BreadCrumb } from '@/core/models/bread-crumb';
import { Component } from '@angular/core';

@Component({
  selector: 'app-class-report',
  templateUrl: './class-report.component.html',
  styleUrl: './class-report.component.scss'
})
export class ClassReportComponent {
breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/reports/class'], title: 'Reports: Class'}
    ];
    dashboardTitle = 'Reports: Class';
}
