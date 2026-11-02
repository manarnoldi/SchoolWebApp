#!/usr/bin/env node
// Fills an anonymised demo copy with the year's student council election, run the
// way a school runs one, from start to finish:
//  - set up by the head teacher early in Term 1, with positions of every kind:
//    a President on a joint ticket with a Deputy, Games Captains (a boy and a girl
//    per section), a Girls' Representative voted for by girls, an Environment &
//    Health Prefect per section and a Class President per class;
//  - nominations over a fortnight, from learners whose values ratings mark them
//    out, with slogans and running mates; one nomination rejected, one withdrawn;
//  - the voter register frozen when nominations close, a polling station per class
//    run by its class teacher, and polling one morning: most learners vote, for
//    candidates their classmates and their own grade tend to favour;
//  - one class race tied at the top and settled by drawing lots, the results
//    declared, and the winners written to class leadership and responsibilities.
// Everything goes through the API - the head teacher manages, each class teacher
// issues their station's ballots - and the dates are then set back to January.
//
//   MYSQL_PWD=... node tools/demo-data/elections.js --db shulenova --api http://localhost:5198
//
// Options: --db <database> (required; refuses names of live schools)
//          --api <API origin> (default http://localhost:5198)
//          --mysql <path to mysql.exe> (default MySQL 8.4 on Windows)
//          --user <mysql user> (default root)
//          --password <the demo logins' password> (default Demo@2026)
//          --year <academic year name> (default 2026)
//          --manager <user name of whoever runs it> (default purity.haji, the head teacher)
//          --poll <yyyy-mm-dd> (polling day; default the last Thursday of January)
//          --seed <number> (default 2026)
// Every election in the copy is deleted first, with the leadership records past
// elections wrote and the year's class presidents (the election elects them), so
// it can be run again. Run it after cbe.js. Candidate photos are not added.

const {execFileSync} = require('child_process');

const opt = {mysql: 'C:/Program Files/MySQL/MySQL Server 8.4/bin/mysql.exe', user: 'root', api: 'http://localhost:5198',
    password: 'Demo@2026', year: '2026', manager: 'purity.haji', seed: '2026'};
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
let seed = (+opt.seed + 41) >>> 0;
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
const shuffle = (list) => {
    const l = list.slice();
    for (let i = l.length - 1; i > 0; i--) {
        const j = Math.floor(rnd() * (i + 1));
        [l[i], l[j]] = [l[j], l[i]];
    }
    return l;
};

// ---------- Dates: Kenyan time (UTC+3), stored as UTC ----------
const pad = (n) => String(n).padStart(2, '0');
// A local date and time ('2026-01-29', 8, 5) as the UTC 'yyyy-mm-dd hh:mm:ss' the database keeps.
function utc(date, h, m = 0, s = 0) {
    const d = new Date(Date.UTC(+date.slice(0, 4), +date.slice(5, 7) - 1, +date.slice(8, 10), h - 3, m, s));
    return `${d.getUTCFullYear()}-${pad(d.getUTCMonth() + 1)}-${pad(d.getUTCDate())} ${pad(d.getUTCHours())}:${pad(d.getUTCMinutes())}:${pad(d.getUTCSeconds())}`;
}
const isoUtc = (date, h, m = 0) => utc(date, h, m).replace(' ', 'T') + 'Z';
function addDays(date, n) {
    const d = new Date(Date.UTC(+date.slice(0, 4), +date.slice(5, 7) - 1, +date.slice(8, 10) + n));
    return `${d.getUTCFullYear()}-${pad(d.getUTCMonth() + 1)}-${pad(d.getUTCDate())}`;
}
// The last Thursday of January of the year.
function lastThursdayOfJanuary(year) {
    const d = new Date(Date.UTC(+year, 0, 31));
    while (d.getUTCDay() !== 4) d.setUTCDate(d.getUTCDate() - 1);
    return `${year}-01-${pad(d.getUTCDate())}`;
}

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

