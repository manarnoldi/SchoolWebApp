import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {SubStrand} from '../models/sub-strand';

@Injectable({
    providedIn: 'root'
})
export class SubStrandService extends ResourceService<SubStrand> {
    constructor(private http: HttpClient) {
        super(http, SubStrand);
    }
}
