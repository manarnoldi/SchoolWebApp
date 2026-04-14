import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {KeyQuestion} from '../models/key-question';

@Injectable({
    providedIn: 'root'
})
export class KeyQuestionService extends ResourceService<KeyQuestion> {
    constructor(private http: HttpClient) {
        super(http, KeyQuestion);
    }
}
