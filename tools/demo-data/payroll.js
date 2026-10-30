#!/usr/bin/env node
// Fills an anonymised demo copy with a year of payroll: complete staff payroll
// details (KRA PIN, ID, NSSF and SHA numbers, bank accounts), salary structures
// with allowances and deductions (union dues, SACCO, welfare, HELB, insurance,
// pension), NITA paid by the school as the law has it, all of last year (for its P9s), loans and salary advances, and a payroll for every month from
// January to the last full month before --through. Each month is processed by the
// accountant, reviewed by one administrator and approved by another, and its
// journal posts to the ledger. Everything goes through the API, so the payslips,
// loan balances, journals and approval trails are what the app itself records.
//
// The year has the changes a real one has: support staff on overtime in term
// time, an acting allowance, leave allowance in August, a promotion in July, a
// teacher who joins in May and one who leaves at the end of June, and loans that
// start, run and finish through the year.
//
// Run it after finance.js (it needs the accountant's login), with the API running
// on the same database:
//
//   MYSQL_PWD=... node tools/demo-data/payroll.js --db shulenova --api http://localhost:5198 --through 2026-11-05
//
// Options: --db <database> (required; refuses names of live schools)
//          --api <API origin> (default http://localhost:5198)
//          --mysql <path to mysql.exe> (default MySQL 8.4 on Windows)
//          --user <mysql user> (default root)
//          --password <the demo logins' password> (default Demo@2026)
//          --through <yyyy-mm-dd> (payroll runs for every month that has ended by
//                    then; default two weeks from today)
//          --seed <number> (default 2026)
// All payroll data already in the copy is deleted first, so it can be run again.
// The terms finance.js closed are reopened while payroll posts into them, and
// closed again at the end.

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
let seed = (+opt.seed + 7) >>> 0;
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
const digits = (n) => Array.from({length: n}, (_, i) => (i === 0 ? between(1, 9) : between(0, 9))).join('');
const letter = () => 'ABCDEFGHJKLMNPQRSTUVWXYZ'[between(0, 23)];

