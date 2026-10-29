#!/usr/bin/env node
// Fills an anonymised demo copy with a year of invented school finance: fee
// structures, invoices for every term, fee payments (M-Pesa, bank and cash),
// sponsorships and sponsor remittances, a 2026 budget and the monthly expenses
// against it. Everything goes through the API, so the journals, allocations and
// approval trails are exactly what the app itself would record.
//
// Run it after anonymize.js, with the API running on the same database:
//
//   MYSQL_PWD=... node tools/demo-data/finance.js --db shulenova --api http://localhost:5198
//
// Options: --db <database> (required; refuses names of live schools)
//          --api <API origin> (default http://localhost:5198)
//          --mysql <path to mysql.exe> (default MySQL 8.4 on Windows)
//          --user <mysql user> (default root)
//          --password <the demo logins' password> (default Demo@2026)
//          --through <yyyy-mm-dd> (last date to record payments and expenses on;
//                    default two weeks from today, so the dashboard shows money
//                    coming in on the day of the demo)
//          --seed <number> (default 2026)
// Any finance data already in the copy (test invoices, payments, expenses,
// budgets, non-payroll journals) is cleared first, so it can be run again.
//
// It also gives the school's accountant a login (the Accounts role, with the
// finance menus), who records the fees and expenses; expenses are approved by
// two administrators, and budgets by the accountant and the head teacher.

const {execFileSync} = require('child_process');

const opt = {mysql: 'C:/Program Files/MySQL/MySQL Server 8.4/bin/mysql.exe', user: 'root', api: 'http://localhost:5198',
    password: 'Demo@2026', seed: '2026'};
const a = process.argv.slice(2);
for (let i = 0; i < a.length; i += 2) opt[a[i].replace(/^--/, '')] = a[i + 1];
if (!opt.db) fail('--db is required');
if (/swikunda/i.test(opt.db)) fail(`refusing to touch "${opt.db}" - run this on the demo copy`);

function fail(m) {
    console.error('Error: ' + m);
    process.exit(1);
}

// ---------- MySQL ----------
function query(sql) {
    const out = execFileSync(opt.mysql, ['-u', opt.user, '--default-character-set=utf8mb4', '-B', '--raw', opt.db, '-e', sql],
        {encoding: 'utf8', maxBuffer: 256 * 1024 * 1024, stdio: ['ignore', 'pipe', 'pipe']});
    const lines = out.split(/\r?\n/).filter((l) => l.length);
    if (!lines.length) return [];
    const head = lines[0].split('\t');
    return lines.slice(1).map((l) => {
        const v = l.split('\t');
        return Object.fromEntries(head.map((h, i) => [h, v[i] === 'NULL' ? null : v[i]]));
    });
}

function exec(sql) {
    execFileSync(opt.mysql, ['-u', opt.user, '--default-character-set=utf8mb4', opt.db],
        {input: 'SET NAMES utf8mb4;\n' + sql, stdio: ['pipe', 'inherit', 'inherit']});
}

const q = (v) => (v === null || v === undefined ? 'NULL' : `'${String(v).replace(/\\/g, '\\\\').replace(/'/g, "''")}'`);

// ---------- Random, repeatable ----------
let seed = +opt.seed >>> 0;
function rnd() {
    seed = (seed + 0x6d2b79f5) >>> 0;
    let t = seed;
    t = Math.imul(t ^ (t >>> 15), t | 1);
    t ^= t + Math.imul(t ^ (t >>> 7), t | 61);
    return ((t ^ (t >>> 14)) >>> 0) / 4294967296;
}
const pick = (list) => list[Math.floor(rnd() * list.length)];
const between = (lo, hi) => lo + Math.floor(rnd() * (hi - lo + 1));
const round = (v, to = 100) => Math.round(v / to) * to;
const shuffle = (list) => list.map((v) => [rnd(), v]).sort((x, y) => x[0] - y[0]).map(([, v]) => v);
const code = (n, chars) => Array.from({length: n}, () => chars[Math.floor(rnd() * chars.length)]).join('');
const mpesaRef = () => 'T' + code(9, 'ABCDEFGHJKLMNPQRSTUVWXYZ0123456789');
const bankRef = () => 'FT26' + code(8, '0123456789');

