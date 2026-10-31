#!/usr/bin/env node
// Fills an anonymised demo copy with a year of teaching documents: for every
// teacher's subject allocation, each term's scheme of work (laid out from the
// KICD curriculum, the next term carrying on where the last stopped, spare
// weeks given to revision and assessment), a lesson plan for every lesson of
// every week, the record of work, and the head of department's weekly check.
// Schemes and lesson plan weeks are submitted by the teacher and approved by
// the head of the subject's department, through the real workflow. Everything
// goes through the API as the teachers and heads of department themselves.
//
// The last term is left as a term really ends: a few teachers' final week of
// plans not yet written, a few waiting on their head of department.
//
// Run it with the API running on the same database:
//
//   MYSQL_PWD=... node tools/demo-data/teaching.js --db shulenova --api http://localhost:5198 --through 2026-11-05
//
// Options: --db <database> (required; refuses names of live schools)
//          --api <API origin> (default http://localhost:5198)
//          --mysql <path to mysql.exe> (default MySQL 8.4 on Windows)
//          --user <mysql user> (default root)
//          --password <the demo logins' password> (default Demo@2026)
//          --through <yyyy-mm-dd> (terms that have started by then are filled;
//                    default two weeks from today)
//          --seed <number> (default 2026)
// The teaching documents already in the copy are deleted first, so it can be
// run again. Allocations whose subject and grade have no curriculum loaded (the
// special unit, day care, sign language) are left without schemes.

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
let seed = (+opt.seed + 11) >>> 0;
function rnd() {
    seed = (seed + 0x6d2b79f5) >>> 0;
    let t = seed;
    t = Math.imul(t ^ (t >>> 15), t | 1);
    t ^= t + Math.imul(t ^ (t >>> 7), t | 61);
    return ((t ^ (t >>> 14)) >>> 0) / 4294967296;
}
const pick = (list) => list[Math.floor(rnd() * list.length)];
const between = (lo, hi) => lo + Math.floor(rnd() * (hi - lo + 1));
const chance = (p) => rnd() < p;

// ---------- Dates ----------
const day = (s) => new Date(s.slice(0, 10) + 'T00:00:00');
const iso = (d) => `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
const addDays = (d, n) => new Date(d.getFullYear(), d.getMonth(), d.getDate() + n);
const today = day(iso(new Date()));
const through = opt.through ? day(opt.through) : addDays(today, 14);
// Week 1 starts on the Monday of the week the term opens in (as the app has it).
const week1 = (start) => addDays(start, -((start.getDay() + 6) % 7));

// ---------- API ----------
async function api(token, method, url, body) {
    const r = await fetch(opt.api + '/api' + url, {method,
        headers: {'Content-Type': 'application/json', ...(token ? {Authorization: `Bearer ${token}`} : {})},
        body: body === undefined ? undefined : JSON.stringify(body)});
    const text = await r.text();
    if (!r.ok) {
        const e = new Error(`${method} ${url} -> ${r.status} ${text.slice(0, 300)}`);
        e.status = r.status;
        e.body = text;
        throw e;
    }
    return text ? JSON.parse(text) : null;
}
const tokens = {};
const tok = async (userName) => tokens[userName] ??= (await api(null, 'POST', '/auth/login', {userName, password: opt.password})).token;

// Runs fn over items, a few at a time.
async function pool(items, size, fn) {
    let next = 0;
    await Promise.all(Array.from({length: size}, async () => {
        while (next < items.length) await fn(items[next++]);
    }));
}

// ---------- What teachers write ----------
const PUBLISHERS = ['KLB', 'Longhorn', 'Oxford', 'Moran', 'Spotlight', 'Mentor'];
const ORGANISATION = [
    'Learners seated in groups of five, sharing the learning resources.',
    'Whole class first, then learners work in pairs.',
    'Learners in groups of four; each group reports back to the class.',
    'Learners at their desks, then in mixed-ability groups for the activity.',
];
const ORGANISATION_EARLY = [
    'Learners seated in a circle on the mat.',
    'Learners in small groups at activity corners.',
    'Whole class on the mat, then learners at their tables in groups.',
];
const ASSESSMENT = [
    'Oral questions; observation',
    'Oral questions; written exercise',
    'Observation; checklist',
    'Written exercise; peer assessment',
    'Oral questions; project work; portfolio',
    'Observation; oral questions; self-assessment',
];
const CONCLUSION = [
    (s) => `Learners summarise what they have learnt about ${s}; the teacher clears up any misconceptions.`,
    (s) => `A short oral quiz on ${s}; learners say one thing they found interesting.`,
    (s) => `Groups present their work on ${s}; the teacher gives feedback and summarises the key points.`,
    (s) => `Learners write down two key points on ${s} in their exercise books.`,
];
const EXTENDED = [
    (s) => `Practise ${s} at home with a parent or guardian.`,
    (s) => `Find an example of ${s} in the home or community and share it in the next lesson.`,
    (s) => `Complete the exercise on ${s} in the learner's book.`,
    () => '',
];
const NOT_TAUGHT = ['Sports day.', 'Teacher attending a CBC training workshop.', 'Mid-term assessment.', 'Prize-giving day preparations.',
    'Teacher on sick leave; class given work.', 'County music festival rehearsals.'];
