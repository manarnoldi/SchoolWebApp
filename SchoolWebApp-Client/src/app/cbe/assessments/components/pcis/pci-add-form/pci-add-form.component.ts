import {Curriculum} from '@/academics/models/curriculum';
import {Subject} from '@/academics/models/subject';
import {PCI} from '@/cbe/assessments/models/pci';
import {Strand} from '@/cbe/assessments/models/strand';
import {SubStrand} from '@/cbe/assessments/models/sub-strand';
import {LearningLevel} from '@/class/models/learning-level';
import {AcademicYear} from '@/school/models/academic-year';
import {
    Component,
    ElementRef,
    EventEmitter,
    Input,
    OnInit,
    Output,
    ViewChild
} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';

@Component({
    selector: 'app-pci-add-form',
    templateUrl: './pci-add-form.component.html',
    styleUrl: './pci-add-form.component.scss'
})
export class PCIAddFormComponent implements OnInit {
    @ViewChild('closeButton') closeButton: ElementRef;

    @Input() pci: PCI;
    @Input() strands: Strand[];
    @Input() subStrands: SubStrand[];
    @Input() subjects: Subject[];
    @Input() learningLevels: LearningLevel[];
    @Input() curricula: Curriculum[] = [];
    @Input() academicYears: AcademicYear[] = [];
    @Input() editMode: boolean = false;

    @Output() addItemEvent = new EventEmitter<PCI>();
    @Output() errorEvent = new EventEmitter<string>();
    @Output() curriculumChangedEvent = new EventEmitter<number>();
    @Output() academicYearChangedEvent = new EventEmitter<number>();
    @Output() learningLevelChangedEvent = new EventEmitter<number>();
    @Output() subjectChangedEvent = new EventEmitter<number>();
    @Output() strandChangedEvent = new EventEmitter<number>();

    pciForm: FormGroup;

    constructor(private formBuilder: FormBuilder) {}

    ngOnInit(): void {
        this.refreshItems();
    }

    refreshItems = () => {
        this.pciForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            rank: [0, [Validators.required]],
            description: [''],
            academicYearId: [null, [Validators.required]],
            curriculumId: [null, [Validators.required]],
            subjectId: [null, [Validators.required]],
            learningLevelId: [null, [Validators.required]],
            strandId: [null, [Validators.required]],
            subStrandId: [null, [Validators.required]]
        });
    };

    setFormControls = (pci: PCI) => {
        this.pciForm.setValue({
            name: pci.name,
            rank: pci.rank,
            description: pci.description,
            academicYearId: pci.academicYearId ?? null,
            curriculumId: pci.curriculumId ?? null,
            subjectId: pci.subjectId ?? null,
            learningLevelId: pci.learningLevelId ?? null,
            strandId: pci.strandId ?? null,
            subStrandId: pci.subStrandId ?? null
        });
    };

    get f() {
        return this.pciForm.controls;
    }

    closePCIForm = () => {
        this.closeButton.nativeElement.click();
        this.resetFormControls();
    };

    resetFormControls() {
        this.editMode = false;
        this.pciForm.reset();
    }

    onAcademicYearChange = () => {
        let academicYearId = this.pciForm.get('academicYearId').value;
        this.academicYearChangedEvent.emit(academicYearId);
    };

    onCurriculumChange = () => {
        let curriculumId = this.pciForm.get('curriculumId').value;
        this.curriculumChangedEvent.emit(curriculumId);
    };

    onLearningLevelChange = () => {
        let learningLevelId = this.pciForm.get('learningLevelId').value;
        this.learningLevelChangedEvent.emit(learningLevelId);
    };

    onSubjectChange = () => {
        let subjectId = this.pciForm.get('subjectId').value;
        this.subjectChangedEvent.emit(subjectId);
    };

    onStrandChange = () => {
        let strandId = this.pciForm.get('strandId').value;
        this.strandChangedEvent.emit(strandId);
    };

    onSubmit = () => {
        if (this.editMode) {
            let pciId = this.pci.id;
            this.pci = new PCI(this.pciForm.value);
            this.pci.id = pciId;
        } else {
            this.pci = new PCI(this.pciForm.value);
        }
        this.addItemEvent.emit(this.pci);
    };
}