// ---------- Dates (local, date only) ----------
const day = (s) => new Date(s.slice(0, 10) + 'T00:00:00');
const iso = (d) => `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
const addDays = (d, n) => new Date(d.getFullYear(), d.getMonth(), d.getDate() + n);
const min = (x, y) => (x < y ? x : y);
const today = day(iso(new Date()));
const through = opt.through ? day(opt.through) : addDays(today, 14);

// ---------- API ----------
async function api(token, method, url, body) {
    const r = await fetch(opt.api + '/api' + url, {method,
        headers: {'Content-Type': 'application/json', ...(token ? {Authorization: `Bearer ${token}`} : {})},
        body: body === undefined ? undefined : JSON.stringify(body)});
    const text = await r.text();
    if (!r.ok) throw new Error(`${method} ${url} -> ${r.status} ${text.slice(0, 400)}`);
    return text ? JSON.parse(text) : null;
}
const login = async (userName) => (await api(null, 'POST', '/auth/login', {userName, password: opt.password})).token;

// Runs fn over items, a few at a time.
async function pool(items, size, fn) {
    let next = 0;
    await Promise.all(Array.from({length: size}, async () => {
        while (next < items.length) await fn(items[next++]);
    }));
}

const FINANCE_MENUS = ['/finance/budget-register', '/finance/budgets', '/finance/budget-amendments', '/finance/fee-structures',
    '/finance/invoices', '/finance/payments', '/finance/sponsors', '/finance/sponsorships', '/finance/sponsor-payments',
    '/finance/expenses', '/finance/journal-entries', '/finance/reports', '/finance/reports/fees', '/finance/reports/expenses',
    '/finance/reports/statements', '/finance/accounts', '/finance/fee-categories', '/finance/expense-categories'];
// Who sees what: the accountant runs finance; administrators approve expenses;
// the head teacher approves budgets and sees the fees card.
const MENUS = {
    Accounts: ['/', '/dashboard/cards/fees', '/settings/dropdowns', ...FINANCE_MENUS],
    Administrator: ['/dashboard/cards/fees', '/finance/expenses'],
    HeadTeacher: ['/dashboard/cards/fees', '/finance/budgets', '/finance/reports', '/finance/reports/fees'],
};

async function main() {
    const ver = query('SELECT COUNT(*) n FROM person')[0];
    if (!ver) fail(`database ${opt.db} not found`);

    // ---------- 1. Clear earlier finance test data ----------
    console.log(`Clearing finance data in ${opt.db}...`);
    exec(`SET FOREIGN_KEY_CHECKS = 0;
DELETE FROM paymentallocations; DELETE FROM payments; DELETE FROM studentinvoiceitems; DELETE FROM studentinvoices;
DELETE FROM sponsorshipfeecategories; DELETE FROM sponsorships; DELETE FROM sponsorpayments;
DELETE FROM feestructureitems; DELETE FROM feestructures; DELETE FROM expenselines; DELETE FROM expenses;
DELETE FROM budgetamendmentlines; DELETE FROM budgetamendments; DELETE FROM budgetlines; DELETE FROM budgets; DELETE FROM budgetmasters;
DELETE a FROM approvalstepactions a JOIN approvalrequests r ON r.Id = a.ApprovalRequestId
    WHERE r.EntityType IN ('Expense', 'Budget', 'BudgetAmendment', 'JournalEntry', 'CreditDebitNote');
DELETE FROM approvalrequests WHERE EntityType IN ('Expense', 'Budget', 'BudgetAmendment', 'JournalEntry', 'CreditDebitNote');
DELETE l FROM journallines l JOIN journalentries j ON j.Id = l.JournalEntryId WHERE j.Description NOT LIKE 'Payroll%';
DELETE FROM journalentries WHERE Description NOT LIKE 'Payroll%';
SET FOREIGN_KEY_CHECKS = 1;
INSERT INTO departments (Name, Code, Description, Created, CreatedBy, Modified, ModifiedBy)
    SELECT 'Administration', 'DEPT004', 'School office, utilities, catering and transport', '2025-12-01', 'admin', '2025-12-01', 'admin'
    FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM departments WHERE Name = 'Administration');
${Object.entries(MENUS).flatMap(([role, paths]) => paths.map((p) => `INSERT INTO menupermissions (RoleId, MenuPath, MenuName, Created, CreatedBy, Modified, ModifiedBy)
    SELECT ${q(role)}, ${q(p)}, (SELECT MAX(MenuName) FROM menupermissions m WHERE m.MenuPath = ${q(p)}), NOW(), 'admin', NOW(), 'admin'
    FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM menupermissions WHERE RoleId = ${q(role)} AND MenuPath = ${q(p)});`)).join('\n')}
