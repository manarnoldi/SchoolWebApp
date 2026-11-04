#!/usr/bin/env node
/*
 * Takes the user guides' screenshots from a running ShuleNova (the demo data, never
 * a school's live site) into SchoolWebApp-Client/src/assets/guide/.
 *
 * Each chapter's NN-name.shots.json lists its screenshots:
 *   [{"name": "fee-payments",            -> assets/guide/fee-payments.png
 *     "path": "/finance/payments",       page to open
 *     "waitFor": "table tbody tr",       optional: wait for this before the steps
 *     "steps": [{"click": "text=Load"}, {"fill": ["#search", "Amina"]}, {"select": ["#classId", "3"]},
 *               {"wait": 800}, {"waitFor": ".modal-content"}, {"press": "Escape"}, {"eval": "window.scrollTo(0,0)"}],
 *     "element": ".modal-content",       optional: shoot just this element (else the window)
 *     "fullPage": false,                 optional: the whole page, not just the window
 *     "height": 900,                     optional: window height for this one
 *     "anonymous": true}]                optional: signed out (the sign-in page)
 *
 * Needs playwright-core (not a project dependency) and Edge or Chrome:
 *   npm i --prefix %TEMP%\sn-guide-tools playwright-core
 *   set PW=%TEMP%\sn-guide-tools\node_modules\playwright-core
 *   node tools/user-guide/capture.js --base http://127.0.0.1:5299 --user ryan --password ... [names...]
 */
const fs = require('fs');
const path = require('path');
const {chromium} = require(process.env.PW || 'playwright-core');

const args = process.argv.slice(2);
const opt = (k, d) => { const i = args.indexOf('--' + k); return i >= 0 ? args.splice(i, 2)[1] : d; };
const BASE = opt('base', 'http://127.0.0.1:5299').replace(/\/$/, '');
const USER = opt('user', 'ryan');
const PASSWORD = opt('password', process.env.GUIDE_PASSWORD || '');
const only = new Set(args);

const OUT = path.resolve(__dirname, '../../SchoolWebApp-Client/src/assets/guide');
const WIDTH = 1440, HEIGHT = 810;
// No toasts in the pictures, nor the View only badge: capture with a viewer login
// (Administrator + Viewer) so a step that clicks Save changes nothing.
const HIDE = '#toast-container, span[title^="This account can look"] {display: none !important}';

function shotLists() {
    const all = [];
    for (const kind of ['user', 'admin']) {
        const dir = path.join(__dirname, kind);
        if (!fs.existsSync(dir)) continue;
        for (const f of fs.readdirSync(dir).filter((f) => f.endsWith('.shots.json')).sort()) {
            for (const s of JSON.parse(fs.readFileSync(path.join(dir, f), 'utf8'))) all.push({...s, from: `${kind}/${f}`});
        }
    }
    return all;
}

async function run(page, step) {
    if (step.click) await page.locator(step.click).first().click();
    else if (step.fill) await page.locator(step.fill[0]).first().fill(step.fill[1]);
    else if (step.select) await page.locator(step.select[0]).first().selectOption(step.select[1]);
    else if (step.waitFor) await page.locator(step.waitFor).first().waitFor({timeout: 20000});
    else if (step.press) await page.keyboard.press(step.press);
    else if (step.hover) await page.locator(step.hover).first().hover();
    else if (step.eval) await page.evaluate(step.eval);
    if (step.wait) await page.waitForTimeout(step.wait);
}

(async () => {
    if (!PASSWORD) throw new Error('Give --password (or GUIDE_PASSWORD).');
    fs.mkdirSync(OUT, {recursive: true});
    const shots = shotLists().filter((s) => !only.size || only.has(s.name));
    const browser = await chromium.launch({channel: process.env.PW_CHANNEL || 'msedge'});
    const context = await browser.newContext({viewport: {width: WIDTH, height: HEIGHT}});
    const signedIn = await context.newPage();

    await signedIn.goto(BASE + '/login');
    await signedIn.fill('input[formControlName="username"]', USER);
    await signedIn.fill('input[formControlName="password"]', PASSWORD);
    await signedIn.click('button[type="submit"]');
    await signedIn.waitForURL((u) => !u.pathname.startsWith('/login'), {timeout: 30000});

    let failed = 0;
    for (const s of shots) {
        // "anonymous": true - signed out (the sign-in page), in a window of its own.
        const own = s.anonymous ? await browser.newContext({viewport: {width: WIDTH, height: HEIGHT}}) : null;
        const page = own ? await own.newPage() : signedIn;
        try {
            await page.setViewportSize({width: WIDTH, height: s.height || HEIGHT});
            await page.goto(BASE + s.path, {waitUntil: 'networkidle'});
            await page.addStyleTag({content: HIDE});
            if (s.waitFor) await page.locator(s.waitFor).first().waitFor({timeout: 20000});
            for (const step of s.steps || []) await run(page, step);
            await page.waitForLoadState('networkidle');
            await page.waitForTimeout(s.settle ?? 600);
            // The capture login's Viewer role badge (user menu, profile).
            await page.evaluate(() => document.querySelectorAll('.badge').forEach((b) => {
                if (b.textContent.trim() === 'Viewer') b.remove();
            }));
            const file = path.join(OUT, s.name + '.png');
            if (s.element) await page.locator(s.element).first().screenshot({path: file});
            else await page.screenshot({path: file, fullPage: !!s.fullPage});
            console.log('  ok   ' + s.name);
        } catch (e) {
            failed++;
            console.log(`  FAIL ${s.name} (${s.from}): ${String(e.message).split('\n')[0]}`);
        }
        if (own) await own.close();
    }
    await browser.close();
    console.log(`${shots.length - failed}/${shots.length} screenshots in ${path.relative(process.cwd(), OUT)}`);
    process.exit(failed ? 1 : 0);
})();