const PARTLY = ['Lesson cut short by the assembly; to be completed next lesson.', 'Most learners needed more time; continued in the next lesson.',
    'Power cut affected the demonstration; finished the next day.'];
const CHECK_COMMENTS = ['', '', '', 'Good coverage.', 'Well kept.', 'Ensure the work covered is filled in for every lesson.',
    'Good progress - keep it up.', 'Catch up on the lessons missed.'];
const APPROVE = ['Approved.', 'Approved - well planned.', 'OK.', 'Approved. Good use of the curriculum design.', ''];

// Daily timetable slots (start times), with the break and lunch between them.
const SLOTS = ['08:00', '08:40', '09:20', '10:30', '11:10', '11:50', '14:00', '14:40', '15:20'];
// Lesson length by grade: pre-primary and lower primary 30, upper primary 35, junior school 40 minutes.
const minutesFor = (levelRank) => (levelRank <= 5 || levelRank >= 12 ? 30 : levelRank <= 8 ? 35 : 40);
const addMinutes = (hhmm, m) => {
    const [h, mm] = hhmm.split(':').map(Number);
    const t = h * 60 + mm + m;
    return `${String(Math.floor(t / 60)).padStart(2, '0')}:${String(t % 60).padStart(2, '0')}`;
};
// The words a teacher writes around the curriculum's own: English, or Kiswahili for the Kiswahili subjects.
const WORDING = {
    en: {
        assessment: {strand: 'Assessment', sub: 'End of term assessment', written: 'Written assessment',
            outcomes: 'By the end of the lesson, the learner should be able to:\n- demonstrate the competencies acquired during the term',
            experiences: 'The learner is guided to:\n- attempt the end of term assessment tasks\n- discuss the answers with the teacher'},
        revision: {strand: 'Revision', sub: (x) => `Revision: ${x}`,
            outcomes: 'By the end of the lesson, the learner should be able to:\n- apply what was learnt earlier in the term',
            experiences: 'The learner is guided to:\n- work through revision exercises in groups\n- share answers with the class'},
        assessmentMethods: ASSESSMENT, organisation: ORGANISATION, organisationEarly: ORGANISATION_EARLY,
        intro: (qn, sub) => (qn ? `Review the previous lesson, then pose the question: "${qn}"` : `Review the previous lesson and introduce ${sub}.`),
        conclusion: CONCLUSION, extended: EXTENDED,
    },
    sw: {
        assessment: {strand: 'Tathmini', sub: 'Tathmini ya mwisho wa muhula', written: 'Tathmini ya maandishi',
            outcomes: 'Kufikia mwisho wa kipindi, mwanafunzi aweze:\n- kuonyesha umilisi aliopata katika muhula',
            experiences: 'Mwanafunzi aongozwe:\n- kufanya kazi za tathmini ya mwisho wa muhula\n- kujadili majibu pamoja na mwalimu'},
        revision: {strand: 'Marudio', sub: (x) => `Marudio: ${x}`,
            outcomes: 'Kufikia mwisho wa kipindi, mwanafunzi aweze:\n- kutumia aliyojifunza awali katika muhula',
            experiences: 'Mwanafunzi aongozwe:\n- kufanya mazoezi ya marudio katika vikundi\n- kueleza majibu yao darasani'},
        assessmentMethods: ['Maswali ya mdomo; uchunguzi', 'Zoezi la kuandika; maswali ya mdomo', 'Uchunguzi; orodha hakiki',
            'Zoezi la kuandika; tathmini ya wenzao'],
        organisation: ['Wanafunzi wameketi katika vikundi vya watano wakichangia vifaa.', 'Darasa zima, kisha wanafunzi wawili wawili.',
            'Wanafunzi katika vikundi vya wanne; kila kikundi kinawasilisha kazi yake.'],
        organisationEarly: ['Wanafunzi wameketi kwenye mkeka kwa duara.', 'Wanafunzi katika vikundi vidogo kwenye kona za shughuli.'],
        intro: (qn, sub) => (qn ? `Kukumbusha somo lililopita, kisha kuuliza swali: "${qn}"` : `Kukumbusha somo lililopita na kutambulisha ${sub}.`),
        conclusion: [(x) => `Wanafunzi wanafupisha waliyojifunza kuhusu ${x}; mwalimu anarekebisha makosa.`,
            (x) => `Maswali mafupi ya mdomo kuhusu ${x}; wanafunzi wanataja jambo moja walilopenda.`,
            (x) => `Vikundi vinawasilisha kazi yao kuhusu ${x}; mwalimu anatoa maoni.`],
        extended: [(x) => `Kujizoeza ${x} nyumbani pamoja na mzazi au mlezi.`, (x) => `Kufanya zoezi kuhusu ${x} katika kitabu cha mwanafunzi.`, () => ''],
    },
};
const wordingFor = (subjectName) => WORDING[/kiswahili/i.test(subjectName) ? 'sw' : 'en'];
const firstLine = (s) => (s || '').split('\n').map((x) => x.trim()).find((x) => x && !x.endsWith(':')) || '';

