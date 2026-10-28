#!/usr/bin/env node
// Issues a ShuleNova licence key, signed with Kodetek's private key.
//
//   node tools/licensing/issue-licence.js \
//     --school "Swikunda Primary School" --host swikunda.kodetek.co.ke \
//     --modules Academics,Finance,Payroll,Elections,Communications --expires 2027-12-31
//
// Options: --key <private key .pem> (default licensing-keys/shulenova-licence-private.pem)
//          --id <licence id> (default generated). --host can be repeated or comma-separated.
// Modules: Academics (incl. Teaching Management), Finance, Payroll, Elections, Communications.
// Every key issued is appended to licensing-keys/issued-licences.csv as a record.
// The school pastes the key into Settings > Licence.

const crypto = require('crypto');
const fs = require('fs');
const path = require('path');

const MODULES = ['Academics', 'Finance', 'Payroll', 'Elections', 'Communications'];
const ROOT = path.resolve(__dirname, '..', '..');

function args() {
    const out = {host: []};
    const a = process.argv.slice(2);
    for (let i = 0; i < a.length; i++) {
        const name = a[i].replace(/^--/, '');
        const value = a[i + 1];
        if (value === undefined || value.startsWith('--')) fail(`--${name} needs a value`);
        if (name === 'host') out.host.push(...value.split(',').map((h) => h.trim().toLowerCase()).filter(Boolean));
        else out[name] = value;
        i++;
    }
    return out;
}

function fail(msg) {
    console.error('Error: ' + msg);
    process.exit(1);
}

const b64url = (buf) => buf.toString('base64').replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '');

const o = args();
if (!o.school) fail('--school is required');
if (!o.expires || !/^\d{4}-\d{2}-\d{2}$/.test(o.expires)) fail('--expires YYYY-MM-DD is required');
const modules = (o.modules || '').split(',').map((m) => m.trim()).filter(Boolean);
const unknown = modules.filter((m) => !MODULES.includes(m));
if (unknown.length) fail(`unknown module(s): ${unknown.join(', ')}. Use: ${MODULES.join(', ')}`);
if (o.host.length === 0) console.warn('Warning: no --host given, so the key works on any site.');

const keyFile = path.resolve(ROOT, o.key || 'licensing-keys/shulenova-licence-private.pem');
if (!fs.existsSync(keyFile)) fail(`private key not found: ${keyFile}`);
const privateKey = crypto.createPrivateKey(fs.readFileSync(keyFile));

const today = new Date().toISOString().slice(0, 10);
const payload = {
    l: o.id || `SN-${today.replace(/-/g, '')}-${crypto.randomBytes(3).toString('hex').toUpperCase()}`,
    s: o.school,
    h: o.host,
    m: modules,
    i: today,
    e: o.expires
};
const signedPart = 'SNL1.' + b64url(Buffer.from(JSON.stringify(payload), 'utf8'));
const signature = crypto.sign('sha256', Buffer.from(signedPart, 'ascii'), {key: privateKey, dsaEncoding: 'ieee-p1363'});
const licence = signedPart + '.' + b64url(signature);
if (licence.length > 500) fail(`the key is ${licence.length} characters; it must fit in 500 - shorten the school name or hosts`);

const register = path.resolve(ROOT, 'licensing-keys', 'issued-licences.csv');
if (!fs.existsSync(register)) fs.writeFileSync(register, 'issued,licenceId,school,hosts,modules,expires,key\n');
const csv = (v) => `"${String(v).replace(/"/g, '""')}"`;
fs.appendFileSync(register, [today, payload.l, payload.s, payload.h.join(' '), payload.m.join(' '), payload.e, licence].map(csv).join(',') + '\n');

console.log(`Licence ${payload.l} for ${payload.s}`);
console.log(`  Sites:   ${payload.h.join(', ') || 'any'}`);
console.log(`  Modules: ${payload.m.join(', ') || 'core only'}`);
console.log(`  Expires: ${payload.e}`);
console.log('\n' + licence + '\n');
