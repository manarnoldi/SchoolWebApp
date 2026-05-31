export interface AuditLog {
    id: number;
    timestamp: string;
    userId: string | null;
    userName: string | null;
    action: string;
    entityType: string | null;
    entityId: string | null;
    oldValues: string | null;
    newValues: string | null;
    ipAddress: string | null;
    userAgent: string | null;
    requestPath: string | null;
    notes: string | null;
}

export interface AuditLogPage {
    total: number;
    items: AuditLog[];
}