async function main() {
    if (!query('SELECT COUNT(*) n FROM person')[0]) fail(`database ${opt.db} not found`);
    const year = query(`SELECT Id, Name FROM academicyears WHERE StartDate <= '${iso(today)}' AND EndDate >= '${iso(today)}'`)[0];
    if (!year) fail('no academic year covers today');
    const terms = query(`SELECT Id, SessionName, StartDate, EndDate FROM sessions WHERE AcademicYearId = ${year.Id}
        AND StartDate <= '${iso(through)}' ORDER BY StartDate`).map((s) => ({id: +s.Id, name: `${s.SessionName} ${year.Name}`,
        start: day(s.StartDate), end: day(s.EndDate)}));
    if (!terms.length) fail('no term has started');

    // ---------- 1. Who teaches what, and who approves it ----------
    const allocations = query(`SELECT a.StaffDetailsId staff, a.SubjectId subject, a.SchoolClassId cls, s.Name subjectName, s.DepartmentId dept,
            c.Name className, l.Name levelName, l.\`Rank\` levelRank, c.LearningLevelId level, u.UserName teacher,
            (SELECT COUNT(DISTINCT x.StudentId) FROM studentclasses x JOIN person p ON p.Id = x.StudentId WHERE x.SchoolClassId = c.Id AND p.Status = 0) roll
        FROM staffsubjects a JOIN subjects s ON s.Id = a.SubjectId JOIN schoolclasses c ON c.Id = a.SchoolClassId
        JOIN learninglevels l ON l.Id = c.LearningLevelId JOIN aspnetusers u ON u.PersonId = a.StaffDetailsId
        WHERE c.AcademicYearId = ${year.Id}
          AND EXISTS (SELECT 1 FROM strands st JOIN substrands ss ON ss.StrandId = st.Id WHERE st.SubjectId = a.SubjectId AND st.LearningLevelId = c.LearningLevelId)
        ORDER BY u.UserName, l.\`Rank\`, c.Name, s.\`Rank\``).map((r) => ({...r, staff: +r.staff, subject: +r.subject, cls: +r.cls,
        dept: +r.dept, level: +r.level, levelRank: +r.levelRank, roll: Math.max(+r.roll, between(18, 32))}));
    if (!allocations.length) fail('no subject allocations with curriculum for this year');
    const hodOf = Object.fromEntries(query(`SELECT d.Id, u.UserName FROM departments d JOIN aspnetusers u ON u.PersonId = d.StaffDetailsId`)
        .map((r) => [+r.Id, r.UserName]));
    const hods = [...new Set(query(`SELECT u.UserName FROM aspnetusers u JOIN aspnetuserroles ur ON ur.UserId = u.Id
        JOIN aspnetroles r ON r.Id = ur.RoleId WHERE r.Name = 'HeadOfDepartment' ORDER BY u.Id`).map((r) => r.UserName))];
    const admins = query(`SELECT u.UserName FROM aspnetusers u JOIN aspnetuserroles ur ON ur.UserId = u.Id JOIN aspnetroles r ON r.Id = ur.RoleId
        WHERE r.Name = 'Administrator' AND u.UserName <> 'admin' ORDER BY u.Id`).map((r) => r.UserName);
    if (!hods.length || !admins.length) fail('need HeadOfDepartment and Administrator logins');
    const uid = Object.fromEntries(query('SELECT Id, UserName FROM aspnetusers').map((r) => [r.UserName, +r.Id]));
    // The approver is the head of the subject's department - or, for a head of department's own subject,
    // another head of department (the maker cannot approve their own document).
    const approverFor = (al) => {
        const hod = hodOf[al.dept];
        return hod && hod !== al.teacher ? hod : hods.find((h) => h !== al.teacher);
    };
    // The record of work is checked by the department head, or an administrator for a head's own subject.
    const checkerFor = (al) => {
        const hod = hodOf[al.dept];
        return hod && hod !== al.teacher ? hod : admins.find((x) => x !== al.teacher);
    };
    console.log(`${allocations.length} allocations with curriculum, ${new Set(allocations.map((x) => x.teacher)).size} teachers, `
        + `${terms.length} term(s).`);

    // ---------- 2. Clear the teaching documents ----------
    console.log(`Clearing teaching documents in ${opt.db}...`);
    exec(`SET FOREIGN_KEY_CHECKS = 0;
DELETE a FROM approvalstepactions a JOIN approvalrequests r ON r.Id = a.ApprovalRequestId WHERE r.EntityType IN ('SchemeOfWork', 'LessonPlanWeek');
DELETE FROM approvalrequests WHERE EntityType IN ('SchemeOfWork', 'LessonPlanWeek');
DELETE FROM recordofworkchecks; DELETE FROM recordofworkentries; DELETE FROM lessonplansteps; DELETE FROM lessonplans;
DELETE FROM lessonplanweeks; DELETE FROM schemeofworklessons; DELETE FROM schemesofwork;
SET FOREIGN_KEY_CHECKS = 1;`);
    for (const u of new Set([...allocations.map((x) => x.teacher), ...hods, ...admins])) await tok(u);

    // ---------- 3. The terms ----------
    // The curriculum's lessons for each subject and grade, and the year's weeks, to share them between the terms.
    const curriculumLessons = Object.fromEntries(query(`SELECT st.SubjectId, st.LearningLevelId, SUM(GREATEST(ss.LessonNo, 1)) n
        FROM strands st JOIN substrands ss ON ss.StrandId = st.Id GROUP BY st.SubjectId, st.LearningLevelId`)
        .map((r) => [`${r.SubjectId}|${r.LearningLevelId}`, +r.n]));
    const allTerms = query(`SELECT StartDate, EndDate FROM sessions WHERE AcademicYearId = ${year.Id}`).map((s) => ({start: day(s.StartDate), end: day(s.EndDate)}));
    const termWeeks = (t) => Math.max(1, Math.ceil(((t.end - week1(t.start)) / 86400000 + 1) / 7));
    const yearWeeks = allTerms.reduce((s, t) => s + termWeeks(t), 0);
    const yearTaught = {}; // subject|grade -> sub-strands taught so far this year
    const busy = new Map(); // teacher|date -> slots taken, so a teacher is never in two classes at once
    const stats = {schemes: 0, weeks: 0, plans: 0, pending: 0, missing: 0, recorded: 0, checks: 0};
    const lastTerm = terms[terms.length - 1];
    for (const term of terms) {
        const termEnded = term.end <= through;
        await pool(allocations, 6, async (al) => {
            const T = tokens[al.teacher];
            const approver = approverFor(al), checker = checkerFor(al);

            // The scheme: created, laid out from the curriculum, finished by hand, submitted and approved.
            const {id} = await api(T, 'POST', '/schemesOfWork', {subjectId: al.subject, schoolClassId: al.cls, sessionId: term.id});
            let scheme = await api(T, 'GET', `/schemesOfWork/${id}`);
            const references = `${pick(PUBLISHERS)} ${al.subjectName} ${al.levelName} Learner's Book and Teacher's Guide; KICD curriculum design`;
            scheme = await api(T, 'PUT', '/schemesOfWork', {id, references, numberOfWeeks: scheme.numberOfWeeks,
                lessonsPerWeek: scheme.lessonsPerWeek, lessons: scheme.lessons});
            try {
                scheme = await api(T, 'POST', `/schemesOfWork/${id}/generate`, {replace: true});
            } catch (e) {
                if (e.status !== 400) throw e; // the year's curriculum already covered: the term is revision
            }
            // The year's curriculum is shared between the terms by their length, cut at whole sub-strands; the
            // generator would otherwise give the first terms all of it. What a term leaves goes to the next
            // (the generator skips only sub-strands an earlier term's scheme has). The last term takes the rest.
            const yearKey = `${al.subject}|${al.level}`;
            if (term !== lastTerm) {
                const share = Math.ceil((curriculumLessons[yearKey] || 0) * termWeeks(term) / yearWeeks);
                let kept = 0, cut = false;
                for (const subId of [...new Set(scheme.lessons.filter((l) => l.subStrandId).map((l) => l.subStrandId))]) {
                    const n = scheme.lessons.filter((l) => l.subStrandId === subId).length;
                    if (cut || (kept > 0 && kept + n > share)) {
                        cut = true;
                        for (const l of scheme.lessons.filter((x) => x.subStrandId === subId))
                            Object.assign(l, {strandId: null, subStrandId: null, strandName: null, subStrandName: null, learningOutcomes: null,
                                learningExperiences: null, keyInquiryQuestions: null, learningResources: null});
                    } else kept += n;
                }
            }
            const taught = scheme.lessons.filter((l) => l.subStrandName).map((l) => l.subStrandName);
            (yearTaught[yearKey] ??= []).push(...taught.filter((x) => !yearTaught[yearKey].includes(x)));
            // Revision is of this term's work, or for a term with none left, of the year's.
            const revise = [...new Set(taught.length ? taught : yearTaught[yearKey].length ? yearTaught[yearKey] : [al.subjectName])];
            const say = wordingFor(al.subjectName);
            let r = 0;
            for (const l of scheme.lessons) {
                if (l.weekNo === scheme.numberOfWeeks) {
                    l.strandName = say.assessment.strand;
                    l.subStrandName = say.assessment.sub;
                    l.learningOutcomes = say.assessment.outcomes;
                    l.learningExperiences = say.assessment.experiences;
                } else if (!l.subStrandName) {
                    l.strandName = say.revision.strand;
                    l.subStrandName = say.revision.sub(revise[r++ % revise.length]);
                    l.learningOutcomes = say.revision.outcomes;
                    l.learningExperiences = say.revision.experiences;
                }
                l.learningResources ||= references;
                l.assessmentMethods = l.weekNo === scheme.numberOfWeeks ? say.assessment.written : pick(say.assessmentMethods);
            }
            scheme = await api(T, 'PUT', '/schemesOfWork', {id, references, numberOfWeeks: scheme.numberOfWeeks,
                lessonsPerWeek: scheme.lessonsPerWeek, lessons: scheme.lessons});
            const req = await api(T, 'POST', '/approvalRequests/submit', {entityType: 'SchemeOfWork', entityId: id, formKey: 'SchemeOfWork',
                stepAssignments: [{stepRank: 1, assignedToUserId: uid[approver]}]});
            await api(tokens[approver], 'POST', `/approvalRequests/${req.id}/action`, {action: 'approve', comment: pick(APPROVE)});
            stats.schemes++;

            // Lesson plans, week by week; then what was taught.
            const start = week1(term.start);
            const duration = minutesFor(al.levelRank);
            const entries = [];
            const recordedWeeks = [];
            for (let w = 1; w <= scheme.numberOfWeeks; w++) {
                const weekStart = addDays(start, 7 * (w - 1));
                if (weekStart > through) break;
                const last = term === lastTerm && w === scheme.numberOfWeeks;
                // The end of the last term: a few final weeks not yet written, a few waiting on the head of department.
                const skip = last ? chance(0.15) : chance(0.03);
                const lessons = scheme.lessons.filter((l) => l.weekNo === w);
                const dates = new Map();
                if (skip) {
                    stats.missing++;
                } else {
                    const {id: weekId} = await api(T, 'POST', '/lessonPlans/weeks', {schemeOfWorkId: id, weekNo: w});
                    const week = await api(T, 'GET', `/lessonPlans/weeks/${weekId}`);
                    for (const p of week.plans) {
                        // A last week cut short by the end of term keeps only the lessons on the days it still has.
                        let date = (p.lessonDate || iso(weekStart)).slice(0, 10);
                        if (date > iso(term.end)) {
                            await api(T, 'DELETE', `/lessonPlans/plans/${p.id}`);
                            continue;
                        }
                        // The teacher's first free slot that day, or later that week if the day is full.
                        let slot = -1;
                        for (let d = day(date); slot < 0 && d <= addDays(weekStart, 4) && d <= term.end; d = addDays(d, 1)) {
                            const taken = busy.get(`${al.teacher}|${iso(d)}`) || new Set();
                            slot = SLOTS.findIndex((_, i) => !taken.has(i));
                            if (slot >= 0) date = iso(d);
                        }
                        const key = `${al.teacher}|${date}`;
                        busy.set(key, (busy.get(key) || new Set()).add(Math.max(slot, 0)));
                        const startTime = SLOTS[Math.max(slot, 0)];
                        const sub = p.subStrandName || al.subjectName;
                        const qn = firstLine(p.keyInquiryQuestions);
                        const intro = say.intro(qn, sub);
                        await api(T, 'PUT', `/lessonPlans/plans/${p.id}`, {...p, lessonDate: date, startTime, endTime: addMinutes(startTime, duration),
                            roll: al.roll, organisationOfLearning: pick(al.levelRank <= 3 || al.levelRank >= 12 ? say.organisationEarly : say.organisation),
                            introduction: intro, introductionMinutes: 5, developmentMinutes: duration - 10,
                            conclusion: pick(say.conclusion)(sub), conclusionMinutes: 5, extendedActivities: pick(say.extended)(sub)});
                        if (p.schemeOfWorkLessonId) dates.set(p.schemeOfWorkLessonId, date);
                        stats.plans++;
                    }
                    const wreq = await api(T, 'POST', '/approvalRequests/submit', {entityType: 'LessonPlanWeek', entityId: weekId,
                        formKey: 'LessonPlanWeek', stepAssignments: [{stepRank: 1, assignedToUserId: uid[approver]}]});
                    if (last && chance(0.12)) stats.pending++;
                    else await api(tokens[approver], 'POST', `/approvalRequests/${wreq.id}/action`, {action: 'approve', comment: pick(APPROVE)});
                    stats.weeks++;
                }
                // The record of work, once the week is over.
                if (addDays(weekStart, 4) > through || (!termEnded && addDays(weekStart, 4) > today)) continue;
                for (const [i, l] of lessons.entries()) {
                    // Lessons that would fall after the term ends (a last week cut short) were never taught.
                    const taughtOn = dates.get(l.id) || iso(addDays(weekStart, Math.min(4, Math.floor(i * 5 / Math.max(lessons.length, 1)))));
                    if (taughtOn > iso(term.end) || (!skip && !dates.has(l.id))) continue;
                    const roll = rnd();
                    const status = roll < 0.88 ? 1 : roll < 0.95 ? 2 : 3;
                    entries.push({schemeOfWorkLessonId: l.id, status, taughtOn: status === 3 ? null : taughtOn,
                        workCovered: status === 3 ? null : l.subStrandName,
                        remarks: status === 3 ? pick(NOT_TAUGHT) : status === 2 ? pick(PARTLY) : (chance(0.1) ? 'Learners did well.' : null)});
                }
                recordedWeeks.push(w);
            }
            if (entries.length) {
                await api(T, 'PUT', `/recordOfWork/entries?schemeOfWorkId=${id}`, entries);
                stats.recorded += entries.length;
                for (const w of recordedWeeks) {
                    if (!chance(0.9)) continue;
                    try {
                        await api(tokens[checker], 'POST', '/recordOfWork/checks', {schemeOfWorkId: id, weekNo: w, comment: pick(CHECK_COMMENTS)});
                        stats.checks++;
                    } catch (e) {
                        if (e.status !== 400) throw e; // a week with nothing taught has nothing to check
                    }
                }
            }
        });
        console.log(`${term.name}: ${stats.schemes} schemes, ${stats.weeks} weeks of plans (${stats.plans} lessons), `
            + `${stats.recorded} lessons recorded, ${stats.checks} weeks checked so far.`);
    }

    // ---------- 4. Make it look written on the day ----------
    // Schemes in the week before term, approved within a couple of days; each week's plans the Friday
    // before, approved over the weekend; lessons recorded the afternoon they were taught; checks on Friday.
    exec(`
UPDATE schemesofwork s JOIN sessions t ON t.Id = s.SessionId
    SET s.Created = t.StartDate - INTERVAL (3 + s.Id % 6) DAY + INTERVAL (9 + s.Id % 8) HOUR, s.Modified = s.Created;
UPDATE schemeofworklessons l JOIN schemesofwork s ON s.Id = l.SchemeOfWorkId SET l.Created = s.Created, l.Modified = s.Created;
UPDATE approvalrequests r JOIN schemesofwork s ON r.EntityType = 'SchemeOfWork' AND r.EntityId = s.Id
    SET r.SubmittedAt = s.Created + INTERVAL 2 HOUR + INTERVAL (s.Id % 50) MINUTE, r.Created = r.SubmittedAt, r.Modified = r.SubmittedAt;
UPDATE lessonplanweeks w JOIN schemesofwork s ON s.Id = w.SchemeOfWorkId JOIN sessions t ON t.Id = s.SessionId
    SET w.Created = t.StartDate - INTERVAL WEEKDAY(t.StartDate) DAY + INTERVAL (7 * (w.WeekNo - 1) - 3) DAY + INTERVAL (14 + w.Id % 4) HOUR,
        w.Modified = w.Created;
UPDATE lessonplans p JOIN lessonplanweeks w ON w.Id = p.LessonPlanWeekId SET p.Created = w.Created, p.Modified = w.Created;
UPDATE lessonplansteps x JOIN lessonplans p ON p.Id = x.LessonPlanId SET x.Created = p.Created, x.Modified = p.Created;
UPDATE approvalrequests r JOIN lessonplanweeks w ON r.EntityType = 'LessonPlanWeek' AND r.EntityId = w.Id
    SET r.SubmittedAt = w.Created + INTERVAL 1 HOUR + INTERVAL (w.Id % 40) MINUTE, r.Created = r.SubmittedAt, r.Modified = r.SubmittedAt;
UPDATE approvalstepactions a JOIN approvalrequests r ON r.Id = a.ApprovalRequestId
    SET a.Created = r.SubmittedAt, a.ActionedAt = IF(a.ActionedAt IS NULL, NULL,
        IF(r.EntityType = 'SchemeOfWork', r.SubmittedAt + INTERVAL (20 + a.Id % 40) HOUR, r.SubmittedAt + INTERVAL (18 + a.Id % 40) HOUR)),
        a.Modified = COALESCE(a.ActionedAt, r.SubmittedAt)
    WHERE r.EntityType IN ('SchemeOfWork', 'LessonPlanWeek');
UPDATE recordofworkentries e SET e.Created = COALESCE(e.TaughtOn, e.Created) + INTERVAL 15 HOUR + INTERVAL (e.Id % 90) MINUTE, e.Modified = e.Created
    WHERE e.TaughtOn IS NOT NULL;
UPDATE recordofworkchecks c JOIN schemesofwork s ON s.Id = c.SchemeOfWorkId JOIN sessions t ON t.Id = s.SessionId
    SET c.CheckedAt = LEAST(t.StartDate - INTERVAL WEEKDAY(t.StartDate) DAY + INTERVAL (7 * (c.WeekNo - 1) + 4) DAY, t.EndDate)
        + INTERVAL (15 + c.Id % 3) HOUR + INTERVAL (c.Id % 60) MINUTE,
        c.Created = c.CheckedAt, c.Modified = c.CheckedAt;
DELETE FROM auditlogs;
`);

    console.log(`Done. ${stats.schemes} schemes approved, ${stats.weeks} weeks of lesson plans (${stats.plans} lessons; `
        + `${stats.pending} waiting on the HOD, ${stats.missing} not written), ${stats.recorded} lessons in the record of work, `
        + `${stats.checks} weeks checked.`);
}

main().catch((e) => fail(e.stack || e.message));
