import {TodoList} from '@/school/models/todolist';
import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';

@Component({
    selector: 'app-todolist-form',
    templateUrl: './todolist-form.component.html',
    styleUrls: ['./todolist-form.component.scss']
})
export class TodolistFormComponent implements OnInit {
    @Output() addToDoListItemEvent = new EventEmitter<TodoList>();
    todoListItemAddForm: FormGroup;
    @Input() editmode: Boolean = false;

    constructor(private formBuilder: FormBuilder) {}

    ngOnInit(): void {
        this.initForm();
    }

    onSubmit = () => {
        let todoListItem = new TodoList(this.todoListItemAddForm.value);

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

    setFormControls = (todoList: TodoList) => {
        this.todoListItemAddForm.setValue({
            itemName: todoList?.itemName,
            completeBy: todoList?.completeBy
        });
    };

    resetFormControls() {
        this.editmode = false;
        this.todoListItemAddForm.reset();
    }

    clearControls = () => {
        this.resetFormControls();
    };
}
