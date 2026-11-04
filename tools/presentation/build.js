#!/usr/bin/env node
/*
 * Builds Kodetek's client presentation into one HTML page:
 *   tools/presentation/deck.json + slides/<id>.html  ->  SchoolWebApp-API/Project.API/Guides/presentation.html
 *
 * The API serves it (GET api/userGuide/presentation) to SuperAdministrators only, and the app
 * shows it from the top bar (help/presentation). Each slide is one <section> on a 1920x1080
 * canvas with inline styles; the page scales it to the screen. Images are the app's own assets
 * (assets/guide, assets/presentation, assets/img). <x-icon name="..."> becomes an inline icon,
 * data-crop="1 X% Y%" an object-position, and the <aside> is the speaker notes (N shows them).
 *
 *   node tools/presentation/build.js
 */
const fs = require('fs');
const path = require('path');

const ROOT = path.resolve(__dirname, '../..');
const OUT = path.join(ROOT, 'SchoolWebApp-API/Project.API/Guides/presentation.html');
const ASSETS = path.join(ROOT, 'SchoolWebApp-Client/src');

// Feather-style line icons, drawn in the icon's color.
const ICONS = {
    Home: '<path d="M3 9l9-7 9 7v11a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z"/><polyline points="9 22 9 12 15 12 15 22"/>',
    Star: '<polygon points="12 2 15.09 8.26 22 9.27 17 14.14 18.18 21.02 12 17.77 5.82 21.02 7 14.14 2 9.27 8.91 8.26 12 2"/>',
    Code: '<polyline points="16 18 22 12 16 6"/><polyline points="8 6 2 12 8 18"/>',
    Verified: '<circle cx="12" cy="8" r="7"/><polyline points="8.21 13.89 7 23 12 20 17 23 15.79 13.88"/>',
    CheckCircle: '<path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/><polyline points="22 4 12 14.01 9 11.01"/>',
    Book: '<path d="M4 19.5A2.5 2.5 0 0 1 6.5 17H20"/><path d="M6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5v-15A2.5 2.5 0 0 1 6.5 2z"/>',
    Chart: '<line x1="18" y1="20" x2="18" y2="10"/><line x1="12" y1="20" x2="12" y2="4"/><line x1="6" y1="20" x2="6" y2="14"/>',
    PaperPlane: '<line x1="22" y1="2" x2="11" y2="13"/><polygon points="22 2 15 22 11 13 2 9 22 2"/>',
    Chat: '<path d="M21 11.5a8.38 8.38 0 0 1-.9 3.8 8.5 8.5 0 0 1-7.6 4.7 8.38 8.38 0 0 1-3.8-.9L3 21l1.9-5.7a8.38 8.38 0 0 1-.9-3.8 8.5 8.5 0 0 1 4.7-7.6 8.38 8.38 0 0 1 3.8-.9h.5a8.48 8.48 0 0 1 8 8v.5z"/>',
    Globe: '<circle cx="12" cy="12" r="10"/><line x1="2" y1="12" x2="22" y2="12"/><path d="M12 2a15.3 15.3 0 0 1 4 10 15.3 15.3 0 0 1-4 10 15.3 15.3 0 0 1-4-10 15.3 15.3 0 0 1 4-10z"/>'
};

let problems = 0;
const problem = (msg) => { problems++; console.error('  ! ' + msg); };

const deck = JSON.parse(fs.readFileSync(path.join(__dirname, 'deck.json'), 'utf8'));
const fonts = Object.values(deck.faces || {}).map((f) => f.href).filter(Boolean);

const slides = deck.order.map((id) => {
    const file = path.join(__dirname, 'slides', `${id}.html`);
    if (!fs.existsSync(file)) { problem(`no slides/${id}.html`); return ''; }
    let html = fs.readFileSync(file, 'utf8').trim();
    if (!html.startsWith(`<section id="${id}"`)) problem(`${id}.html: must start with <section id="${id}"`);
    html = html.replace(/<x-icon name="(\w+)" style="([^"]*)"><\/x-icon>/g, (m, name, style) => {
        if (!ICONS[name]) { problem(`${id}.html: unknown icon ${name}`); return ''; }
        return `<svg class="xi" viewBox="0 0 24 24" style="${style}" aria-hidden="true">${ICONS[name]}</svg>`;
    });
    html = html.replace(/<img([^>]*?) data-crop="[\d.]+ ([\d.]+%) ([\d.]+%)"([^>]*?) style="([^"]*)"/g,
        (m, a, x, y, b, style) => `<img${a}${b} style="${style};object-position:${x} ${y}"`);
    for (const img of html.matchAll(/<img[^>]+src="([^"]+)"/g)) {
        if (!fs.existsSync(path.join(ASSETS, img[1]))) problem(`${id}.html: missing image ${img[1]}`);
    }
    if (/<x-/.test(html)) problem(`${id}.html: an <x-...> element the build does not handle`);
    return html;
});

