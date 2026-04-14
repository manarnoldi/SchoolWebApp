import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {CommunityServiceActivity} from '../models/community-service-activity';

@Injectable({
    providedIn: 'root'
})
export class CommunityServiceActivityService extends ResourceService<CommunityServiceActivity> {
    constructor(private http: HttpClient) {
        super(http, CommunityServiceActivity);
    }
}
