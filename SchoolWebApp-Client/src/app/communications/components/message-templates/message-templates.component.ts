import {Component, OnInit} from '@angular/core';
import {BreadCrumb} from '@/core/models/bread-crumb';
import {ToastrService} from 'ngx-toastr';
import Swal from 'sweetalert2';
import {
    CHANNEL_LABELS,
    MessageChannel,
    MessageTemplate,
    Placeholder,
    smsInfo
} from '@/communications/models/communication-models';
import {MessageTemplateService} from '@/communications/services/communication-services';
import {apiError} from '../communication-settings/communication-settings.component';

/**
 * Message types and templates. The standard types (results, invoices, balance
 * reminders, payments) are system templates: the school picks SMS / email /
 * both for each and edits the wording. Other templates are saved wording for
 * custom messages.
 */
@Component({
    selector: 'app-message-templates',
    templateUrl: './message-templates.component.html'
})
export class MessageTemplatesComponent implements OnInit {
    breadcrumbs: BreadCrumb[] = [
        {link: ['/'], title: 'Dashboard'},
        {link: ['/communications/templates'], title: 'Message Templates'}
    ];
    dashboardTitle = 'Communications: Message Types & Templates';

    channelLabels = CHANNEL_LABELS;
    channels = [MessageChannel.Sms, MessageChannel.Email, MessageChannel.Both];
    MessageChannel = MessageChannel;

    templates: MessageTemplate[] = [];
    item: MessageTemplate | null = null;
    placeholders: Placeholder[] = [];
    // Which field a clicked placeholder is inserted into.
    activeField: 'smsBody' | 'emailSubject' | 'emailBody' = 'smsBody';
    private cursor: {[k: string]: number} = {};

    constructor(private toastr: ToastrService, private svc: MessageTemplateService) {}

    ngOnInit(): void {
        this.load();
    }

    get systemTemplates() { return this.templates.filter((t) => t.isSystem); }
    get customTemplates() { return this.templates.filter((t) => !t.isSystem); }

    load() {
        this.svc.getAll().subscribe({
            next: (r) => this.templates = r,
            error: (err) => this.toastr.error(apiError(err, 'Could not load the templates.'))
        });
    }

    sms(text?: string) { return smsInfo(text); }

    usesSms(t: MessageTemplate) { return t.channel === MessageChannel.Sms || t.channel === MessageChannel.Both; }
    usesEmail(t: MessageTemplate) { return t.channel === MessageChannel.Email || t.channel === MessageChannel.Both; }

    addNew() {
        this.open({name: '', channel: MessageChannel.Sms, isActive: true});
    }

    edit(t: MessageTemplate) {
        this.open({...t});
    }

    private open(t: MessageTemplate) {
        this.item = t;
        this.activeField = 'smsBody';
        this.cursor = {};
        this.svc.placeholders(t.code || 'Custom').subscribe({
            next: (r) => this.placeholders = r,
            error: () => this.placeholders = []
        });
    }

    cancel() { this.item = null; }

    track(field: 'smsBody' | 'emailSubject' | 'emailBody', ev: any) {
        this.activeField = field;
        this.cursor[field] = ev?.target?.selectionStart ?? (this.item?.[field] || '').length;
    }

    insert(p: Placeholder) {
        if (!this.item) return;
        let field = this.activeField;
        let text = this.item[field] || '';
        let at = this.cursor[field] ?? text.length;
        this.item[field] = text.slice(0, at) + p.key + text.slice(at);
        this.cursor[field] = at + p.key.length;
    }

    save() {
        let t = this.item;
        if (!t) return;
        if (!t.name?.trim()) { this.toastr.warning('Enter the template name.'); return; }
        let req = t.id ? this.svc.update(t) : this.svc.create(t);
        req.subscribe({
            next: () => { this.toastr.success('Template saved.'); this.item = null; this.load(); },
            error: (err) => this.toastr.error(apiError(err))
        });
    }

    // Quick channel / on-off change from the list, without opening the editor.
    quickSave(t: MessageTemplate) {
        this.svc.update(t).subscribe({
            next: () => this.toastr.success(`${t.name}: ${t.isActive ? this.channelLabels[t.channel] : 'switched off'}.`),
            error: (err) => { this.toastr.error(apiError(err)); this.load(); }
        });
    }

    delete(t: MessageTemplate) {
        Swal.fire({
            title: 'Delete template?',
            text: `"${t.name}" will be deleted. Messages already sent with it are kept.`,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Delete',
            confirmButtonColor: '#d33'
        }).then((r) => {
            if (!r.isConfirmed || !t.id) return;
            this.svc.delete(t.id).subscribe({
                next: () => { this.toastr.success('Template deleted.'); this.load(); },
                error: (err) => this.toastr.error(apiError(err))
            });
        });
    }
}
