import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {Parent} from '../models/parent';

@Injectable({
    providedIn: 'root'
})
export class ParentsService extends ResourceService<Parent> {
    constructor(private http: HttpClient) {
        super(http, Parent);
    }
}
