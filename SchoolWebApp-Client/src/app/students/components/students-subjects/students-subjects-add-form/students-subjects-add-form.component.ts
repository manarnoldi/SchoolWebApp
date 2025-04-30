import {Curriculum} from '@/academics/models/curriculum';
import {Subject} from '@/academics/models/subject';
import {SchoolClass} from '@/class/models/school-class';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {AcademicYear} from '@/school/models/academic-year';
import {EducationLevel} from '@/school/models/educationLevel';
import { StudentDetails } from '@/students/models/student-details';
import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';

@Component({
    selector: 'app-students-subjects-add-form',
    templateUrl: './students-subjects-add-form.component.html',
    styleUrl: './students-subjects-add-form.component.scss'
})
export class StudentsSubjectsAddFormComponent implements OnInit {
    academicYears: AcademicYear[] = [];
    curricula: Curriculum[] = [];
    subjects: Subject[] = [];
    students: StudentDetails[] = [];
    educationLevels: EducationLevel[] = [];
    schoolClasses: SchoolClass[] = [];

    isLoading: boolean = true;

    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/students/students-subjects'], title: 'Students Subject List'},
        {
            link: ['/students/students-subjects/add'],
            title: 'Add Students Subjects'
        }
    ];
    dashboardTitle = 'Academics: Add Students Subjects';
    studentSubjectId: number;
    studentSubjectsAddForm: FormGroup;
    editMode: boolean = false;

    constructor(private formBuilder: FormBuilder) {
        // private examTypesSvc: ExamTypesService, // private academicYearsSvc: AcademicYearsService, // private curriculaSvc: CurriculumService, // private sessionSvc: SessionsService, // private formBuilder: FormBuilder,
        // private examsSvc: ExamsService,
        // private subjectsSvc: SubjectsService,
        // private router: Router,
        // private route: ActivatedRoute,
        // private educationLevelSubjectSvc: EducationLevelSubjectService,
        // private schoolClassesSvc: SchoolClassesService,
        // private educationLevelSvc: EducationLevelService,
        // private toastr: ToastrService,
        // private datePipe: DatePipe
    }
    ngOnInit(): void {
        this.initializeForm();
    }

    academicYearCurriculumChanged = () => {};

    educationLevelChanged = () => {};

    initializeForm = () => {
        this.studentSubjectsAddForm = this.formBuilder.group({
            academicYearId: [null, Validators.required],
            schoolClassId: [null, Validators.required],
            curriculumId: [null, Validators.required],
            educationLevelId: [null, Validators.required]
        });
    };

    onSubmit = () => {};

    get f() {
        return this.studentSubjectsAddForm.controls;
    }
}
