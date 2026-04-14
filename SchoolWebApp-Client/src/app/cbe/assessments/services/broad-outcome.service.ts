import { ResourceService } from '@/core/services/resource.service';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BroadOutcome } from '../models/broad-outcome';

@Injectable({
  providedIn: 'root'
})
export class BroadOutcomeService  extends ResourceService<BroadOutcome> {
    constructor(private http: HttpClient) {
        super(http, BroadOutcome);
    }
}
