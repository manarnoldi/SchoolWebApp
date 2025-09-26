import { ResourceService } from '@/core/services/resource.service';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { GeneralOutcome } from '../models/general-outcome';

@Injectable({
  providedIn: 'root'
})
export class GeneralOutcomeService  extends ResourceService<GeneralOutcome> {
    constructor(private http: HttpClient) {
        super(http, GeneralOutcome);
    }
}
