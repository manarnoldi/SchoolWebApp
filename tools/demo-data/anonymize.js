#!/usr/bin/env node
// Turns a copy of a school's database into demo data: every student and staff
// member gets an invented name, phone, email, address and ID numbers; user
// logins follow their staff member; logs and message history are cleared; the
// school becomes "ShuleNova Demo School". Marks, fees, payroll, timetables and
// teaching documents are kept, so the demo shows a real, working school.
//
// Run it on a COPY, never on a school's own database:
//
//   MYSQL_PWD=... node tools/demo-data/anonymize.js --db shulenova
//
// Options: --db <database> (required; refuses names of live schools)
//          --mysql <path to mysql.exe> (default MySQL 8.4 on Windows)
//          --user <mysql user> (default root)
//          --password <demo password for every login except admin> (default Demo@2026)
//          --licence <licence key to install> (optional)
//          --seed <number> (same seed, same names; default 2026)
// The SQL it runs is saved next to this script as last-run.sql (git-ignored).

const {execFileSync} = require('child_process');
const crypto = require('crypto');
const fs = require('fs');
const path = require('path');

const opt = {mysql: 'C:/Program Files/MySQL/MySQL Server 8.4/bin/mysql.exe', user: 'root', password: 'Demo@2026', seed: '2026'};
const a = process.argv.slice(2);
for (let i = 0; i < a.length; i += 2) opt[a[i].replace(/^--/, '')] = a[i + 1];
if (!opt.db) fail('--db is required');
if (/swikunda/i.test(opt.db)) fail(`refusing to anonymise "${opt.db}" - run this on a copy`);

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

