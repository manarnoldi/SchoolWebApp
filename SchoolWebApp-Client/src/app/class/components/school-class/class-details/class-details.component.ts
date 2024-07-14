import {ClassLeadership} from '@/class/models/class-leadership';
import {SchoolClass} from '@/class/models/school-class';
import {SchoolClassesService} from '@/class/services/school-classes.service';
import {PersonType} from '@/core/enums/personTypes';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';

@Component({
    selector: 'app-class-details',
    templateUrl: './class-details.component.html',
    styleUrl: './class-details.component.scss'
})
export class ClassDetailsComponent implements OnInit {
    schoolClassId: number = 0;
    schoolClass: SchoolClass;
    classLeaderships: ClassLeadership[] = [];

    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/class/classDetails'], title: 'Class: Class details'}
    ];

    dashboardTitle = 'Class details';
    backLinkUrl: string = '/class/classes';
    personType = PersonType;
    personTypes;

    constructor(
        private route: ActivatedRoute,
        private schoolClassesSvc: SchoolClassesService,
        private toastr: ToastrService
    ) {
        this.personTypes = Object.keys(this.personType).filter((k) =>
            isNaN(Number(k))
        );
    }

    ngOnInit(): void {
        this.loadSelectedSchoolClass();
    }

    loadSelectedSchoolClass = () => {
        this.route.queryParams.subscribe((params) => {
            this.schoolClassId = params['id'];

            let schoolClassByIdReq = this.schoolClassesSvc.getById(
                this.schoolClassId,
                '/schoolClasses'
            );
            let schoolClassLeadersReq = this.schoolClassesSvc.get(
                '/schoolClassLeaders/bySchoolClassId/' +
                    this.schoolClassId.toString()
            );

            forkJoin([schoolClassByIdReq, schoolClassLeadersReq]).subscribe(
                ([schoolClass, classLeaderships]) => {
                    this.schoolClass = schoolClass;
                this.classLeaderships = classLeaderships;
                },
                (err) => {
                    this.toastr.error(err.error);
                }
            );
        });
    };
}
