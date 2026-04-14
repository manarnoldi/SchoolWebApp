import {ResourceService} from '@/core/services/resource.service';
import {HttpClient} from '@angular/common/http';
import {Injectable} from '@angular/core';
import {PCI} from '../models/pci';

@Injectable({
    providedIn: 'root'
})
export class PCIService extends ResourceService<PCI> {
    constructor(private http: HttpClient) {
        super(http, PCI);
    }
}
