#!/usr/bin/env node
// Fills an anonymised demo copy with a year of daily attendance:
//  - learners: every school day of the year's terms (not public holidays or the
//    half-term breaks), each class marked by its class teacher in the morning.
//    Most learners miss the odd day, a few miss many; illness keeps a child home
//    two to four days running; Mondays, Fridays, the long rains (April-May) and the
//    first days of term see more absence, pre-primary and special units more than
//    the juniors. Most absences have a reason. One rainy day keeps many at home;
//  - staff: marked by the head teacher, with arrival and departure times; a few
//    sick days, workshops and official duties, compassionate leave and late arrivals.
// Everything goes through the API (the registers' own batch endpoints).
//
//   MYSQL_PWD=... node tools/demo-data/attendance.js --db shulenova --api http://localhost:5198 --through 2026-11-05
//
// Options: --db <database> (required; refuses names of live schools)
//          --api <API origin> (default http://localhost:5198)
//          --mysql <path to mysql.exe> (default MySQL 8.4 on Windows)
//          --user <mysql user> (default root)
//          --password <the demo logins' password> (default Demo@2026)
//          --year <academic year name> (default 2026)
//          --through <yyyy-mm-dd> (school days up to then; default today)
//          --head <user name who marks staff attendance> (default purity.haji)
//          --seed <number> (default 2026)
// All attendance in the copy is deleted first, so it can be run again.

const {execFileSync} = require('child_process');

const opt = {mysql: 'C:/Program Files/MySQL/MySQL Server 8.4/bin/mysql.exe', user: 'root', api: 'http://localhost:5198',
    password: 'Demo@2026', year: '2026', head: 'purity.haji', seed: '2026'};
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

const q = (s) => `'${String(s).replace(/\\/g, '\\\\').replace(/'/g, "''")}'`;

// ---------- Random, repeatable ----------
let seed = (+opt.seed + 71) >>> 0;
function rnd() {
    seed = (seed + 0x6d2b79f5) >>> 0;
    let t = seed;
    t = Math.imul(t ^ (t >>> 15), t | 1);
    t ^= t + Math.imul(t ^ (t >>> 7), t | 61);
    return ((t ^ (t >>> 14)) >>> 0) / 4294967296;
}
const pick = (list) => list[Math.floor(rnd() * list.length)];
const chance = (p) => rnd() < p;
const gauss = () => Math.sqrt(-2 * Math.log(1 - rnd())) * Math.cos(2 * Math.PI * rnd());
const weighted = (pairs) => {
    let r = rnd() * pairs.reduce((s, [, w]) => s + w, 0);
    for (const [x, w] of pairs) if ((r -= w) < 0) return x;
    return pairs[pairs.length - 1][0];
};
const pad = (n) => String(n).padStart(2, '0');

// ---------- Dates ----------
const iso = (d) => `${d.getUTCFullYear()}-${pad(d.getUTCMonth() + 1)}-${pad(d.getUTCDate())}`;
const day = (s) => new Date(s.slice(0, 10) + 'T00:00:00Z');
const addDays = (d, n) => new Date(d.getTime() + n * 86400000);
const today = iso(new Date());
const through = opt.through || today;
const time = (minutes) => `${pad(Math.floor(minutes / 60))}:${pad(Math.floor(minutes % 60))}:00`;

// Kenyan public holidays that can fall in term (2026), and the half-term breaks the
// school calendar does not list as events (Term 2's comes from the events).
const HOLIDAYS = {
    '2026-01-01': 'New Year', '2026-03-20': 'Idd-ul-Fitr', '2026-04-03': 'Good Friday', '2026-04-06': 'Easter Monday',
    '2026-05-01': 'Labour Day', '2026-05-27': 'Idd-ul-Adha', '2026-06-01': 'Madaraka Day', '2026-10-10': 'Mazingira Day',
    '2026-10-20': 'Mashujaa Day', '2026-12-12': 'Jamhuri Day', '2026-12-25': 'Christmas', '2026-12-26': 'Boxing Day'
};
const HALF_TERMS = [['2026-02-25', '2026-03-01']];

