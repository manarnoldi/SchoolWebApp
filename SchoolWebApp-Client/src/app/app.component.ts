import {ChangeDetectorRef, Component, OnInit, ViewChild, inject} from '@angular/core';
import {DEFAULT_INTERRUPTSOURCES, Idle} from '@ng-idle/core';
import {AuthService} from './core/services/auth.service';
import {AppService} from './core/services/app.service';
import {NgbModal} from '@ng-bootstrap/ng-bootstrap';
import {Router} from '@angular/router';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit{
    @ViewChild('childModal', {static: false}) private childModal;
    private modalService = inject(NgbModal);
    modalReference: any;
    // some fields to store our state so we can display it in the UI
    idleState = 'Not started.';
    countdown?: number = null;
    lastPing?: Date = null;
    isIdleState = false;
    timedOut = false;

    // add parameters for Idle and Keepalive (if using) so Angular will inject them from the module
    constructor(
        private idle: Idle,
        cd: ChangeDetectorRef,
        private authService: AuthService,
        private appService: AppService,
        public router: Router
    ) {
        // sets an idle timeout of 5 seconds, for testing purposes.
        idle.setIdle(60);
        // sets a timeout period of 5 seconds. after 10 seconds of inactivity, the user will be considered timed out.
        idle.setTimeout(60);
        // sets the default interrupts, in this case, things like clicks, scrolls, touches to the document
        idle.setInterrupts(DEFAULT_INTERRUPTSOURCES);

        idle.onIdleEnd.subscribe(() => {
            this.idleState = 'No longer idle.';
            // idle.setInterrupts(DEFAULT_INTERRUPTSOURCES);
            // idle.watch();
            this.timedOut = false;
        });

        idle.onTimeout.subscribe(() => {
            this.modalReference.close();
            this.idleState = 'Timed out!';
            this.timedOut = true;
            idle.stop();
            this.authService.doLogout();
        });

        idle.onIdleStart.subscribe(() => {
            this.idleState = "You've gone idle!";
            this.isIdleState = true;
            idle.clearInterrupts();
            this.modalReference = this.modalService.open(this.childModal, {
                backdrop: 'static',
                backdropClass: 'light-blue-backdrop'
            });

            this.modalReference.result.then((result) => {
                if (result == 'logout') {
                    this.idleState = 'Logged out!';
                    this.appService.setUserLoggedIn(false);
                    this.authService.doLogout();
                } else if (result == 'resume') {
                    idle.setInterrupts(DEFAULT_INTERRUPTSOURCES);
                    idle.watch();
                    this.timedOut = false;
                    this.idleState = 'Resumed.';
                }
            });
        });

        idle.onTimeoutWarning.subscribe((countdown) => {
            this.idleState =
                'You will be logged out in ' + countdown + ' seconds!';
        });

        

        // this.reset();
    }
    ngOnInit(): void {
        this.appService.getUserLoggedIn().subscribe((userLoggedIn) => {
            if (userLoggedIn) {
                this.idle.setInterrupts(DEFAULT_INTERRUPTSOURCES);
                this.idle.watch();
                this.timedOut = false;
                this.idleState = 'Watching.';
            } else {
                this.idle.stop();
                this.router.navigate(['/login']);
            }
        });
    }
}
