import {TodoList} from '@/school/models/todolist';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {DateTime} from 'luxon';

@Component({
    selector: 'app-todolist-form',
    templateUrl: './todolist-form.component.html',
    styleUrls: ['./todolist-form.component.scss']
})
export class TodolistFormComponent implements OnInit {
    @Output() addToDoListItemEvent = new EventEmitter<TodoList>();
    // Editing was cancelled with the clear button.
    @Output() cancelEvent = new EventEmitter<void>();
    todoListItemAddForm: FormGroup;
    @Input() editmode: Boolean = false;

    constructor(private formBuilder: FormBuilder) {}

    ngOnInit(): void {
        this.initForm();
    }

    // The picker gives local time; the API keeps UTC (and returns it marked as UTC),
    // so it is sent as UTC - otherwise a 15:30 task comes back as 18:30.
    onSubmit = () => {
        let value = this.todoListItemAddForm.value;
        let due = value.completeBy ? new Date(value.completeBy) : null;
        let todoListItem = new TodoList({...value, completeBy: due && !isNaN(due.getTime()) ? (due.toISOString() as any) : value.completeBy});

        this.addToDoListItemEvent.emit(todoListItem);
    };

    initForm = () => {
        this.todoListItemAddForm = this.formBuilder.group({
            itemName: ['', [Validators.required]],
            completeBy: [null, [Validators.required]]
        });
    };

    get f() {
        return this.todoListItemAddForm.controls;
    }

    // The date picker takes local "yyyy-MM-ddTHH:mm", whatever form the date came in.
    setFormControls = (todoList: TodoList) => {
        const due = todoList?.completeBy ? DateTime.fromJSDate(new Date(todoList.completeBy as any)) : null;
        this.todoListItemAddForm.setValue({
            itemName: todoList?.itemName,
            completeBy: due?.isValid ? due.toFormat("yyyy-LL-dd'T'HH:mm") : null
        });
    };

    resetFormControls() {
        this.editmode = false;
        this.todoListItemAddForm.reset();
    }

    clearControls = () => {
        const editing = this.editmode;
        this.resetFormControls();
        if (editing) this.cancelEvent.emit();
    };
}