// ---------- Dates ----------
const day = (s) => new Date(s.slice(0, 10) + 'T00:00:00');
const iso = (d) => `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
const addDays = (d, n) => new Date(d.getFullYear(), d.getMonth(), d.getDate() + n);
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

const PAYROLL_MENUS = ['/payroll/employee-salaries', '/payroll/loan-advances', '/payroll/periods', '/payroll/reports',
    '/payroll/earning-types', '/payroll/deduction-types', '/payroll/tax-bands', '/payroll/nssf-bands', '/payroll/reliefs', '/payroll/settings'];
// The accountant runs payroll; administrators review and approve it; the head teacher reads the reports.
const MENUS = {
    Accounts: PAYROLL_MENUS,
    Administrator: ['/payroll/periods', '/payroll/reports'],
    HeadTeacher: ['/payroll/reports'],
};

const SURNAMES = ['Kazungu', 'Mwambire', 'Chengo', 'Kahindi', 'Baya', 'Katana', 'Mwaringa', 'Nyiro', 'Charo', 'Kenga', 'Masha', 'Tsuma'];
const BANKS = [
    {name: 'KCB Bank', branches: ['Kilifi', 'Malindi', 'Mombasa Moi Avenue'], digits: 10},
    {name: 'Equity Bank', branches: ['Kilifi', 'Mtwapa', 'Malindi'], digits: 13},
    {name: 'Co-operative Bank', branches: ['Kilifi', 'Mombasa Nkrumah Road'], digits: 14},
    {name: 'NCBA Bank', branches: ['Mombasa Nyali'], digits: 10},
    {name: 'Family Bank', branches: ['Kilifi', 'Malindi'], digits: 12},
];
const MONTH = ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'];

async function main() {
    if (!query('SELECT COUNT(*) n FROM person')[0]) fail(`database ${opt.db} not found`);
    const year = query(`SELECT Id, Name FROM academicyears WHERE StartDate <= '${iso(today)}' AND EndDate >= '${iso(today)}'`)[0];
    if (!year) fail('no academic year covers today');
    const Y = +year.Name;
    // Every month that has ended by --through.
    const months = Array.from({length: 12}, (_, i) => i + 1).filter((m) => new Date(Y, m, 0) <= through);
    if (!months.length) fail(`no month of ${Y} has ended by ${iso(through)}`);

    // ---------- 1. Logins ----------
    const roleUsers = (role) => query(`SELECT u.UserName, u.Id, u.PersonId FROM aspnetusers u JOIN aspnetuserroles ur ON ur.UserId = u.Id
        JOIN aspnetroles r ON r.Id = ur.RoleId WHERE r.Name = ${q(role)} AND u.UserName <> 'admin' ORDER BY u.Id`);
    const bursar = roleUsers('Accounts')[0]?.UserName;
    if (!bursar) fail('no accountant login - run finance.js first');
    const boss = roleUsers('Administrator').map((r) => r.UserName);
    if (boss.length < 2) fail('need two Administrator logins (other than admin)');
    exec(Object.entries(MENUS).flatMap(([role, paths]) => paths.map((p) => `INSERT INTO menupermissions (RoleId, MenuPath, MenuName, Created, CreatedBy, Modified, ModifiedBy)
    SELECT ${q(role)}, ${q(p)}, (SELECT MAX(MenuName) FROM menupermissions m WHERE m.MenuPath = ${q(p)}), NOW(), 'admin', NOW(), 'admin'
    FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM menupermissions WHERE RoleId = ${q(role)} AND MenuPath = ${q(p)})
        AND EXISTS (SELECT 1 FROM menupermissions m WHERE m.MenuPath = ${q(p)});`)).join('\n'));
    const uid = Object.fromEntries(query('SELECT Id, UserName FROM aspnetusers').map((r) => [r.UserName, +r.Id]));
    const tok = {[bursar]: await login(bursar), [boss[0]]: await login(boss[0]), [boss[1]]: await login(boss[1])};
    const B = tok[bursar];
    const [reviewer, approver] = [boss[1], boss[0]];
    console.log(`Payroll run by ${bursar}; reviewed by ${reviewer}, approved by ${approver}.`);

    const workflow = query(`SELECT w.Id FROM approvalworkflows w WHERE w.FormKey = 'Payroll' AND w.IsActive = 1`)[0];
    if (!workflow) fail('no active Payroll approval workflow - start the API once so its migrations run');

    // ---------- 2. Reopen the closed terms (payroll posts into them) ----------
    const reopened = [];
    for (;;) {
        const {periods} = await api(tok[approver], 'GET', `/accountingPeriods?academicYearId=${year.Id}`);
        const latest = periods.filter((p) => p.closed).pop();
        if (!latest) break;
        await api(tok[approver], 'POST', `/accountingPeriods/reopen?key=${encodeURIComponent(latest.key)}`);
        reopened.push(latest.name);
    }
    if (reopened.length) console.log(`Reopened ${reopened.reverse().join(', ')} while payroll posts into them.`);

    // ---------- 3. Clear the payroll ----------
    console.log(`Clearing payroll data in ${opt.db}...`);
    exec(`SET FOREIGN_KEY_CHECKS = 0;
DELETE a FROM approvalstepactions a JOIN approvalrequests r ON r.Id = a.ApprovalRequestId
    WHERE r.EntityType = 'PayrollPeriod' OR (r.EntityType = 'JournalEntry' AND r.EntityId IN (SELECT Id FROM journalentries WHERE ReferenceNumber LIKE 'PRL-JNL-%'));
DELETE FROM approvalrequests WHERE EntityType = 'PayrollPeriod'
    OR (EntityType = 'JournalEntry' AND EntityId IN (SELECT Id FROM journalentries WHERE ReferenceNumber LIKE 'PRL-JNL-%'));
