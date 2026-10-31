#!/usr/bin/env node
// Fills an anonymised demo copy with a year of CBE records beside the exams:
//  - formative assessments: every specific outcome of the sub-strands each term's
//    scheme of work covered, graded for every learner taking the subject, by the
//    subject teacher in the week the sub-strand was finished, with the sub-strand
//    and strand grades worked out from them as the Assessments page does. A
//    learner's grades follow their exam marks in the subject that term, so the
//    formative and summative pictures agree;
//  - values, rated by the class teacher at the end of each term;
//  - responsibilities and social skills for the year;
//  - community service: each class's project every term, and a few learners' own;
//  - co-curricular activities (clubs and sports), recorded by each activity's
//    patron, with participation, achievement and effort scores.
// Everything goes through the API as the teachers themselves. Run teaching.js
// first: the assessments follow its schemes of work.
//
//   MYSQL_PWD=... node tools/demo-data/cbe.js --db shulenova --api http://localhost:5198 --through 2026-11-05
//
// Options: --db <database> (required; refuses names of live schools)
//          --api <API origin> (default http://localhost:5198)
//          --mysql <path to mysql.exe> (default MySQL 8.4 on Windows)
//          --user <mysql user> (default root)
//          --password <the demo logins' password> (default Demo@2026)
//          --through <yyyy-mm-dd> (assessments up to then; values and community
//                    service for the terms ended by then; default today)
//          --seed <number> (default 2026)
// The learners' CBE records already in the copy are deleted first, so it can be
// run again; responsibilities recorded from elections are kept. Values are
// switched on in the term report card.

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

// ---------- Random, repeatable ----------
let seed = (+opt.seed + 23) >>> 0;
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
// Picks from [item, weight] pairs.
const weighted = (pairs) => {
    let r = rnd() * pairs.reduce((s, [, w]) => s + w, 0);
    for (const [x, w] of pairs) if ((r -= w) < 0) return x;
    return pairs[pairs.length - 1][0];
};

