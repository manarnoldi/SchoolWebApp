import {BreadCrumb} from '@/core/models/bread-crumb';
import {SettingsTableComponent} from '@/shared/directives/settings-table/settings-table.component';
import {Component, OnInit, ViewChild} from '@angular/core';
import {FormGroup, FormBuilder, Validators} from '@angular/forms';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {SocialSkill} from '../../models/social-skill';
import {SocialSkillService} from '../../services/social-skill.service';

@Component({
    selector: 'app-social-skills',
    templateUrl: './social-skills.component.html',
    styleUrl: './social-skills.component.scss'
})
export class SocialSkillsComponent implements OnInit {
    @ViewChild('closebutton') closeButton;
    @ViewChild(SettingsTableComponent) settingsTblBtn: SettingsTableComponent;
    page = 1;
    pageSize = 10;

    socialSkillForm: FormGroup;

    buttonTitle: string = 'Add social skill';
    tableModel: string = 'socialSkill';
    tableTitle: string = 'Social skills list';
    tableHeaders: string[] = ['Ref#', 'Name', 'Rank', 'Description', 'Action'];

    editMode = false;
    socialSkill: SocialSkill;
    isAuthLoading: boolean;
    socialSkills: SocialSkill[] = [];
    tblShowViewButton: false;

    collectionSize = 0;

    constructor(
        private socialSkillSvc: SocialSkillService,
        private toastr: ToastrService,
        private formBuilder: FormBuilder
    ) {}
    closeResult = '';
    dashboardTitle = 'CBE Responsibilities: Social Skills List';
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Home'},
        {link: ['/cbe/responsibilities/social-skills'], title: 'CBE Responsibilities: Social Skills List'}
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
                this.socialSkillSvc.delete('/socialSkills', id).subscribe(
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
        this.socialSkillSvc.getById(id, '/socialSkills').subscribe(
            (res) => {
                this.socialSkill = new SocialSkill(res);
                this.socialSkillForm.setValue({
                    name: this.socialSkill.name,
                    rank: this.socialSkill.rank,
                    description: this.socialSkill.description
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
        if (this.socialSkillForm.invalid) {
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
                    this.socialSkill.name = this.socialSkillForm.get('name').value;
                    this.socialSkill.description = this.socialSkillForm.get('description').value;
                    this.socialSkill.rank = this.socialSkillForm.get('rank').value;
                }

                let reqToProcess = this.editMode
                    ? this.socialSkillSvc.update('/socialSkills', this.socialSkill)
                    : this.socialSkillSvc.create('/socialSkills', new SocialSkill(this.socialSkillForm.value));

                let replyMsg = `Social skill ${this.editMode ? 'updated' : 'created'} successfully!`;
                forkJoin([reqToProcess]).subscribe(
                    (res) => {
                        this.editMode = false;
                        this.toastr.success(replyMsg);
                        this.refreshItems();
                        this.socialSkillForm.reset();
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
        return this.socialSkillForm.controls;
    }

    refreshItems() {
        this.socialSkillForm = this.formBuilder.group({
            name: ['', [Validators.required]],
            rank: [0, [Validators.required]],
            description: ['']
        });

        this.socialSkillSvc.get('/socialSkills').subscribe(
            (res) => {
                this.collectionSize = res.length;
                this.socialSkills = res.slice(
                    (this.page - 1) * this.pageSize,
                    (this.page - 1) * this.pageSize + this.pageSize
                );
                this.socialSkills.sort((a, b) => a.rank - b.rank);
                this.isAuthLoading = false;
                this.editMode = false;
            },
            (err) => {
                this.toastr.error(
                    'An error occured while fetching the social skills. Contact system administrator.'
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
        this.socialSkillForm.reset();
        this.editMode = false;
    }
}