// ---------- What candidates say ----------
const SLOGANS = {
    president: ['Together we rise', 'Your voice, our mission', 'Leading by example', 'Unity, discipline, excellence',
        'A school we are all proud of', 'Service before self', 'Listening, leading, delivering', 'One school, one dream'],
    games: ['Play hard, play fair', 'Every learner on the pitch', 'Champions are made in training', 'Teamwork wins games',
        'Fit body, sharp mind', 'From trials to trophies'],
    girls: ['Every girl heard', 'Strong girls, strong school', 'Confidence for every girl', 'Her voice matters',
        'Standing up for every girl'],
    environment: ['A clean school is a healthy school', 'Plant a tree, grow a future', 'Clean hands, healthy class',
        'Keep it green, keep it clean', 'Waste not, want not', 'Every bin, every day'],
    classPresident: ['I will listen to you', 'A class that works together', 'Order, effort and fun', 'Your class, your voice',
        'Helping every classmate', 'Let us top the school', 'Kindness and hard work', 'Pamoja tunaweza',
        'A clean and tidy class', 'Respect for everyone', 'No one left behind', 'Ready to serve']
};
const REJECTED_NOTE = 'Not eligible - did not get the class teacher\'s recommendation';
const WITHDRAWN_NOTE = 'Withdrew from the race - chose to concentrate on studies';

