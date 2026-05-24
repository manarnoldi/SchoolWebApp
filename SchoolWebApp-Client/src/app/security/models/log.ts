export interface Log {
    id: number;
    logged: string;
    level: string;
    message: string;
    logger: string;
    exception: string | null;
    url: string | null;
    callSite: string | null;
    machineName: string | null;
    userName: string | null;
    resolved: boolean;
    resolvedBy: string | null;
    resolvedAt: string | null;
    resolutionNote: string | null;
}

export interface LogsPage {
    total: number;
    items: Log[];
}
