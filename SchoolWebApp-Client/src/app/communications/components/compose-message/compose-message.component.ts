import {Component, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import Swal from 'sweetalert2';
import {
    CHANNEL_LABELS,
    CommunicationStatus,
    ComposeMessage,
    ComposePreview,
    MessageChannel,
    MessageTemplate,
    Placeholder,
    RECIPIENT_TYPE_LABELS,
    RecipientGroup,
    RecipientOptions,
    smsInfo
} from '@/communications/models/communication-models';
import {
    CommunicationSettingsService,
    MessageService,
    MessageTemplateService
} from '@/communications/services/communication-services';
import {apiError} from '../communication-settings/communication-settings.component';

/** Compose a custom message to a group of parents or staff, preview it, and queue it. */
@Component({
    selector: 'app-compose-message',
    templateUrl: './compose-message.component.html',
    styleUrls: ['./compose-message.component.scss']
})
export class ComposeMessageComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/communications/compose'], title: 'Send Message'}
    ];
    dashboardTitle = 'Communications: Send Message';

    RecipientGroup = RecipientGroup;
    MessageChannel = MessageChannel;
    channelLabels = CHANNEL_LABELS;
    recipientTypeLabels = RECIPIENT_TYPE_LABELS;
    channels = [MessageChannel.Sms, MessageChannel.Email, MessageChannel.Both];

    groups = [
        {value: RecipientGroup.AllParents, label: 'All parents', icon: 'fas fa-users'},
        {value: RecipientGroup.ParentsOfClasses, label: 'Parents of classes', icon: 'fas fa-chalkboard'},
        {value: RecipientGroup.ParentsOfEducationLevels, label: 'Parents of education levels', icon: 'fas fa-layer-group'},
        {value: RecipientGroup.ParentsOfStudents, label: 'Parents of selected learners', icon: 'fas fa-user-graduate'},
        {value: RecipientGroup.AllStaff, label: 'All staff', icon: 'fas fa-user-tie'},
        {value: RecipientGroup.StaffByCategory, label: 'Staff by category', icon: 'fas fa-id-badge'},
        {value: RecipientGroup.SelectedStaff, label: 'Selected staff', icon: 'fas fa-user-check'},
        {value: RecipientGroup.CustomContacts, label: 'Other numbers / emails', icon: 'fas fa-address-book'}
    ];

    status: CommunicationStatus | null = null;
    options: RecipientOptions = {classes: [], levels: [], categories: [], staff: [], students: []};
    templates: MessageTemplate[] = [];
    placeholders: Placeholder[] = [];

    msg: ComposeMessage = this.blank();
    studentClassFilter: number | null = null;

    preview: ComposePreview | null = null;
    previewPage = 1;
    previewPageSize = 10;
    busy = false;

    private activeField: 'smsBody' | 'emailSubject' | 'emailBody' = 'smsBody';
    private cursor: {[k: string]: number} = {};

    constructor(
        private toastr: ToastrService,
        private router: Router,
        private settingsSvc: CommunicationSettingsService,
        private messageSvc: MessageService,
        private templateSvc: MessageTemplateService
    ) {}

    ngOnInit(): void {
        this.settingsSvc.status().subscribe({next: (s) => this.status = s});
        this.messageSvc.recipientOptions().subscribe({
            next: (o) => { this.options = o; this.buildStudentPicker(); },
            error: (err) => this.toastr.error(apiError(err, 'Could not load classes and staff.'))
        });
        this.templateSvc.getAll().subscribe({
            next: (t) => this.templates = t.filter((x) => !x.isSystem && x.isActive)
        });
        this.templateSvc.placeholders('Custom').subscribe({next: (p) => this.placeholders = p});
    }

    private blank(): ComposeMessage {
        return {
            channel: MessageChannel.Sms,
            recipients: {
                group: RecipientGroup.ParentsOfClasses,
                schoolClassIds: [], educationLevelIds: [], studentIds: [], staffIds: [], staffCategoryIds: []
            },
            smsBody: '',
            emailSubject: '',
            emailBody: ''
        };
    }

    get isParentGroup() { return this.msg.recipients.group <= RecipientGroup.ParentsOfStudents; }
    get usesSms() { return this.msg.channel === MessageChannel.Sms || this.msg.channel === MessageChannel.Both; }
    get usesEmail() { return this.msg.channel === MessageChannel.Email || this.msg.channel === MessageChannel.Both; }
    get sms() { return smsInfo(this.msg.smsBody); }

    // Built once per class-filter change, not in a getter: a fresh array on every
    // change detection made ng-select reset its items mid-click, so a clicked
    // learner was never selected.
    studentsForPicker: {id: number; fullName: string; admissionNo: string; schoolClassId: number; label: string}[] = [];

    buildStudentPicker() {
        let list = this.options.students;
        if (this.studentClassFilter) list = list.filter((s) => s.schoolClassId === this.studentClassFilter);
        this.studentsForPicker = list.map((s) => ({...s, label: `${s.fullName} (${s.admissionNo})`}));
    }

    get channelWarning(): string | null {
        if (!this.status || this.status.testMode) return null;
        if (this.usesSms && !this.status.smsEnabled) return 'SMS sending is switched off for this school.';
        if (this.usesEmail && !this.status.emailEnabled) return 'Email sending is switched off for this school.';
        return null;
    }

    groupChanged() {
        this.preview = null;
    }

    applyTemplate(id: number | null) {
        let t = this.templates.find((x) => x.id === id);
        if (!t) { this.msg.messageTemplateId = undefined; return; }
        this.msg.messageTemplateId = t.id;
        this.msg.channel = t.channel === MessageChannel.None ? MessageChannel.Sms : t.channel;
        this.msg.smsBody = t.smsBody || '';
        this.msg.emailSubject = t.emailSubject || '';
        this.msg.emailBody = t.emailBody || '';
        this.preview = null;
    }

    track(field: 'smsBody' | 'emailSubject' | 'emailBody', ev: any) {
        this.activeField = field;
        this.cursor[field] = ev?.target?.selectionStart ?? (this.msg[field] || '').length;
    }

    insert(p: Placeholder) {
        let field = this.activeField;
        let text = this.msg[field] || '';
        let at = this.cursor[field] ?? text.length;
        this.msg[field] = text.slice(0, at) + p.key + text.slice(at);
        this.cursor[field] = at + p.key.length;
    }

    private validate(): string | null {
        let r = this.msg.recipients;
        switch (r.group) {
            case RecipientGroup.ParentsOfClasses: if (!r.schoolClassIds.length) return 'Choose at least one class.'; break;
            case RecipientGroup.ParentsOfEducationLevels: if (!r.educationLevelIds.length) return 'Choose at least one education level.'; break;
            case RecipientGroup.ParentsOfStudents: if (!r.studentIds.length) return 'Choose at least one learner.'; break;
            case RecipientGroup.StaffByCategory: if (!r.staffCategoryIds.length) return 'Choose at least one staff category.'; break;
            case RecipientGroup.SelectedStaff: if (!r.staffIds.length) return 'Choose at least one staff member.'; break;
            case RecipientGroup.CustomContacts: if (!r.customContacts?.trim()) return 'Enter at least one phone number or email.'; break;
        }
        if (this.usesSms && !this.msg.smsBody?.trim()) return 'Type the SMS text.';
        if (this.usesEmail && (!this.msg.emailSubject?.trim() || !this.msg.emailBody?.trim())) return 'Type the email subject and body.';
        return null;
    }

    doPreview() {
        let problem = this.validate();
        if (problem) { this.toastr.warning(problem); return; }
        this.busy = true;
        this.messageSvc.preview(this.msg).subscribe({
            next: (p) => { this.busy = false; this.preview = p; this.previewPage = 1; },
            error: (err) => { this.busy = false; this.toastr.error(apiError(err)); }
        });
    }

    send() {
        let problem = this.validate();
        if (problem) { this.toastr.warning(problem); return; }
        this.busy = true;
        // Always preview first so the confirmation states exactly what will go out.
        this.messageSvc.preview(this.msg).subscribe({
            next: (p) => {
                this.busy = false;
                this.preview = p;
                let parts: string[] = [];
                if (p.smsCount) parts.push(`<b>${p.smsCount}</b> SMS (${p.smsParts} SMS part${p.smsParts === 1 ? '' : 's'})`);
                if (p.emailCount) parts.push(`<b>${p.emailCount}</b> email${p.emailCount === 1 ? '' : 's'}`);
                if (!parts.length) { this.toastr.warning('None of the recipients has a contact for the chosen channel.'); return; }
                let html = `This will queue ${parts.join(' and ')}.`;
                if (p.missingContact) html += `<br><small>${p.missingContact} recipient(s) have no contact and will be skipped.</small>`;
                if (p.testMode) html += '<br><br><span class="text-warning"><b>Test mode is on</b> - no real parent or staff member will receive it.</span>';
                Swal.fire({
                    title: 'Send message?',
                    html,
                    icon: 'question',
                    showCancelButton: true,
                    confirmButtonText: 'Send'
                }).then((r) => {
                    if (r.isConfirmed) this.queue();
                });
            },
            error: (err) => { this.busy = false; this.toastr.error(apiError(err)); }
        });
    }

    private queue() {
        this.busy = true;
        this.messageSvc.send(this.msg).subscribe({
            next: (r) => {
                this.busy = false;
                this.toastr.success(`${r.queued} message(s) queued${r.testMode ? ' (test mode)' : ''}.`);
                this.router.navigate(['/communications/queue'], {queryParams: {batchId: r.batchId}});
            },
            error: (err) => { this.busy = false; this.toastr.error(apiError(err)); }
        });
    }

    reset() {
        this.msg = this.blank();
        this.preview = null;
        this.studentClassFilter = null;
        this.buildStudentPicker();
    }
}