// ---------- API ----------
async function api(token, method, url, body) {
    const r = await fetch(opt.api + '/api' + url, {method,
        headers: {'Content-Type': 'application/json', ...(token ? {Authorization: `Bearer ${token}`} : {})},
        body: body === undefined ? undefined : JSON.stringify(body)});
    const text = await r.text();
    if (!r.ok) throw new Error(`${method} ${url} -> ${r.status} ${text.slice(0, 300)}`);
    try {
        return text ? JSON.parse(text) : null;
    } catch {
        return text;
    }
}
const tokens = {};
const tok = async (userName) => tokens[userName] ??= (await api(null, 'POST', '/auth/login', {userName, password: opt.password})).token;

async function pool(items, size, fn) {
    let next = 0;
    await Promise.all(Array.from({length: size}, async () => {
        while (next < items.length) await fn(items[next++]);
    }));
}

// ---------- Why ----------
// [reason, weight, days it tends to last]
const LEARNER_REASONS = [
    ['Sick', 26, 3], ['Sick - malaria', 10, 4], ['Sick - flu', 12, 2], ['Sick - stomach upset', 6, 2], ['Fever', 8, 2],
    ['Hospital appointment', 6, 1], ['Family function', 5, 1], ['Attended a funeral', 4, 2], ['Travelled upcountry', 4, 3],
    ['Parent called - unwell', 8, 1], ['Toothache', 2, 1], ['Eye infection', 2, 3], ['Injured - fell at home', 2, 2]
];
const STAFF_REASONS = [
    ['Sick leave', 30, 2], ['Sick off - doctor\'s note', 12, 3], ['Compassionate leave - bereavement', 6, 3],
    ['Official duty - CBC training', 10, 2], ['Attending a workshop', 8, 1], ['Permission - personal matters', 12, 1],
    ['Official duty - TSC county office', 6, 1], ['Hospital appointment', 10, 1]
];