// ---------- Dates ----------
const day = (s) => new Date(s.slice(0, 10) + 'T00:00:00');
const iso = (d) => `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
const addDays = (d, n) => new Date(d.getFullYear(), d.getMonth(), d.getDate() + n);
const today = day(iso(new Date()));
const through = opt.through ? day(opt.through) : today;
const week1 = (start) => addDays(start, -((start.getDay() + 6) % 7));

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

// A level on the four-point scale (4 EE, 3 ME, 2 AE, 1 BE) from a continuous score.
const level = (x, cuts) => (x > cuts[0] ? 4 : x > cuts[1] ? 3 : x > cuts[2] ? 2 : 1);
const tier = (lvl) => (lvl >= 4 ? 0 : lvl === 3 ? 1 : 2); // top, middle, needs support

// ---------- What teachers write ----------
const OUTCOME_COMMENT = {
    en: {4: ['Excellent work.', 'Very good understanding.', 'Explains the steps confidently.'], 3: ['Good work.', 'Meets the outcome.'],
        2: ['Improving; needs more practice.', 'Needs guidance to complete the task.'], 1: ['Needs more practice; to be reassessed.', 'Needs one-to-one support.']},
    sw: {4: ['Kazi bora sana.', 'Ameelewa vizuri sana.'], 3: ['Kazi nzuri.', 'Amefikia matarajio.'],
        2: ['Anaendelea vizuri; ahitaji mazoezi zaidi.', 'Ahitaji mwongozo kukamilisha kazi.'], 1: ['Ahitaji mazoezi zaidi; atathminiwe tena.', 'Ahitaji msaada wa karibu.']},
};

const VALUE_WORDS = {
    'Love': ['Shows care for classmates and is quick to help anyone in need.', 'Is learning to show care and kindness to others; needs encouragement.',
        'Needs support to be kind and considerate towards others.'],
    'Responsibility': ['Completes tasks and duties without being reminded.', 'Sometimes needs reminders to complete tasks and duties.',
        'Needs close follow-up to complete assigned tasks.'],
    'Respect': ['Courteous to teachers and peers and takes good care of school property.', 'Is learning to listen to others and wait for a turn to speak.',
        'Needs guidance on respecting others and their property.'],
    'Unity': ['Works well with everyone and brings the group together.', 'Prefers working with a few friends; encouraged to mix with others.',
        'Finds group work difficult; needs support to cooperate.'],
    'Peace': ['Settles disagreements calmly and helps others do the same.', 'Occasionally gets into disagreements; learning to resolve them calmly.',
        'Needs support to manage anger and resolve conflicts.'],
    'Patriotism': ['Takes pride in national symbols and leads the anthem with enthusiasm.', 'Is learning about national symbols and their meaning.',
        'Needs encouragement to take part in national events.'],
    'Social Justice': ['Stands up for fairness and includes everyone in activities.', 'Is learning to share fairly and include others.',
        'Needs guidance to treat others fairly.'],
    'Integrity': ['Honest and trustworthy; owns up to mistakes.', 'Is learning to own up to mistakes.', 'Needs guidance on honesty and telling the truth.'],
};

const RESPONSIBILITY_WORDS = {
    'Keeping the Environment Clean': ['Keeps the classroom and compound clean without being asked and leads others in it.',
        'Takes part in cleaning the classroom and compound.', 'Takes part in cleaning duties when reminded.'],
    'Personal Hygiene': ['Always neat and clean; a good example to others.', 'Keeps neat and clean.', 'Is learning to keep neat; needs reminders.'],
    'Class Duties': ['Carries out class duties promptly and reliably.', 'Carries out assigned class duties.', 'Carries out class duties with guidance.'],
    'Peer Mentoring': ['Patiently helps classmates who find the work difficult.', 'Helps classmates with their work when asked.',
        'Is learning to support classmates with their work.'],
    'Community Service': ['Volunteers readily for community activities.', 'Takes part in community activities.', 'Takes part in community activities with encouragement.'],
    'Teamwork': ['Works very well in groups and helps the group finish its task.', 'Works well with others in groups.', 'Is learning to work with others in a group.'],
    'Communication': ['Expresses ideas clearly and listens well to others.', 'Expresses ideas and listens to others.', 'Needs encouragement to share ideas.'],
    'Conflict Resolution': ['Resolves disagreements calmly and fairly.', 'Resolves disagreements with some guidance.', 'Needs support to resolve disagreements.'],
    'Empathy': ['Notices when others are upset and comforts them.', 'Shows concern for others.', 'Is learning to understand how others feel.'],
    'Leadership': ['Takes the lead in group work and motivates others.', 'Takes the lead when given the chance.', 'Is developing confidence to lead.'],
    'Respect for Diversity': ['Welcomes and includes learners from all backgrounds.', 'Gets along with learners from different backgrounds.',
        'Is learning to appreciate differences in others.'],
    'Sharing': ['Shares learning materials readily.', 'Shares with others.', 'Is learning to share with others.'],
    'Turn Taking': ['Waits patiently for a turn.', 'Waits for a turn most of the time.', 'Is learning to wait for a turn.'],
};

const SERVICE_WORDS = {
    'Environmental Conservation': ['Planted and cared for tree seedlings on the school compound; encouraged others to water them.',
        'Took part in planting and watering trees on the school compound.', 'Took part in the tree planting with encouragement.'],
    'School Beautification': ['Worked hard on the school flower beds and kept the class garden tidy.', 'Helped plant and tend the school flower beds.',
        'Helped with the flower beds when guided.'],
    'Visiting the Elderly': ['Helped organise the class visit to the elderly and read to the residents.', 'Took part in the class visit to the elderly.',
        'Joined the class visit to the elderly.'],
    'Feeding Programme': ['Helped serve meals at the community feeding programme with care.', 'Helped at the community feeding programme.',
        'Took part in the feeding programme with guidance.'],
    'Health Awareness': ['Led a group demonstrating handwashing to younger learners.', 'Took part in the handwashing and hygiene campaign.',
        'Took part in the hygiene campaign with guidance.'],
    'Road Safety Campaign': ['Made posters and explained road safety rules to younger learners.', 'Took part in the road safety campaign.',
        'Took part in the road safety campaign with encouragement.'],
    'Library Assistance': ['Helped arrange and label the class library books reliably.', 'Helped arrange books in the library.', 'Helped in the library when asked.'],
    'Peer Tutoring': ['Tutored younger learners in reading every week.', 'Helped younger learners with their reading.', 'Helped younger learners with guidance.'],
};

const ACTIVITY_WORDS = {
    'Football': ['Plays in the school team; scored in the zonal ball games.', 'Trains regularly and is a dependable team player.', 'Attends training; encouraged to be more active.'],
    'Netball': ['Plays centre for the school team; strong passing and movement.', 'Trains regularly and plays well in matches.', 'Attends training; working on catching and passing.'],
    'Athletics': ['Represented the school at the sub-county athletics.', 'Takes part in the school track events.', 'Takes part in athletics; encouraged to train more.'],
    'Swimming': ['Swims confidently and helps others learn.', 'Swims with growing confidence.', 'Is learning to float and kick; needs more practice.'],
    'Volleyball': ['A strong server in the school team.', 'Plays well and trains regularly.', 'Is learning the basic skills.'],
    'Music': ['Sang a solo verse at the music festival.', 'Sings in the school choir.', 'Takes part in music; encouraged to practise more.'],
    'Drama': ['Played a lead role in the school play.', 'Took part in the drama festival set piece.', 'Takes part in drama; building confidence on stage.'],
    'Debate': ['Spoke confidently at the inter-school debate.', 'Takes part in club debates.', 'Attends debates; encouraged to speak up.'],
    'Scouting': ['Earned the first-aid badge and helps lead the patrol.', 'An active member of the troop.', 'Attends meetings; encouraged to take part more.'],
    'Girl Guides': ['Earned the camping badge and helps lead the patrol.', 'An active member of the company.', 'Attends meetings; encouraged to take part more.'],
    'Chess': ['Won games at the inter-school chess tournament.', 'Plays regularly and is improving.', 'Is learning the moves and simple tactics.'],
    'Art Club': ['Artwork displayed on the school open day.', 'Produces neat, creative work.', 'Takes part in art activities.'],
    'Taekwondo': ['Earned a new belt this year; disciplined and focused.', 'Trains regularly and follows instructions well.', 'Attends training; working on the basic moves.'],
};

// Which learners an activity is open to: grade number (0 for pre-primary and day care), gender (1 boy, 2 girl).
const ACTIVITIES = {
    'Football': (g, s) => (g >= 1 ? (s === 1 ? 3 : 0.8) : 0),
    'Netball': (g, s) => (g >= 3 && s === 2 ? 3 : 0),
    'Athletics': (g) => (g >= 1 ? 2 : 0),
    'Swimming': () => 1,
    'Volleyball': (g) => (g >= 6 ? 1.5 : 0),
    'Music': () => 2,
    'Drama': (g) => (g >= 2 ? 1.5 : 0),
    'Debate': (g) => (g >= 5 ? 1 : 0),
    'Scouting': (g, s) => (g >= 1 && s === 1 ? 1.5 : 0),
    'Girl Guides': (g, s) => (g >= 1 && s === 2 ? 1.5 : 0),
    'Chess': (g) => (g >= 3 ? 1 : 0),
    'Art Club': () => 1.5,
    'Taekwondo': (g) => (g >= 3 ? 0.8 : 0),
};
const SPECIAL_ACTIVITIES = ['Music', 'Art Club', 'Athletics', 'Swimming'];

async function main() {
    if (!query('SELECT COUNT(*) n FROM person')[0]) fail(`database ${opt.db} not found`);
    const year = query(`SELECT Id, Name FROM academicyears WHERE StartDate <= '${iso(through)}' AND EndDate >= '${iso(through)}'`)[0];
    if (!year) fail('no academic year covers --through');
    const terms = query(`SELECT Id, SessionName, StartDate, EndDate FROM sessions WHERE AcademicYearId = ${year.Id}
        AND StartDate <= '${iso(through)}' ORDER BY StartDate`).map((s, i) => ({id: +s.Id, idx: i, name: `${s.SessionName} ${year.Name}`,
        start: day(s.StartDate), end: day(s.EndDate)}));
    if (!terms.length) fail('no term has started');
    const ended = terms.filter((t) => t.end <= through);

    // ---------- 1. Classes, learners, teachers ----------
    const classes = query(`SELECT c.Id, c.Name, l.Name levelName,
            (SELECT u.UserName FROM schoolclassleaders x JOIN classleadershiproles r ON r.Id = x.ClassLeadershipRoleId
               JOIN aspnetusers u ON u.PersonId = x.PersonId WHERE x.SchoolClassId = c.Id AND r.PersonType = 1 ORDER BY x.Id LIMIT 1) classTeacher
        FROM schoolclasses c JOIN learninglevels l ON l.Id = c.LearningLevelId WHERE c.AcademicYearId = ${year.Id}`)
        .map((c) => {
            const g = /grade\s*(\d+)/i.exec(c.levelName);
            return {id: +c.Id, name: c.Name, teacher: c.classTeacher, special: /special/i.test(c.levelName),
                daycare: /day\s*care/i.test(c.levelName), grade: g ? +g[1] : 0};
        });
    const classOf = Object.fromEntries(classes.map((c) => [c.id, c]));
    const missingCT = classes.filter((c) => !c.teacher);
    if (missingCT.length) fail('no class teacher with a login for ' + missingCT.map((c) => c.name).join(', '));

    const learners = query(`SELECT p.Id, p.GenderId, sc.SchoolClassId FROM person p JOIN studentclasses sc ON sc.StudentId = p.Id
        JOIN schoolclasses c ON c.Id = sc.SchoolClassId WHERE c.AcademicYearId = ${year.Id} AND p.Discriminator = 'Student' AND p.Status = 0
        ORDER BY sc.SchoolClassId, p.Id`).map((r) => ({id: +r.Id, sex: +r.GenderId, cls: classOf[+r.SchoolClassId],
        // Lasting traits: conduct (values, responsibilities) and how keen the learner is on clubs and sport.
        conduct: gauss(), keen: gauss()}));
    const byClass = {};
    for (const l of learners) (byClass[l.cls.id] ??= []).push(l);
    const learnerById = Object.fromEntries(learners.map((l) => [l.id, l]));

    const teachers = query(`SELECT DISTINCT u.UserName FROM staffsubjects a JOIN schoolclasses c ON c.Id = a.SchoolClassId
        JOIN aspnetusers u ON u.PersonId = a.StaffDetailsId WHERE c.AcademicYearId = ${year.Id} ORDER BY u.UserName`).map((r) => r.UserName);
    console.log(`${learners.length} learners in ${classes.length} classes, ${teachers.length} teachers, ${terms.length} term(s).`);

    const ids = (sql, key = 'Name') => Object.fromEntries(query(sql).map((r) => [r[key].trim(), +r.Id]));
    const valueIds = ids('SELECT Id, Name FROM `values`');
    const valueScoreIds = ids('SELECT Id, Abbreviation FROM valuescores', 'Abbreviation');
    const activityIds = ids('SELECT Id, Name FROM cocurriculumactivities');
    const scoreTypes = query('SELECT Id FROM cocurriculumscoretypes ORDER BY `Rank`').map((r) => +r.Id);
    const coScore = {}; // type|abbr -> score id
    for (const r of query('SELECT Id, CoCurriculumScoreTypeId t, Abbreviation FROM cocurriculumscores')) coScore[`${r.t}|${r.Abbreviation.trim()}`] = +r.Id;
    const responsibilities = query('SELECT Id, Name, Category FROM responsibilities').map((r) => ({id: +r.Id, name: r.Name, category: r.Category}));
    const serviceIds = ids('SELECT Id, Name FROM communityserviceactivities');
    const category = query(`SELECT SettingValue FROM globalsettings WHERE Module = 'Grading' AND SettingKey = 'StudentAssessment'`)[0]?.SettingValue || '4-Point';
    const grades = query(`SELECT Id, Points FROM grades WHERE Category = '${category}'`).map((g) => ({id: +g.Id, points: +g.Points}));
    if (grades.length !== 4) fail(`expected a four-level ${category} grading scale for assessments`);
    const gradeFor = (pts) => grades.reduce((b, g) => (Math.abs(g.points - pts) < Math.abs(b.points - pts) ? g : b)).id;
    const assessmentType = +query("SELECT Id FROM assessmenttypes WHERE Name LIKE 'Formative%' ORDER BY Id LIMIT 1")[0]?.Id;
    if (!assessmentType) fail('no Formative assessment type');
    const ABBR = {4: 'EE', 3: 'ME', 2: 'AE', 1: 'BE'};

    // ---------- 2. Clear ----------
    console.log(`Clearing learners' CBE records in ${opt.db}...`);
    exec(`DELETE FROM studentassessments; DELETE FROM studentvaluescores; DELETE FROM studentcocurriculumscores;
DELETE FROM studentcocurriculumactivities; DELETE FROM studentcommunityserviceactivities;
-- Leadership recorded from election results stays.
DELETE FROM studentresponsibilities WHERE COALESCE(Description, '') NOT LIKE '% - elected, %';
UPDATE globalsettings SET SettingValue = 'true' WHERE Module = 'ReportForm' AND SettingKey = 'ShowValues';`);
    for (const u of new Set([...teachers, ...classes.map((c) => c.teacher)])) await tok(u);
    const stats = {outcomes: 0, rollups: 0, values: 0, responsibilities: 0, service: 0, activities: 0, scores: 0};

    // ---------- 3. Formative assessments ----------
    // Each learner's exam average per subject and term, and overall per term, to grade from.
    const exam = {}, examAll = {};
    for (const r of query(`SELECT r.StudentId s, e.SubjectId subj, se.SessionId t, AVG(r.Score * 100 / e.ExamMark) pct
            FROM examresults r JOIN exams e ON e.Id = r.ExamId JOIN schoolexams se ON se.Id = e.SchoolExamId
            WHERE e.ExamMark > 0 AND r.Score IS NOT NULL GROUP BY r.StudentId, e.SubjectId, se.SessionId`)) {
        exam[`${r.s}|${r.subj}|${r.t}`] = +r.pct;
        (examAll[`${r.s}|${r.t}`] ??= []).push(+r.pct);
    }
    const ability = {}; // learner|subject|term -> continuous level (pct 25/50/75 fall on the 2/3/4 boundaries)
    const abilityFor = (s, subj, t) => ability[`${s}|${subj}|${t}`] ??= (() => {
        let pct = exam[`${s}|${subj}|${t}`];
        if (pct === undefined && examAll[`${s}|${t}`]) pct = examAll[`${s}|${t}`].reduce((x, y) => x + y, 0) / examAll[`${s}|${t}`].length + gauss() * 8;
        if (pct === undefined) pct = 62 + gauss() * 15;
        // Classroom tasks run a little kinder than exams.
        return Math.min(4.4, Math.max(0.8, pct / 25 + 0.5 + 0.15));
    })();

    const outcomesOf = {};
    for (const r of query('SELECT Id, SubStrandId FROM specificoutcomes ORDER BY SubStrandId, `Rank`, Id')) (outcomesOf[+r.SubStrandId] ??= []).push(+r.Id);
    const takers = {};
    for (const r of query(`SELECT DISTINCT sc.SchoolClassId cls, ss.SubjectId subj, sc.StudentId s FROM studentsubjects ss
            JOIN studentclasses sc ON sc.Id = ss.StudentClassId JOIN schoolclasses c ON c.Id = sc.SchoolClassId
            WHERE c.AcademicYearId = ${year.Id}`)) {
        if (learnerById[+r.s]) (takers[`${r.cls}|${r.subj}`] ??= []).push(learnerById[+r.s]);
    }
    // The sub-strands each term's scheme covered, and the week each was finished.
    const schemes = {};
    for (const r of query(`SELECT sw.Id, sw.SessionId, sw.SchoolClassId, sw.SubjectId, sw.StaffDetailsId, ANY_VALUE(s.Name) subjectName, ANY_VALUE(u.UserName) UserName,
            ss.StrandId, l.SubStrandId, MAX(l.WeekNo) lastWeek
            FROM schemesofwork sw JOIN schemeofworklessons l ON l.SchemeOfWorkId = sw.Id JOIN substrands ss ON ss.Id = l.SubStrandId
            JOIN subjects s ON s.Id = sw.SubjectId JOIN aspnetusers u ON u.PersonId = sw.StaffDetailsId
            WHERE l.SubStrandId IS NOT NULL GROUP BY sw.Id, ss.StrandId, l.SubStrandId ORDER BY sw.Id, lastWeek`)) {
        const sc = schemes[r.Id] ??= {id: +r.Id, term: terms.find((t) => t.id === +r.SessionId), cls: +r.SchoolClassId, subject: +r.SubjectId,
            staff: +r.StaffDetailsId, teacher: r.UserName, sw: /kiswahili/i.test(r.subjectName) ? 'sw' : 'en', subs: []};
        sc.subs.push({strand: +r.StrandId, sub: +r.SubStrandId, week: +r.lastWeek});
    }
    const schemeList = Object.values(schemes).filter((s) => s.term && classOf[s.cls]);
    console.log(`Formative assessments for ${schemeList.length} schemes of work...`);
    for (const u of new Set(schemeList.map((s) => s.teacher))) await tok(u);

    await pool(schemeList, 6, async (sc) => {
        const T = tokens[sc.teacher];
        const who = takers[`${sc.cls}|${sc.subject}`]?.length ? takers[`${sc.cls}|${sc.subject}`] : byClass[sc.cls] || [];
        if (!who.length) return;
        const base = (l) => abilityFor(l.id, sc.subject, sc.term.id);
        const points = {}; // learner -> sub-strand -> [points]
        const dateOf = {};
        for (const ss of sc.subs) {
            const outcomes = outcomesOf[ss.sub] || [];
            if (!outcomes.length) continue;
            // Assessed on the Thursday or Friday of the week the sub-strand was finished.
            let d = addDays(week1(sc.term.start), (ss.week - 1) * 7 + (chance(0.5) ? 3 : 4));
            if (d > sc.term.end) d = sc.term.end;
            if (d > through) continue;
            dateOf[ss.sub] = iso(d);
            const rows = [];
            for (const so of outcomes) {
                for (const l of who) {
                    if (chance(0.02)) continue; // absent that day
                    const p = Math.min(4, Math.max(1, Math.round(base(l) + gauss() * 0.55)));
                    ((points[l.id] ??= {})[ss.sub] ??= []).push(p);
                    rows.push({studentId: l.id, schoolClassId: sc.cls, specificOutcomeId: so, gradeId: gradeFor(p), sessionId: sc.term.id,
                        assessmentTypeId: assessmentType, assessmentDate: dateOf[ss.sub], staffDetailsId: sc.staff,
                        description: chance(0.12) ? pick(OUTCOME_COMMENT[sc.sw][p]) : ''});
                }
            }
            if (rows.length) {
                await api(T, 'POST', '/studentAssessments/batch', rows);
                stats.outcomes += rows.length;
            }
        }
        // The sub-strand and strand grades, averaged from the outcomes as the Assessments page works them out.
        const rollups = [];
        const strands = [...new Set(sc.subs.filter((s) => dateOf[s.sub]).map((s) => s.strand))];
        for (const l of who) {
            const mine = points[l.id];
            if (!mine) continue;
            const avg = (xs) => Math.round(xs.reduce((x, y) => x + y, 0) / xs.length);
            const common = {studentId: l.id, schoolClassId: sc.cls, sessionId: sc.term.id, assessmentTypeId: assessmentType, staffDetailsId: sc.staff, description: ''};
            for (const ss of sc.subs) {
                if (mine[ss.sub]) rollups.push({...common, subStrandId: ss.sub, gradeId: gradeFor(avg(mine[ss.sub])), assessmentDate: dateOf[ss.sub]});
            }
            for (const st of strands) {
                const subs = sc.subs.filter((s) => s.strand === st && mine[s.sub]);
                if (!subs.length) continue;
                rollups.push({...common, strandId: st, gradeId: gradeFor(avg(subs.flatMap((s) => mine[s.sub]))),
                    assessmentDate: dateOf[subs[subs.length - 1].sub]});
            }
        }
        for (let i = 0; i < rollups.length; i += 400) await api(T, 'POST', '/studentAssessments/batch', rollups.slice(i, i + 400));
        stats.rollups += rollups.length;
    });
    console.log(`  ${stats.outcomes} outcome grades, ${stats.rollups} sub-strand and strand grades.`);

    // ---------- 4. Values, by the class teacher at the end of each term ----------
    for (const term of ended) {
        for (const c of classes) {
            const rows = [];
            for (const l of byClass[c.id] || []) {
                for (const [name, valueId] of Object.entries(valueIds)) {
                    // Conduct carries from term to term, improving a little through the year.
                    const v = l.conduct * 0.8 + gauss() * 0.5 + term.idx * 0.12 + 0.25;
                    const lvl = level(v, [0.95, -0.65, -1.6]);
                    const words = VALUE_WORDS[name];
                    rows.push({studentId: l.id, valueId, sessionId: term.id, valueScoreId: valueScoreIds[ABBR[lvl]],
                        description: !words || lvl === 3 ? '' : words[lvl === 4 ? 0 : lvl === 2 ? 1 : 2]});
                }
            }
            if (!rows.length) continue;
            await api(tokens[c.teacher], 'POST', '/studentValueScores/batch', rows);
            stats.values += rows.length;
        }
    }
    console.log(`Values: ${stats.values} ratings over ${ended.length} term(s).`);

    // ---------- 5. Responsibilities and social skills for the year ----------
    const respOf = (cat) => responsibilities.filter((r) => r.category === cat && r.name !== 'School Leadership');
    for (const c of classes) {
        const rows = [];
        const older = c.grade >= 4 && !c.special;
        const duties = respOf('Responsibility').filter((r) => older || !['Peer Mentoring', 'Community Service'].includes(r.name));
        const skills = respOf('Social Skill').filter((r) => (older ? r.name !== 'Turn Taking' : ['Sharing', 'Turn Taking', 'Teamwork', 'Communication', 'Empathy'].includes(r.name)));
        for (const l of byClass[c.id] || []) {
            const t = tier(level(l.conduct + gauss() * 0.4 + 0.3, [0.8, -0.7, -9]));
            const chosen = [pick(duties), pick(skills)];
            if (chance(0.3)) chosen.push(pick(skills.filter((s) => s !== chosen[1])));
            for (const r of chosen) {
                rows.push({academicYearId: +year.Id, studentId: l.id, responsibilitySocialSkillId: r.id,
                    description: RESPONSIBILITY_WORDS[r.name]?.[t] || ''});
            }
        }
        if (!rows.length) continue;
        await api(tokens[c.teacher], 'POST', '/studentResponsibilities/batch', rows);
        stats.responsibilities += rows.length;
    }
    console.log(`Responsibilities and social skills: ${stats.responsibilities}.`);

    // ---------- 6. Community service: each class's project every term ----------
    const lastProject = {};
    for (const term of ended) {
        for (const c of classes) {
            if (c.daycare) continue;
            const young = c.grade < 1 || c.special;
            const options = young ? ['School Beautification', 'Environmental Conservation', 'Health Awareness']
                : ['Environmental Conservation', 'School Beautification', 'Health Awareness', 'Road Safety Campaign',
                    ...(c.grade >= 4 ? ['Feeding Programme', 'Visiting the Elderly', 'Library Assistance'] : [])];
            const project = pick(options.filter((o) => o !== lastProject[c.id]));
            lastProject[c.id] = project;
            const rows = [];
            for (const l of byClass[c.id] || []) {
                const t = tier(level(l.conduct * 0.7 + gauss() * 0.6 + 0.3, [0.8, -0.8, -9]));
                rows.push({studentId: l.id, communityServiceActivityId: serviceIds[project], sessionId: term.id, academicYearId: +year.Id,
                    description: SERVICE_WORDS[project][t]});
                // A few older learners take on their own service as well.
                if (c.grade >= 4 && !c.special && chance(0.15)) {
                    const own = pick(['Peer Tutoring', 'Library Assistance'].filter((o) => o !== project));
                    rows.push({studentId: l.id, communityServiceActivityId: serviceIds[own], sessionId: term.id, academicYearId: +year.Id,
                        description: SERVICE_WORDS[own][Math.min(t, 1)]});
                }
            }
            if (!rows.length) continue;
            await api(tokens[c.teacher], 'POST', '/studentCommunityServiceActivities/batch', rows);
            stats.service += rows.length;
        }
    }
    console.log(`Community service: ${stats.service} records.`);

    // ---------- 7. Co-curricular activities, recorded by each activity's patron ----------
    const names = Object.keys(ACTIVITIES).filter((n) => activityIds[n]);
    const patron = Object.fromEntries(names.map((n, i) => [n, teachers[(i * 7 + 3) % teachers.length]]));
    const joins = [];
    for (const l of learners) {
        const g = l.cls.grade;
        const open = l.cls.special ? SPECIAL_ACTIVITIES.filter((n) => activityIds[n]).map((n) => [n, 1])
            : names.map((n) => [n, ACTIVITIES[n](g, l.sex)]).filter(([, w]) => w > 0);
        const first = weighted(open);
        joins.push({l, name: first});
        if (g >= 3 && !l.cls.special && chance(0.35)) {
            const rest = open.filter(([n]) => n !== first);
            if (rest.length) joins.push({l, name: weighted(rest)});
        }
    }
    await pool(joins, 6, async ({l, name}) => {
        const T = tokens[patron[name]];
        const e = l.keen * 0.6 + gauss() * 0.6 + 0.35;
        const lv = [e + 0.15, e - 0.2 + gauss() * 0.4, e + 0.05 + gauss() * 0.3].map((x) => level(x, [0.9, -0.5, -1.4]));
        const rec = await api(T, 'POST', '/studentCoCurriculumActivities',
            {studentId: l.id, coCurriculumActivityId: activityIds[name], description: ACTIVITY_WORDS[name][tier(lv[1])]});
        stats.activities++;
        const rows = scoreTypes.map((t, i) => ({studentCoCurriculumActivityId: rec.id, coCurriculumScoreId: coScore[`${t}|${ABBR[lv[i]]}`], description: ''}))
            .filter((r) => r.coCurriculumScoreId);
        if (rows.length) await api(T, 'POST', '/studentCoCurriculumScores/batch', rows);
        stats.scores += rows.length;
    });
    console.log(`Co-curricular: ${stats.activities} memberships, ${stats.scores} scores.`);

    // ---------- 8. Make it look recorded on the day ----------
    // Assessments the afternoon they were given; values two to six days before the term closed; community service
    // in the last fortnight of term; responsibilities early in the year; clubs in the first weeks of the year, scored
    // at the end of the last term ended.
    const firstTerm = terms[0], lastEnded = ended[ended.length - 1] || terms[terms.length - 1];
    exec(`
UPDATE studentassessments SET Created = AssessmentDate + INTERVAL (14 + Id % 4) HOUR + INTERVAL (Id % 57) MINUTE, Modified = Created
    WHERE AssessmentDate IS NOT NULL;
UPDATE studentvaluescores v JOIN sessions t ON t.Id = v.SessionId JOIN studentclasses sc ON sc.StudentId = v.StudentId
    JOIN schoolclasses c ON c.Id = sc.SchoolClassId AND c.AcademicYearId = t.AcademicYearId
    SET v.Created = t.EndDate - INTERVAL (2 + c.Id % 5) DAY + INTERVAL (10 + c.Id % 6) HOUR + INTERVAL (v.Id % 40) SECOND, v.Modified = v.Created;
UPDATE studentcommunityserviceactivities x JOIN sessions t ON t.Id = x.SessionId JOIN studentclasses sc ON sc.StudentId = x.StudentId
    JOIN schoolclasses c ON c.Id = sc.SchoolClassId AND c.AcademicYearId = t.AcademicYearId
    SET x.Created = t.EndDate - INTERVAL (6 + c.Id % 8) DAY + INTERVAL (13 + c.Id % 4) HOUR + INTERVAL (x.Id % 40) SECOND, x.Modified = x.Created;
UPDATE studentresponsibilities r JOIN studentclasses sc ON sc.StudentId = r.StudentId
    JOIN schoolclasses c ON c.Id = sc.SchoolClassId AND c.AcademicYearId = r.AcademicYearId
    SET r.Created = '${iso(firstTerm.start)}' + INTERVAL (10 + c.Id % 10) DAY + INTERVAL (11 + c.Id % 5) HOUR + INTERVAL (r.Id % 40) SECOND, r.Modified = r.Created
    WHERE COALESCE(r.Description, '') NOT LIKE '% - elected, %';
UPDATE studentcocurriculumactivities a SET a.Created = '${iso(firstTerm.start)}' + INTERVAL (10 + a.CoCurriculumActivityId % 12) DAY
    + INTERVAL (15 + a.Id % 2) HOUR + INTERVAL (a.Id % 50) MINUTE, a.Modified = a.Created;
UPDATE studentcocurriculumscores s JOIN studentcocurriculumactivities a ON a.Id = s.StudentCoCurriculumActivityId
    SET s.Created = '${iso(lastEnded.end)}' - INTERVAL (3 + a.CoCurriculumActivityId % 5) DAY + INTERVAL (15 + a.Id % 2) HOUR + INTERVAL (s.Id % 50) MINUTE,
        s.Modified = s.Created;`);
    console.log('Done.');
}

main().catch((e) => fail(e.stack || e.message));
