#!/usr/bin/env node
// Fills an anonymised demo copy with the weekly exams many schools give their
// candidate classes in the last term, before KPSEA and KJSEA:
//  - an exam type "Weekly Exams", kept off the report forms and allowing several
//    exams a term;
//  - eight of them in the year's last term, one a week - on the Friday, or the
//    nearest free school day in a week that holds the term's main exams - for the
//    candidate grades only (Grade 6 and Grade 9), in the subjects each class sat
//    that term, each paper out of 50;
//  - every learner's marks, entered by the subject teacher, following how they did
//    in the subject that term and creeping up week by week as the national exams
//    near. A learner the register shows absent that day has no mark. Each exam is
//    released once its marks are in.
// Everything goes through the API: the administrator sets up and releases the
// exams, the subject teachers enter the marks. Run after attendance.js (absent
// learners sit no paper).
//
//   MYSQL_PWD=... node tools/demo-data/weekly-exams.js --db shulenova --api http://localhost:5198
//
// Options: --db <database> (required; refuses names of live schools)
//          --api <API origin> (default http://localhost:5198)
//          --mysql <path to mysql.exe> (default MySQL 8.4 on Windows)
//          --user <mysql user> (default root)
//          --password <the demo logins' password> (default Demo@2026)
//          --year <academic year name> (default 2026)
//          --term <term name> (default Term 3)
//          --grades <candidate learning levels, comma-separated> (default "Grade 6,Grade 9")
//          --count <how many weekly exams> (default 8)
//          --out-of <marks per paper> (default 50)
//          --admin <user name who sets up and releases them> (default mohamed)
//          --seed <number> (default 2026)
// The Weekly Exams already in the copy (with their papers and marks) are deleted
// first, so it can be run again.

const {execFileSync} = require('child_process');

const opt = {mysql: 'C:/Program Files/MySQL/MySQL Server 8.4/bin/mysql.exe', user: 'root', api: 'http://localhost:5198',
    password: 'Demo@2026', year: '2026', term: 'Term 3', grades: 'Grade 6,Grade 9', count: '8', 'out-of': '50',
    admin: 'mohamed', seed: '2026'};
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
let seed = (+opt.seed + 83) >>> 0;
function rnd() {
    seed = (seed + 0x6d2b79f5) >>> 0;
    let t = seed;
    t = Math.imul(t ^ (t >>> 15), t | 1);
    t ^= t + Math.imul(t ^ (t >>> 7), t | 61);
    return ((t ^ (t >>> 14)) >>> 0) / 4294967296;
}
const gauss = () => Math.sqrt(-2 * Math.log(1 - rnd())) * Math.cos(2 * Math.PI * rnd());
const pad = (n) => String(n).padStart(2, '0');

// ---------- Dates ----------
const iso = (d) => `${d.getUTCFullYear()}-${pad(d.getUTCMonth() + 1)}-${pad(d.getUTCDate())}`;
const day = (s) => new Date(s.slice(0, 10) + 'T00:00:00Z');
const addDays = (s, n) => iso(new Date(day(s).getTime() + n * 86400000));
const MONTHS = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
const DAYS = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];
const label = (s) => `${DAYS[day(s).getUTCDay()]} ${+s.slice(8, 10)} ${MONTHS[+s.slice(5, 7) - 1]}`;
// Kenyan public holidays that can fall in the last term.
const HOLIDAYS = new Set(['2026-10-10', '2026-10-20']);

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