`);

    // ---------- 2. Reference data ----------
    const year = query(`SELECT Id, Name FROM academicyears WHERE StartDate <= '${iso(today)}' AND EndDate >= '${iso(today)}'`)[0];
    if (!year) fail('no academic year covers today');
    const terms = query(`SELECT Id, SessionName, StartDate, EndDate FROM sessions WHERE AcademicYearId = ${year.Id}
        AND StartDate <= '${iso(through)}' ORDER BY StartDate`).map((s, i) => ({id: +s.Id, n: i + 1, name: `${s.SessionName} ${year.Name}`,
        start: day(s.StartDate), end: day(s.EndDate)}));
    const cat = Object.fromEntries(query('SELECT Id, Name FROM feecategories').map((r) => [r.Name, +r.Id]));
    for (const c of ['Tuition', 'Meals', 'Activity', 'Exam', 'Transport', 'Uniform']) if (!cat[c]) fail(`fee category ${c} missing`);
    const acct = Object.fromEntries(query('SELECT Id, Code FROM accounts').map((r) => [r.Code, +r.Id]));
    const expCat = Object.fromEntries(query('SELECT Id, Name, ExpenseAccountId FROM expensecategories')
        .map((r) => [r.Name, {id: +r.Id, account: +r.ExpenseAccountId}]));
    const dept = Object.fromEntries(query('SELECT Id, Name FROM departments').map((r) => [r.Name, +r.Id]));
    const levels = query(`SELECT DISTINCT l.Id, l.Name FROM learninglevels l JOIN schoolclasses c ON c.LearningLevelId = l.Id
        WHERE c.AcademicYearId = ${year.Id}`).map((r) => ({id: +r.Id, name: r.Name}));
    const classes = query(`SELECT c.Id, c.LearningLevelId FROM schoolclasses c WHERE c.AcademicYearId = ${year.Id}
        AND EXISTS (SELECT 1 FROM studentclasses x JOIN person p ON p.Id = x.StudentId WHERE x.SchoolClassId = c.Id AND p.Status = 0)`);
    const students = query(`SELECT DISTINCT x.StudentId, c.LearningLevelId FROM studentclasses x JOIN schoolclasses c ON c.Id = x.SchoolClassId
        JOIN person p ON p.Id = x.StudentId WHERE c.AcademicYearId = ${year.Id} AND p.Status = 0`)
        .map((r) => ({id: +r.StudentId, level: +r.LearningLevelId}));
    const sponsors = query('SELECT Id, Name FROM sponsors WHERE IsActive = 1 ORDER BY Id').map((r) => ({id: +r.Id, name: r.Name}));

    // ---------- 3. Logins ----------
    const users = () => Object.fromEntries(query('SELECT Id, UserName FROM aspnetusers').map((r) => [r.UserName, +r.Id]));
    const boss = query(`SELECT u.UserName FROM aspnetusers u JOIN aspnetuserroles ur ON ur.UserId = u.Id JOIN aspnetroles r ON r.Id = ur.RoleId
        WHERE r.Name = 'Administrator' AND u.UserName <> 'admin' ORDER BY u.Id`).map((r) => r.UserName);
    const head = query(`SELECT u.UserName FROM aspnetusers u JOIN aspnetuserroles ur ON ur.UserId = u.Id JOIN aspnetroles r ON r.Id = ur.RoleId
        WHERE r.Name = 'HeadTeacher' ORDER BY u.Id LIMIT 1`)[0]?.UserName;
    if (boss.length < 2 || !head) fail('need two Administrator logins and a HeadTeacher login (other than admin)');
    const adminToken = await login(boss[0]);

    let bursar = query(`SELECT u.UserName FROM aspnetusers u JOIN aspnetuserroles ur ON ur.UserId = u.Id JOIN aspnetroles r ON r.Id = ur.RoleId
        WHERE r.Name = 'Accounts' ORDER BY u.Id LIMIT 1`)[0]?.UserName;
    if (!bursar) {
        const acc = query(`SELECT p.Id, p.FullName FROM person p JOIN designations d ON d.Id = p.DesignationId
            WHERE p.Discriminator = 'StaffDetails' AND p.Status = 0 AND d.Name LIKE '%ccount%'
            AND NOT EXISTS (SELECT 1 FROM aspnetusers u WHERE u.PersonId = p.Id) ORDER BY p.Id LIMIT 1`)[0];
        if (!acc) fail('no staff member with an Accountant designation to give a login to');
        const parts = acc.FullName.replace(/^(MR|MRS|MS|MD|MISS|DR|MADAM)\.?\s+/i, '').split(/\s+/);
        const name = (s) => s.charAt(0).toUpperCase() + s.slice(1).toLowerCase();
        const taken = users();
        bursar = parts[0].toLowerCase();
        if (taken[bursar]) bursar = `${parts[0]}.${parts[parts.length - 1]}`.toLowerCase();
        await api(adminToken, 'POST', '/users', {userName: bursar, email: `${bursar}@example.com`, password: opt.password,
            firstName: name(parts[0]), lastName: name(parts[parts.length - 1]), personId: +acc.Id});
        await api(adminToken, 'POST', '/users/userRole', {email: `${bursar}@example.com`, role: 'Accounts'});
        exec(`UPDATE aspnetusers SET MustChangePassword = 0 WHERE UserName = ${q(bursar)};`);
    }
    const uid = users();
    const tok = {[bursar]: await login(bursar), [boss[0]]: adminToken, [boss[1]]: await login(boss[1]), [head]: await login(head)};
    const B = tok[bursar];
    console.log(`Accountant: ${bursar}; expenses approved by ${boss[1]} then ${boss[0]}; budgets by ${bursar} then ${head}.`);

    // ---------- 4. Sponsorships (before invoicing, so invoices carry them) ----------
    const sponsored = new Map(); // studentId -> sponsor
    const upper = shuffle(students.filter((s) => s.level >= 6 && s.level <= 11));
    const sponsorPlans = [
        {take: 12, from: upper, coverageType: 2, percentage: 0, feeCategoryIds: [cat.Tuition], notes: 'Full tuition scholarship'},
        {take: 10, from: shuffle(students), coverageType: 1, percentage: 50, feeCategoryIds: [], notes: '50% bursary on all fees'},
    ];
    for (const [i, plan] of sponsorPlans.entries()) {
        const sponsor = sponsors[i];
        if (!sponsor) break;
        const chosen = plan.from.filter((s) => !sponsored.has(s.id)).slice(0, plan.take);
        for (const s of chosen) {
            await api(B, 'POST', '/sponsorships', {sponsorId: sponsor.id, studentId: s.id, academicYearId: +year.Id, coverageType: plan.coverageType,
                fixedAmount: 0, percentage: plan.percentage, startDate: iso(terms[0].start), endDate: `${year.Name}-12-31`,
                notes: plan.notes, status: 0, feeCategoryIds: plan.feeCategoryIds});
            sponsored.set(s.id, sponsor);
        }
    }
    console.log(`Sponsorships: ${sponsored.size} students.`);

    // ---------- 5. Fee structures ----------
    const band = (level) => (level <= 2 || level === 12 ? {Tuition: 5500, Meals: 3000, Activity: 500}
        : level <= 5 ? {Tuition: 7500, Meals: 3500, Activity: 700, Exam: 400}
            : level <= 8 ? {Tuition: 8500, Meals: 3500, Activity: 800, Exam: 500}
                : level <= 11 ? {Tuition: 10500, Meals: 4000, Activity: 1000, Exam: 700}
                    : {Tuition: 6500, Meals: 3500, Activity: 500});
    const TRANSPORT = 4500, UNIFORM = 3800;
    const structure = new Map(); // `${termId}:${levelId}` -> id
    for (const t of terms) for (const l of levels) {
        const items = Object.entries(band(l.id)).map(([c, amount]) => ({feeCategoryId: cat[c], amount, isMandatory: true}));
        items.push({feeCategoryId: cat.Transport, amount: TRANSPORT, isMandatory: false});
        if (t.n === 1) items.push({feeCategoryId: cat.Uniform, amount: UNIFORM, isMandatory: false});
        const fs = await api(B, 'POST', '/feeStructures', {name: `${l.name} - ${t.name}`, academicYearId: +year.Id, sessionId: t.id,
            learningLevelId: l.id, description: `${t.name} fees for ${l.name}`, isActive: true, items});
        structure.set(`${t.id}:${l.id}`, fs.id);
    }
    console.log(`Fee structures: ${structure.size}.`);

    // ---------- 6. Invoices ----------
    const rides = new Set(shuffle(students).slice(0, Math.round(students.length * 0.28)).map((s) => s.id));
    const uniforms = new Set(shuffle(students).slice(0, Math.round(students.length * 0.18)).map((s) => s.id));
    for (const t of terms) {
        const due = iso(addDays(t.start, 25));
        for (const c of classes)
            await api(B, 'POST', '/studentInvoices/bulk', {feeStructureId: structure.get(`${t.id}:${c.LearningLevelId}`), schoolClassId: +c.Id,
                invoiceDate: iso(t.start), dueDate: due, description: `${t.name} fees`});
        // Transport and uniforms are optional, so they go on a second invoice.
        await pool(students.filter((s) => rides.has(s.id) || (t.n === 1 && uniforms.has(s.id))), 6, async (s) => {
            const items = [];
            if (rides.has(s.id)) items.push({feeCategoryId: cat.Transport, amount: TRANSPORT, discount: 0, paidAmount: 0, description: 'School bus'});
            if (t.n === 1 && uniforms.has(s.id)) items.push({feeCategoryId: cat.Uniform, amount: UNIFORM, discount: 0, paidAmount: 0, description: 'Uniform set'});
            await api(B, 'POST', '/studentInvoices', {invoiceNumber: `INV-${iso(t.start).replace(/-/g, '')}-${s.id}-X`, studentId: s.id,
                academicYearId: +year.Id, sessionId: t.id, invoiceDate: iso(t.start), dueDate: due, discountAmount: 0,
                description: `${t.name} ${items.length > 1 ? 'transport and uniform' : items[0].description.toLowerCase()}`, status: 0, items});
        });
    }
    const invoices = query('SELECT Id, StudentId, SessionId, TotalAmount FROM studentinvoices')
        .map((r) => ({id: +r.Id, student: +r.StudentId, term: terms.find((t) => t.id === +r.SessionId), total: +r.TotalAmount, left: +r.TotalAmount}));
    console.log(`Invoices: ${invoices.length}.`);

    // ---------- 7. Fee payments ----------
    // Most families pay in full, some are slow, a few fall behind; the latest
    // term is still being paid.
    const kind = new Map(students.map((s) => { const r = rnd(); return [s.id, r < 0.62 ? 'good' : r < 0.88 ? 'slow' : 'poor']; }));
    const share = (k, n, isLast) => {
        if (n === 1) return k === 'poor' && rnd() < 0.5 ? 0.6 + rnd() * 0.3 : 1;
        if (k === 'good') return isLast ? (rnd() < 0.85 ? 1 : 0.6 + rnd() * 0.3) : 1;
        if (k === 'slow') return isLast ? 0.3 + rnd() * 0.5 : n === 2 ? (rnd() < 0.7 ? 1 : 0.5 + rnd() * 0.4) : 1;
        return isLast ? (rnd() < 0.5 ? 0 : 0.2 + rnd() * 0.3) : n === 2 ? (rnd() < 0.4 ? 0 : 0.3 + rnd() * 0.4) : 0.5 + rnd() * 0.4;
    };
    const plan = []; // {invoice, date, amount}
    const last = terms[terms.length - 1];
    for (const inv of invoices) {
        if (inv.total <= 0) continue;
        let toPay = round(inv.total * share(kind.get(inv.student), inv.term.n, inv.term === last));
        if (toPay > inv.total || inv.total - toPay < 500) toPay = inv.total;
        if (toPay < 500) continue;
        const parts = rnd() < 0.55 ? 1 : rnd() < 0.78 ? 2 : 3;
        let date = addDays(inv.term.start, Math.floor(rnd() * rnd() * 24));
        const stop = min(addDays(inv.term.end, 20), through);
        for (let p = 0; p < parts && toPay > 0; p++) {
            const amount = p === parts - 1 ? toPay : Math.min(toPay, Math.max(500, round(toPay * (0.35 + rnd() * 0.35))));
            if (date > stop) break;
            plan.push({inv, date, amount});
            inv.left -= amount;
            toPay -= amount;
            date = addDays(date, between(9, 35));
        }
    }
    // A steady trickle of arrears payments every day up to --through, so the
    // dashboard's "collected today / this week" is never empty on demo day.
    for (let d = addDays(today, -7); d <= through; d = addDays(d, 1)) {
        const owing = shuffle(invoices.filter((i) => i.term === last && i.left >= 1000));
        const n = d.getDay() === 0 ? 1 : d.getDay() === 6 ? 3 : between(4, 8);
        for (const inv of owing.slice(0, n)) {
            let amount = round(inv.left * (0.4 + rnd() * 0.6), 500);
            if (amount < 500 || inv.left - amount < 500) amount = inv.left;
            plan.push({inv, date: d, amount});
            inv.left -= amount;
        }
    }
    plan.sort((x, y) => x.date - y.date);
    const perDay = new Map();
    for (const p of plan) {
        const k = iso(p.date).replace(/-/g, '');
        perDay.set(k, (perDay.get(k) || 0) + 1);
        p.receipt = `RCP-${k}-${String(perDay.get(k)).padStart(3, '0')}`;
        const r = rnd();
        p.method = r < 0.6 ? 1 : r < 0.9 ? 2 : 0; // M-Pesa, bank, cash
    }
    // Payments on one invoice go in order; different invoices in parallel.
    const byInvoice = new Map();
    for (const p of plan) byInvoice.set(p.inv.id, [...(byInvoice.get(p.inv.id) || []), p]);
    let done = 0;
    await pool([...byInvoice.values()], 6, async (chain) => {
        for (const p of chain) {
            await api(B, 'POST', '/payments', {receiptNumber: p.receipt, paymentType: 0, studentId: p.inv.student, studentInvoiceId: p.inv.id,
                paymentDate: iso(p.date), amount: p.amount, paymentMethod: p.method,
                transactionReference: p.method === 1 ? mpesaRef() : p.method === 2 ? bankRef() : null,
                bankAccountId: p.method === 1 ? acct['1110'] : p.method === 2 ? acct['1100'] : acct['1000'],
                description: `${p.inv.term.name} fees`});
            if (++done % 250 === 0) console.log(`  payments ${done}/${plan.length}`);
        }
    });
    console.log(`Payments: ${plan.length}.`);

    // ---------- 8. Sponsor remittances ----------
    const owed = query(`SELECT s.SponsorId, i.SessionId, SUM(ii.Discount) Amount FROM studentinvoiceitems ii
        JOIN studentinvoices i ON i.Id = ii.StudentInvoiceId JOIN sponsorships s ON s.Id = ii.SponsorshipId
        GROUP BY s.SponsorId, i.SessionId`);
    let sp = 0;
    for (const o of owed) {
        const t = terms.find((x) => x.id === +o.SessionId);
        // The first sponsor pays every term (the latest in part); the second is behind on the latest term.
        const first = +o.SponsorId === sponsors[0]?.id;
        const fraction = t !== last ? 1 : first ? 0.5 : 0;
        const date = addDays(t.start, between(30, 45));
        if (!fraction || date > through) continue;
        await api(B, 'POST', '/sponsorPayments', {sponsorId: +o.SponsorId, referenceNumber: `SP-${year.Name}-${String(++sp).padStart(3, '0')}`,
            paymentDate: iso(date), amount: round(+o.Amount * fraction), paymentMethod: 2, transactionReference: bankRef(),
            bankAccountId: acct['1100'], description: `${t.name} sponsorship${fraction < 1 ? ' (first half)' : ''}`});
    }
    console.log(`Sponsor payments: ${sp}.`);

    // ---------- 9. Budget ----------
    // Submits for approval and has the first `acted` approvers approve it.
    const approve = async (entityType, entityId, submitter, steps, comment, acted = steps.length) => {
        const req = await api(tok[submitter], 'POST', '/approvalRequests/submit', {entityType, entityId, formKey: entityType,
            stepAssignments: steps.map((u, i) => ({stepRank: i + 1, assignedToUserId: uid[u]}))});
        for (const u of steps.slice(0, acted)) await api(tok[u], 'POST', `/approvalRequests/${req.id}/action`, {action: 'approve', comment});
    };
    const master = await api(B, 'POST', '/budgetMasters', {name: `${year.Name} Annual Budget`, description: `School budget for ${year.Name}`,
        academicYearId: +year.Id, startDate: `${year.Name}-01-01`, endDate: `${year.Name}-12-31`, status: 1});
    const BUDGETS = {
        Administration: {'4000': 12000000, '4010': 1700000, '4020': 4800000, '4040': 1100000, '4050': 300000, '5100': 230000, '5110': 66000, '5120': 96000, '5210': 150000, '5220': 180000, '5300': 520000, '5400': 3100000},
        STEM: {'5200': 260000, '5220': 60000, '5900': 40000},
        'Social Sciences': {'5200': 170000, '5300': 90000, '5900': 45000},
        CAS: {'5300': 120000, '5900': 160000, '5200': 50000},
    };
    for (const [d, lines] of Object.entries(BUDGETS)) {
        if (!dept[d]) continue;
        const b = await api(B, 'POST', '/budgets', {name: `${d} ${year.Name}`, description: `${d} budget for ${year.Name}`,
            startDate: `${year.Name}-01-01`, endDate: `${year.Name}-12-31`, academicYearId: +year.Id, departmentId: dept[d],
            budgetMasterId: master.id, isActive: true,
            lines: Object.entries(lines).map(([c, amount]) => ({accountId: acct[c], budgetedAmount: amount}))});
        await approve('Budget', b.id, boss[0], [bursar, head], 'Approved for the year.');
    }
    const budgetLine = Object.fromEntries(query(`SELECT l.Id, l.AccountId, d.Name FROM budgetlines l JOIN budgets b ON b.Id = l.BudgetId
        JOIN departments d ON d.Id = b.DepartmentId`).map((r) => [`${r.Name}:${r.AccountId}`, +r.Id]));
    console.log('Budget: 1 register, ' + Object.keys(BUDGETS).length + ' department budgets.');

    // ---------- 10. Expenses ----------
    const expenses = [];
    const add = (date, description, method, lines, waiting) => { if (date <= through) expenses.push({date, description, method, lines, waiting}); };
    const line = (category, amount, vendor, department, description) => ({category, amount, vendor, department, description});
    const termMonth = (m) => terms.some((t) => t.start.getMonth() <= m && t.end.getMonth() >= m);
    for (let m = 0; m <= through.getMonth(); m++) {
        const on = (d) => new Date(+year.Name, m, d);
        const month = on(1).toLocaleString('en-GB', {month: 'long'});
        add(on(5), `Internet - ${month}`, 1, [line('Internet & Phones', 7500, 'Safaricom Fibre', 'Administration', `${month} fibre bundle`)]);
        add(on(8), `Electricity - ${month}`, 1, [line('Electricity', round(termMonth(m) ? between(16000, 21000) : between(9000, 12000)),
            'Kenya Power', 'Administration', `${month} bill`)]);
        add(on(11), `Water - ${month}`, 1, [line('Water', round(between(3800, 6500), 50), 'Nairobi Water & Sewerage Co.', 'Administration', `${month} bill`)]);
        add(on(14), `Cleaning supplies - ${month}`, 0, [line('Cleaning Supplies', round(between(9000, 15000)), 'Usafi Supplies Ltd', 'Administration',
            'Detergents, disinfectant and tissue')]);
        if (termMonth(m)) {
            add(on(3), `Food stores - ${month}`, 2, [line('Food Supplies', round(between(165000, 215000), 500), 'Mwangaza Wholesalers', 'Administration',
                'Rice, maize flour, beans and cooking oil')]);
            add(on(17), `Fresh produce - ${month}`, 2, [line('Food Supplies', round(between(85000, 120000), 500), 'Shamba Fresh Produce', 'Administration',
                'Vegetables, fruit and milk')]);
            add(on(20), `School bus fuel - ${month}`, 2, [line('Fuel & Transport', round(between(38000, 52000)), 'TotalEnergies Kasarani', 'Administration',
                'Diesel for the two school buses')]);
        }
    }
    for (const t of terms) {
        const d = addDays(t.start, 3);
        add(d, `${t.name} teaching supplies`, 2, [
            line('Stationery', round(between(60000, 80000), 500), 'Text Book Centre', 'STEM', 'Exercise books, maths sets and lab consumables'),
            line('Stationery', round(between(40000, 55000), 500), 'Text Book Centre', 'Social Sciences', 'Exercise books, atlases and readers')]);
        add(addDays(d, 2), `${t.name} art and PE supplies`, 0, [line('Stationery', round(between(12000, 16000)), 'Crafts & Colours Ltd', 'CAS',
            'Paints, brushes and balls')]);
    }
    const y = +year.Name;
    add(new Date(y, 1, 19), 'Inter-house sports day', 0, [line('Miscellaneous', 35000, 'Various', 'CAS', 'Trophies, refreshments and first aid')]);
    add(new Date(y, 2, 13), 'Classroom roof repairs', 2, [line('Repairs', 68000, 'Fundi Works Ltd', 'Administration', 'Iron sheets and labour, Grade 3 block')]);
    add(new Date(y, 5, 9), 'Music festival - county level', 2, [
        line('Fuel & Transport', 42000, 'Safari Coaches', 'CAS', 'Bus hire for the choir and dancers'),
        line('Miscellaneous', 28000, 'Tailor Point', 'CAS', 'Costumes')]);
    add(new Date(y, 6, 15), 'Grade 8 museum trip', 2, [
        line('Fuel & Transport', 45000, 'Safari Coaches', 'Social Sciences', 'Bus hire'),
        line('Miscellaneous', 12000, 'National Museums of Kenya', 'Social Sciences', 'Entry fees')]);
    add(new Date(y, 6, 22), 'Plumbing repairs', 0, [line('Repairs', 24500, 'Fundi Works Ltd', 'Administration', 'Washroom taps and cisterns')]);
    add(new Date(y, 8, 16), 'Science lab equipment repair', 2, [line('Repairs', 18000, 'LabTech Services', 'STEM', 'Microscopes serviced')]);
    add(new Date(y, 8, 23), 'Science and engineering fair', 0, [line('Miscellaneous', 15000, 'Various', 'STEM', 'Materials and transport')]);
    add(new Date(y, 9, 9), 'Drama festival', 0, [line('Miscellaneous', 22000, 'Various', 'CAS', 'Props and transport')]);
    add(new Date(y, 9, 27), 'Desk repairs', 0, [line('Repairs', 16500, 'Fundi Works Ltd', 'Administration', '30 desks repaired')]);
    add(addDays(today, 1), 'Grade 9 KJSEA revision books', 2, [line('Stationery', 38500, 'Text Book Centre', 'STEM', 'Revision papers')], 'second approver');
    add(addDays(today, 2), 'Generator service', 0, [line('Repairs', 9500, 'Power Gen Services', 'Administration', 'Annual service')], 'draft');
    expenses.sort((x, y) => x.date - y.date);
    // Two recent ones are still waiting: one with its second approver, one not yet submitted.
    for (const [i, e] of expenses.entries()) {
        const ref = `EXP-${year.Name}-${String(i + 1).padStart(4, '0')}`;
        const created = await api(B, 'POST', '/expenses', {referenceNumber: ref, expenseDate: iso(e.date), paymentMethod: e.method,
            transactionReference: e.method === 1 ? mpesaRef() : e.method === 2 ? bankRef() : null,
            paidFromAccountId: e.method === 1 ? acct['1110'] : e.method === 2 ? acct['1100'] : acct['1000'], status: 0, description: e.description,
            lines: e.lines.map((l) => ({expenseCategoryId: expCat[l.category].id, amount: l.amount, vendor: l.vendor, description: l.description,
                budgetLineId: budgetLine[`${l.department}:${expCat[l.category].account}`] ?? null}))});
        e.id = created.id;
        if (e.waiting === 'draft') continue;
        await approve('Expense', created.id, bursar, [boss[1], boss[0]], 'OK', e.waiting ? 1 : 2);
    }
    console.log(`Expenses: ${expenses.length}.`);

    // ---------- 11. Make it look recorded on the day ----------
    // Bulk invoices are numbered by the time they were made; number them by
    // their date instead, and stamp every record with its own date, not today's.
    exec(`
