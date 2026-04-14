import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {Responsibility} from '../models/responsibility';

@Injectable({
    providedIn: 'root'
})
export class ResponsibilityService extends ResourceService<Responsibility> {
    constructor(private http: HttpClient) {
        super(http, Responsibility);
    }
}
