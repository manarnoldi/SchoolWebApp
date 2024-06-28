import {AuthService} from '@/core/services/auth.service';
import {TableSettingsService} from '@/shared/services/table-settings.service';
import {Component, OnInit, ViewChild} from '@angular/core';
import {ToastrService} from 'ngx-toastr';
import {forkJoin, Subscription} from 'rxjs';
import Swal from 'sweetalert2';
import {TodoList} from '../../models/todolist';
import {TodolistsService} from '../../services/todolists.service';
import {TodolistFormComponent} from './todolist-form/todolist-form.component';

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
    completeBtnColor = 'blue';
    editMode = false;

    curUserId: number;

    constructor(
        private todoListSvc: TodolistsService,
        private authService: AuthService,
        private toastr: ToastrService
    ) {}

    ngOnInit(): void {
        this.loadUserTodoLists();
    }

    loadUserTodoLists = () => {
        let curUser = this.authService.getCurrentUser();
        this.curUserId = curUser.personId;
        this.todoListSvc.get('/ToDoLists/byStaffId/' + this.curUserId).subscribe(
            (res) => {
                this.todoLists = res;
            },
            (err) => {
                this.toastr.error(err);
            }
        );
    };

    addItem = (todolistItem: TodoList) => {
        Swal.fire({
            title: `${this.editMode ? 'Update' : 'Add'} record?`,
            text: `Confirm if you want to ${
                this.editMode ? 'edit' : 'add'
            } record.`,
            width: 400,
            position: 'top',
            padding: '1em',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: `${this.editMode ? 'Update' : 'Add'}`,
            cancelButtonText: 'Cancel'
        }).then((result) => {
            if (result.value) {
                todolistItem.staffDetailsId = this.curUserId;
                todolistItem.completed = false;

                if (this.editMode) {
                    this.todoList.itemName = todolistItem.itemName;
                    this.todoList.completed = false;
                    this.todoList.completeBy = todolistItem.completeBy;
                    this.todoList.staffDetailsId = this.curUserId;
                }

                let reqToProcess = this.editMode
                    ? this.todoListSvc.update('/todolists', this.todoList)
                    : this.todoListSvc.create(
                          '/todolists',
                          new TodoList(todolistItem)
                      );

                let replyMsg = `Todo list item ${
                    this.editMode ? 'updated' : 'created'
                } successfully!`;

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
            } else if (result.dismiss === Swal.DismissReason.cancel) {
            }
        });
    };

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