UPDATE journallines l JOIN journalentries j ON j.Id = l.JournalEntryId JOIN studentinvoices i ON j.ReferenceNumber = CONCAT('INV-JNL-', i.InvoiceNumber)
    SET l.Description = REPLACE(l.Description, i.InvoiceNumber, CONCAT('INV-', DATE_FORMAT(i.InvoiceDate, '%Y%m%d'), '-', i.StudentId))
    WHERE i.InvoiceNumber NOT LIKE '%-X';
UPDATE journalentries j JOIN studentinvoices i ON j.ReferenceNumber = CONCAT('INV-JNL-', i.InvoiceNumber)
    SET j.ReferenceNumber = CONCAT('INV-JNL-INV-', DATE_FORMAT(i.InvoiceDate, '%Y%m%d'), '-', i.StudentId),
        j.Description = CONCAT('Auto-posted: Invoice INV-', DATE_FORMAT(i.InvoiceDate, '%Y%m%d'), '-', i.StudentId)
    WHERE i.InvoiceNumber NOT LIKE '%-X';
UPDATE studentinvoices SET InvoiceNumber = CONCAT('INV-', DATE_FORMAT(InvoiceDate, '%Y%m%d'), '-', StudentId) WHERE InvoiceNumber NOT LIKE '%-X';
UPDATE studentinvoices SET Created = InvoiceDate + INTERVAL 8 HOUR + INTERVAL (Id % 50) MINUTE, Modified = Created;
UPDATE studentinvoices i JOIN (SELECT StudentInvoiceId, MAX(PaymentDate) d FROM payments GROUP BY StudentInvoiceId) p ON p.StudentInvoiceId = i.Id
    SET i.Modified = p.d + INTERVAL 16 HOUR;
