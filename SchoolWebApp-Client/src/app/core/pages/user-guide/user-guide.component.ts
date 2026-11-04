import {Component, OnDestroy, OnInit} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {DomSanitizer, SafeHtml} from '@angular/platform-browser';
import {ActivatedRoute} from '@angular/router';
import {Subscription} from 'rxjs';

/**
 * The user guide or the administrator guide (route data `kind`), as the API serves it:
 * only to a signed-in user, only while guides are switched on, and without the
 * chapters of modules the school has not bought. Shown in a frame so its own
 * styles and print layout stay apart from the app's. Kind 'presentation' is Kodetek's
 * client presentation, for SuperAdministrators.
 */
@Component({
    selector: 'app-user-guide',
    templateUrl: './user-guide.component.html',
    styleUrls: ['./user-guide.component.scss']
})
export class UserGuideComponent implements OnInit, OnDestroy {
    kind: 'user' | 'admin' | 'presentation' = 'user';
    html: SafeHtml | null = null;
    isLoading = false;
    unavailable = false;
    private sub: Subscription;

    constructor(private http: HttpClient, private route: ActivatedRoute, private sanitizer: DomSanitizer) {}

    get title() {
        if (this.kind === 'presentation') return 'Presentation';
        return this.kind === 'admin' ? 'Administrator Guide' : 'User Guide';
    }

    ngOnInit(): void {
        this.sub = this.route.data.subscribe((d) => {
            this.kind = d['kind'] === 'admin' || d['kind'] === 'presentation' ? d['kind'] : 'user';
            this.load();
        });
    }

    ngOnDestroy(): void {
        this.sub?.unsubscribe();
    }

    private load() {
        this.html = null;
        this.unavailable = false;
        this.isLoading = true;
        this.http.get(`/userGuide/${this.kind}`, {responseType: 'text'}).subscribe({
            next: (html) => {
                // Our own page from our API, with its own script (contents links, print).
                this.html = this.sanitizer.bypassSecurityTrustHtml(html);
                this.isLoading = false;
            },
            error: () => {
                this.unavailable = true;
                this.isLoading = false;
            }
        });
    }
}
