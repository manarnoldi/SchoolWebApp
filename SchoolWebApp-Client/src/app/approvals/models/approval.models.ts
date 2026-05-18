export enum ApprovalRequestStatus {
    Draft = 0,
    Submitted = 1,
    Approved = 2,
    Rejected = 3,
    Returned = 4,
    Reversed = 5
}

export enum StepActionStatus {
    Pending = 0,
    Approved = 1,
    Rejected = 2,
    Skipped = 3,
    Returned = 4
}

export interface ApprovalWorkflowStep {
    id?: number;
    approvalWorkflowId?: number;
    rank: number;
    name: string;
    roleId: number;
    roleName?: string;
    isFinal: boolean;
    notifyNextApprover: boolean;
    notifyPreviousApprover: boolean;
    notifyApplicant: boolean;
}

export interface ApprovalWorkflow {
    id?: number;
    name: string;
    formKey: string;
    description?: string;
    isMakerChecker: boolean;
    isActive: boolean;
    steps: ApprovalWorkflowStep[];
}

export interface ApprovalStepAction {
    id?: number;
    approvalRequestId?: number;
    stepRank: number;
    stepName: string;
    assignedToUserId: number;
    assignedToName?: string;
    actionedByUserId?: number;
    actionedByName?: string;
    status: StepActionStatus;
    comment?: string;
    actionedAt?: string;
}

export interface ApprovalRequest {
    id?: number;
    approvalWorkflowId: number;
    approvalWorkflow?: ApprovalWorkflow;
    entityType: string;
    entityId: number;
    status: ApprovalRequestStatus;
    submittedById?: number;
    submittedByName?: string;
    submittedAt?: string;
    currentStepRank: number;
    actions: ApprovalStepAction[];
}

export interface UserInRole {
    id: number;
    userName?: string;
    email?: string;
    firstName?: string;
    lastName?: string;
    fullName?: string;
}