const page = `<!DOCTYPE html>
<html lang="en">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<title>${deck.title.replace(/&/g, "&amp;")}</title>
${fonts.map((h) => `<link rel="stylesheet" href="${h}">`).join('\n')}
<style>
  html, body { margin: 0; height: 100%; background: #0B1424; overflow: hidden; font-family: 'DM Sans', Arial, sans-serif; }
  #stage { position: absolute; left: 50%; top: 50%; width: 1920px; height: 1080px; transform-origin: center; }
  #stage * { box-sizing: border-box; margin: 0; }
  #stage section { position: absolute; left: 0; top: 0; width: 1920px; height: 1080px; overflow: hidden;
                   opacity: 0; visibility: hidden; transition: opacity .45s ease, visibility .45s; }
  #stage section.on { opacity: 1; visibility: visible; }
  #stage h1 { font-size: 96px; font-weight: 600; line-height: 1.1; }
  #stage h2 { font-size: 64px; font-weight: 600; line-height: 1.15; }
  #stage h3 { font-size: 44px; font-weight: 600; line-height: 1.2; }
  #stage p { font-size: 32px; line-height: 1.4; }
  #stage b { font-weight: 700; }
  #stage img, #stage svg { display: block; flex-shrink: 0; }
  #stage hr { border: 0; width: 100%; }
  #stage aside { display: none; }
  .xi { fill: none; stroke: currentColor; stroke-width: 2; stroke-linecap: round; stroke-linejoin: round; }
  #bar { position: fixed; right: 16px; bottom: 16px; display: flex; gap: 6px; align-items: center; padding: 6px 10px;
         background: rgba(14, 26, 47, .85); color: #F3F5FA; border-radius: 10px; font: 14px 'DM Sans', Arial, sans-serif;
         transition: opacity .4s; z-index: 2; }
  #bar.idle { opacity: 0; }
  #bar button { background: none; border: 0; color: inherit; font: inherit; cursor: pointer; padding: 4px 8px; border-radius: 6px; }
  #bar button:hover { background: rgba(255, 255, 255, .15); }
  #count { min-width: 60px; text-align: center; }
  #notes { position: fixed; left: 16px; right: 16px; bottom: 64px; max-height: 30%; overflow: auto; display: none; z-index: 2;
           background: rgba(14, 26, 47, .92); color: #F3F5FA; padding: 14px 18px; border-radius: 10px; font: 18px/1.5 'DM Sans', Arial, sans-serif; }
  #notes.show { display: block; }
  @media print {
    @page { size: 1920px 1080px; margin: 0; }
    html, body { overflow: visible; height: auto; }
    #stage { position: static; transform: none !important; }
    #stage section { position: relative; opacity: 1; visibility: visible; page-break-after: always; }
    #bar, #notes { display: none !important; }
  }
</style>
</head>
<body>
<div id="stage">
${slides.join('\n')}
</div>
<div id="notes"></div>
<div id="bar">
  <button id="prev" title="Previous (Left arrow)">&#9664;</button>
  <span id="count"></span>
  <button id="next" title="Next (Right arrow or Space)">&#9654;</button>
  <button id="notesBtn" title="Speaker notes (N)">Notes</button>
  <button id="full" title="Full screen (F)">Full screen</button>
</div>
<script>
(function () {
  var stage = document.getElementById('stage');
  var slides = stage.querySelectorAll('section');
  var bar = document.getElementById('bar');
  var notes = document.getElementById('notes');
  var i = 0, idle;
  function fit() {
    var s = Math.min(window.innerWidth / 1920, window.innerHeight / 1080);
    stage.style.transform = 'translate(-50%, -50%) scale(' + s + ')';
  }
  function show(n) {
    i = Math.max(0, Math.min(slides.length - 1, n));
    for (var k = 0; k < slides.length; k++) slides[k].classList.toggle('on', k === i);
    document.getElementById('count').textContent = (i + 1) + ' / ' + slides.length;
    var aside = slides[i].querySelector('aside');
    notes.textContent = aside ? aside.textContent : '';
  }
  function full() {
    if (document.fullscreenElement) document.exitFullscreen();
    else if (document.documentElement.requestFullscreen) document.documentElement.requestFullscreen();
  }
  function wake() {
    bar.classList.remove('idle');
    clearTimeout(idle);
    idle = setTimeout(function () { bar.classList.add('idle'); }, 2500);
  }
  document.addEventListener('keydown', function (e) {
    var k = e.key;
    if (k === 'ArrowRight' || k === 'PageDown' || k === ' ' || k === 'Enter') show(i + 1);
    else if (k === 'ArrowLeft' || k === 'PageUp' || k === 'Backspace') show(i - 1);
    else if (k === 'Home') show(0);
    else if (k === 'End') show(slides.length - 1);
    else if (k === 'f' || k === 'F') full();
    else if (k === 'n' || k === 'N') notes.classList.toggle('show');
    else return;
    e.preventDefault();
  });
  stage.addEventListener('click', function (e) { show(e.clientX < window.innerWidth / 3 ? i - 1 : i + 1); });
  document.getElementById('prev').onclick = function () { show(i - 1); };
  document.getElementById('next').onclick = function () { show(i + 1); };
  document.getElementById('notesBtn').onclick = function () { notes.classList.toggle('show'); };
  document.getElementById('full').onclick = full;
  document.addEventListener('mousemove', wake);
  window.addEventListener('resize', fit);
  fit(); show(0); wake(); window.focus();
})();
</script>
</body>
</html>
`;

if (problems) {
    console.error(`${problems} problem(s).`);
    process.exit(1);
}
fs.writeFileSync(OUT, page);
console.log(`${slides.length} slides written to ${path.relative(ROOT, OUT)}`);
