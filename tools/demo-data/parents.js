#!/usr/bin/env node
// Fills an anonymised demo copy with the learners' parents and guardians:
//  - families: learners who share a surname are sometimes brothers and sisters in
//    different classes, with the same parents;
//  - most learners have a father (who carries the family surname) and a mother;
//    some only a mother or a father, a few a guardian (a grandmother, aunt or uncle);
//  - each parent has an invented Kenyan mobile number, an occupation,
//    a postal address near the coast and an age to suit their children; some an
//    email (always @example.com). Names follow the family's religion;
//  - who pays the fees, who picks the child up and who gets the school's SMS are
//    set as a school would record them.
// Everything goes through the API as the school administrator (the parent form's
// own endpoints), dated when the eldest child was admitted.
//
//   MYSQL_PWD=... node tools/demo-data/parents.js --db shulenova --api http://localhost:5198
//
// Options: --db <database> (required; refuses names of live schools)
//          --api <API origin> (default http://localhost:5198)
//          --mysql <path to mysql.exe> (default MySQL 8.4 on Windows)
//          --user <mysql user> (default root)
//          --password <the demo logins' password> (default Demo@2026)
//          --clerk <user name of whoever enters them> (default mohamed, the administrator)
//          --seed <number> (default 2026)
// Every parent in the copy is deleted first, so it can be run again. The phone
// numbers are invented; the demo has SMS and email switched off (anonymize.js).

const {execFileSync} = require('child_process');

const opt = {mysql: 'C:/Program Files/MySQL/MySQL Server 8.4/bin/mysql.exe', user: 'root', api: 'http://localhost:5198',
    password: 'Demo@2026', clerk: 'mohamed', seed: '2026'};
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
let seed = (+opt.seed + 59) >>> 0;
function rnd() {
    seed = (seed + 0x6d2b79f5) >>> 0;
    let t = seed;
    t = Math.imul(t ^ (t >>> 15), t | 1);
    t ^= t + Math.imul(t ^ (t >>> 7), t | 61);
    return ((t ^ (t >>> 14)) >>> 0) / 4294967296;
}
const pick = (list) => list[Math.floor(rnd() * list.length)];
const chance = (p) => rnd() < p;
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
const pad = (n, w = 2) => String(n).padStart(w, '0');

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

async function pool(items, size, fn) {
    let next = 0;
    await Promise.all(Array.from({length: size}, async () => {
        while (next < items.length) await fn(items[next++]);
    }));
}

// ---------- Names (only the anonymiser's, split by religion where it shows - others may match
// the real school's names, which check.js flags) ----------
const MALE = {
    muslim: ['Abdalla', 'Ali', 'Hassan', 'Juma', 'Bakari', 'Omari', 'Salim', 'Rashid', 'Hamisi', 'Saidi', 'Yusuf', 'Ibrahim',
        'Musa', 'Idris', 'Khalid', 'Faraji', 'Zuberi', 'Nassoro', 'Shabani', 'Ramadhani', 'Athumani', 'Mbwana', 'Kassim', 'Hemedi', 'Mohamed'],
    christian: ['Kahindi', 'Kazungu', 'Baya', 'Charo', 'Katana', 'Mwangi', 'Kamau', 'Otieno', 'Kiprono', 'Wafula', 'Mutua', 'Daniel',
        'David', 'Joseph', 'Samuel', 'Peter', 'John', 'James', 'Emmanuel', 'Felix', 'Moses', 'Isaac', 'Stephen', 'George', 'Martin',
        'Patrick', 'Gideon', 'Caleb', 'Elijah', 'Joel', 'Amani', 'Brian', 'Kevin', 'Dennis', 'Victor', 'Collins']
};
const FEMALE = {
    muslim: ['Amina', 'Fatuma', 'Mwanaisha', 'Zainabu', 'Rehema', 'Halima', 'Mariamu', 'Saida', 'Mwanajuma', 'Asha', 'Khadija',
        'Riziki', 'Salma', 'Hadija', 'Aisha', 'Nasra', 'Mwanahawa', 'Tatu', 'Mwanamisi', 'Siti', 'Zuhura', 'Rukia', 'Hawa', 'Maryam'],
    christian: ['Kadzo', 'Dama', 'Sidi', 'Mapenzi', 'Wanjiku', 'Njeri', 'Akinyi', 'Atieno', 'Chebet', 'Nafula', 'Mwende', 'Mueni',
        'Faith', 'Mercy', 'Grace', 'Joy', 'Esther', 'Ruth', 'Lucy', 'Purity', 'Neema', 'Zawadi', 'Tabitha', 'Winnie', 'Lilian',
        'Caroline', 'Agnes', 'Beatrice', 'Doreen', 'Eunice', 'Jackline', 'Sharon', 'Diana', 'Brenda', 'Cynthia']
};
const SURNAMES = ['Mwamburi', 'Mwakio', 'Kombo', 'Mwinyi', 'Bakari', 'Hamisi', 'Salim', 'Juma', 'Mohamed', 'Athumani',
    'Kahindi', 'Kazungu', 'Charo', 'Katana', 'Baya', 'Mwaro', 'Ngala', 'Tsuma', 'Mwangi', 'Kamau', 'Njoroge', 'Odhiambo',
    'Otieno', 'Ochieng', 'Wekesa', 'Wafula', 'Barasa', 'Mutua', 'Musyoka', 'Kiptoo', 'Rotich', 'Kiprop', 'Chege', 'Gitau',
    'Onyango', 'Okoth', 'Mwakazi', 'Mwasaru', 'Nzai', 'Mramba', 'Dzombo', 'Mbaru', 'Kitsao', 'Ruwa', 'Mwangemi', 'Chai',
    'Rashid', 'Abdalla', 'Saidi', 'Omari', 'Mwachai', 'Mwadzaya', 'Kalama', 'Karisa', 'Kenga', 'Masha', 'Menza', 'Nyale'];
