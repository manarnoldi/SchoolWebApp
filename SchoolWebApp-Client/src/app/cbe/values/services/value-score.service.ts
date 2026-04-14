import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {ValueScore} from '../models/value-score';

@Injectable({
    providedIn: 'root'
})
export class ValueScoreService extends ResourceService<ValueScore> {
    constructor(private http: HttpClient) {
        super(http, ValueScore);
    }
}
