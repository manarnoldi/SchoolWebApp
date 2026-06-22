import {
    Component,
    OnInit,
    OnDestroy,
    Renderer2,
    HostBinding
} from '@angular/core';
import {DateTime} from 'luxon';

import {UntypedFormGroup, UntypedFormControl, Validators} from '@angular/forms';
import {ToastrService} from 'ngx-toastr';
import {AuthService} from '@/core/services/auth.service';
import {User} from '@/core/models/User';
import {Router} from '@angular/router';
import {AppService} from '@/core/services/app.service';
import {retry, timer} from 'rxjs';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit, OnDestroy {
    @HostBinding('class') class = 'login-box';
    public loginForm: UntypedFormGroup;
    public isAuthLoading = false;
    public showPassword = false;

    currentUser = {};
    public currentYear: string = DateTime.now().toFormat('y');

    constructor(
        private renderer: Renderer2,
        private toastr: ToastrService,
        private authService: AuthService,
        private appService: AppService,
        public router: Router
    ) {}

    ngOnInit() {
        this.renderer.addClass(
            document.querySelector('app-root'),
            'login-page'
        );
        this.loginForm = new UntypedFormGroup({
            username: new UntypedFormControl('', Validators.required),
            password: new UntypedFormControl('', Validators.required)
        });
    }

    loginByAuth() {
        if (this.loginForm.valid) {
            this.isAuthLoading = true;
            // Trim stray leading/trailing spaces from the credentials - users
            // sometimes paste/auto-fill a username (or password) with spaces and
            // get an "invalid credentials" error.
            let credentials: any = {
                username: (this.loginForm.value.username || '').trim(),
                password: (this.loginForm.value.password || '').trim()
            };
            // Cold-start mitigation: shared-host workers go idle and the first
            // request after that window dies with ERR_NETWORK_CHANGED while the
            // worker is spinning up. Auto-retry once after 2s so the user
            // doesn't have to click Login twice. The retry hits a warm worker.
            this.authService.signIn(credentials).pipe(
                retry({
                    count: 1,
                    delay: (err: any) => {
                        if (err?.status === 0) {
                            this.toastr.info('Connecting to server, please wait...');
                            return timer(2000);
                        }
                        throw err;
                    }
                })
            ).subscribe(
                (result: any) => {
                    localStorage.setItem('ssw_token', result.token);
                    this.authService.getUserProfile(result.id).subscribe(
                        (res) => {
                            let cuUser: User = {...res};
                            cuUser.roles = result.roles;
                            cuUser.mustChangePassword = !!result.mustChangePassword;
                            // Backend returns the linked Person row's id as `personId`
                            // (PersonId on the .NET DTO). For teachers this Person row
                            // IS their StaffDetails row, so map it to `staffId` for the
                            // per-subject allocation check on the frontend.
                            cuUser.staffId = res?.personId ?? res?.PersonId ?? null;

                            cuUser.roles.forEach((r) => {
                                if (r.toString() == 'Parent')
                                    cuUser.currentUserParent = true;
                                if (r.toString() == 'Student')
                                    cuUser.currentUserStudent = true;
                                if (r.toString() == 'Teacher')
                                    cuUser.currentUserTeacher = true;
                                if (r.toString() == 'Administrator')
                                    cuUser.currentUserAdministrator = true;
                                if (r.toString() == 'HeadTeacher')
                                    cuUser.currentUserHeadTeacher = true;
                                if (r.toString() == 'Visitor')
                                    cuUser.currentUserVisitor = true;
                                if (r.toString() == 'Accounts')
                                    cuUser.currentUserAccounts = true;
                            });
                            this.authService.setCurrentUser(cuUser);
                            this.appService.setUserLoggedIn(true);
                            this.isAuthLoading = false;
                            if (cuUser.mustChangePassword) {
                                this.toastr.info('Please change your password before continuing.');
                                this.router.navigate(['/change-password']);
                            } else {
                                this.router.navigate(['/']);
                            }
                        },
                        (err) => {
                            this.isAuthLoading = false;
                            this.toastr.error(
                                'An error occurred while logging in. Contact the system administrator.'
                            );
                            console.log(err);
                        }
                    );
                },
                (err) => {
                    this.isAuthLoading = false;
                    if (err.status === 401) {
                        this.toastr.error('Invalid username or password.');
                    } else {
                        this.toastr.error(
                            'An error occurred while logging in. Contact the system administrator.'
                        );
                    }
                }
            );
        } else {
            this.loginForm.markAllAsTouched();
            this.toastr.warning('Please fill in all required fields.');
        }
    }

    ngOnDestroy() {
        this.renderer.removeClass(
            document.querySelector('app-root'),
            'login-page'
        );
    }
}