function run(file) {
    execFileSync(opt.mysql, ['-u', opt.user, '--default-character-set=utf8mb4', opt.db],
        {input: fs.readFileSync(file), stdio: ['pipe', 'inherit', 'inherit']});
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
const digit = () => String(Math.floor(rnd() * 10));
const letter = () => String.fromCharCode(65 + Math.floor(rnd() * 26));

// Same shape, new characters: digits stay digits, letters stay letters; the
// first `keep` characters (e.g. "07" of a phone number) are kept.
function reshape(v, keep = 0) {
    if (!v || !v.trim() || /^admin$/i.test(v)) return v;
    return v.slice(0, keep) + v.slice(keep).replace(/[0-9]/g, digit).replace(/[A-Za-z]/g, letter);
}

// ---------- Names ----------
const MALE = ['Abdalla', 'Ali', 'Hassan', 'Juma', 'Bakari', 'Omari', 'Salim', 'Rashid', 'Hamisi', 'Saidi', 'Kahindi', 'Kazungu',
    'Baya', 'Charo', 'Katana', 'Mwangi', 'Kamau', 'Otieno', 'Kiprono', 'Wafula', 'Mutua', 'Brian', 'Kevin', 'Dennis', 'Victor',
    'Collins', 'Elvis', 'Ian', 'Ryan', 'Ethan', 'Jayden', 'Daniel', 'David', 'Joseph', 'Samuel', 'Peter', 'John', 'James',
    'Emmanuel', 'Felix', 'Allan', 'Lewis', 'Trevor', 'Moses', 'Isaac', 'Yusuf', 'Ibrahim', 'Musa', 'Idris', 'Khalid', 'Faraji',
    'Zuberi', 'Baraka', 'Jabari', 'Nassoro', 'Shabani', 'Ramadhani', 'Athumani', 'Mbwana', 'Kassim', 'Hemedi', 'Mohamed',
    'Stephen', 'George', 'Martin', 'Patrick', 'Kelvin', 'Gideon', 'Caleb', 'Nathan', 'Elijah', 'Joel', 'Amani', 'Tony', 'Mark'];
const FEMALE = ['Amina', 'Fatuma', 'Mwanaisha', 'Zainabu', 'Rehema', 'Halima', 'Mariamu', 'Saida', 'Mwanajuma', 'Asha',
    'Khadija', 'Riziki', 'Kadzo', 'Dama', 'Sidi', 'Mapenzi', 'Wanjiku', 'Njeri', 'Akinyi', 'Atieno', 'Chebet', 'Nafula',
    'Mwende', 'Mueni', 'Faith', 'Mercy', 'Grace', 'Joy', 'Sharon', 'Diana', 'Esther', 'Ruth', 'Lucy', 'Ivy', 'Natasha',
    'Michelle', 'Brenda', 'Cynthia', 'Purity', 'Neema', 'Salma', 'Hadija', 'Aisha', 'Nasra', 'Zawadi', 'Imani', 'Tabitha',
    'Precious', 'Mwanahawa', 'Subira', 'Tatu', 'Mwanamisi', 'Nuru', 'Siti', 'Zuhura', 'Rukia', 'Hawa', 'Maryam', 'Winnie',
    'Lilian', 'Caroline', 'Agnes', 'Beatrice', 'Doreen', 'Eunice', 'Jackline', 'Valentine', 'Sheila', 'Stacy', 'Abigail'];
const SURNAMES = ['Mwamburi', 'Mwakio', 'Kombo', 'Mwinyi', 'Bakari', 'Hamisi', 'Salim', 'Juma', 'Mohamed', 'Athumani',
    'Kahindi', 'Kazungu', 'Charo', 'Katana', 'Baya', 'Mwaro', 'Ngala', 'Tsuma', 'Mwangi', 'Kamau', 'Njoroge', 'Odhiambo',
    'Otieno', 'Ochieng', 'Wekesa', 'Wafula', 'Barasa', 'Mutua', 'Musyoka', 'Kiptoo', 'Rotich', 'Kiprop', 'Chege', 'Gitau',
    'Onyango', 'Okoth', 'Mwakazi', 'Mwasaru', 'Nzai', 'Mramba', 'Dzombo', 'Mbaru', 'Kitsao', 'Ruwa', 'Mwangemi', 'Chai',
    'Rashid', 'Abdalla', 'Saidi', 'Omari', 'Mwachai', 'Mwadzaya', 'Kalama', 'Karisa', 'Kenga', 'Masha', 'Menza', 'Nyale',
    'Shume', 'Tunje', 'Mwatela', 'Wanjala', 'Simiyu', 'Nyongesa', 'Kilonzo', 'Muthoka', 'Ndungu', 'Karanja', 'Langat',
    'Cheruiyot', 'Owino', 'Achieng', 'Mbugua', 'Maina', 'Kariuki', 'Mwakidudu', 'Mwanzia', 'Swaleh', 'Haji', 'Bwana'];
const TOWNS = ['Mombasa', 'Kilifi', 'Malindi', 'Voi', 'Kwale', 'Nairobi', 'Nakuru', 'Machakos', 'Kisumu', 'Eldoret', 'Thika',
    'Nyeri', 'Kitui', 'Mtwapa', 'Watamu', 'Mariakani', 'Kakamega', 'Embu', 'Naivasha', 'Kericho'];
const TITLE = /^((?:MR|MRS|MS|MD|MISS|DR|MADAM|TR|MADAME|SIR)\.?)\s+/i;

const used = new Set();
// An invented name with the same number of parts, title and capitalisation.
function newName(full, female) {
    if (!full || /^admin$/i.test(full.trim())) return null;
    const m = full.trim().match(TITLE);
    const title = m ? m[1] : '';
    const rest = full.trim().slice(m ? m[0].length : 0).split(/\s+/).filter(Boolean);
    const upper = rest.join(' ') === rest.join(' ').toUpperCase();
    const n = Math.max(1, Math.min(rest.length, 4));
    const first = female ? FEMALE : MALE;
    for (let tries = 0; ; tries++) {
        const parts = [pick(first)];
        if (n >= 3) parts.push(rnd() < 0.5 ? pick(first) : pick(SURNAMES));
        if (n >= 4) parts.push(pick(SURNAMES));
        if (n >= 2) parts.push(pick(SURNAMES));
        if (new Set(parts).size < parts.length) continue;
        let name = parts.join(' ');
        if (upper) name = name.toUpperCase();
        if (used.has(name) && tries < 200) continue;
        used.add(name);
        return {full: (title ? title + ' ' : '') + name, title, parts};
    }
}

// A title (Mr., Md., Mrs.) wins over the recorded gender, which is sometimes wrong.
function isFemale(genderId, full) {
    const t = (full || '').match(TITLE);
    if (t && !/^(DR|TR)/i.test(t[1])) return /^(MD|MRS|MS|MISS|MADAM|MADAME)/i.test(t[1]);
    return genderId === '2';
}

// ASP.NET Core Identity v3 hash: PBKDF2 HMAC-SHA512, 100,000 iterations.
function identityHash(password) {
    const salt = crypto.randomBytes(16);
    const key = crypto.pbkdf2Sync(password, salt, 100000, 32, 'sha512');
    const head = Buffer.alloc(13);
    head.writeUInt8(1, 0);
    head.writeUInt32BE(2, 1);
    head.writeUInt32BE(100000, 5);
    head.writeUInt32BE(16, 9);
    return Buffer.concat([head, salt, key]).toString('base64');
}

// ---------- Build the SQL ----------
const sql = ['SET NAMES utf8mb4;', 'SET FOREIGN_KEY_CHECKS = 0;'];
const people = query('SELECT Id, Discriminator, FullName, GenderId, PhoneNumber, Email, Address, IdNumber, KraPinNo, NhifNo, NssfNo, UPI FROM person');
const byPerson = new Map();
let n = 0;
for (const p of people) {
    const name = newName(p.FullName, isFemale(p.GenderId, p.FullName));
    if (!name) continue;
    byPerson.set(p.Id, name);
    const handle = name.parts.map((s) => s.toLowerCase()).filter((_, i, all) => i === 0 || i === all.length - 1).join('.');
    const set = {
        FullName: name.full,
        PhoneNumber: p.PhoneNumber ? reshape(p.PhoneNumber, p.PhoneNumber.startsWith('+') ? 5 : 2) : p.PhoneNumber,
        Email: p.Email && p.Email.trim() ? `${handle}${++n}@example.com` : p.Email,
        Address: p.Address && p.Address.trim() ? `P.O. Box ${100 + Math.floor(rnd() * 9000)}, ${pick(TOWNS)}` : p.Address,
        IdNumber: reshape(p.IdNumber),
        KraPinNo: reshape(p.KraPinNo, 1),
        NhifNo: reshape(p.NhifNo),
        NssfNo: reshape(p.NssfNo),
        // Staff numbers (e.g. TSC numbers) identify a teacher; admission numbers are kept.
        UPI: p.Discriminator === 'StaffDetails' ? reshape(p.UPI) : p.UPI
    };
    // The recorded gender follows the title, so a "Mr." is not shown as female.
    if (name.title && !/^(DR|TR)/i.test(name.title)) set.GenderId = /^(MD|MRS|MS|MISS|MADAM)/i.test(name.title) ? 2 : 1;
    const shift = Math.floor(rnd() * 241) - 120;
    sql.push(`UPDATE person SET ${Object.entries(set).map(([k, v]) => `${k} = ${q(v)}`).join(', ')}, ` +
        `DateOfBirth = DATE_ADD(DateOfBirth, INTERVAL ${shift} DAY), HealthConcerns = NULL, Pickup = NULL, Payer = NULL WHERE Id = ${p.Id};`);
    // Who declared an election's results is stored by full name.
    if (p.Discriminator === 'StaffDetails') sql.push(`UPDATE elections SET DeclaredBy = ${q(name.full)} WHERE DeclaredBy = ${q(p.FullName)};`);
}
// Notes from the old system's import name whoever registered the learner ("regBy=...").
sql.push(`UPDATE studentclasses SET Description = REGEXP_REPLACE(Description, ';? ?regBy=[^;]*', '') WHERE Description LIKE '%regBy=%';`,
    `UPDATE schoolclasses SET Description = REGEXP_REPLACE(Description, ';? ?regBy=[^;]*', '') WHERE Description LIKE '%regBy=%';`);

// Logins follow their staff member; admin is left as it is. Every other login
// gets the demo password, so the demo can sign in as a teacher, head teacher...
const users = query('SELECT Id, UserName, Email, PersonId FROM aspnetusers');
const userMap = [];
const handles = new Set(users.map((u) => u.UserName.toLowerCase()));
for (const u of users) {
    if (u.UserName === 'admin') continue;
    const name = byPerson.get(u.PersonId) || newName(u.UserName, false);
    let handle = name.parts[0].toLowerCase();
    if (handles.has(handle) && handle !== u.UserName.toLowerCase()) handle = (name.parts[0] + '.' + name.parts[name.parts.length - 1]).toLowerCase();
    handles.add(handle);
    const words = name.full.split(' ');
    const email = `${handle}@example.com`;
    sql.push(`UPDATE aspnetusers SET UserName = ${q(handle)}, NormalizedUserName = ${q(handle.toUpperCase())}, Email = ${q(email)}, ` +
        `NormalizedEmail = ${q(email.toUpperCase())}, FirstName = ${q(words.slice(0, -1).join(' ') || words[0])}, LastName = ${q(words[words.length - 1])}, ` +
        `PhoneNumber = NULL, PasswordHash = ${q(identityHash(opt.password))}, SecurityStamp = UUID(), ConcurrencyStamp = UUID(), ` +
        `MustChangePassword = 0, LockoutEnd = NULL, AccessFailedCount = 0 WHERE Id = ${q(u.Id)};`);
    sql.push(`UPDATE person SET Email = ${q(email)} WHERE Id = ${u.PersonId || 0};`);
    userMap.push([u.UserName, handle], [u.Email, handle]);
}

// "Created by" / "modified by" stamps hold user names: point them at the new ones.
sql.push('DROP TABLE IF EXISTS _demo_users;', 'CREATE TABLE _demo_users (Old VARCHAR(256) PRIMARY KEY, New VARCHAR(256));');
const seen = new Set();
for (const [o, nw] of userMap) if (o && !seen.has(o.toLowerCase())) { seen.add(o.toLowerCase()); sql.push(`INSERT INTO _demo_users VALUES (${q(o)}, ${q(nw)});`); }
const stamps = query(`SELECT TABLE_NAME t, COLUMN_NAME c FROM information_schema.COLUMNS WHERE TABLE_SCHEMA = DATABASE()
    AND COLUMN_NAME IN ('CreatedBy','ModifiedBy','DeclaredBy','ReleasedBy','CompleteBy','ResolvedBy','UserName')
    AND DATA_TYPE IN ('varchar','char','text','longtext') AND TABLE_NAME NOT IN ('aspnetusers','_demo_users','logs','auditlogs')`);
// Stamps of logins since deleted, which no longer map to anyone, become admin.
const keep = ['admin', 'system', 'migration', 'legacy-migration', 'jss-link-seed', 'manual-update', ...new Set(userMap.map(([, h]) => h))].map(q).join(', ');
for (const s of stamps) {
    sql.push(`UPDATE \`${s.t}\` x JOIN _demo_users m ON x.\`${s.c}\` = m.Old COLLATE utf8mb4_general_ci SET x.\`${s.c}\` = m.New;`);
    if (s.c !== 'UserName') sql.push(`UPDATE \`${s.t}\` SET \`${s.c}\` = 'admin' WHERE \`${s.c}\` <> '' AND \`${s.c}\` NOT IN (${keep});`);
}
sql.push('DROP TABLE _demo_users;');

// History that names people or records their activity.
sql.push('DELETE FROM auditlogs;', 'DELETE FROM logs;', 'DELETE FROM outboundmessages;', 'DELETE FROM messagebatches;',
    'DELETE FROM aspnetusertokens;', 'DELETE FROM aspnetuserlogins;');

// Messaging can never reach a real person from the demo.
sql.push(`UPDATE communicationsettings SET SmsEnabled = 0, EmailEnabled = 0, TestMode = 1, SmsApiKey = NULL, SmsPartnerId = NULL,
    SmsSenderId = NULL, SmtpHost = NULL, SmtpUsername = NULL, SmtpPassword = NULL, FromEmail = NULL, ReplyToEmail = NULL,
    TestPhoneNumber = NULL, TestEmail = NULL;`);

// The school itself.
const logo = 'data:image/png;base64,' + fs.readFileSync(path.resolve(__dirname, '../../SchoolWebApp-Client/src/assets/img/shule-nova-logo-only.png')).toString('base64');
sql.push(`UPDATE schooldetails SET Name = 'SHULENOVA DEMO SCHOOL', Initials = 'S.D.S', Motto = 'Learning for Life',
    Mission = 'To give every learner the knowledge, skills and values to thrive.', Vision = 'A centre of excellence in competency based education.',
    Address = 'P.O. Box 1000-00100\\nNairobi Kenya', Email = 'info@example.com', Telephone = '0700 000 000', Website = '',
    ReportHeader = 'SHULENOVA DEMO SCHOOL', ReportSubTitle = 'SHULENOVA DEMO SCHOOL', ReportTitle = '', LogoUrl = '', LogoAsBase64 = ${q(logo)};`);

// Other names that would identify the school or its people.
sql.push(`UPDATE sponsors SET Name = CASE Id % 3 WHEN 1 THEN 'Elimu Bora Foundation' WHEN 2 THEN 'Tumaini Bursary Fund' ELSE 'Jamii Scholarship Trust' END,
    ContactName = NULL, Email = NULL, Phone = NULL, Address = NULL;`);
sql.push(`UPDATE formerschools SET SchoolName = ELT(1 + Id % 4, 'Sunrise Academy', 'Hillside Primary School', 'Bahari Junior Academy', 'Greenfield School');`);
sql.push('UPDATE electioncandidates SET PhotoFile = NULL, RunningMatePhotoFile = NULL;');
if (opt.licence) sql.push(`UPDATE globalsettings SET SettingValue = ${q(opt.licence)} WHERE Module = 'Licence' AND SettingKey = 'Key';`);
else sql.push(`DELETE FROM globalsettings WHERE Module = 'Licence' AND SettingKey = 'Key';`);
sql.push('SET FOREIGN_KEY_CHECKS = 1;');

const file = path.join(__dirname, 'last-run.sql');
fs.writeFileSync(file, sql.join('\n') + '\n');
console.log(`Anonymising ${opt.db}: ${byPerson.size} people, ${userMap.length / 2} logins, ${stamps.length} stamp columns...`);
run(file);
console.log(`Done. Logins other than admin use the password ${opt.password}.`);
