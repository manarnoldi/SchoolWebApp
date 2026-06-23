import {Component, OnInit, ViewChild, ElementRef} from '@angular/core';
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
                    fields: [{key: 'name', label: 'Name', type: 'text', required: true}, {key: 'personType', label: 'Person Type', type: 'select', required: true, options: [{value: 0, label: 'Student'}, {value: 1, label: 'Teacher'}, {value: 2, label: 'Parent'}]}, {key: 'rank', label: 'Rank', type: 'number', required: true}, {key: 'description', label: 'Description', type: 'textarea'}]},
                {name: 'departments', label: 'Departments', endpoint: '/departments', category: 'System Settings',
                    fields: [{key: 'name', label: 'Name', type: 'text', required: true}, {key: 'code', label: 'Code (auto)', type: 'text', readonly: true}, {key: 'staffDetailsId', label: 'Head of Department', type: 'select', optionsEndpoint: '/staffDetails', optionValue: 'id', optionLabel: 'fullName'}, {key: 'description', label: 'Description', type: 'textarea'}]},
                {name: 'educationLevels', label: 'Education Levels', endpoint: '/educationLevels', category: 'System Settings',
                    fields: [{key: 'name', label: 'Name', type: 'text', required: true}, {key: 'abbr', label: 'Abbreviation', type: 'text', required: true}, {key: 'numOfYears', label: 'No. of Years', type: 'number', required: true}, {key: 'educationLevelTypeId', label: 'Education Level Type', type: 'select', required: true, optionsEndpoint: '/educationLevelTypes', optionValue: 'id', optionLabel: 'name'}, {key: 'curriculumId', label: 'Curriculum', type: 'select', required: true, optionsEndpoint: '/curricula', optionValue: 'id', optionLabel: 'name'}, {key: 'rank', label: 'Rank', type: 'number', required: true}, {key: 'description', label: 'Description', type: 'textarea'}]},
                {name: 'learningLevels', label: 'Learning Levels', endpoint: '/learningLevels', category: 'System Settings',
                    fields: [{key: 'name', label: 'Name', type: 'text', required: true}, {key: 'educationLevelId', label: 'Education Level', type: 'select', required: true, optionsEndpoint: '/educationLevels', optionValue: 'id', optionLabel: 'name'}, {key: 'rank', label: 'Rank', type: 'number', required: true}, {key: 'description', label: 'Description', type: 'textarea'}]},
                {name: 'schoolStreams', label: 'Streams', endpoint: '/schoolStreams', category: 'System Settings',
                    fields: [{key: 'name', label: 'Name', type: 'text', required: true}, {key: 'abbreviation', label: 'Abbreviation', type: 'text'}, {key: 'rank', label: 'Rank', type: 'number', required: true}, {key: 'description', label: 'Description', type: 'textarea'}]}
            ],
            links: []
        },
        {
            name: 'Finance',
            icon: 'fas fa-coins',
            color: 'info',
            configs: [
                {name: 'accounts', label: 'Chart of Accounts', endpoint: '/accounts', category: 'Finance',
                    fields: [{key: 'code', label: 'Code', type: 'text', required: true}, {key: 'name', label: 'Name', type: 'text', required: true}, {key: 'accountType', label: 'Type', type: 'select', required: true, options: [{value: 1, label: 'Asset'}, {value: 2, label: 'Liability'}, {value: 3, label: 'Equity'}, {value: 4, label: 'Income'}, {value: 5, label: 'Expense'}]}, {key: 'parentAccountId', label: 'Parent Account', type: 'select', optionsEndpoint: '/accounts', optionValue: 'id', optionLabel: ['code', 'name']}, {key: 'isActive', label: 'Active', type: 'boolean', default: true}, {key: 'description', label: 'Description', type: 'textarea'}]},
                {name: 'feeCategories', label: 'Fee Categories', endpoint: '/feeCategories', category: 'Finance',
                    fields: [{key: 'name', label: 'Name', type: 'text', required: true}, {key: 'incomeAccountId', label: 'Income Account', type: 'select', optionsEndpoint: '/accounts', optionValue: 'id', optionLabel: ['code', 'name'], optionFilter: {field: 'accountType', equals: 4}}, {key: 'rank', label: 'Rank', type: 'number'}, {key: 'isActive', label: 'Active', type: 'boolean', default: true}, {key: 'description', label: 'Description', type: 'textarea'}]},
                {name: 'expenseCategories', label: 'Expense Categories', endpoint: '/expenseCategories', category: 'Finance',
                    fields: [{key: 'name', label: 'Name', type: 'text', required: true}, {key: 'expenseAccountId', label: 'Expense Account', type: 'select', optionsEndpoint: '/accounts', optionValue: 'id', optionLabel: ['code', 'name'], optionFilter: {field: 'accountType', equals: 5}}, {key: 'rank', label: 'Rank', type: 'number'}, {key: 'isActive', label: 'Active', type: 'boolean', default: true}, {key: 'description', label: 'Description', type: 'textarea'}]}
            ],
            links: []
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

    // Hidden trigger used to open the add/edit modal programmatically (on edit);
    // closeBtn dismisses it after a successful save.
    @ViewChild('modalTrigger') modalTrigger: ElementRef;
    @ViewChild('closeBtn') closeBtn: ElementRef;

    // Server-loaded options for relational (select) fields, keyed by field key.
    fieldOptions: {[key: string]: {value: any; label: string}[]} = {};
    private optionsCache: {[endpoint: string]: any[]} = {};

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
        this.loadFieldOptions(config);
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

    // Loads dropdown options for any select field that pulls from a server
    // endpoint (e.g. Curriculum, Education Level, Account). Responses are cached
    // so shared endpoints (like /accounts) are fetched once.
    loadFieldOptions = (config: any) => {
        this.fieldOptions = {};
        (config?.fields || [])
            .filter((f: any) => f.type === 'select' && f.optionsEndpoint)
            .forEach((f: any) => {
                let build = (rows: any[]) => {
                    let filtered = f.optionFilter
                        ? (rows || []).filter((r) => r[f.optionFilter.field] === f.optionFilter.equals)
                        : (rows || []);
                    this.fieldOptions[f.key] = filtered.map((r) => ({
                        value: r[f.optionValue || 'id'],
                        label: Array.isArray(f.optionLabel)
                            ? f.optionLabel.map((k: string) => r[k]).filter((v) => v !== null && v !== undefined && v !== '').join(' - ')
                            : r[f.optionLabel || 'name']
                    }));
                };
                if (this.optionsCache[f.optionsEndpoint]) {
                    build(this.optionsCache[f.optionsEndpoint]);
                } else {
                    this.http.get<any[]>(f.optionsEndpoint).subscribe({
                        next: (rows) => { this.optionsCache[f.optionsEndpoint] = rows || []; build(rows || []); },
                        error: () => { this.fieldOptions[f.key] = []; }
                    });
                }
            });
    };

    // Human-readable cell value: resolve select ids to their label, booleans to
    // Yes/No, everything else as-is.
    displayValue = (item: any, field: any): any => {
        if (field.type === 'boolean') return item[field.key] ? 'Yes' : 'No';
        if (field.type === 'select') {
            let opts = field.options || this.fieldOptions[field.key] || [];
            let match = opts.find((o: any) => o.value == item[field.key]);
            if (match) return match.label;
        }
        return item[field.key];
    };

    resetForm = () => {
        this.formData = {};
        this.editMode = false;
        this.editItem = null;
        if (this.activeConfig) {
            this.activeConfig.fields.forEach((f) => {
                if (f.type === 'number') this.formData[f.key] = 0;
                else if (f.type === 'boolean') this.formData[f.key] = f.default ?? false;
                else if (f.type === 'select') this.formData[f.key] = null;
                else this.formData[f.key] = '';
            });
        }
    };

    // Add: the modal is opened by the "Add" button (data-bs-toggle); we just
    // clear the form so it starts blank.
    openAdd = () => {
        this.resetForm();
    };

    startEdit = (item: any) => {
        this.editMode = true;
        this.editItem = item;
        this.formData = {};
        this.activeConfig.fields.forEach((f) => {
            this.formData[f.key] = item[f.key] ?? '';
        });
        // Open the same add/edit modal, pre-filled.
        this.modalTrigger?.nativeElement?.click();
    };

    private closeModal = () => {
        this.closeBtn?.nativeElement?.click();
    };

    saveItem = () => {
        let nameField = this.activeConfig.fields.find((f) => f.key === 'name');
        if (nameField?.required && !this.formData.name) {
            this.toastr.info('Name is required.');
            return;
        }

        let data = {...this.formData};
        let done = () => { this.closeModal(); this.resetForm(); this.loadItems(); };
        if (this.editMode) {
            data.id = this.editItem.id;
            this.http.put(this.activeConfig.endpoint, data).subscribe({
                next: () => { this.toastr.success('Updated!'); done(); },
                error: (err) => this.toastr.error(err.error?.message || 'Error updating.')
            });
        } else {
            this.http.post(this.activeConfig.endpoint, data).subscribe({
                next: () => { this.toastr.success('Added!'); done(); },
                error: (err) => this.toastr.error(err.error?.message || 'Error adding.')
            });
        }
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
