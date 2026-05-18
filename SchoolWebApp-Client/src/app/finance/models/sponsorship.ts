export enum SponsorType {
    External = 0,
    School = 1,
    Government = 2,
    Parent = 3,
    Other = 4
}

export enum SponsorshipCoverageType {
    FixedAmount = 0,
    Percentage = 1,
    FullCoverage = 2
}

export enum SponsorshipStatus {
    Active = 0,
    Ended = 1,
    Cancelled = 2
}

export interface Sponsor {
    id?: number;
    name: string;
    description?: string;
    sponsorType: SponsorType;
    contactName?: string;
    email?: string;
    phone?: string;
    address?: string;
    receivableAccountId?: number | null;
    receivableAccountName?: string;
    isActive: boolean;
}

export interface Sponsorship {
    id?: number;
    sponsorId: number;
    sponsorName?: string;
    studentId?: number | null;
    studentName?: string;
    studentUPI?: string;
    schoolClassId?: number | null;
    schoolClassName?: string;
    academicYearId: number;
    academicYearName?: string;
    sessionId?: number | null;
    sessionName?: string;
    coverageType: SponsorshipCoverageType;
    fixedAmount: number;
    percentage: number;
    startDate: string;
    endDate?: string | null;
    notes?: string;
    status: SponsorshipStatus;
    feeCategoryIds: number[];
}

export interface SponsorPayment {
    id?: number;
    sponsorId: number;
    sponsorName?: string;
    referenceNumber?: string;
    paymentDate: string;
    amount: number;
    paymentMethod: number;
    transactionReference?: string;
    bankAccountId?: number | null;
    bankAccountName?: string;
    description?: string;
}