async function main() {
    // ---------- 1. The school ----------
    const year = query(`SELECT Id FROM academicyears WHERE Name = ${q(opt.year)}`)[0];
    if (!year) fail(`no academic year "${opt.year}"`);
    const yearId = +year.Id;
    const poll = opt.poll || lastThursdayOfJanuary(opt.year);
    const levels = Object.fromEntries(query(`SELECT Id, Name FROM learninglevels`).map((l) => [l.Name, +l.Id]));
    const grade = (n) => levels[`Grade ${n}`] ?? fail(`no learning level "Grade ${n}"`);
    const genders = Object.fromEntries(query(`SELECT Id, Name FROM genders`).map((g) => [g.Name, +g.Id]));
    const presidentRole = query(`SELECT Id FROM classleadershiproles WHERE PersonType = 0 AND Name LIKE 'Class president'`)[0];
    const schoolLeadership = query(`SELECT Id FROM responsibilities WHERE Name = 'School Leadership'`)[0];
    if (!presidentRole || !schoolLeadership) fail('the "Class president" role or the "School Leadership" responsibility is missing');
    if (!query(`SELECT 1 FROM aspnetusers WHERE UserName = ${q(opt.manager)}`).length) fail(`no user "${opt.manager}"`);

    // How each learner is rated on values over the year (1 = EE ... 4 = BE): the
    // learners teachers would put forward, and classmates would vote for.
    const values = Object.fromEntries(query(`SELECT StudentId, AVG(ValueScoreId) v FROM studentvaluescores GROUP BY StudentId`)
        .map((r) => [r.StudentId, +r.v]));
    const merit = (studentId) => (4 - (values[studentId] ?? 2.2)) + gauss() * 0.35;

    console.log(`Election for ${opt.year}, polling ${poll}, run by ${opt.manager}.`);

    // ---------- 2. Clear past elections ----------
    console.log(`Clearing elections in ${opt.db}...`);
    exec(`
DELETE FROM electionballotchoices;
DELETE FROM electionvotecasts;
DELETE FROM electionvoters;
DELETE FROM electionstationclasses;
DELETE FROM electionstations;
DELETE FROM electioncandidates;
DELETE FROM electioncontests;
DELETE FROM electionpositionlevels;
DELETE FROM electionpositions;
DELETE FROM electionstagechanges;
DELETE FROM elections;
DELETE FROM studentresponsibilities WHERE Description LIKE '% - elected, %';
-- The election elects the year's class presidents; those entered by hand go.
DELETE l FROM schoolclassleaders l JOIN schoolclasses c ON c.Id = l.SchoolClassId
 WHERE l.ClassLeadershipRoleId = ${presidentRole.Id} AND (c.AcademicYearId = ${yearId} OR l.Description LIKE '% - elected, %');
`);

    const manager = await tok(opt.manager);

    // ---------- 3. The election and its positions ----------
    const name = `${opt.year} Student Council Elections`;
    let e = await api(manager, 'POST', '/elections', {
        name,
        description: 'Election of the student council for the year: the School President and Deputy, Games Captains, ' +
            'the Girls\' Representative, Environment & Health Prefects and a Class President for every class.',
        academicYearId: yearId,
        registerMode: 0,
        pollingStart: isoUtc(poll, 8),
        pollingEnd: isoUtc(poll, 13),
        dashboardRefreshSeconds: 10
    });
    const electionId = e.id;
    const upper = [4, 5, 6, 7, 8, 9].map(grade);
    const all = [1, 2, 3, 4, 5, 6, 7, 8, 9].map(grade);
    // kind: which slogans, how many to put forward per race (per gender for Games Captain).
    const positions = [
        {kind: 'president', name: 'School President', runningMateTitle: 'Deputy President', scope: 0, seats: 1, genderRule: 0,
            voterLevelIds: upper, candidateLevelIds: [grade(8)], stand: [4, 4], mateLevel: grade(7)},
        {kind: 'games', name: 'Games Captain', scope: 1, seats: 1, genderRule: 2,
            voterLevelIds: upper, candidateLevelIds: [grade(6), grade(9)], stand: [2, 3]},
        {kind: 'girls', name: 'Girls\' Representative', scope: 0, seats: 1, genderRule: 1, genderId: genders.Female,
            votersSameGenderOnly: true, voterLevelIds: upper, candidateLevelIds: [grade(7), grade(8)], stand: [3, 3]},
        {kind: 'environment', name: 'Environment & Health Prefect', scope: 1, seats: 1, genderRule: 0,
            voterLevelIds: all, candidateLevelIds: [grade(3), grade(5), grade(7)], stand: [2, 3]},
        {kind: 'classPresident', name: 'Class President', scope: 3, seats: 1, genderRule: 0,
            voterLevelIds: all, candidateLevelIds: [], stand: [2, 4]}
    ];
    for (const [i, p] of positions.entries()) {
        await api(manager, 'POST', `/elections/${electionId}/positions`, {
            name: p.name, runningMateTitle: p.runningMateTitle || null, scope: p.scope, seats: p.seats, genderRule: p.genderRule,
            genderId: p.genderId || null, votersSameGenderOnly: !!p.votersSameGenderOnly, rank: i + 1,
            voterLevelIds: p.voterLevelIds, candidateLevelIds: p.candidateLevelIds
        });
    }
    e = await api(manager, 'GET', `/elections/${electionId}`);
    for (const p of positions) p.dto = e.positionList.find((x) => x.name === p.name);
    console.log(`Positions: ${positions.length}, races: ${positions.reduce((n, p) => n + p.dto.contests.length, 0)}.`);

    await api(manager, 'POST', `/elections/${electionId}/stage`, {toStage: 1, note: 'Nominations open - forms from the class teachers by Wednesday next week.'});

    // ---------- 4. Nominations ----------
    const nominated = []; // {candidateId, contest, position}
    // No two candidates in a race share a slogan; across races they may.
    const usedSlogans = new Set();
    const slogan = (kind, contestId) => {
        const left = SLOGANS[kind].filter((s) => !usedSlogans.has(contestId + s));
        const s = pick(left.length ? left : SLOGANS[kind]);
        usedSlogans.add(contestId + s);
        return s;
    };
    for (const p of positions) {
        for (const c of p.dto.contests) {
            const eligible = (await api(manager, 'GET', `/elections/contests/${c.id}/eligible?role=candidate`))
                .filter((s) => !s.taken).map((s) => ({...s, merit: merit(s.studentId)}))
                .sort((x, y) => y.merit - x.merit);
            const count = () => p.stand[0] + Math.floor(rnd() * (p.stand[1] - p.stand[0] + 1));
            // The likeliest from the top of the class or section, not always the very top.
            const choose = (pool, n) => {
                const out = [];
                const left = pool.slice(0, Math.max(n * 3, 6));
                while (out.length < n && left.length) out.push(left.splice(Math.floor(rnd() * Math.min(left.length, 4)), 1)[0]);
                return out;
            };
            let standing = p.genderRule === 2
                ? [...choose(eligible.filter((s) => s.genderName === 'Male'), count()), ...choose(eligible.filter((s) => s.genderName === 'Female'), count())]
                : choose(eligible, count());
            for (const s of shuffle(standing)) {
                let mateId = null;
                if (p.runningMateTitle) {
                    // A running mate from Grade 7, usually of the other gender.
                    const mates = (await api(manager, 'GET', `/elections/contests/${c.id}/eligible?role=mate`))
                        .filter((m) => !m.taken && m.studentId !== s.studentId && nominated.every((n) => n.mateId !== m.studentId));
                    const fromGrade = mates.filter((m) => /Grade 7/.test(m.className));
                    const other = fromGrade.filter((m) => m.genderName !== s.genderName);
                    const options = (chance(0.8) && other.length ? other : fromGrade.length ? fromGrade : mates)
                        .map((m) => ({...m, merit: merit(m.studentId)})).sort((x, y) => y.merit - x.merit).slice(0, 6);
                    mateId = options.length ? pick(options).studentId : null;
                }
                const res = await api(manager, 'POST', `/elections/contests/${c.id}/candidates`,
                    {studentId: s.studentId, runningMateStudentId: mateId, slogan: slogan(p.kind, c.id)});
                const cand = res.positions.flatMap((x) => x.contests).find((x) => x.id === c.id).candidates
                    .find((x) => x.studentId === s.studentId);
                nominated.push({candidateId: cand.id, studentId: s.studentId, mateId, contestId: c.id, position: p});
            }
        }
    }
    console.log(`Nominations: ${nominated.length}.`);

    // The elections committee's decisions: one rejected and one withdrawn, in class
    // races with candidates to spare; everyone else approved.
    const spare = shuffle(positions.find((p) => p.kind === 'classPresident').dto.contests
        .filter((c) => nominated.filter((n) => n.contestId === c.id).length >= 3));
    const rejected = spare[0] && nominated.filter((n) => n.contestId === spare[0].id).slice(-1)[0];
    const withdrawn = spare[1] && nominated.filter((n) => n.contestId === spare[1].id).slice(-1)[0];
    for (const n of nominated) {
        if (n === rejected) await api(manager, 'POST', `/elections/candidates/${n.candidateId}/status`, {status: 2, note: REJECTED_NOTE});
        else if (n === withdrawn) await api(manager, 'POST', `/elections/candidates/${n.candidateId}/status`, {status: 3, note: WITHDRAWN_NOTE});
        else await api(manager, 'POST', `/elections/candidates/${n.candidateId}/status`, {status: 1});
    }
    await api(manager, 'POST', `/elections/${electionId}/stage`, {toStage: 2, note: 'Nominations closed; the candidates are final and the voter register is taken.'});

    // ---------- 5. Polling stations: one per class, run by the class teacher ----------
    let stations = (await api(manager, 'POST', `/elections/${electionId}/stations/generate`)).stations;
    for (const s of stations.filter((x) => !x.officerUserId)) {
        // A class whose first class teacher has no login: another teacher of the class, else the manager.
        const other = query(`SELECT u.Id FROM schoolclassleaders l JOIN classleadershiproles r ON r.Id = l.ClassLeadershipRoleId
            JOIN aspnetusers u ON u.PersonId = l.PersonId WHERE r.PersonType = 1 AND l.SchoolClassId = ${s.classes[0].id} LIMIT 1`)[0]
            ?? query(`SELECT Id FROM aspnetusers WHERE UserName = ${q(opt.manager)}`)[0];
        await api(manager, 'PUT', `/elections/stations/${s.id}`, {name: s.name, officerUserId: +other.Id, schoolClassIds: s.classes.map((c) => c.id)});
    }
    stations = (await api(manager, 'GET', `/elections/${electionId}/stations`)).stations;
    const officers = Object.fromEntries(query(`SELECT Id, UserName FROM aspnetusers WHERE Id IN (${stations.map((s) => s.officerUserId).join(',')})`)
        .map((u) => [u.Id, u.UserName]));
    console.log(`Polling stations: ${stations.length}, voters: ${stations.reduce((n, s) => n + s.voters, 0)}.`);

    await api(manager, 'POST', `/elections/${electionId}/stage`, {toStage: 3, note: 'Polling is open. Voting is class by class at the class polling stations.'});

    // ---------- 6. Polling day: who votes and how ----------
    // Each candidate's pull: their standing with the learners and a campaign that
    // caught on or not. Voters lean to candidates from their own class and grade.
    const appeal = Object.fromEntries(nominated.map((n) => [n.candidateId, Math.exp(merit(n.studentId) * 0.35 + gauss() * 0.55)]));
    const ballots = []; // {station, officer, studentId, choices: {contestId: [candidateIds]}, races}
    for (const s of stations) {
        const officer = officers[s.officerUserId];
        const t = await tok(officer);
        const register = (await api(t, 'GET', `/elections/stations/${s.id}/register`)).voters;
        // Turnout by class: most vote; a few are absent on the day.
        const turnout = Math.min(0.99, 0.9 + gauss() * 0.04);
        for (const v of register) {
            if (!chance(turnout)) continue;
            const b = await api(t, 'GET', `/elections/stations/${s.id}/ballot/${v.studentId}`);
            const choices = {};
            for (const r of b.races) {
                const young = /Grade [123]E/.test(b.className);
                if (chance(young ? 0.04 : 0.015)) { // left blank
                    choices[r.contestId] = [];
                    continue;
                }
                const pull = (c) => appeal[c.candidateId] * (c.className === b.className ? 2.2 : 1)
                    * (c.className.replace(/\s\S+$/, '').slice(0, 7) === b.className.slice(0, 7) ? 1.3 : 1)
                    * (c.genderName === v.genderName ? 1.15 : 1);
                const groups = r.seatsPerGender ? ['Male', 'Female'].map((g) => r.candidates.filter((c) => c.genderName === g)) : [r.candidates];
                choices[r.contestId] = groups.filter((g) => g.length).map((g) => weighted(g.map((c) => [c.candidateId, pull(c)])));
            }
            ballots.push({station: s, officer, studentId: v.studentId, choices, races: b.races});
        }
    }

    // A count of the planned ballots, per race (and per gender where seats are per gender).
    const races = Object.fromEntries(ballots.flatMap((b) => b.races).map((r) => [r.contestId, r]));
    const genderOf = Object.fromEntries(Object.values(races).flatMap((r) => r.candidates).map((c) => [c.candidateId, c.genderName]));
    function tally(contestId) {
        const r = races[contestId];
        const groups = {};
        for (const c of r.candidates) (groups[r.seatsPerGender ? c.genderName : 'all'] ??= {})[c.candidateId] = 0;
        for (const b of ballots) for (const id of b.choices[contestId] || []) groups[r.seatsPerGender ? genderOf[id] : 'all'][id]++;
        return Object.values(groups).map((g) => Object.entries(g).map(([id, n]) => ({id: +id, n})).sort((x, y) => y.n - x.n));
    }
    // Moves one vote in a race from one candidate to another.
    function move(contestId, from, to) {
        const b = shuffle(ballots).find((x) => (x.choices[contestId] || []).includes(from));
        if (!b) return false;
        b.choices[contestId] = b.choices[contestId].map((id) => (id === from ? to : id));
        return true;
    }
    // One class race ends level at the top, to be settled by lots; any other tie for a seat is broken.
    const classRaces = positions.find((p) => p.kind === 'classPresident').dto.contests.map((c) => c.id)
        .filter((id) => races[id] && races[id].candidates.length >= 2);
    const tieRace = classRaces.map((id) => ({id, top: tally(id)[0]}))
        .filter((x) => x.top[0].n - x.top[1].n <= 6).sort((x, y) => (x.top[0].n - x.top[1].n) - (y.top[0].n - y.top[1].n))[0]
        ?? {id: classRaces[0], top: tally(classRaces[0])[0]};
    {
        let [first, second] = tieRace.top;
        let gap = first.n - second.n;
        while (gap >= 2 && move(tieRace.id, first.id, second.id)) gap -= 2;
        if (gap === 1) { // one ballot for the leader left blank instead
            const b = ballots.find((x) => (x.choices[tieRace.id] || []).includes(first.id));
            b.choices[tieRace.id] = [];
        }
    }
    for (const id of Object.keys(races).map(Number).filter((id) => id !== tieRace.id)) {
        for (const g of tally(id)) if (g.length >= 2 && g[0].n === g[1].n && g[0].n > 0) move(id, g[1].id, g[0].id);
    }

    // ---------- 7. Casting, station by station ----------
    let cast = 0;
    await pool(ballots, 4, async (b) => {
        const choices = Object.entries(b.choices).map(([contestId, candidateIds]) => ({contestId: +contestId, candidateIds}));
        await api(await tok(b.officer), 'POST', `/elections/stations/${b.station.id}/cast`, {studentId: b.studentId, choices});
        if (++cast % 100 === 0) console.log(`  ${cast} ballots cast...`);
    });
    console.log(`Ballots cast: ${cast}.`);

    await api(manager, 'POST', `/elections/${electionId}/stage`, {toStage: 4, note: 'Polling closed at all stations; counting under way.'});

    // ---------- 8. Results: lots for the tie, then declared ----------
    const results = await api(manager, 'GET', `/elections/${electionId}/results`);
    for (const r of results.races.filter((x) => x.tied)) {
        const order = shuffle(r.candidates.filter((c) => c.inTie).map((c) => c.candidateId));
        await api(manager, 'POST', `/elections/contests/${r.contestId}/tiebreak`, {candidateIds: order});
        console.log(`Tie in ${r.name} (${r.positionName}): settled by drawing lots.`);
    }
    await api(manager, 'POST', `/elections/${electionId}/stage`, {toStage: 5, note: 'Results announced at the school assembly.'});

    // ---------- 9. Winners into the leadership records ----------
    const recorded = await api(manager, 'POST', `/elections/${electionId}/leadership`, {
        positions: positions.map((p) => ({
            positionId: p.dto.id,
            classLeadershipRoleId: p.kind === 'classPresident' ? +presidentRole.Id : null,
            responsibilityId: +schoolLeadership.Id
        }))
    });
    console.log(`Leadership records added: ${recorded.added}.`);

    // ---------- 10. Back to January ----------
    console.log('Setting the dates back...');
    const open = addDays(poll, -16); // the Tuesday two weeks and two days before polling
    const created = addDays(open, -1);
    const close = addDays(poll, -6);
    const stationsDay = addDays(poll, -3);
    const next = addDays(poll, 1);
    // Nominations through the fortnight, decisions on the last day.
    const nominationAt = nominated.map((n, i) => {
        const d = addDays(open, Math.min(8, Math.floor((i / nominated.length) * 9)));
        return `WHEN ${n.candidateId} THEN '${utc(d, 9 + Math.floor(rnd() * 6), Math.floor(rnd() * 60))}'`;
    }).join(' ');
    // Each station votes in its slot through the morning, a ballot a minute or so apart.
    const slots = stations.slice().sort((x, y) => x.name.localeCompare(y.name, undefined, {numeric: true}));
    const castRows = query(`SELECT Id, ElectionStationId FROM electionvotecasts WHERE ElectionId = ${electionId} ORDER BY Id`);
    const castAt = castRows.map((r) => {
        const i = slots.findIndex((s) => s.id === +r.ElectionStationId);
        const n = castRows.filter((x) => x.ElectionStationId === r.ElectionStationId && +x.Id <= +r.Id).length;
        const minutes = 8 * 60 + 10 + (i % 6) * 45 + Math.floor(i / 6) * 8 + Math.round(n * 1.1);
        return `WHEN ${r.Id} THEN '${utc(poll, Math.floor(minutes / 60), minutes % 60, Math.floor(rnd() * 60))}'`;
    }).join(' ');
    exec(`
UPDATE elections SET Created = '${utc(created, 9, 10)}', Modified = '${utc(poll, 13, 30)}', CreatedBy = ${q(opt.manager)},
    RegisterFrozenAt = '${utc(close, 15, 0)}', DeclaredAt = '${utc(poll, 13, 30)}' WHERE Id = ${electionId};
UPDATE electionstagechanges SET Created = CASE ToStage
    WHEN 1 THEN '${utc(open, 8, 30)}' WHEN 2 THEN '${utc(close, 15, 0)}' WHEN 3 THEN '${utc(poll, 8, 0)}'
    WHEN 4 THEN '${utc(poll, 13, 5)}' WHEN 5 THEN '${utc(poll, 13, 30)}' END, Modified = Created WHERE ElectionId = ${electionId};
UPDATE electionpositions SET Created = '${utc(created, 9, 25)}', Modified = Created WHERE ElectionId = ${electionId};
UPDATE electionpositionlevels l JOIN electionpositions p ON p.Id = l.ElectionPositionId SET l.Created = p.Created, l.Modified = p.Created WHERE p.ElectionId = ${electionId};
UPDATE electioncontests c JOIN electionpositions p ON p.Id = c.ElectionPositionId SET c.Created = p.Created, c.Modified = p.Created WHERE p.ElectionId = ${electionId};
UPDATE electioncandidates SET Created = CASE Id ${nominationAt} END,
    Modified = '${utc(close, 11, 0)}' WHERE Id IN (${nominated.map((n) => n.candidateId).join(',')});
UPDATE electioncandidates SET LeadershipRecordedAt = '${utc(next, 10, 0)}' WHERE LeadershipRecordedAt IS NOT NULL
    AND Id IN (${nominated.map((n) => n.candidateId).join(',')});
UPDATE electionvoters SET Created = '${utc(close, 15, 0)}', Modified = Created WHERE ElectionId = ${electionId};
UPDATE electionstations SET Created = '${utc(stationsDay, 10, 0)}', Modified = Created WHERE ElectionId = ${electionId};
UPDATE electionstationclasses c JOIN electionstations s ON s.Id = c.ElectionStationId SET c.Created = s.Created, c.Modified = s.Created WHERE s.ElectionId = ${electionId};
UPDATE electionvotecasts SET Created = CASE Id ${castAt} END, Modified = Created WHERE ElectionId = ${electionId};
UPDATE schoolclassleaders SET Created = '${utc(next, 10, 0)}', Modified = Created, CreatedBy = ${q(opt.manager)}, ModifiedBy = ${q(opt.manager)}
    WHERE Description LIKE '% - elected, ${name.replace(/'/g, "''")}';
UPDATE studentresponsibilities SET Created = '${utc(next, 10, 0)}', Modified = Created, CreatedBy = ${q(opt.manager)}, ModifiedBy = ${q(opt.manager)}
    WHERE Description LIKE '% - elected, ${name.replace(/'/g, "''")}';
`);

    const summary = query(`SELECT (SELECT COUNT(*) FROM electionvoters WHERE ElectionId = ${electionId}) registered,
        (SELECT COUNT(*) FROM electionvotecasts WHERE ElectionId = ${electionId}) voted,
        (SELECT COUNT(*) FROM electioncandidates c JOIN electioncontests x ON x.Id = c.ElectionContestId
            JOIN electionpositions p ON p.Id = x.ElectionPositionId WHERE p.ElectionId = ${electionId} AND c.IsElected) elected`)[0];
    console.log(`Done: ${summary.voted} of ${summary.registered} voted (${Math.round(100 * summary.voted / summary.registered)}%), ` +
        `${summary.elected} elected.`);
}

main().catch((err) => {
    console.error('Error:', err);
    process.exit(1);
});
