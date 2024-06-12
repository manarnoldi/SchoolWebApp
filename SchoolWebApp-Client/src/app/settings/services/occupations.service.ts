import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {Occupation} from '../models/occupation';

@Injectable({
    providedIn: 'root'
})
export class OccupationsService extends ResourceService<Occupation> {
    constructor(private http: HttpClient) {
        super(http, Occupation);
    }
}
