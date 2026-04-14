import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {SocialSkill} from '../models/social-skill';

@Injectable({
    providedIn: 'root'
})
export class SocialSkillService extends ResourceService<SocialSkill> {
    constructor(private http: HttpClient) {
        super(http, SocialSkill);
    }
}
