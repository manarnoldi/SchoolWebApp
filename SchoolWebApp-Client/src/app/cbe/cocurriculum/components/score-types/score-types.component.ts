import {BreadCrumb} from '@/core/models/bread-crumb';
import {SettingsTableComponent} from '@/shared/directives/settings-table/settings-table.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {FormGroup, FormBuilder, Validators} from '@angular/forms';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {CoCurriculumScoreType} from '../../models/co-curriculum-score-type';
import {CoCurriculumScoreTypeService} from '../../services/co-curriculum-score-type.service';

@Component({
    selector: 'app-score-types',
    templateUrl: './score-types.component.html',
    styleUrl: './score-types.component.scss'
})
export class ScoreTypesComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(SettingsTableComponent) settingsTblBtn: SettingsTableComponent;
    page = 1;
    pageSize = 10;

    scoreTypeForm: FormGroup;

    buttonTitle: string = 'Add score type';
    tableModel: string = 'coCurriculumScoreType';
    tableTitle: string = 'Co-curriculum score types list';
    tableHeaders: string[] = ['Ref#', 'Name', 'Rank', 'Description', 'Action'];

    editMode = false;
    coCurriculumScoreType: CoCurriculumScoreType;
    isAuthLoading: boolean;
    coCurriculumScoreTypes: CoCurriculumScoreType[] = [];
    tblShowViewButton: false;

    collectionSize = 0;

    constructor(
        private coCurriculumScoreTypeSvc: CoCurriculumScoreTypeService,
        private toastr: ToastrService,
        private formBuilder: FormBuilder
    ) {}
    closeResult = '';
    dashboardTitle = 'CBE Co-curricular: Score Types';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/cbe/cocurriculum/score-types'], title: 'CBE Co-curricular: Score Types'}
    ];

    deleteItem(id) {
        Swal.fire({
            title: `Delete record?`,
            text: `Confirm if you want to delete record.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `Delete`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                this.coCurriculumScoreTypeSvc.delete('/coCurriculumScoreTypes', id).subscribe(
                    (res) => {
                        this.refreshItems();
                        this.toastr.success('Record deleted successfully!');
                    },
                    (err) => {
                        this.toastr.error(err);
                    }
                );
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    }

    editItem(id) {
        this.coCurriculumScoreTypeSvc.getById(id, '/coCurriculumScoreTypes').subscribe(
            (res) => {
                this.coCurriculumScoreType = new CoCurriculumScoreType(res);
                this.scoreTypeForm.setValue({
                    name: this.coCurriculumScoreType.name,
                    rank: this.coCurriculumScoreType.rank,
                    description: this.coCurriculumScoreType.description
                });
                this.editMode = true;
                this.settingsTblBtn.onButtonClick();
            },
            (err) => {
                this.toastr.error(err);
            }
        );
    }

    onSubmit() {
        if (this.scoreTypeForm.invalid) {
            return;
        }

        Swal.fire({
            title: `${this.editMode ? 'Update' : 'Add'} record?`,
            text: `Confirm if you want to ${this.editMode ? 'edit' : 'add'} record.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.editMode ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                if (this.editMode) {
                    this.coCurriculumScoreType.name = this.scoreTypeForm.get('name').value;
                    this.coCurriculumScoreType.description = this.scoreTypeForm.get('description').value;
                    this.coCurriculumScoreType.rank = this.scoreTypeForm.get('rank').value;
                }

                let reqToProcess = this.editMode
                    ? this.coCurriculumScoreTypeSvc.update('/coCurriculumScoreTypes', this.coCurriculumScoreType)
                    : this.coCurriculumScoreTypeSvc.create('/coCurriculumScoreTypes', new CoCurriculumScoreType(this.scoreTypeForm.value));

                let replyMsg = `Co-curriculum score type ${this.editMode ? 'updated' : 'created'} successfully!`;
                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.editMode = false;
                        this.toastr.success(replyMsg);
                        this.refreshItems();
                        this.scoreTypeForm.reset();
                        this.closeButton.nativeElement.click();
                    },
                    (err) => {
                        this.toastr.error(err.error?.message);
                    }
                );
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    }

    get f() {
        return this.scoreTypeForm.controls;
    }

    refreshItems() {
        this.scoreTypeForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            rank: [0, [Validators.required]],
            description: ['']
        });

        this.coCurriculumScoreTypeSvc.get('/coCurriculumScoreTypes').subscribe(
            (res) => {
                this.collectionSize = res.length;
                this.coCurriculumScoreTypes = res.slice(
                    (this.page - 1) * this.pageSize,
                    (this.page - 1) * this.pageSize + this.pageSize
                );
                this.coCurriculumScoreTypes.sort((a, b) => a.rank - b.rank);
                this.isAuthLoading = false;
                this.editMode = false;
            },
            (err) => {
                this.toastr.error(
                    'An error occured while fetching the co-curriculum score types. Contact system administrator.'
                );
                this.isAuthLoading = false;
            }
        );
    }

    ngOnInit(): void {
        this.refreshItems();
    }
    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };

    resetForm() {
        this.scoreTypeForm.reset();
        this.editMode = false;
    }
}
