// Values match the API enums (SchoolWebApp.Core.Entities.Communications).
export enum MessageChannel {
    None = 0,
    Sms = 1,
    Email = 2,
    Both = 3
}

export enum MessageStatus {
    Queued = 0,
    Sending = 1,
    Sent = 2,
    Delivered = 3,
    Failed = 4,
    Cancelled = 5,
    Undelivered = 6
}

export enum RecipientType {
    Parent = 0,
    Staff = 1,
    StudentContact = 2,
    Other = 3
}

export enum RecipientGroup {
    AllParents = 0,
    ParentsOfClasses = 1,
    ParentsOfEducationLevels = 2,
    ParentsOfStudents = 3,
    AllStaff = 4,
    StaffByCategory = 5,
    SelectedStaff = 6,
    CustomContacts = 7
}

export const CHANNEL_LABELS: {[k: number]: string} = {
    [MessageChannel.None]: 'Off',
    [MessageChannel.Sms]: 'SMS',
    [MessageChannel.Email]: 'Email',
    [MessageChannel.Both]: 'SMS & Email'
};

export const STATUS_LABELS: {[k: number]: string} = {
    [MessageStatus.Queued]: 'Queued',
    [MessageStatus.Sending]: 'Sending',
    [MessageStatus.Sent]: 'Sent',
    [MessageStatus.Delivered]: 'Delivered',
    [MessageStatus.Failed]: 'Failed',
    [MessageStatus.Cancelled]: 'Cancelled',
    [MessageStatus.Undelivered]: 'Undelivered'
};

export const STATUS_BADGES: {[k: number]: string} = {
    [MessageStatus.Queued]: 'bg-secondary',
    [MessageStatus.Sending]: 'bg-info',
    [MessageStatus.Sent]: 'bg-primary',
    [MessageStatus.Delivered]: 'bg-success',
    [MessageStatus.Failed]: 'bg-danger',
    [MessageStatus.Cancelled]: 'bg-dark',
    [MessageStatus.Undelivered]: 'bg-warning text-dark'
};

export const RECIPIENT_TYPE_LABELS: {[k: number]: string} = {
    [RecipientType.Parent]: 'Parent',
    [RecipientType.Staff]: 'Staff',
    [RecipientType.StudentContact]: 'Student record',
    [RecipientType.Other]: 'Other'
};

export const MESSAGE_TYPE_LABELS: {[k: string]: string} = {
    ExamResults: 'Exam results',
    FeeInvoice: 'Fee invoice',
    FeeBalance: 'Fee balance reminder',
    FeePayment: 'Fee payment',
    Custom: 'Custom message'
};

export interface CommunicationSetting {
    smsEnabled: boolean;
    smsApiUrl?: string;
    smsApiKey?: string;
    hasSmsApiKey?: boolean;
    smsPartnerId?: string;
    smsSenderId?: string;
    smsUnitPrice: number;
    emailEnabled: boolean;
    smtpHost?: string;
    smtpPort: number;
    smtpUseSsl: boolean;
    smtpUsername?: string;
    smtpPassword?: string;
    hasSmtpPassword?: boolean;
    fromEmail?: string;
    fromName?: string;
    replyToEmail?: string;
    testMode: boolean;
    testPhoneNumber?: string;
    testEmail?: string;
}

export interface CommunicationStatus {
    smsEnabled: boolean;
    emailEnabled: boolean;
    testMode: boolean;
    parentContactSource: number;
}

export interface MessageTemplate {
    id?: number;
    code?: string;
    name: string;
    description?: string;
    isSystem?: boolean;
    channel: MessageChannel;
    isActive: boolean;
    smsBody?: string;
    emailSubject?: string;
    emailBody?: string;
    modified?: string;
    modifiedBy?: string;
}

export interface Placeholder {
    key: string;
    description: string;
}

export interface RecipientCriteria {
    group: RecipientGroup;
    schoolClassIds: number[];
    educationLevelIds: number[];
    studentIds: number[];
    staffIds: number[];
    staffCategoryIds: number[];
    customContacts?: string;
}

export interface ComposeMessage {
    title?: string;
    messageTemplateId?: number;
    channel: MessageChannel;
    recipients: RecipientCriteria;
    smsBody?: string;
    emailSubject?: string;
    emailBody?: string;
}