DELETE l FROM journallines l JOIN journalentries j ON j.Id = l.JournalEntryId WHERE j.ReferenceNumber LIKE 'PRL-JNL-%';
DELETE FROM journalentries WHERE ReferenceNumber LIKE 'PRL-JNL-%';
DELETE FROM payslipdeductions; DELETE FROM payslipearnings; DELETE FROM payslips; DELETE FROM payrollperiods;
DELETE FROM loanadvances; DELETE FROM employeesalaryitems; DELETE FROM employeesalaries;
SET FOREIGN_KEY_CHECKS = 1;
-- Approved payroll posts straight to the ledger: the two-step approval is the review.
UPDATE globalsettings SET SettingValue = 'auto' WHERE Module = 'Finance' AND SettingKey = 'PayrollJournalMode';
-- NITA as the law has it: KES 50 a month per employee, paid by the school, not deducted from pay.
INSERT INTO accounts (Code, Name, AccountType, Description, IsActive, Created, CreatedBy, Modified, ModifiedBy)
    SELECT '2150', 'NITA Levy Payable', 2, 'Industrial training levy owed to NITA', 1, '2025-12-01', 'admin', '2025-12-01', 'admin'
    FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM accounts WHERE Code = '2150');
UPDATE deductiontypes SET IsEmployerCost = 1, IsTaxDeductible = 0, IsRetirementContribution = 0,
    LiabilityAccountId = (SELECT Id FROM accounts WHERE Code = '2150'),
    Description = 'Industrial training levy, KES 50 a month per employee, paid by the school'
    WHERE Code = 'NITA';
