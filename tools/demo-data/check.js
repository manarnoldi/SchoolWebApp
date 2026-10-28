#!/usr/bin/env node
// Checks an anonymised copy for anything left from the original school: it
// dumps the copy and looks for every original name part, phone number, email,
// ID number and the school's name, and reports each one found, by table.
//
//   MYSQL_PWD=... node tools/demo-data/check.js --source swikunda --db shulenova
//
// Name parts that the anonymiser itself uses (Juma, Salim...) are skipped -
// they are common names, and a match on them means nothing.

const {execFileSync} = require('child_process');
const fs = require('fs');
const path = require('path');

const opt = {bin: 'C:/Program Files/MySQL/MySQL Server 8.4/bin', user: 'root'};
const a = process.argv.slice(2);
for (let i = 0; i < a.length; i += 2) opt[a[i].replace(/^--/, '')] = a[i + 1];
if (!opt.source || !opt.db) { console.error('--source and --db are required'); process.exit(1); }

const mysql = (db, sql) => execFileSync(path.join(opt.bin, 'mysql.exe'), ['-u', opt.user, '--default-character-set=utf8mb4', '-N', '-B', '--raw', db, '-e', sql],
    {encoding: 'utf8', maxBuffer: 256 * 1024 * 1024}).split(/\r?\n/).filter(Boolean).map((l) => l.split('\t'));

// The anonymiser's own name lists.
const src = fs.readFileSync(path.join(__dirname, 'anonymize.js'), 'utf8');
const ours = new Set((src.match(/'[A-Z][a-z]+'/g) || []).map((s) => s.slice(1, -1).toUpperCase()));
const TITLES = new Set(['MR', 'MRS', 'MS', 'MD', 'MISS', 'DR', 'MADAM', 'TR', 'ADMIN', 'SIR']);

const needles = new Map(); // text -> what it is
// Placeholders such as 000000 are skipped.
const add = (v, kind) => { if (v && v.length >= 4 && !/^(.)\1+$/.test(v) && !needles.has(v.toUpperCase())) needles.set(v.toUpperCase(), kind); };
for (const [full, phone, email, idno, kra, nhif, nssf, upi, disc, addr] of mysql(opt.source,
    'SELECT FullName, PhoneNumber, Email, IdNumber, KraPinNo, NhifNo, NssfNo, UPI, Discriminator, Address FROM person WHERE FullName <> \x27Admin\x27')) {
    for (const part of (full || '').toUpperCase().replace(/[.,]/g, ' ').split(/\s+/))
        if (part.length >= 4 && !TITLES.has(part) && !ours.has(part)) add(part, 'name part');
    for (const v of [phone, idno, kra, nhif, nssf].filter((x) => x && x !== 'NULL' && x !== 'Admin')) add(v.replace(/\s/g, ''), 'number');
    if (phone && phone.length >= 9) add(phone.replace(/\D/g, '').slice(-9), 'phone');
    if (email && email.includes('@') && !email.endsWith('kodetek.co.ke')) add(email, 'email');
    if (disc === 'StaffDetails' && upi && upi.length >= 5) add(upi, 'staff number');
}
for (const [u, e] of mysql(opt.source, 'SELECT UserName, Email FROM aspnetusers')) {
    if (u !== 'admin' && u.length >= 4 && !ours.has(u.toUpperCase())) add(u, 'login');
    if (e && !e.startsWith('admin@')) add(e, 'email');
}
add('SWIKUNDA', 'school'); add('UKUNDA', 'school');

// Dump the copy, one row per line, and search it.
const dump = execFileSync(path.join(opt.bin, 'mysqldump.exe'), ['-u', opt.user, '--default-character-set=utf8mb4', '--skip-extended-insert',
    '--no-create-info', '--skip-comments', opt.db], {encoding: 'utf8', maxBuffer: 1024 * 1024 * 1024});
const CURRICULUM = /^(__efmigrationshistory|assessmentrubrics|broadoutcomes|curriculumdesign.*|strands|substrands|substrand.*|generaloutcomes|specificoutcomes|outcomes|learningexperience|keyquestion|pci|values|competencies|themes|learningoutcomes|suggested.*)$/;
const hits = new Map();
const words = [...needles.keys()].sort((x, y) => y.length - x.length);
const re = new RegExp(words.map((w) => w.replace(/[.*+?^${}()|[\]\\]/g, '\\$&')).join('|'), 'gi');
for (const line of dump.split('\n')) {
    if (!line.startsWith('INSERT INTO')) continue;
    const table = line.match(/^INSERT INTO `([^`]+)`/)[1];
    // KICD curriculum content (shared by every school) and the migrations list are not the school's data.
    if (CURRICULUM.test(table)) continue;
    for (const m of line.matchAll(re)) {
        const w = m[0].toUpperCase();
        // A name part must stand as a word (MASHA is a name, not part of "WAMASHAMBA").
        if (needles.get(w) === 'name part' && /[A-Za-z]/.test((line[m.index - 1] || '') + (line[m.index + w.length] || ''))) continue;
        const k = `${table}: ${w} (${needles.get(w)})`;
        hits.set(k, (hits.get(k) || 0) + 1);
    }
}
console.log(`Checked ${opt.db} for ${needles.size} identifiers from ${opt.source}.`);
if (!hits.size) console.log('Nothing found.');
else [...hits.entries()].sort().forEach(([k, n]) => console.log(`  ${k} x${n}`));