UPDATE studentinvoiceitems ii JOIN studentinvoices i ON i.Id = ii.StudentInvoiceId SET ii.Created = i.Created, ii.Modified = i.Modified;
UPDATE payments SET Created = PaymentDate + INTERVAL 7 HOUR + INTERVAL (Id * 37 % 600) MINUTE, Modified = Created;
UPDATE paymentallocations a JOIN payments p ON p.Id = a.PaymentId SET a.Created = p.Created, a.Modified = p.Created;
UPDATE sponsorpayments SET Created = PaymentDate + INTERVAL 10 HOUR, Modified = Created;
UPDATE sponsorships SET Created = '${terms[0].start.getFullYear()}-01-03 09:30', Modified = Created;
UPDATE sponsorshipfeecategories SET Created = '${terms[0].start.getFullYear()}-01-03 09:30', Modified = Created;
UPDATE feestructures f JOIN sessions s ON s.Id = f.SessionId SET f.Created = s.StartDate - INTERVAL 21 DAY + INTERVAL 10 HOUR, f.Modified = f.Created;
UPDATE feestructureitems i JOIN feestructures f ON f.Id = i.FeeStructureId SET i.Created = f.Created, i.Modified = f.Created;
UPDATE budgetmasters SET Created = '${+year.Name - 1}-11-24 10:00', Modified = Created;
UPDATE budgets SET Created = '${+year.Name - 1}-11-28 11:00', Modified = '${+year.Name - 1}-12-05 15:00';
UPDATE budgetlines SET Created = '${+year.Name - 1}-11-28 11:00', Modified = Created;
UPDATE expenses SET Created = ExpenseDate + INTERVAL 9 HOUR + INTERVAL (Id * 13 % 300) MINUTE, Modified = Created;
UPDATE expenselines l JOIN expenses e ON e.Id = l.ExpenseId SET l.Created = e.Created, l.Modified = e.Created;
UPDATE approvalrequests r JOIN expenses e ON r.EntityType = 'Expense' AND r.EntityId = e.Id
    SET r.SubmittedAt = e.Created + INTERVAL 15 MINUTE, r.Created = r.SubmittedAt, r.Modified = r.SubmittedAt;
