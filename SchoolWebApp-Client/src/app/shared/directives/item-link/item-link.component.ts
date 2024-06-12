import {Component, Input, OnInit} from '@angular/core';

@Component({
    selector: 'app-item-link',
    templateUrl: './item-link.component.html',
    styleUrls: ['./item-link.component.scss']
})
export class ItemLinkComponent implements OnInit {
    @Input() itemId: number;
    @Input() sourceUrl: string;
    @Input() destinationUrl: string;
    @Input() itemName: string;
    @Input() sourceUrlName: string;

    constructor() {}

    ngOnInit(): void {}
}