async function main() {
    // ---------- 1. The calendar ----------
    const year = query(`SELECT Id FROM academicyears WHERE Name = ${q(opt.year)}`)[0];
    if (!year) fail(`no academic year "${opt.year}"`);
    const terms = query(`SELECT Id, SessionName, StartDate, EndDate FROM sessions WHERE AcademicYearId = ${year.Id} ORDER BY StartDate`);
    // Breaks the school put in its calendar ("mid term break", "half term").
    const breaks = query(`SELECT StartDate, EndDate FROM events WHERE EventName LIKE '%mid%term%break%' OR EventName LIKE '%half%term%'`)
        .map((e) => [e.StartDate.slice(0, 10), e.EndDate.slice(0, 10)]).concat(HALF_TERMS);
    const inBreak = (d) => breaks.some(([s, e]) => d >= s && d <= e);
    const days = []; // {date, term, first, last, index}
    for (const t of terms) {
        const list = [];
        for (let d = day(t.StartDate); d <= day(t.EndDate); d = addDays(d, 1)) {
            const s = iso(d);
            const dow = d.getUTCDay();
            if (dow === 0 || dow === 6 || HOLIDAYS[s] || inBreak(s) || s > through) continue;
            list.push(s);
        }
        list.forEach((s, i) => days.push({date: s, dow: day(s).getUTCDay(), term: t.SessionName, index: i, last: i === list.length - 1}));
    }
    if (!days.length) fail('no school days to mark');
    // One wet morning in the long rains keeps many at home.
    const rainy = days.filter((d) => d.date.slice(5, 7) === '04' || d.date.slice(5, 7) === '05');
    const floodDay = rainy.length ? rainy[Math.floor(rainy.length * 0.4)].date : null;
    console.log(`${days.length} school days in ${terms.length} term(s) of ${opt.year}${floodDay ? `, heavy rains on ${floodDay}` : ''}.`);

    // ---------- 2. Learners and staff ----------
    const classes = query(`SELECT c.Id, c.Name, ll.Name level, ll.Rank,
            (SELECT u.UserName FROM schoolclassleaders x JOIN classleadershiproles r ON r.Id = x.ClassLeadershipRoleId
             JOIN aspnetusers u ON u.PersonId = x.PersonId WHERE x.SchoolClassId = c.Id AND r.PersonType = 1 ORDER BY x.Id LIMIT 1) teacher
        FROM schoolclasses c JOIN learninglevels ll ON ll.Id = c.LearningLevelId WHERE c.AcademicYearId = ${year.Id}`);
    // Each learner in their current class of the year (the latest, if they moved).
    const roll = query(`SELECT sc.Id studentClassId, sc.SchoolClassId classId, p.Id studentId, p.AdmissionDate
        FROM studentclasses sc JOIN person p ON p.Id = sc.StudentId JOIN schoolclasses c ON c.Id = sc.SchoolClassId
        WHERE c.AcademicYearId = ${year.Id} AND p.Discriminator = 'Student' AND p.Status = 0
          AND sc.Id = (SELECT MAX(x.Id) FROM studentclasses x JOIN schoolclasses xc ON xc.Id = x.SchoolClassId
                       WHERE x.StudentId = p.Id AND xc.AcademicYearId = ${year.Id})`);
    const staff = query(`SELECT p.Id, p.FullName, p.GenderId, p.EmploymentDate, sc.ForTeaching FROM person p
        LEFT JOIN staffcategories sc ON sc.Id = p.StaffCategoryId WHERE p.Discriminator = 'StaffDetails' AND p.Status = 0`);
    const noTeacher = classes.filter((c) => !c.teacher);
    if (noTeacher.length) console.log(`  (no class teacher login for ${noTeacher.map((c) => c.Name).join(', ')} - marked by ${opt.head})`);
    console.log(`${roll.length} learners in ${classes.length} classes, ${staff.length} staff.`);

    // ---------- 3. Clear ----------
    console.log(`Clearing attendance in ${opt.db}...`);
    exec(`DELETE FROM studentattendances;
DELETE FROM staffattendances;`);

    // ---------- 4. Learners ----------
    // Each learner's own tendency to be away: most near 2%, a few far more.
    const classOf = Object.fromEntries(classes.map((c) => [c.Id, c]));
    const levelFactor = (c) => /day care|pp/i.test(c.level) ? 1.5 : /special/i.test(c.level) ? 1.4 : +c.Rank >= 9 ? 0.85 : 1;
    const learners = roll.map((r) => ({
        ...r,
        p: Math.min(0.2, Math.max(0.004, Math.exp(Math.log(0.022) + gauss() * 0.75))) * levelFactor(classOf[r.classId]),
        away: 0, reason: null, // a run of days off under way
        from: r.AdmissionDate && r.AdmissionDate.slice(0, 4) === opt.year ? r.AdmissionDate.slice(0, 10) : null
    }));
    const byClass = {};
    for (const l of learners) (byClass[l.classId] ??= []).push(l);
    const registers = []; // {classId, teacher, rows}
    for (const d of days) {
        for (const c of classes) {
            const rows = [];
            for (const l of byClass[c.Id] || []) {
                if (l.from && d.date < l.from) continue; // not yet admitted
                let present = true, remarks = null;
                if (l.away > 0) {
                    l.away--;
                    present = false;
                    remarks = l.reason;
                } else {
                    let p = l.p * (d.dow === 1 ? 1.25 : d.dow === 5 ? 1.2 : 1);
                    if (['04', '05'].includes(d.date.slice(5, 7))) p *= 1.3;
                    if (d.index < 2) p *= 1.8;
                    if (d.last) p *= 1.4;
                    if (d.date === floodDay) p = Math.max(p, 0.22);
                    if (chance(p)) {
                        present = false;
                        if (d.date === floodDay) remarks = 'Heavy rains - roads flooded';
                        else if (d.index < 2) remarks = pick(['Not yet reported', 'Travelled upcountry', 'Not yet reported']);
                        else {
                            const [reason, , len] = weighted(LEARNER_REASONS.map((r) => [r, r[1]]));
                            remarks = reason;
                            l.reason = reason;
                            for (let i = 1; i < len; i++) if (chance(0.5)) l.away++;
                        }
                        if (!chance(0.8)) remarks = null; // not every absence is explained
                    }
                }
                rows.push({date: `${d.date}T00:00:00`, present, remarks, timeIn: '08:00:00', timeOut: '08:00:00', studentClassId: +l.studentClassId});
            }
            if (rows.length) registers.push({classId: c.Id, teacher: c.teacher || opt.head, date: d.date, rows});
        }
    }

    // ---------- 5. Staff ----------
    // A few days each across the year; teachers trained together on the same days.
    const trainingDays = days.filter((d) => d.term === terms[0]?.SessionName && d.index >= 15).slice(0, 2).map((d) => d.date);
    const trained = new Set(staff.filter((s) => +s.ForTeaching === 1 && chance(0.3)).map((s) => s.Id));
    const people = staff.map((s) => ({...s, p: Math.min(0.06, Math.max(0.005, Math.exp(Math.log(0.014) + gauss() * 0.6))),
        away: 0, reason: null, punctual: 7 * 60 + 5 + gauss() * 12}));
    const staffRegisters = [];
    for (const d of days) {
        const rows = [];
        for (const s of people) {
            if (s.EmploymentDate && s.EmploymentDate.slice(0, 10) > d.date) continue;
            let present = true, remarks = null, timeIn = null, timeOut = null;
            if (s.away > 0) {
                s.away--;
                present = false;
                remarks = s.reason;
            } else if (trained.has(s.Id) && trainingDays.includes(d.date)) {
                present = false;
                remarks = 'Official duty - CBC training';
            } else if (chance(s.p * (d.dow === 1 || d.dow === 5 ? 1.2 : 1))) {
                const [reason, , len] = weighted(STAFF_REASONS.map((r) => [r, r[1]]));
                present = false;
                remarks = reason;
                s.reason = reason;
                for (let i = 1; i < len; i++) if (chance(0.6)) s.away++;
            }
            if (present) {
                // Most in by half past seven; now and then someone is late.
                let arrive = s.punctual + gauss() * 8;
                if (chance(0.03)) {
                    arrive = 8 * 60 + 5 + rnd() * 40;
                    remarks = pick(['Reported late - transport', 'Reported late - hospital visit', 'Reported late']);
                }
                const leave = (d.dow === 5 ? 15 * 60 + 45 : 16 * 60 + 30) + Math.abs(gauss()) * 25;
                timeIn = time(Math.max(6 * 60 + 30, arrive));
                timeOut = time(leave);
            }
            rows.push({date: `${d.date}T00:00:00`, present, remarks, timeIn, timeOut, staffDetailsId: +s.Id});
        }
        if (rows.length) staffRegisters.push({date: d.date, rows});
    }

    // ---------- 6. Through the API ----------
    let done = 0;
    await pool(registers, 6, async (r) => {
        await api(await tok(r.teacher), 'POST', '/studentAttendances/batch', r.rows);
        if (++done % 500 === 0) console.log(`  ${done} of ${registers.length} class registers...`);
    });
    const head = await tok(opt.head);
    await pool(staffRegisters, 4, async (r) => api(head, 'POST', '/staffAttendances/batch', r.rows));

    // ---------- 7. Marked that morning ----------
    // Class registers between 08:05 and 08:45, staff at about 07:50 (Kenyan time,
    // UTC+3, stored as UTC).
    console.log('Setting the dates back...');
    exec(`
UPDATE studentattendances a JOIN studentclasses sc ON sc.Id = a.StudentClassId
   SET a.Created = a.Date + INTERVAL (5 * 60 + 5 + (sc.SchoolClassId * 7 + DAYOFYEAR(a.Date)) % 40) MINUTE, a.Modified = a.Created;
UPDATE staffattendances SET Created = Date + INTERVAL (4 * 60 + 45 + DAYOFYEAR(Date) % 15) MINUTE, Modified = Created,
   CreatedBy = ${q(opt.head)}, ModifiedBy = ${q(opt.head)};
`);

    const s = query(`SELECT (SELECT COUNT(*) FROM studentattendances) rows_, (SELECT ROUND(100 * AVG(Present), 1) FROM studentattendances) rate,
        (SELECT COUNT(*) FROM staffattendances) staffRows, (SELECT ROUND(100 * AVG(Present), 1) FROM staffattendances) staffRate`)[0];
    console.log(`Done: ${s.rows_} learner marks (${s.rate}% present), ${s.staffRows} staff marks (${s.staffRate}% present).`);
}

main().catch((err) => {
    console.error('Error:', err);
    process.exit(1);
});