// Coastal post offices, with their postal codes.
const POST_OFFICES = [['Mombasa', '80100', 30], ['Kilifi', '80108', 18], ['Mtwapa', '80109', 14], ['Malindi', '80200', 8],
    ['Mariakani', '80113', 8], ['Kwale', '80403', 6], ['Watamu', '80202', 4], ['Voi', '80300', 3], ['Nairobi', '00100', 3]];
// Kenyan mobile prefixes (Safaricom and Airtel), the 07 range far more common than 01.
const PREFIXES = [...['0710', '0711', '0712', '0713', '0714', '0715', '0716', '0717', '0718', '0719', '0720', '0721', '0722',
    '0723', '0724', '0725', '0726', '0727', '0728', '0729', '0740', '0741', '0742', '0743', '0745', '0746', '0748', '0757',
    '0758', '0759', '0768', '0769', '0790', '0791', '0792', '0793', '0794', '0795', '0796', '0797', '0798', '0799'].map((p) => [p, 5]),
    ...['0730', '0731', '0732', '0733', '0734', '0735', '0736', '0737', '0738', '0739', '0750', '0751', '0752', '0753', '0754',
        '0755', '0756', '0785', '0786', '0787', '0788', '0789'].map((p) => [p, 2]),
    ...['0110', '0111', '0112', '0113', '0114', '0115', '0100', '0101', '0102'].map((p) => [p, 1])];

const usedPhones = new Set();
function phone() {
    for (;;) {
        const p = weighted(PREFIXES) + pad(Math.floor(rnd() * 1e6), 6);
        if (!usedPhones.has(p)) {
            usedPhones.add(p);
            return p;
        }
    }
}
const upper = (s) => s.toUpperCase();

