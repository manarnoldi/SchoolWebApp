import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {Value} from '../models/value';

@Injectable({
    providedIn: 'root'
})
export class ValueService extends ResourceService<Value> {
    constructor(private http: HttpClient) {
        super(http, Value);
    }
}
