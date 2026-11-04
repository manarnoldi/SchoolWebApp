#!/usr/bin/env node
/*
 * Builds the two user guides from their chapters:
 *   tools/user-guide/user/*.html   -> SchoolWebApp-API/Project.API/Guides/user-guide.html
 *   tools/user-guide/admin/*.html  -> SchoolWebApp-API/Project.API/Guides/admin-guide.html
 *
 * Each chapter file is one <section class="chapter" id="..."> with an <h1 class="ch"> and
 * <h2 class="sub" id="..."> sections, taken in file-name order. A paid module's chapter
 * carries data-module="Finance"; a part of a chapter is wrapped in
 * <!--module:Finance--> ... <!--/module:Finance-->. The API cuts those out (chapter, sections
 * and their contents entries) for a school whose licence does not include the module.
 * Numbering is done by CSS, so nothing is left with a gap.
 *
 * Screenshots live in SchoolWebApp-Client/src/assets/guide/ (taken by capture.js).
 *
 *   node tools/user-guide/build.js
 */
const fs = require('fs');
const path = require('path');

const ROOT = path.resolve(__dirname, '../..');
const OUT = path.join(ROOT, 'SchoolWebApp-API/Project.API/Guides');
const SHOTS = path.join(ROOT, 'SchoolWebApp-Client/src/assets/guide');
const MODULES = ['Academics', 'Finance', 'Payroll', 'Elections', 'Communications'];

const GUIDES = [
    {kind: 'user', title: 'User Guide', subtitle: 'Day-to-day use of ShuleNova for staff and teachers'},
    {kind: 'admin', title: 'Administrator Guide', subtitle: 'Setting up and running ShuleNova for your school'}
];

const strip = (html) => html.replace(/<[^>]+>/g, '').replace(/\s+/g, ' ').trim();
let problems = 0;
const problem = (msg) => { problems++; console.error('  ! ' + msg); };

function buildChapter(file, ids) {
    const name = path.basename(file);
    let html = fs.readFileSync(file, 'utf8').trim();
    const sec = html.match(/^<section\s+class="chapter"\s+id="([\w-]+)"(?:\s+data-module="(\w+)")?\s*>/);
    if (!sec) { problem(`${name}: must start with <section class="chapter" id="...">`); return null; }
    const [, chapterId, module] = sec;
    if (module && !MODULES.includes(module)) problem(`${name}: unknown module ${module}`);
    const h1 = html.match(/<h1 class="ch">([\s\S]*?)<\/h1>/);
    if (!h1) problem(`${name}: no <h1 class="ch">`);

    // Contents entries for the sections, each inside the module markers it sits in.
    const subs = [];
    const open = [];
    const re = /<!--(\/?)module:(\w+)-->|<h2 class="sub" id="([\w-]+)">([\s\S]*?)<\/h2>|\sid="([\w-]+)"/g;
    let m;
    while ((m = re.exec(html))) {
        if (m[2]) {
            if (!MODULES.includes(m[2])) problem(`${name}: unknown module marker ${m[2]}`);
            if (m[1]) { if (open.pop() !== m[2]) problem(`${name}: module marker ${m[2]} closed out of order`); }
            else open.push(m[2]);
            continue;
        }
        const id = m[3] || m[5];
        if (ids.has(id)) problem(`${name}: id "${id}" used twice`);
        ids.add(id);
        if (m[3]) {
            let li = `<li><a href="#${m[3]}">${strip(m[4])}</a></li>`;
            for (let i = open.length - 1; i >= 0; i--) li = `<!--module:${open[i]}-->${li}<!--/module:${open[i]}-->`;
            subs.push(li);
        }
    }
    if (open.length) problem(`${name}: module marker ${open.join(', ')} not closed`);

    for (const img of html.matchAll(/<img[^>]+src="assets\/guide\/([^"]+)"/g)) {
        if (!fs.existsSync(path.join(SHOTS, img[1]))) problem(`${name}: missing screenshot ${img[1]}`);
    }

    let toc = `<li><a href="#${chapterId}">${strip(h1 ? h1[1] : chapterId)}</a>` +
        (subs.length ? `<ul>${subs.join('')}</ul>` : '') + '</li>';
    if (module) {
        toc = `<!--module:${module}-->${toc}<!--/module:${module}-->`;
        html = `<!--module:${module}-->\n${html}\n<!--/module:${module}-->`;
    }
    return {toc, html};
}

const template = fs.readFileSync(path.join(__dirname, 'template.html'), 'utf8');
const date = new Date().toLocaleDateString('en-GB', {month: 'long', year: 'numeric'});
fs.mkdirSync(OUT, {recursive: true});

for (const g of GUIDES) {
    const dir = path.join(__dirname, g.kind);
    const files = fs.existsSync(dir) ? fs.readdirSync(dir).filter((f) => f.endsWith('.html')).sort() : [];
    console.log(`${g.title}: ${files.length} chapters`);
    const ids = new Set();
    const chapters = files.map((f) => buildChapter(path.join(dir, f), ids)).filter(Boolean);
    // A link may only point within its own guide, and not into a module it isn't itself inside of
    // (for a school without that module the target is cut out and the link would lead nowhere).
    const body = chapters.map((c) => c.html).join('\n');
    const where = new Map();
    const links = [];
    const open = [];
    for (const m of body.matchAll(/<!--(\/?)module:(\w+)-->|\sid="([\w-]+)"|href="#([\w-]+)"/g)) {
        if (m[2]) m[1] ? open.pop() : open.push(m[2]);
        else if (m[3]) where.set(m[3], [...open]);
        else links.push({id: m[4], modules: [...open]});
    }
    for (const l of links) {
        if (!where.has(l.id)) problem(`${g.kind}: link to #${l.id}, which is not in the ${g.title}`);
        else for (const mod of where.get(l.id)) {
            if (!l.modules.includes(mod)) problem(`${g.kind}: link to #${l.id} (${mod} only) from outside a ${mod} part`);
        }
    }
    const out = template
        .replace(/{{TITLE}}/g, g.title)
        .replace('{{SUBTITLE}}', g.subtitle)
        .replace(/{{DATE}}/g, date)
        .replace('{{TOC}}', chapters.map((c) => '        ' + c.toc).join('\n'))
        .replace('{{BODY}}', chapters.map((c) => c.html).join('\n\n'));
    fs.writeFileSync(path.join(OUT, `${g.kind}-guide.html`), out);
}

if (problems) {
    console.error(`${problems} problem(s).`);
    process.exit(1);
}
console.log('Written to ' + path.relative(ROOT, OUT));
