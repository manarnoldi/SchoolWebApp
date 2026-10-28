import {AppModule} from '@/core/services/licence.service';

export type DashboardCardKey = 'examPerformance' | 'finance' | 'teaching' | 'payroll' | 'elections' | 'communications';

/**
 * The dashboard's cards. Each is given to roles in Security > Menu Permissions
 * (Dashboard > Cards) by its path - not a page, only a permission - and shows
 * when the role holds it and the licence has its module. The API checks the
 * same paths (DashboardCardsController).
 */
export const DASHBOARD_CARDS: {key: DashboardCardKey; name: string; path: string; module: AppModule}[] = [
    {key: 'examPerformance', name: 'Class exam performance', path: '/dashboard/cards/exam-performance', module: 'Academics'},
    {key: 'finance', name: 'Fees (collections, arrears, spending)', path: '/dashboard/cards/fees', module: 'Finance'},
    {key: 'teaching', name: 'Teaching documents', path: '/dashboard/cards/teaching', module: 'Academics'},
    {key: 'payroll', name: 'Payroll', path: '/dashboard/cards/payroll', module: 'Payroll'},
    {key: 'elections', name: 'Elections', path: '/dashboard/cards/elections', module: 'Elections'},
    {key: 'communications', name: 'Messages', path: '/dashboard/cards/messages', module: 'Communications'}
];

export const cardPath = (key: DashboardCardKey): string => DASHBOARD_CARDS.find((c) => c.key === key)!.path;
