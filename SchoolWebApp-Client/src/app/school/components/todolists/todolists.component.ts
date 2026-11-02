import {AuthService} from '@/core/services/auth.service';
import {GlobalSettingService} from '@/settings/services/global-setting.service';
import {Component, OnInit, ViewChild} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {forkJoin} from 'rxjs';
import Swal from 'sweetalert2';
import {TodoList} from '../../models/todolist';
import {TodolistsService} from '../../services/todolists.service';
import {TodolistFormComponent} from './todolist-form/todolist-form.component';
import {DateTime} from 'luxon';

@Component({
    selector: 'app-todolists',
    templateUrl: './todolists.component.html',
    styleUrls: ['./todolists.component.scss']
})
export class TodolistsComponent implements OnInit {
    @ViewChild(TodolistFormComponent)
    todolistFormComponent: TodolistFormComponent;

    todoLists: TodoList[] = [];
    todoList: TodoList;

    page = 1;
    pageSize = 4;
    completeBtnColors: string[] = [];
    editMode = false;

    curUserId: number;

    pageSizeChanged = (pageSize: number) => {
        this.pageSize = pageSize;
    };

    pageChanged = (page: number) => {
        this.page = page;
    };

    constructor(
        private todoListSvc: TodolistsService,
        private authService: AuthService,
        private toastr: ToastrService,
        private globalSettingSvc: GlobalSettingService
    ) {}

    ngOnInit(): void {
        this.globalSettingSvc.getByKey('General', 'TodoListPageSize').subscribe({
            next: (setting) => {
                if (setting?.settingValue) {
                    this.pageSize = parseInt(setting.settingValue) || 4;
                }
                this.loadUserTodoLists();
            },
            error: () => this.loadUserTodoLists()
        });
    }

    onMouseOver(index: number) {
        this.completeBtnColors[index] = 'text-success';
    }

    onMouseOut(index: number) {
        this.completeBtnColors[index] = 'text-primary';
    }
    loadUserTodoLists = () => {
        let curUser = this.authService.getCurrentUser();
        this.curUserId = curUser?.id;
        if (!this.curUserId) return;
        this.todoListSvc
            .get('/ToDoLists/byUserId/' + this.curUserId)
            .subscribe(
                (res) => {
                    // Overdue and soonest due first.
                    this.todoLists = res
                        .filter((t) => !t.completed)
                        .sort(
                            (a, b) =>
                                new Date(a.completeBy).getTime() -
                                new Date(b.completeBy).getTime()
                        );
                    this.completeBtnColors = this.todoLists.map((todo) =>
                        todo.completed ? 'text-success' : 'text-primary'
                    );
                },
                (err) => {
                    this.toastr.error(err);
                }
            );
    };

    // Adding or changing a task is quick to undo, so it saves without asking.
    addItem = (todolistItem: TodoList) => {
        todolistItem.userId = this.curUserId;
        todolistItem.completed = false;

        if (this.editMode) {
            this.todoList.itemName = todolistItem.itemName;
            this.todoList.completed = false;
            this.todoList.completeBy = todolistItem.completeBy;
            this.todoList.userId = this.curUserId;
        }

        let reqToProcess = this.editMode
            ? this.todoListSvc.update('/todolists', this.todoList)
            : this.todoListSvc.create('/todolists', new TodoList(todolistItem));

        let replyMsg = this.editMode ? 'Task updated.' : 'Task added.';

        forkJoin([reqToProcess]).subscribe(
            (res) => {
                this.editMode = false;
                this.toastr.success(replyMsg);
                this.todolistFormComponent.resetFormControls();
                this.loadUserTodoLists();
            },
            (err) => {
                this.toastr.error(err.error);
            }
        );
    };

    // "Overdue 2 days", "Due in 25 min", "Due today 14:00", "Due tomorrow 08:00", "Due Fri 09:00", "Due 21 Nov".
    dueLabel(t: TodoList): string {
        const due = DateTime.fromJSDate(new Date(t.completeBy as any));
        if (!due.isValid) return '';
        const now = DateTime.now();
        const mins = Math.round(due.diff(now, 'minutes').minutes);
        if (mins < 0) {
            const late = -mins;
            if (late < 60) return `Overdue ${late} min`;
            if (late < 1440) return `Overdue ${Math.floor(late / 60)} hr${late >= 120 ? 's' : ''}`;
            const days = Math.floor(late / 1440);
            return `Overdue ${days} day${days === 1 ? '' : 's'}`;
        }
        if (mins < 60) return `Due in ${mins} min`;
        const days = Math.round(due.startOf('day').diff(now.startOf('day'), 'days').days);
        if (days === 0) return `Due today ${due.toFormat('HH:mm')}`;
        if (days === 1) return `Due tomorrow ${due.toFormat('HH:mm')}`;
        if (days < 7) return `Due ${due.toFormat('ccc HH:mm')}`;
        return `Due ${due.toFormat(due.year === now.year ? 'd LLL' : 'd LLL yyyy')}`;
    }

    // Red overdue, amber within the hour, blue later today, grey after that.
    dueClass(t: TodoList): string {
        const mins = (new Date(t.completeBy as any).getTime() - Date.now()) / 60000;
        if (mins < 0) return 'text-bg-danger';
        if (mins < 60) return 'text-bg-warning';
        return DateTime.fromJSDate(new Date(t.completeBy as any)).hasSame(DateTime.now(), 'day')
            ? 'text-bg-info' : 'bg-light text-dark border';
    }

    get overdueCount(): number {
        return this.todoLists.filter((t) => new Date(t.completeBy as any).getTime() < Date.now()).length;
    }

    cancelEdit() {
        this.editMode = false;
        this.todolistFormComponent.resetFormControls();
    }

    markCompleted = (todolistItem: TodoList) => {
        todolistItem.completed = true;
        this.todoListSvc.update('/todolists', todolistItem).subscribe(
            (res) => {
                this.editMode = false;
                this.toastr.success('Todo list item marked as completed.');
                this.todolistFormComponent.resetFormControls();
                this.loadUserTodoLists();
            },
            (err) => {
                this.toastr.error(err.error);
            }
        );
    };

    editRecord = (id) => {
        this.todoListSvc.getById(id, '/todolists').subscribe(
            (res) => {
                this.todoList = new TodoList(res);
                this.todolistFormComponent.setFormControls(this.todoList);
                this.todolistFormComponent.editmode = true;
                this.editMode = true;
            },
            (err) => {
                this.toastr.error(err);
            }
        );
    };

    deleteRecord = (id) => {
        Swal.fire({
            title: `Delete todo list item?`,
            text: `Confirm if you want to delete todo list item.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Delete',
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.isConfirmed) {
                this.todoListSvc.delete('/todolists', id).subscribe(
                    (res) => {
                        this.loadUserTodoLists();
                        this.toastr.success(
                            'Todo list item deleted successfully.'
                        );
                    },
                    (err) => {
                        this.toastr.error(err);
                    }
                );
            }
        });
    };
}
