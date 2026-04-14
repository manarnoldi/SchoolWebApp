import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {SpecificOutcome} from '../models/specific-outcome';

@Injectable({
    providedIn: 'root'
})
export class SpecificOutcomeService extends ResourceService<SpecificOutcome> {
    constructor(private http: HttpClient) {
        super(http, SpecificOutcome);
    }
}