export interface RecipientPreview {
    name?: string;
    recipientType: RecipientType;
    studentNames?: string;
    phone?: string;
    email?: string;
}

export interface ComposePreview {
    recipients: number;
    smsCount: number;
    emailCount: number;
    missingContact: number;
    smsParts: number;
    sampleSms?: string;
    sampleEmailSubject?: string;
    sampleEmailBody?: string;
    testMode: boolean;
    recipientList: RecipientPreview[];
}

export interface QueueResult {
    batchId: number;
    queued: number;
    skipped: number;
    testMode: boolean;
}

export interface OutboundMessage {
    id: number;
    messageBatchId: number;
    batchTitle?: string;
    messageTypeCode: string;
    channel: MessageChannel;
    recipientType: RecipientType;
    recipientName?: string;
    destination: string;
    originalDestination?: string;
    subject?: string;
    body: string;
    status: MessageStatus;
    attempts: number;
    nextAttemptAt?: string;
    sentAt?: string;
    deliveredAt?: string;
    providerMessageId?: string;
    providerStatus?: string;
    errorMessage?: string;
    smsParts: number;
    isTest: boolean;
    created?: string;
    createdBy?: string;
}

export interface MessageQueuePage {
    data: OutboundMessage[];
    totalCount: number;
    statusCounts: {status: MessageStatus; count: number}[];
}

export interface MessageBatch {
    id: number;
    title: string;
    messageTypeCode: string;
    channel: MessageChannel;
    recipientSummary?: string;
    totalMessages: number;
    isTest: boolean;
    created?: string;
    createdBy?: string;
    queued: number;
    sent: number;
    delivered: number;
    failed: number;
    cancelled: number;
    smsParts: number;
}

export interface SmsReportRow {
    label: string;
    messages: number;
    parts: number;
}

export interface SmsReport {
    year: number;
    month: number;
    messages: number;
    parts: number;
    delivered: number;
    undelivered: number;
    awaitingReport: number;
    failed: number;
    testMessages: number;
    unitPrice: number;
    amount: number;
    byMessageType: SmsReportRow[];
    byDay: SmsReportRow[];
    details: OutboundMessage[];
}

export interface RecipientOptions {
    classes: {id: number; name: string; educationLevelId: number; learners: number}[];
    levels: {id: number; name: string}[];
    categories: {id: number; name: string}[];
    staff: {id: number; fullName: string; staffCategoryId: number; hasPhone: boolean; hasEmail: boolean}[];
    students: {id: number; fullName: string; admissionNo: string; schoolClassId: number}[];
}

// Mirrors MessageText.SmsParts on the API: 160 GSM-7 characters in one part,
// 153 per part when longer; 70 / 67 when the text needs Unicode. Curly quotes
// and dashes are counted as their plain equivalents, as the API sends them.
const GSM_BASIC = '@£$¥èéùìòÇ\nØø\rÅåΔ_ΦΓΛΩΠΨΣΘΞÆæßÉ !"#¤%&\'()*+,-./0123456789:;<=>?' +
    '¡ABCDEFGHIJKLMNOPQRSTUVWXYZÄÖÑÜ§¿abcdefghijklmnopqrstuvwxyzäöñüà';
const GSM_EXTENDED = '^{}\[~]|€\f';

export function smsInfo(text: string | null | undefined): {length: number; parts: number; unicode: boolean} {
    if (!text) return {length: 0, parts: 0, unicode: false};
    let t = text.replace(/\r\n/g, '\n')
        .replace(/[\u2018\u2019\u201B\u2032]/g, "'")
        .replace(/[\u201C\u201D\u201F\u2033]/g, '"')
        .replace(/[\u2013\u2014\u2212]/g, '-')
        .replace(/[\u00A0\u2007\u202F\t]/g, ' ')
        .replace(/\u2026/g, '.')
        .trim();
    let length = 0;
    for (let c of t) {
        if (GSM_BASIC.includes(c)) length++;
        else if (GSM_EXTENDED.includes(c)) length += 2;
        else {
            let u = t.length;
            return {length: u, parts: u <= 70 ? 1 : Math.ceil(u / 67), unicode: true};
        }
    }
    return {length, parts: length <= 160 ? 1 : Math.ceil(length / 153), unicode: false};
}