async function main() {
    const outOf = +opt['out-of'];
    const count = +opt.count;

    // ---------- 1. The term, the candidate classes and their subjects ----------
    const term = query(`SELECT s.Id, s.StartDate, s.EndDate FROM sessions s JOIN academicyears y ON y.Id = s.AcademicYearId
        WHERE y.Name = ${q(opt.year)} AND s.SessionName = ${q(opt.term)}`)[0];
    if (!term) fail(`no "${opt.term}" in ${opt.year}`);
    const grades = opt.grades.split(',').map((g) => g.trim());
    const classes = query(`SELECT c.Id, c.Name FROM schoolclasses c JOIN learninglevels ll ON ll.Id = c.LearningLevelId
        JOIN sessions s ON s.AcademicYearId = c.AcademicYearId
        WHERE s.Id = ${term.Id} AND ll.Name IN (${grades.map(q).join(', ')}) ORDER BY ll.Rank, c.Name`);
    if (!classes.length) fail(`no ${grades.join(' or ')} classes in ${opt.year}`);
    // The subjects each class sat in the term's other exams.
    const subjects = query(`SELECT DISTINCT e.SchoolClassId classId, e.SubjectId subjectId, s.Name,
            (SELECT u.UserName FROM staffsubjects ss JOIN aspnetusers u ON u.PersonId = ss.StaffDetailsId
             WHERE ss.SchoolClassId = e.SchoolClassId AND ss.SubjectId = e.SubjectId LIMIT 1) teacher
        FROM exams e JOIN schoolexams se ON se.Id = e.SchoolExamId JOIN subjects s ON s.Id = e.SubjectId
        JOIN examtypes et ON et.Id = se.ExamTypeId
        WHERE se.SessionId = ${term.Id} AND et.ShowOnReportForm AND e.SchoolClassId IN (${classes.map((c) => c.Id).join(',')})`);

    // ---------- 2. Clear earlier weekly exams ----------
    let type = query(`SELECT Id FROM examtypes WHERE Name = 'Weekly Exams'`)[0];
    if (type) {
        console.log(`Clearing earlier Weekly Exams in ${opt.db}...`);
        exec(`DELETE r FROM examresults r JOIN exams e ON e.Id = r.ExamId JOIN schoolexams se ON se.Id = e.SchoolExamId WHERE se.ExamTypeId = ${type.Id};
DELETE e FROM exams e JOIN schoolexams se ON se.Id = e.SchoolExamId WHERE se.ExamTypeId = ${type.Id};
DELETE FROM schoolexams WHERE ExamTypeId = ${type.Id};`);
    }

    // ---------- 3. The exam type ----------
    const admin = await tok(opt.admin);
    if (!type) {
        const rank = +(query(`SELECT COALESCE(MAX(\`Rank\`), 0) + 1 r FROM examtypes`)[0].r);
        await api(admin, 'POST', '/examTypes', {
            name: 'Weekly Exams', abbreviation: 'WKLY', rank, showOnReportForm: false, allowMultiplePerTerm: true,
            description: 'Weekly tests for the candidate classes (Grade 6 and Grade 9) in the last term, before KPSEA and KJSEA. Not on the report forms.'
        });
        type = query(`SELECT Id FROM examtypes WHERE Name = 'Weekly Exams'`)[0];
    }

    // ---------- 4. When: one a week, on the Friday if it is free ----------
    // Days the term's main exams take, the term's school days, then for each week the
    // Friday or the latest free school day; a week with none is skipped.
    const busy = new Set();
    for (const e of query(`SELECT se.ExamStartDate s, se.ExamEndDate e FROM schoolexams se JOIN examtypes et ON et.Id = se.ExamTypeId
        WHERE se.SessionId = ${term.Id} AND et.Id <> ${type.Id}`)) {
        for (let d = e.s.slice(0, 10); d <= e.e.slice(0, 10); d = addDays(d, 1)) busy.add(d);
    }
    const weeks = new Map();
    for (let d = term.StartDate.slice(0, 10); d <= term.EndDate.slice(0, 10); d = addDays(d, 1)) {
        const dow = day(d).getUTCDay();
        if (dow === 0 || dow === 6 || HOLIDAYS.has(d) || busy.has(d)) continue;
        const monday = addDays(d, -((dow + 6) % 7));
        (weeks.get(monday) ?? weeks.set(monday, []).get(monday)).push(d);
    }
    const dates = [...weeks.values()].map((free) => free.find((d) => day(d).getUTCDay() === 5) ?? free[free.length - 1]).slice(0, count);
    if (dates.length < count) console.log(`  (only ${dates.length} weeks of ${opt.term} have a free day)`);
    console.log(`Weekly exams on ${dates.map(label).join(', ')}; ${classes.map((c) => c.Name).join(' and ')}.`);

    // ---------- 5. Ability: how each learner did in each subject this term ----------
    const pct = query(`SELECT r.StudentId s, e.SubjectId subj, AVG(100 * r.Score / NULLIF(e.ExamMark, 0)) p
        FROM examresults r JOIN exams e ON e.Id = r.ExamId JOIN schoolexams se ON se.Id = e.SchoolExamId
        WHERE se.SessionId = ${term.Id} AND se.ExamTypeId <> ${type.Id} AND e.SchoolClassId IN (${classes.map((c) => c.Id).join(',')})
        GROUP BY r.StudentId, e.SubjectId`);
    const ability = {}, overall = {};
    for (const r of pct) {
        ability[`${r.s}|${r.subj}`] = +r.p;
        (overall[r.s] ??= []).push(+r.p);
    }
    const learnerMean = (s) => overall[s] ? overall[s].reduce((x, y) => x + y, 0) / overall[s].length : 55;
    // Who takes each subject in each class, and who was away on each day.
    const takers = query(`SELECT sc.SchoolClassId classId, ss.SubjectId subjectId, sc.StudentId studentId
        FROM studentsubjects ss JOIN studentclasses sc ON sc.Id = ss.StudentClassId JOIN person p ON p.Id = sc.StudentId
        WHERE sc.SchoolClassId IN (${classes.map((c) => c.Id).join(',')}) AND p.Status = 0`);
    const absent = new Set(query(`SELECT sc.StudentId s, DATE(a.Date) d FROM studentattendances a JOIN studentclasses sc ON sc.Id = a.StudentClassId
        WHERE a.Present = 0 AND sc.SchoolClassId IN (${classes.map((c) => c.Id).join(',')})`).map((r) => `${r.s}|${r.d}`));
    // Each learner's own drift over the weeks: most improve as the national exams near.
    const drift = {};

    // ---------- 6. Set up, marked, released ----------
    let papers = 0, marks = 0, missed = 0;
    for (const [i, date] of dates.entries()) {
        const n = i + 1;
        const se = await api(admin, 'POST', '/schoolExams', {
            examStartDate: date, examEndDate: date, examMarkEntryEndDate: addDays(date, 4),
            description: `Weekly Exam ${n} - ${label(date)}`, examTypeId: +type.Id, sessionId: +term.Id
        });
        for (const c of classes) {
            for (const s of subjects.filter((x) => +x.classId === +c.Id)) {
                const exam = await api(admin, 'POST', '/exams', {
                    examMark: outOf, schoolExamId: se.id, schoolClassId: +c.Id, subjectId: +s.subjectId, sessionId: +term.Id,
                    examTypeId: +type.Id, examStartDate: date, examEndDate: date, examMarkEntryEndDate: addDays(date, 4),
                    description: `Weekly Exam ${n}`
                });
                papers++;
                const rows = [];
                for (const t of takers.filter((x) => +x.classId === +c.Id && +x.subjectId === +s.subjectId)) {
                    if (absent.has(`${t.studentId}|${date}`)) {
                        missed++;
                        continue;
                    }
                    const base = ability[`${t.studentId}|${s.subjectId}`] ?? learnerMean(t.studentId);
                    const own = drift[t.studentId] ??= 0.4 + gauss() * 0.6;
                    // A weekly test runs a little below the term's exams at first; learners
                    // gain about a mark a week on it as revision goes on.
                    const p = Math.max(2, Math.min(100, base - 5 + n * (0.9 + own) + gauss() * 7));
                    rows.push({studentId: +t.studentId, examId: exam.id, score: Math.round((p * outOf) / 100), description: null});
                }
                if (rows.length) await api(await tok(s.teacher || opt.admin), 'POST', '/examResults/batch', rows);
                marks += rows.length;
            }
        }
        await api(admin, 'POST', `/schoolExams/${se.id}/release?release=true`);
        console.log(`  Weekly Exam ${n} (${label(date)}): marked and released.`);
    }

    // ---------- 7. Dated as they happened ----------
    // Set up the week before, marked over the next days, released when marking closed.
    console.log('Setting the dates back...');
    exec(`
UPDATE schoolexams SET Created = DATE_SUB(ExamStartDate, INTERVAL 6 DAY) + INTERVAL 7 HOUR, Modified = Created,
    ReleasedDate = ExamMarkEntryEndDate + INTERVAL 13 HOUR, ReleasedBy = ${q(opt.admin)} WHERE ExamTypeId = ${type.Id};
UPDATE exams e JOIN schoolexams se ON se.Id = e.SchoolExamId SET e.Created = se.Created + INTERVAL 20 MINUTE, e.Modified = e.Created
 WHERE se.ExamTypeId = ${type.Id};
UPDATE examresults r JOIN exams e ON e.Id = r.ExamId JOIN schoolexams se ON se.Id = e.SchoolExamId
   SET r.Created = se.ExamStartDate + INTERVAL (1 + e.SubjectId % 3) DAY + INTERVAL (11 + e.SubjectId % 5) HOUR + INTERVAL (r.Id % 50) SECOND,
       r.Modified = r.Created
 WHERE se.ExamTypeId = ${type.Id};
`);
    console.log(`Done: ${dates.length} weekly exams, ${papers} papers, ${marks} marks (${missed} missed by learners absent that day).`);
}

main().catch((err) => {
    console.error('Error:', err);
    process.exit(1);
});
