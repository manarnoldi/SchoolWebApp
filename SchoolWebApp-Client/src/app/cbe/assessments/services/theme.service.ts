import {Injectable} from '@angular/core';
import {Theme} from '../models/theme';
import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';

@Injectable({
    providedIn: 'root'
})
export class ThemeService extends ResourceService<Theme> {
    constructor(private http: HttpClient) {
        super(http, Theme);
    }
}