async function main() {
    // ---------- 1. The learners and the lookups ----------
    const learners = query(`SELECT p.Id, p.FullName, p.GenderId, p.ReligionId, p.NationalityId, p.DateOfBirth, p.AdmissionDate,
            (SELECT sc.SchoolClassId FROM studentclasses sc WHERE sc.StudentId = p.Id ORDER BY sc.Id DESC LIMIT 1) classId
        FROM person p WHERE p.Discriminator = 'Student' AND p.Status = 0 ORDER BY p.Id`);
    const religions = Object.fromEntries(query(`SELECT Id, Name FROM religions`).map((r) => [r.Id, r.Name]));
    const isMuslim = (religionId) => /islam|muslim/i.test(religions[religionId] || '');
    const genders = Object.fromEntries(query(`SELECT Id, Name FROM genders`).map((g) => [g.Name, +g.Id]));
    const relationships = Object.fromEntries(query(`SELECT Id, Name FROM relationships`).map((r) => [r.Name, +r.Id]));
    const occupations = Object.fromEntries(query(`SELECT Id, Name FROM occupations`).map((o) => [o.Name, +o.Id]));
    const kenyan = +(query(`SELECT Id FROM nationalities WHERE Name = 'Kenyan'`)[0]?.Id ?? learners[0]?.NationalityId);
    if (!relationships.Parent || !relationships.Guardian) fail('the "Parent" and "Guardian" relationships are missing');
    const occupation = () => occupations[weighted([['Business', 40], ['Others', 30], ['Teacher', 12], ['Accountant', 6],
        ['Engineer', 5], ['Doctor', 3]].filter(([n]) => occupations[n]))];
    console.log(`${learners.length} active learners.`);

    // ---------- 2. Clear ----------
    console.log(`Clearing parents in ${opt.db}...`);
    exec(`DELETE FROM studentparents;
DELETE FROM person WHERE Discriminator = 'Parent';`);

    // ---------- 3. Families ----------
    // Learners with the same surname and religion, in different classes, are sometimes
    // brothers and sisters: about one learner in five shares a family.
    const surname = (l) => l.FullName.trim().split(/\s+/).slice(-1)[0];
    const bySurname = {};
    for (const l of shuffle(learners)) (bySurname[`${surname(l)}|${isMuslim(l.ReligionId)}`] ??= []).push(l);
    const families = [];
    for (const group of Object.values(bySurname)) {
        const left = group.slice();
        while (left.length) {
            const first = left.shift();
            const children = [first];
            const size = weighted([[1, 80], [2, 15], [3, 5]]);
            for (let i = left.length - 1; i >= 0 && children.length < size; i--) {
                if (children.every((c) => c.classId !== left[i].classId)) children.push(left.splice(i, 1)[0]);
            }
            families.push(children);
        }
    }

    // Who each family has.
    let parentNo = 0;
    const parents = []; // {dto, children: [{studentId, relationShipId, otherDetails}], admitted}
    for (const children of families) {
        const eldest = children.slice().sort((x, y) => (x.DateOfBirth || '9999').localeCompare(y.DateOfBirth || '9999'))[0];
        const childBorn = eldest.DateOfBirth ? +eldest.DateOfBirth.slice(0, 4) : 2016;
        const admitted = children.map((c) => c.AdmissionDate).filter(Boolean).sort()[0] || null;
        const muslim = isMuslim(eldest.ReligionId);
        const faith = muslim ? 'muslim' : 'christian';
        const family = upper(surname(eldest));
        const office = weighted(POST_OFFICES.map(([t, c, w]) => [[t, c], w]));
        const address = `P.O. Box ${10 + Math.floor(rnd() * 9000)} - ${office[1]}, ${office[0]}`;
        const shape = weighted([['both', 70], ['mother', 17], ['father', 5], ['guardian', 8]]);

        const person = (gender, name, born, extra) => ({
            fullName: name,
            upi: `PRT${pad(++parentNo, 4)}`,
            dateOfBirth: `${born}-${pad(1 + Math.floor(rnd() * 12))}-${pad(1 + Math.floor(rnd() * 28))}T00:00:00`,
            address,
            phoneNumber: chance(0.97) ? phone() : null,
            email: null,
            status: 0,
            nationalityId: kenyan,
            religionId: +eldest.ReligionId,
            genderId: genders[gender],
            notifiable: false,
            payer: false,
            pickup: false,
            occupationId: occupation(),
            ...extra
        });
        const given = (gender) => pick((gender === 'Male' ? MALE : FEMALE)[faith]);
        // A first name and, sometimes, a second one - never the same twice, nor the surname.
        const names = (gender, surname) => {
            let first = given(gender);
            while (first.toUpperCase() === (surname || '').toUpperCase()) first = given(gender);
            let second = chance(0.4) ? given(gender) : null;
            if (second === first || (second || '').toUpperCase() === (surname || '').toUpperCase()) second = null;
            return second ? `${first} ${second}` : first;
        };
        const withEmail = (p) => {
            if (chance(0.3)) {
                const [f, ...rest] = p.fullName.toLowerCase().split(' ');
                p.email = `${f}.${rest.slice(-1)[0]}${Math.floor(rnd() * 90) + 10}@example.com`;
            }
            return p;
        };

        const fatherBorn = childBorn - (26 + Math.floor(rnd() * 15));
        const motherBorn = childBorn - (21 + Math.floor(rnd() * 13));
        const link = (relationShipId, otherDetails) => children.map((c) => ({studentId: +c.Id, relationShipId, otherDetails}));
        const added = [];
        if (shape === 'both' || shape === 'father') {
            const name = upper(`${names('Male', family)} ${family[0] + family.slice(1).toLowerCase()}`);
            added.push({dto: withEmail(person('Male', name, fatherBorn, {})), children: link(relationships.Parent, 'Father'), role: 'father'});
        }
        if (shape === 'both' || shape === 'mother') {
            // A mother often keeps her own surname, Muslim mothers nearly always.
            const own = chance(muslim ? 0.85 : 0.45) ? pick(SURNAMES) : family[0] + family.slice(1).toLowerCase();
            const name = upper(`${names('Female', own)} ${own}`);
            added.push({dto: withEmail(person('Female', name, motherBorn, {})), children: link(relationships.Parent, 'Mother'), role: 'mother'});
        }
        if (shape === 'guardian') {
            const [kind, gender, born] = weighted([[['Grandmother', 'Female', childBorn - 50 - Math.floor(rnd() * 15)], 50],
                [['Aunt', 'Female', motherBorn - 3 + Math.floor(rnd() * 8)], 30], [['Uncle', 'Male', fatherBorn - 3 + Math.floor(rnd() * 8)], 20]]);
            const theirs = kind === 'Aunt' ? pick(SURNAMES) : family[0] + family.slice(1).toLowerCase();
            const name = upper(`${names(gender, theirs)} ${theirs}`);
            added.push({dto: withEmail(person(gender, name, born, kind === 'Grandmother' ? {occupationId: occupations.Others ?? occupation()} : {})),
                children: link(relationships.Guardian, kind), role: 'guardian'});
        }
        // The fees fall to the father where there is one; the mother usually gets the
        // school's messages and collects the children; a lone parent or guardian does all three.
        const father = added.find((p) => p.role === 'father'), mother = added.find((p) => p.role === 'mother');
        const only = added.length === 1 ? added[0] : null;
        if (only) Object.assign(only.dto, {payer: true, pickup: true, notifiable: true});
        else {
            father.dto.payer = true;
            mother.dto.payer = chance(0.25);
            mother.dto.notifiable = true;
            father.dto.notifiable = chance(0.6);
            mother.dto.pickup = true;
            father.dto.pickup = chance(0.5);
        }
        // Nobody gets messages without a phone.
        for (const p of added) if (!p.dto.phoneNumber) p.dto.notifiable = false;
        for (const p of added) parents.push({...p, admitted});
    }
    console.log(`Families: ${families.length} (${families.filter((f) => f.length > 1).length} with brothers or sisters), parents and guardians: ${parents.length}.`);

    // ---------- 4. Entered through the API ----------
    const token = (await api(null, 'POST', '/auth/login', {userName: opt.clerk, password: opt.password})).token;
    let done = 0;
    await pool(parents, 6, async (p) => {
        const created = await api(token, 'POST', '/parents', p.dto);
        p.id = created.id;
        for (const c of p.children) await api(token, 'POST', '/students/studentParent', {...c, parentId: p.id});
        if (++done % 200 === 0) console.log(`  ${done} entered...`);
    });

    // ---------- 5. Dated when the eldest child was admitted ----------
    console.log('Setting the dates back...');
    // Kenyan office hours (UTC+3), stored as UTC: 08:30 to 15:30 local is 05:30 to 12:30.
    const when = (p) => {
        const day = p.admitted ? p.admitted.slice(0, 10) : '2026-01-05';
        return `'${day} ${pad(5 + Math.floor(rnd() * 7))}:${pad(Math.floor(rnd() * 60))}:${pad(Math.floor(rnd() * 60))}'`;
    };
    const cases = parents.map((p) => `WHEN ${p.id} THEN ${when(p)}`).join(' ');
    exec(`UPDATE person SET Created = CASE Id ${cases} END, Modified = Created WHERE Discriminator = 'Parent';
UPDATE studentparents sp JOIN person p ON p.Id = sp.ParentId SET sp.Created = p.Created, sp.Modified = p.Created;`);

    const s = query(`SELECT (SELECT COUNT(*) FROM person WHERE Discriminator = 'Parent') parents,
        (SELECT COUNT(*) FROM studentparents) links,
        (SELECT COUNT(*) FROM person s WHERE s.Discriminator = 'Student' AND s.Status = 0
            AND NOT EXISTS (SELECT 1 FROM studentparents sp WHERE sp.StudentId = s.Id)) without`)[0];
    console.log(`Done: ${s.parents} parents and guardians, ${s.links} links to learners, ${s.without} active learners without one.`);
}

main().catch((err) => {
    console.error('Error:', err);
    process.exit(1);
});
