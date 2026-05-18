import {Component, OnInit} from '@angular/core';
import {ActivatedRoute} from '@angular/router';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import {HttpClient} from '@angular/common/http';
import Swal from 'sweetalert2';

interface DropdownConfig {
    name: string;
    label: string;
    endpoint: string;
    category: string;
    fields: {key: string; label: string; type: string; required?: boolean; options?: any[]}[];
}

@Component({
    selector: 'app-dropdown-management',
    templateUrl: './dropdown-management.component.html',
    styleUrl: './dropdown-management.component.scss'
})
export class DropdownManagementComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/settings/dropdowns'], title: 'Dropdown Management'}
    ];
    dashboardTitle = 'Dropdown Management';

    categories = [
        {
            name: 'System Settings',
            icon: 'fas fa-cog',
            color: 'primary',
            configs: [
                {name: 'designations', label: 'Designations', endpoint: '/designations', category: 'System Settings',
                    fields: [{key: 'name', label: 'Name', type: 'text', required: true}, {key: 'rank', label: 'Rank', type: 'number', required: true}, {key: 'description', label: 'Description', type: 'textarea'}]},
                {name: 'occupations', label: 'Occupations', endpoint: '/occupations', category: 'System Settings',
                    fields: [{key: 'name', label: 'Name', type: 'text', required: true}, {key: 'rank', label: 'Rank', type: 'number', required: true}, {key: 'description', label: 'Description', type: 'textarea'}]},
                {name: 'employmentTypes', label: 'Employment Types', endpoint: '/employmentTypes', category: 'System Settings',
                    fields: [{key: 'name', label: 'Name', type: 'text', required: true}, {key: 'rank', label: 'Rank', type: 'number', required: true}, {key: 'description', label: 'Description', type: 'textarea'}]},
                {name: 'genders', label: 'Genders', endpoint: '/genders', category: 'System Settings',
                    fields: [{key: 'name', label: 'Name', type: 'text', required: true}, {key: 'rank', label: 'Rank', type: 'number', required: true}, {key: 'description', label: 'Description', type: 'textarea'}]},
                {name: 'nationalities', label: 'Nationalities', endpoint: '/nationalities', category: 'System Settings',
                    fields: [{key: 'name', label: 'Name', type: 'text', required: true}, {key: 'rank', label: 'Rank', type: 'number', required: true}, {key: 'description', label: 'Description', type: 'textarea'}]},
                {name: 'occurenceTypes', label: 'Occurrence Types', endpoint: '/occurenceTypes', category: 'System Settings',
                    fields: [{key: 'name', label: 'Name', type: 'text', required: true}, {key: 'abbreviation', label: 'Abbreviation', type: 'text'}, {key: 'rank', label: 'Rank', type: 'number', required: true}, {key: 'description', label: 'Description', type: 'textarea'}]},
                {name: 'outcomes', label: 'Outcomes', endpoint: '/outcomes', category: 'System Settings',
                    fields: [{key: 'name', label: 'Name', type: 'text', required: true}, {key: 'rank', label: 'Rank', type: 'number', required: true}, {key: 'description', label: 'Description', type: 'textarea'}]},
                {name: 'relationships', label: 'Relationships', endpoint: '/relationships', category: 'System Settings',
                    fields: [{key: 'name', label: 'Name', type: 'text', required: true}, {key: 'rank', label: 'Rank', type: 'number', required: true}, {key: 'description', label: 'Description', type: 'textarea'}]},
                {name: 'religions', label: 'Religions', endpoint: '/religions', category: 'System Settings',
                    fields: [{key: 'name', label: 'Name', type: 'text', required: true}, {key: 'rank', label: 'Rank', type: 'number', required: true}, {key: 'description', label: 'Description', type: 'textarea'}]},
                {name: 'sessionTypes', label: 'Session Types', endpoint: '/sessionTypes', category: 'System Settings',
                    fields: [{key: 'name', label: 'Name', type: 'text', required: true}, {key: 'rank', label: 'Rank', type: 'number', required: true}, {key: 'description', label: 'Description', type: 'textarea'}]},
                {name: 'staffCategories', label: 'Staff Categories', endpoint: '/staffCategories', category: 'System Settings',
                    fields: [{key: 'name', label: 'Name', type: 'text', required: true}, {key: 'code', label: 'Code', type: 'text', required: true}, {key: 'forTeaching', label: 'For Teaching', type: 'boolean'}, {key: 'rank', label: 'Rank', type: 'number', required: true}, {key: 'description', label: 'Description', type: 'textarea'}]},
                {name: 'learningModes', label: 'Learning Modes', endpoint: '/learningModes', category: 'System Settings',
                    fields: [{key: 'name', label: 'Name', type: 'text', required: true}, {key: 'rank', label: 'Rank', type: 'number', required: true}, {key: 'description', label: 'Description', type: 'textarea'}]},
                {name: 'educationLevelTypes', label: 'Education Level Types', endpoint: '/educationLevelTypes', category: 'System Settings',
                    fields: [{key: 'name', label: 'Name', type: 'text', required: true}, {key: 'abbreviation', label: 'Abbreviation', type: 'text'}, {key: 'rank', label: 'Rank', type: 'number', required: true}, {key: 'description', label: 'Description', type: 'textarea'}]},
                {name: 'classLeadershipRoles', label: 'Leadership Roles', endpoint: '/classLeadershipRoles', category: 'System Settings',
                    fields: [{key: 'name', label: 'Name', type: 'text', required: true}, {key: 'personType', label: 'Person Type', type: 'select', required: true, options: [{value: 0, label: 'Student'}, {value: 1, label: 'Teacher'}, {value: 2, label: 'Parent'}]}, {key: 'rank', label: 'Rank', type: 'number', required: true}, {key: 'description', label: 'Description', type: 'textarea'}]}
            ],
            links: [
                {label: 'Departments', path: '/school/departments', icon: 'fas fa-building'},
                {label: 'Education Levels', path: '/school/educationLevels', icon: 'fas fa-layer-group'}
            ]
        },
        {
            name: 'Finance',
            icon: 'fas fa-coins',
            color: 'info',
            configs: [],
            links: [
                {label: 'Chart of Accounts', path: '/finance/accounts', icon: 'fas fa-book'},
                {label: 'Fee Categories', path: '/finance/fee-categories', icon: 'fas fa-tags'},
                {label: 'Expense Categories', path: '/finance/expense-categories', icon: 'fas fa-list'}
            ]
        },
        {
            name: 'Academics',
            icon: 'fas fa-book-reader',
            color: 'warning',
            configs: [
                {name: 'curricula', label: 'Curricula', endpoint: '/curricula', category: 'Academics',
                    fields: [{key: 'name', label: 'Name', type: 'text', required: true}, {key: 'code', label: 'Code', type: 'text', required: true}, {key: 'rank', label: 'Rank', type: 'number', required: true}, {key: 'description', label: 'Description', type: 'textarea'}]},
                {name: 'subjectGroups', label: 'Subject Groups', endpoint: '/subjectGroups', category: 'Academics',
                    fields: [{key: 'name', label: 'Name', type: 'text', required: true}, {key: 'abbreviation', label: 'Abbreviation', type: 'text'}, {key: 'rank', label: 'Rank', type: 'number', required: true}, {key: 'description', label: 'Description', type: 'textarea'}]}
            ],
            links: [
                {label: 'Subjects', path: '/academics/subjects', icon: 'fas fa-book'},
                {label: 'Edu-Level Subjects', path: '/academics/educationLevelSubjects', icon: 'fas fa-layer-group'},
                {label: 'Grading System', path: '/academics/grades', icon: 'fas fa-chart-bar'}
            ]
        },
        {
            name: 'CBE Settings',
            icon: 'fas fa-graduation-cap',
            color: 'success',
            configs: [
                {name: 'assessmentTypes', label: 'Assessment Types', endpoint: '/assessmentTypes', category: 'CBE Settings',
                    fields: [{key: 'name', label: 'Name', type: 'text', required: true}, {key: 'rank', label: 'Rank', type: 'number', required: true}, {key: 'description', label: 'Description', type: 'textarea'}]},
                {name: 'competencies', label: 'Competencies', endpoint: '/competencies', category: 'CBE Settings',
                    fields: [{key: 'name', label: 'Name', type: 'text', required: true}, {key: 'rank', label: 'Rank', type: 'number', required: true}, {key: 'description', label: 'Description', type: 'textarea'}]},
                {name: 'values', label: 'Values', endpoint: '/values', category: 'CBE Settings',
                    fields: [{key: 'name', label: 'Name', type: 'text', required: true}, {key: 'rank', label: 'Rank', type: 'number', required: true}, {key: 'description', label: 'Description', type: 'textarea'}]},
                {name: 'responsibilities', label: 'Responsibilities & Social Skills', endpoint: '/responsibilities', category: 'CBE Settings',
                    fields: [{key: 'name', label: 'Name', type: 'text', required: true}, {key: 'category', label: 'Category', type: 'text', required: true}, {key: 'rank', label: 'Rank', type: 'number', required: true}, {key: 'description', label: 'Description', type: 'textarea'}]},
                {name: 'coCurriculumActivities', label: 'Co-curricular Activities', endpoint: '/coCurriculumActivities', category: 'CBE Settings',
                    fields: [{key: 'name', label: 'Name', type: 'text', required: true}, {key: 'rank', label: 'Rank', type: 'number', required: true}, {key: 'description', label: 'Description', type: 'textarea'}]},
                {name: 'coCurriculumScoreTypes', label: 'Co-curricular Score Types', endpoint: '/coCurriculumScoreTypes', category: 'CBE Settings',
                    fields: [{key: 'name', label: 'Name', type: 'text', required: true}, {key: 'rank', label: 'Rank', type: 'number', required: true}, {key: 'description', label: 'Description', type: 'textarea'}]},
                {name: 'communityServiceActivities', label: 'Community Service Activities', endpoint: '/communityServiceActivities', category: 'CBE Settings',
                    fields: [{key: 'name', label: 'Name', type: 'text', required: true}, {key: 'rank', label: 'Rank', type: 'number', required: true}, {key: 'description', label: 'Description', type: 'textarea'}]}
            ],
            links: [
                {label: 'Themes', path: '/cbe/assessments/themes', icon: 'fas fa-layer-group'},
                {label: 'Strands', path: '/cbe/assessments/strands', icon: 'fas fa-stream'},
                {label: 'Sub-Strands', path: '/cbe/assessments/sub-strands', icon: 'fas fa-list-ul'},
                {label: 'Specific Outcomes', path: '/cbe/assessments/specific-outcomes', icon: 'fas fa-bullseye'},
                {label: 'General Outcomes', path: '/cbe/assessments/general-outcomes', icon: 'fas fa-globe'},
                {label: 'Broad/Subject Outcomes', path: '/cbe/assessments/broad-outcomes', icon: 'fas fa-book-open'},
                {label: 'Key Questions', path: '/cbe/assessments/key-questions', icon: 'fas fa-question-circle'},
                {label: 'Learning Experiences', path: '/cbe/assessments/learning-experiences', icon: 'fas fa-chalkboard-teacher'},
                {label: 'Lesson Allocations', path: '/cbe/assessments/lesson-allocations', icon: 'fas fa-clock'},
                {label: 'PCIs', path: '/cbe/assessments/pcis', icon: 'fas fa-exclamation-triangle'}
            ]
        }
    ];

    collapsedCategories: Set<string> = new Set();

    activeCategory: any = null;
    activeConfig: any = null;
    items: any[] = [];
    editItem: any = null;
    editMode: boolean = false;
    formData: any = {};

    page: number = 1;
    pageSize: number = 10;
    pageChanged = (page: number) => { this.page = page; };
    pageSizeChanged = (pageSize: number) => { this.pageSize = pageSize; };

    constructor(
        private toastr: ToastrService,
        private http: HttpClient,
        private route: ActivatedRoute
    ) {}

    ngOnInit(): void {
        let openCategory = this.route.snapshot.queryParamMap.get('category');
        if (openCategory) {
            let match = this.categories.find((c) => c.name === openCategory);
            this.activeCategory = match || this.categories[0];
        } else {
            this.activeCategory = this.categories[0];
        }
    }

    selectCategory = (category: any) => {
        if (this.activeCategory === category) {
            // Toggle collapse
            if (this.collapsedCategories.has(category.name)) {
                this.collapsedCategories.delete(category.name);
            } else {
                this.collapsedCategories.add(category.name);
            }
        } else {
            this.activeCategory = category;
            this.collapsedCategories.delete(category.name);
        }
        this.activeConfig = null;
        this.items = [];
    };

    isCategoryExpanded = (category: any): boolean => {
        return this.activeCategory === category && !this.collapsedCategories.has(category.name);
    };

    selectConfig = (config: any) => {
        this.activeConfig = config;
        this.editMode = false;
        this.page = 1;
        this.resetForm();
        this.loadItems();
    };

    loadItems = () => {
        this.http.get<any[]>(this.activeConfig.endpoint).subscribe({
            next: (items) => {
                this.items = items.sort((a, b) => (a.rank || 0) - (b.rank || 0));
            },
            error: (err) => this.toastr.error('Error loading items.')
        });
    };

    resetForm = () => {
        this.formData = {};
        this.editMode = false;
        this.editItem = null;
        if (this.activeConfig) {
            this.activeConfig.fields.forEach((f) => {
                if (f.type === 'number') this.formData[f.key] = 0;
                else if (f.type === 'boolean') this.formData[f.key] = false;
                else if (f.type === 'select') this.formData[f.key] = null;
                else this.formData[f.key] = '';
            });
        }
    };

    startEdit = (item: any) => {
        this.editMode = true;
        this.editItem = item;
        this.formData = {};
        this.activeConfig.fields.forEach((f) => {
            this.formData[f.key] = item[f.key] ?? '';
        });
    };

    saveItem = () => {
        let nameField = this.activeConfig.fields.find((f) => f.key === 'name');
        if (nameField?.required && !this.formData.name) {
            this.toastr.info('Name is required.');
            return;
        }

        Swal.fire({
            title: `${this.editMode ? 'Update' : 'Add'} ${this.activeConfig.label}?`,
            width: 400, position: 'top', padding: '1em', icon: 'question',
            showCancelButton: true, confirmButtonText: this.editMode ? 'Update' : 'Add', cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                let data = {...this.formData};
                if (this.editMode) {
                    data.id = this.editItem.id;
                    this.http.put(this.activeConfig.endpoint, data).subscribe({
                        next: () => { this.toastr.success('Updated!'); this.resetForm(); this.loadItems(); },
                        error: (err) => this.toastr.error(err.error?.message || 'Error updating.')
                    });
                } else {
                    this.http.post(this.activeConfig.endpoint, data).subscribe({
                        next: () => { this.toastr.success('Added!'); this.resetForm(); this.loadItems(); },
                        error: (err) => this.toastr.error(err.error?.message || 'Error adding.')
                    });
                }
            }
        });
    };

    deleteItem = (item: any) => {
        Swal.fire({
            title: `Delete ${item.name}?`, text: 'This cannot be undone.',
            width: 400, position: 'top', padding: '1em', icon: 'warning',
            showCancelButton: true, confirmButtonText: 'Delete', cancelButtonText: 'Cancel', confirmButtonColor: '#d33'
        }).then((result) => {
            if (result.value) {
                this.http.delete(`${this.activeConfig.endpoint}/${item.id}`).subscribe({
                    next: () => { this.toastr.success('Deleted!'); this.loadItems(); },
                    error: (err) => this.toastr.error(err.error?.message || 'Error deleting.')
                });
            }
        });
    };
}
