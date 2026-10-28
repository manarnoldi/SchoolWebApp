import Swal from 'sweetalert2';

declare const bootstrap: any;

/**
 * App-wide: dialogs only close from their own buttons (or Esc), never from a
 * click outside them, so a stray click can't lose a half-filled form or
 * dismiss a confirmation. ng-bootstrap's default is set in AppComponent.
 */
export function applyModalDefaults(): void {
    // SweetAlert2 has no global defaults; add ours as the lowest-priority
    // params of every popup, so a call can still pass allowOutsideClick itself.
    const proto = (Swal as any).prototype;
    const main = proto._main;
    proto._main = function (userParams: any, mixinParams: any = {}) {
        const isToast = userParams?.toast ?? mixinParams?.toast;
        return main.call(this, userParams, isToast ? mixinParams : {allowOutsideClick: false, ...mixinParams});
    };

    // Bootstrap modals opened by data-bs-toggle or bootstrap.Modal.
    if (typeof bootstrap !== 'undefined' && bootstrap.Modal) {
        bootstrap.Modal.Default.backdrop = 'static';
    }
}