UPDATE approvalrequests r JOIN budgets b ON r.EntityType = 'Budget' AND r.EntityId = b.Id
    SET r.SubmittedAt = '${+year.Name - 1}-12-01 09:00', r.Created = r.SubmittedAt, r.Modified = r.SubmittedAt;
UPDATE approvalstepactions a JOIN approvalrequests r ON r.Id = a.ApprovalRequestId
    SET a.ActionedAt = r.SubmittedAt + INTERVAL a.StepRank * 3 HOUR, a.Created = r.SubmittedAt, a.Modified = a.ActionedAt
    WHERE r.EntityType IN ('Expense', 'Budget') AND a.ActionedAt IS NOT NULL;
UPDATE journalentries SET Created = EntryDate + INTERVAL 17 HOUR, Modified = Created WHERE Description NOT LIKE 'Payroll%';
UPDATE journallines l JOIN journalentries j ON j.Id = l.JournalEntryId SET l.Created = j.Created, l.Modified = j.Created
    WHERE j.Description NOT LIKE 'Payroll%';
DELETE FROM auditlogs;
`);

    const sum = query(`SELECT (SELECT SUM(TotalAmount) FROM studentinvoices) billed, (SELECT SUM(PaidAmount) FROM studentinvoices) paid,
        (SELECT SUM(Amount) FROM sponsorpayments) sponsors, (SELECT SUM(Amount) FROM expenselines) spent,
        (SELECT SUM(Debit) - SUM(Credit) FROM journallines) imbalance`)[0];
    const kes = (v) => 'KES ' + Math.round(+v || 0).toLocaleString('en-KE');
    console.log(`Done. Billed ${kes(sum.billed)}, collected ${kes(sum.paid)}, sponsors paid ${kes(sum.sponsors)}, spent ${kes(sum.spent)}; `
        + `journals ${+sum.imbalance === 0 ? 'balance' : 'are OUT by ' + sum.imbalance}.`);
    console.log(`The accountant signs in as ${bursar} with ${opt.password}.`);
}

main().catch((e) => fail(e.stack || e.message));
