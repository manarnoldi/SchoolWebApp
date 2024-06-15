import {
    Component,
    OnInit,
    OnDestroy,
    Renderer2,
    HostBinding
} from '@angular/core';
import {DateTime} from 'Luxon';

import {UntypedFormGroup, UntypedFormControl, Validators} from '@angular/forms';
import {ToastrService} from 'ngx-toastr';
import {AuthService} from '@/core/services/auth.service';
import {User} from '@/core/models/User';
import {Router} from '@angular/router';
import { AppService } from '@/core/services/app.service';

@Component({
    selector: 'app-login',
    templateUrl: './login.component.html',
    styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit, OnDestroy {
    @HostBinding('class') class = 'login-box';
    public loginForm: UntypedFormGroup;
    public isAuthLoading = false;
    public isGoogleLoading = false;
    public isFacebookLoading = false;

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
            username: new UntypedFormControl(
                'admin',
                Validators.required
            ),
            password: new UntypedFormControl('Admin@123', Validators.required)
        });
    }

    loginByAuth() {
        if (this.loginForm.valid) {
            //this.spinner.show();
            this.authService.signIn(this.loginForm.value).subscribe(
                (result: any) => {
                    sessionStorage.setItem('ssw_token', result.token);
                    this.authService.getUserProfile(result.id).subscribe(
                        (res) => {
                            let cuUser: User = {...res};
                            cuUser.roles = result.roles;

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
                            this.router.navigate(['/']);
                            // this.spinner.hide();
                        },
                        (err) => {
                            this.toastr.error(
                                'An error occured while logging to the application. Contact the system administrator.'
                            );
                            console.log(err);
                            // this.spinner.hide();
                        }
                    );
                },
                (err) => {
                    this.toastr.error(
                        'An error occured while logging to the application. Contact the system administrator.'
                    );
                    // this.spinner.hide();
                }
            );
        } else {
            this.toastr.error('Form is not valid!');
        }
    }

    ngOnDestroy() {
        this.renderer.removeClass(
            document.querySelector('app-root'),
            'login-page'
        );
    }
}