INSERT INTO designations (Name, Description, \`Rank\`, Created, CreatedBy, Modified, ModifiedBy)
    SELECT d.n, d.n, d.r, NOW(), 'admin', NOW(), 'admin' FROM (SELECT 'Deputy Head Teacher' n, 4 r UNION ALL SELECT 'Cook', 10
        UNION ALL SELECT 'Driver', 11 UNION ALL SELECT 'Cleaner', 12 UNION ALL SELECT 'Security Guard', 13) d
    WHERE NOT EXISTS (SELECT 1 FROM designations x WHERE x.Name = d.n);
`);

    // ---------- 4. Staff ----------
    const desig = Object.fromEntries(query('SELECT Id, Name FROM designations').map((r) => [r.Name, +r.Id]));
    const empType = Object.fromEntries(query('SELECT Id, Name FROM employmenttypes').map((r) => [r.Name, +r.Id]));
    const cat = Object.fromEntries(query('SELECT Id, Name FROM staffcategories').map((r) => [r.Name, +r.Id]));
    const STAFF = `SELECT p.Id, p.FullName, p.Status, p.StaffCategoryId, p.DesignationId, d.Name Designation, p.EmploymentDate,
        p.KraPinNo, p.IdNumber, p.NssfNo, p.NhifNo FROM person p JOIN designations d ON d.Id = p.DesignationId
        WHERE p.Discriminator = 'StaffDetails'`;
    // The July promotion is made again below, so a deputy from an earlier run starts the year as a teacher.
    exec(`UPDATE person SET DesignationId = ${desig.Teacher} WHERE DesignationId = ${desig['Deputy Head Teacher']};`);
    const inactive = query(`${STAFF} AND p.Status <> 0 ORDER BY p.Id`);
    const updates = [];
    // A school with two buses and a kitchen has support staff: five of the former staff come back as them
    // (or, on a second run, the five who already did).
    const supportBefore = query(`${STAFF} AND p.Status = 0 AND d.Name IN ('Cook', 'Driver', 'Cleaner', 'Security Guard') ORDER BY p.Id`);
    const support = supportBefore.length >= 5 ? supportBefore.slice(0, 5)
        : inactive.filter((s) => +s.StaffCategoryId === cat['Non-teaching']).slice(0, 5);
    const ROLES = [['Cook', 'Permanent', 18500], ['Driver', 'Contract', 22000], ['Driver', 'Contract', 21000],
        ['Cleaner', 'Casual', 15500], ['Security Guard', 'Contract', 16500]];
    const supportBasic = {};
    support.forEach((s, i) => {
        const [role, type, basic] = ROLES[i];
        let name = s.FullName;
        if (name.replace(/^(MR|MRS|MS|MD|MISS|DR|MADAM)\.?\s+/i, '').trim().split(/\s+/).length < 2)
            name = `${name} ${pick(SURNAMES).toUpperCase()}`;
        supportBasic[+s.Id] = basic;
        updates.push(`UPDATE person SET Status = 0, FullName = ${q(name)}, DesignationId = ${desig[role]}, EmploymentTypeId = ${empType[type]},
            EndofEmploymentDate = NULL, CurrentlyEmployed = 1, ExcludeFromPayroll = 0 WHERE Id = ${s.Id};`);
    });
    // One teacher leaves at the end of June: on the payroll until then, off the staff list after.
    const leaver = inactive.find((s) => +s.StaffCategoryId === cat.Teaching
        && s.FullName.replace(/^(MR|MRS|MS|MD|MISS|DR|MADAM)\.?\s+/i, '').split(/\s+/).length >= 3);
    if (leaver) updates.push(`UPDATE person SET Status = 0, EndofEmploymentDate = '${Y}-06-30', ExcludeFromPayroll = 0 WHERE Id = ${leaver.Id};`);
    exec(updates.join('\n'));

    const staff = query(`${STAFF} AND p.Status = 0 AND p.ExcludeFromPayroll = 0 ORDER BY p.Id`).map((s) => ({...s, id: +s.Id}));
    // One teacher with no start date joins in May (on a second run, the one who joined then).
    const joiner = staff.find((s) => (s.EmploymentDate || '').startsWith(`${Y}-05-04`))
        ?? staff.find((s) => +s.StaffCategoryId === cat.Teaching && !s.EmploymentDate && s.id !== +leaver?.Id);
    const hods = new Set([...roleUsers('HeadOfDepartment'), ...roleUsers('Administrator')].map((r) => +r.PersonId).filter(Boolean));
    const promoted = staff.find((s) => hods.has(s.id) && s.Designation === 'Teacher');

    const valid = {kra: /^A\d{9}[A-Z]$/, id: /^\d{7,8}$/, nssf: /^\d{9,10}$/, sha: /^[A-Z]{2}\d{13}-\d$/};
    const staffSql = [];
    for (const s of staff) {
        const bank = pick(BANKS);
        s.bank = bank.name;
        const start = s.id === joiner?.id ? `${Y}-05-04` : s.EmploymentDate ? null : `${between(2014, 2025)}-${String(between(1, 12)).padStart(2, '0')}-0${between(1, 9)}`;
        s.started = day(start || s.EmploymentDate);
        staffSql.push(`UPDATE person SET
            KraPinNo = ${q(valid.kra.test(s.KraPinNo || '') ? s.KraPinNo : `A${digits(9)}${letter()}`)},
            IdNumber = ${q(valid.id.test(s.IdNumber || '') ? s.IdNumber : digits(8))},
            NssfNo = ${q(valid.nssf.test(s.NssfNo || '') ? s.NssfNo : digits(10))},
            NhifNo = ${q(valid.sha.test(s.NhifNo || '') ? s.NhifNo : `CR${digits(13)}-${between(1, 9)}`)},
            BankName = ${q(bank.name)}, BankBranch = ${q(pick(bank.branches))}, BankAccountNumber = ${q(digits(bank.digits))}
            ${start ? `, EmploymentDate = '${start}'` : ''} WHERE Id = ${s.id};`);
    }
    exec(staffSql.join('\n'));
    console.log(`Staff on payroll: ${staff.length} (support staff ${support.length}; joins in May: ${joiner?.FullName || '-'}; `
        + `leaves end of June: ${leaver?.FullName || '-'}).`);

    // ---------- 5. Salary structures ----------
    const et = Object.fromEntries(query('SELECT Id, Code FROM earningtypes').map((r) => [r.Code, +r.Id]));
    const dt = Object.fromEntries(query('SELECT Id, Code FROM deductiontypes').map((r) => [r.Code, +r.Id]));
    for (const c of ['OVRTME', 'LVALL', 'ACTALL', 'RSPALL', 'AIRALL']) if (!et[c]) fail(`earning type ${c} missing`);
    for (const c of ['UNION', 'SACCO', 'WELFARE', 'HELB', 'INSURANCE', 'PENSION']) if (!dt[c]) fail(`deduction type ${c} missing`);

    // Basic pay by role, and for teachers by years of service: a new teacher starts at 18,000 and gains
    // about 1,000 a year, up to 34,000 - a low-cost private school, where payroll is about 60% of fees.
    // Support staff are paid as their role above.
    const years = (s) => Math.max(0, Y - s.started.getFullYear());
    const basicOf = (s) => supportBasic[s.id] ?? ({'Head Teacher': 55000, 'Accountant': 32000}[s.Designation]
        ?? (+s.StaffCategoryId === cat.Teaching || s.Designation === 'Teacher'
            ? Math.min(34000, round(18000 + years(s) * 1000 + between(0, 3) * 500, 500))
            : round(between(24000, 28000), 500)));
    for (const s of staff) s.basic = basicOf(s);
    const teaching = staff.filter((s) => +s.StaffCategoryId === cat.Teaching || s.Designation === 'Head Teacher');
    const isSupport = (s) => supportBasic[s.id] !== undefined;
    const shuffled = shuffle(staff);
    const sacco = new Set(shuffled.slice(0, Math.round(staff.length * 0.55)).map((s) => s.id));
    const union = new Set(shuffle(teaching).slice(0, Math.round(teaching.length * 0.7)).map((s) => s.id));
    const helb = new Set(shuffle(staff.filter((s) => !isSupport(s) && s.basic < 30000)).slice(0, 3).map((s) => s.id));
    const insured = new Set(shuffle(staff).slice(0, 5).map((s) => s.id));
    const pension = new Set([...staff].sort((x, y) => y.basic - x.basic).slice(0, 4).map((s) => s.id));

    const sal = new Map(); // staffId -> salary model, as sent to the API
    for (const s of staff) {
        const basic = s.basic;
        const head = s.Designation === 'Head Teacher';
        const items = [];
        const earn = (code, amount) => items.push({earningTypeId: et[code], amount});
        const ded = (code, amount) => items.push({deductionTypeId: dt[code], amount});
        if (head) { earn('RSPALL', 10000); earn('AIRALL', 1500); }
        else if (hods.has(s.id)) earn('RSPALL', 3000);
        if (s.Designation === 'Accountant') earn('AIRALL', 1000);
        if (union.has(s.id)) ded('UNION', round(basic * 0.02, 10));
        if (sacco.has(s.id)) ded('SACCO', pick([1000, 1500, 2000, 3000, 4000, 5000]));
        ded('WELFARE', 300);
        if (helb.has(s.id)) ded('HELB', pick([2500, 3000, 4000]));
        if (insured.has(s.id)) ded('INSURANCE', pick([1500, 2000, 2500, 3000]));
        if (pension.has(s.id)) ded('PENSION', round(basic * 0.05, 100));
        const model = {staffDetailsId: s.id, basicSalary: basic,
            houseAllowance: isSupport(s) ? 1500 : Math.max(2000, round(basic * 0.10, 500)),
            transportAllowance: isSupport(s) ? 1000 : 1500, otherAllowances: 0,
            effectiveDate: s.id === joiner?.id ? `${Y}-05-04` : `${Y}-01-01`, isActive: true,
            notes: head ? 'Head teacher: responsibility and airtime allowances' : null, items};
        const created = await api(B, 'POST', '/employeeSalaries', model);
        sal.set(s.id, {...model, id: created.id});
    }
    // Changes during the year are edits to the structure, as a payroll officer would make them.
    const setItem = (s, kind, code, amount) => {
        const m = sal.get(s.id);
        const key = kind === 'earn' ? 'earningTypeId' : 'deductionTypeId';
        const typeId = kind === 'earn' ? et[code] : dt[code];
        m.items = m.items.filter((i) => i[key] !== typeId);
        if (amount > 0) m.items.push({[key]: typeId, amount});
    };
    const save = (s) => { const m = sal.get(s.id); return api(B, 'PUT', `/employeeSalaries/${m.id}`, m); };
    console.log(`Salary structures: ${sal.size}.`);

    // A month's payroll: the accountant processes it and submits it, one administrator reviews it and
    // another approves it.
    const runPeriod = async (m, year) => {
        const period = await api(B, 'POST', '/payrollPeriods', {month: m, year});
        await api(B, 'POST', `/payrollPeriods/${period.id}/process`);
        const req = await api(B, 'POST', '/approvalRequests/submit', {entityType: 'PayrollPeriod', entityId: period.id, formKey: 'Payroll',
            stepAssignments: [{stepRank: 1, assignedToUserId: uid[reviewer]}, {stepRank: 2, assignedToUserId: uid[approver]}]});
        await api(tok[reviewer], 'POST', `/approvalRequests/${req.id}/action`, {action: 'approve',
            comment: pick(['Checked against the muster roll.', 'Totals agree with last month and the changes listed.', 'Reviewed - OK.'])});
        await api(tok[approver], 'POST', `/approvalRequests/${req.id}/action`, {action: 'approve', comment: 'Approved for payment.'});
        const t = query(`SELECT COUNT(*) n, SUM(GrossPay) g, SUM(NetPay) net FROM payslips WHERE PayrollPeriodId = ${period.id}`)[0];
        console.log(`${MONTH[m - 1]} ${year}: ${t.n} payslips, gross ${Math.round(+t.g).toLocaleString('en-KE')}, net ${Math.round(+t.net).toLocaleString('en-KE')}.`);
        return period;
    };
    const drivers = staff.filter((s) => isSupport(s) && ['Driver', 'Cook'].includes(ROLES[support.findIndex((x) => +x.Id === s.id)]?.[0]));
    const holiday = (m) => m === 4 || m === 8 || m === 12; // the April, August and December school holidays

    // ---------- 5b. Last year ----------
    // A full year before this one, so staff have a closed year's P9 on their profile. It is paid at last
    // year's rates - about 5% below this year's, which the January increment brings them up to - and posts
    // no journals: the demo's books start this January.
    const lastYear = Y - 1;
    const rates = new Map([...sal].map(([id, m]) => [id, {basic: m.basicSalary, house: m.houseAllowance, notes: m.notes}]));
    exec(`UPDATE globalsettings SET SettingValue = 'off' WHERE Module = 'Finance' AND SettingKey = 'PayrollJournalMode';`);
    for (const s of staff) {
        const m = sal.get(s.id);
        m.basicSalary = isSupport(s) ? m.basicSalary - 500 : round(m.basicSalary * 0.95, 500);
        if (!isSupport(s)) m.houseAllowance = Math.max(2000, round(m.basicSalary * 0.10, 500));
        await save(s);
    }
    for (let m = 1; m <= 12; m++) {
        const changed = new Set();
        for (const s of drivers) { setItem(s, 'earn', 'OVRTME', holiday(m) ? 0 : round(between(1500, 4500))); changed.add(s); }
        if (m === 8 || m === 9) for (const s of teaching) { setItem(s, 'earn', 'LVALL', m === 8 ? 4500 : 0); changed.add(s); }
        for (const s of changed) await save(s);
        await runPeriod(m, lastYear);
    }
    for (const s of staff) {
        const m = sal.get(s.id), r = rates.get(s.id);
        m.basicSalary = r.basic;
        m.houseAllowance = r.house;
        m.notes = [r.notes, `Annual increment from 1 January ${Y}`].filter(Boolean).join('. ');
        await save(s);
    }
    exec(`UPDATE globalsettings SET SettingValue = 'auto' WHERE Module = 'Finance' AND SettingKey = 'PayrollJournalMode';`);

    // ---------- 6. Loans and advances, issued in the month they start ----------
    const loanees = shuffle(staff.filter((s) => !isSupport(s)));
    const LOANS = [
        {m: 2, s: loanees[0], d: 'Salary advance', p: 10000, i: 5000},
        {m: 2, s: support[1] && staff.find((x) => x.id === +support[1].Id), d: 'Salary advance', p: 8000, i: 4000},
        {m: 3, s: loanees[1], d: 'Staff loan - school fees', p: 60000, i: 6000},
        {m: 3, s: staff.find((x) => x.Designation === 'Accountant'), d: 'Staff loan - motorbike', p: 45000, i: 4500},
        {m: 6, s: loanees[2], d: 'Emergency loan - medical', p: 20000, i: 5000},
        {m: 8, s: loanees[3], d: 'School fees advance', p: 15000, i: 5000},
        {m: 8, s: support[0] && staff.find((x) => x.id === +support[0].Id), d: 'School fees advance', p: 9000, i: 3000},
        {m: 10, s: loanees[4], d: 'Salary advance', p: 8000, i: 8000},
    ].filter((l) => l.s && months.includes(l.m));

    // ---------- 7. The months ----------
    const acting = [...teaching].filter((s) => s.id !== promoted?.id && s.Designation === 'Teacher').sort((x, y) => y.basic - x.basic)[0];
    for (const m of months) {
        const changed = new Set();
        // Overtime for the drivers and the cook in term time.
        for (const s of drivers) { setItem(s, 'earn', 'OVRTME', holiday(m) ? 0 : round(between(1500, 4500))); changed.add(s); }
        // An acting deputy for March to May, while the post was filled.
        if (acting && [3, 6].includes(m)) { setItem(acting, 'earn', 'ACTALL', m === 3 ? 7500 : 0); changed.add(acting); }
        // Leave allowance in August, to permanent and contract teachers alike.
        if (m === 8 || m === 9) for (const s of teaching) { setItem(s, 'earn', 'LVALL', m === 8 ? 5000 : 0); changed.add(s); }
        // July: a head of department becomes deputy head teacher.
        if (promoted && m === 7) {
            const p = sal.get(promoted.id);
            p.basicSalary += 6000;
            p.houseAllowance = Math.max(2000, round(p.basicSalary * 0.10, 500));
            p.notes = `Promoted to deputy head teacher from 1 July ${Y}`;
            setItem(promoted, 'earn', 'RSPALL', 6000);
            changed.add(promoted);
            exec(`UPDATE person SET DesignationId = ${desig['Deputy Head Teacher']} WHERE Id = ${promoted.id};`);
        }
        for (const s of changed) await save(s);
        for (const l of LOANS.filter((x) => x.m === m))
            await api(B, 'POST', '/loanAdvances', {staffDetailsId: l.s.id, description: l.d, principalAmount: l.p, monthlyDeduction: l.i,
                balance: l.p, issueDate: `${Y}-${String(m).padStart(2, '0')}-${String(between(5, 15)).padStart(2, '0')}`, status: 1,
                notes: `${Math.ceil(l.p / l.i)} instalment(s) of ${l.i.toLocaleString('en-KE')}`});

        await runPeriod(m, Y);

        // The leaver is paid to the end of June and then leaves the staff list.
        if (leaver && m === 6) exec(`UPDATE person SET Status = ${leaver.Status} WHERE Id = ${leaver.Id};`);
    }

    // ---------- 8. Close the terms again ----------
    for (;;) {
        const {periods} = await api(B, 'GET', `/accountingPeriods?academicYearId=${year.Id}`);
        const next = periods.find((p) => !p.closed && !p.blockers.length);
        if (!next) break;
        await api(B, 'POST', `/accountingPeriods/close?key=${encodeURIComponent(next.key)}`);
        console.log(`Closed ${next.name}.`);
    }

    // ---------- 9. Make it look recorded on the day ----------
    // Each payroll is run on the 24th (the 22nd in December), reviewed that
    // afternoon and approved the next morning; loans on their issue dates.
    exec(`
UPDATE payrollperiods SET Created = CONCAT(Year, '-', LPAD(Month, 2, '0'), '-20 09:00'),
    ProcessedDate = CONCAT(Year, '-', LPAD(Month, 2, '0'), '-24 10:15'),
    ApprovedDate = CONCAT(Year, '-', LPAD(Month, 2, '0'), '-25 09:40'), Modified = ApprovedDate;
UPDATE payslips s JOIN payrollperiods p ON p.Id = s.PayrollPeriodId SET s.Created = p.ProcessedDate, s.Modified = p.ProcessedDate;
UPDATE payslipearnings e JOIN payslips s ON s.Id = e.PayslipId SET e.Created = s.Created, e.Modified = s.Created;
UPDATE payslipdeductions d JOIN payslips s ON s.Id = d.PayslipId SET d.Created = s.Created, d.Modified = s.Created;
UPDATE approvalrequests r JOIN payrollperiods p ON r.EntityType = 'PayrollPeriod' AND r.EntityId = p.Id
    SET r.SubmittedAt = p.ProcessedDate + INTERVAL 30 MINUTE, r.Created = r.SubmittedAt, r.Modified = p.ApprovedDate;
UPDATE approvalstepactions a JOIN approvalrequests r ON r.Id = a.ApprovalRequestId JOIN payrollperiods p ON r.EntityType = 'PayrollPeriod' AND r.EntityId = p.Id
    SET a.ActionedAt = IF(a.StepRank = 1, r.SubmittedAt + INTERVAL 4 HOUR, p.ApprovedDate), a.Created = r.SubmittedAt, a.Modified = a.ActionedAt;
UPDATE journalentries j JOIN payrollperiods p ON j.ReferenceNumber = CONCAT('PRL-JNL-', REPLACE(p.Name, ' ', '-'))
    SET j.Created = p.ApprovedDate, j.Modified = p.ApprovedDate;
UPDATE journallines l JOIN journalentries j ON j.Id = l.JournalEntryId SET l.Created = j.Created, l.Modified = j.Created
    WHERE j.ReferenceNumber LIKE 'PRL-JNL-%';
UPDATE loanadvances SET Created = IssueDate + INTERVAL 11 HOUR, Modified = Created;
UPDATE employeesalaries SET Created = '${Y - 2}-12-18 10:00', Modified = Created;
UPDATE employeesalaries s JOIN person p ON p.Id = s.StaffDetailsId SET s.Created = p.EmploymentDate - INTERVAL 3 DAY + INTERVAL 10 HOUR, s.Modified = s.Created
    WHERE p.EmploymentDate > '${Y - 1}-01-01';
UPDATE employeesalaryitems i JOIN employeesalaries s ON s.Id = i.EmployeeSalaryId SET i.Created = s.Created, i.Modified = s.Created;
DELETE FROM auditlogs;
`);

    const kesY = (v) => 'KES ' + Math.round(+v || 0).toLocaleString('en-KE');
    for (const r of query(`SELECT p.Year, COUNT(DISTINCT p.Id) months, COUNT(*) slips, SUM(s.GrossPay) gross, SUM(s.Paye) paye, SUM(s.NetPay) net
        FROM payslips s JOIN payrollperiods p ON p.Id = s.PayrollPeriodId GROUP BY p.Year ORDER BY p.Year`))
        console.log(`${r.Year}: ${r.months} months, ${r.slips} payslips, gross ${kesY(r.gross)}, PAYE ${kesY(r.paye)}, net ${kesY(r.net)}.`);
    const sum = query(`SELECT SUM(GrossPay) gross, SUM(Paye) paye, SUM(NetPay) net, SUM(NssfEmployee + NssfEmployer) nssf, COUNT(*) slips,
        (SELECT SUM(Balance) FROM loanadvances WHERE Status = 1) loans, (SELECT SUM(Debit) - SUM(Credit) FROM journallines) imbalance
        FROM payslips s JOIN payrollperiods p ON p.Id = s.PayrollPeriodId WHERE p.Year = ${Y}`)[0];
    const kes = (v) => 'KES ' + Math.round(+v || 0).toLocaleString('en-KE');
    console.log(`Done. ${months.length} months, ${sum.slips} payslips: gross ${kes(sum.gross)}, PAYE ${kes(sum.paye)}, NSSF ${kes(sum.nssf)}, `
        + `net ${kes(sum.net)}; loans still owed ${kes(sum.loans)}; journals ${+sum.imbalance === 0 ? 'balance' : 'are OUT by ' + sum.imbalance}.`);
}

main().catch((e) => fail(e.stack || e.message));
