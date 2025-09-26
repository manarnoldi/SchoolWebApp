import { ResourceService } from '@/core/services/resource.service';
import { HttpClient } from '@angular/common/http';
import {Injectable} from '@angular/core';
import { Competency } from '../models/competency';

@Injectable({
    providedIn: 'root'
})
export class CompetencyService extends ResourceService<Competency> {
    constructor(private http: HttpClient) {
        super(http, Competency);
    }
}
