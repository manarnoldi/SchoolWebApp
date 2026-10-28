import {ProfileExam} from '@/core/services/profiles.service';
import {escapeHtml} from '@/shared/utils/print-frame';

// Against the class mean: at or above, below; blue when there is no class mean.
export const PROGRESS_ABOVE = '#198754';
export const PROGRESS_BELOW = '#dc3545';
export const PROGRESS_NEUTRAL = '#2a78d6';

/** For printInFrame: one learner a page. */
export const PROGRESS_PRINT_CSS = `
    @page { size: A4 landscape; margin: 10mm; }
    body { font-family: Arial, Helvetica, sans-serif; color: #212529; margin: 0; }
    .progress-page { page-break-after: always; break-after: page; }
    .progress-page:last-child { page-break-after: auto; break-after: auto; }
    .school { font-size: 13px; font-weight: bold; color: #084298; text-transform: uppercase; margin-bottom: 4px; }
    h1 { font-size: 18px; margin: 0 0 2px; } .sub { color: #52514e; font-size: 12px; margin-bottom: 8px; }
    svg { width: 100%; height: auto; }
    table { width: 100%; border-collapse: collapse; font-size: 11.5px; margin-top: 8px; }
    th, td { border: 1px solid #dee2e6; padding: 3px 6px; text-align: left; } th { background: #e7f1ff; }
    td.n { text-align: right; } .up { color: ${PROGRESS_ABOVE}; font-weight: bold; } .down { color: ${PROGRESS_BELOW}; font-weight: bold; }
`;

/** The year's exams oldest first: by term, then exam-type order. */
export function chronological(exams: ProfileExam[]): ProfileExam[] {
    return [...exams].sort((a, b) => a.sessionStart.localeCompare(b.sessionStart)
        || a.examTypeRank - b.examTypeRank || a.startDate.localeCompare(b.startDate));
}

export function shortExamName(e: ProfileExam): string {
    return e.name.replace(/\s+exam\b/i, '').split(' - ')[0];
}

/**
 * The progress chart as an SVG string, for printing: the student's mean in each
 * exam (green at or above the class mean, red below, value above and grade below
 * each point) against the class mean (dashed), terms marked across the top.
 */
export function progressChartSvg(examsOldestFirst: ProfileExam[], width = 1000, height = 230): string {
    const padL = 40, padR = 16, padT = 26, padB = 24;
    const n = examsOldestFirst.length;
    const span = width - padL - padR - 50;
    const x = (i: number) => padL + 25 + (n <= 1 ? span / 2 : (span * i) / (n - 1));
    const y = (v: number) => padT + (height - padT - padB) * (1 - Math.max(0, Math.min(100, v)) / 100);
    const colour = (e: ProfileExam) => e.classMean === null ? PROGRESS_NEUTRAL : e.mean >= e.classMean ? PROGRESS_ABOVE : PROGRESS_BELOW;
    const t = (s: string) => escapeHtml(s);
    const p: string[] = [];
    p.push(`<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 ${width} ${height}" font-family="Arial, Helvetica, sans-serif">`);
    [0, 25, 50, 75, 100].forEach((g) => {
        p.push(`<line x1="${padL}" x2="${width - padR}" y1="${y(g)}" y2="${y(g)}" stroke="#e9ecef"/>`);
        p.push(`<text x="${padL - 6}" y="${y(g) + 4}" text-anchor="end" font-size="11" fill="#52514e">${g}%</text>`);
    });
    examsOldestFirst.forEach((e, i) => {
        if (i === 0 || examsOldestFirst[i - 1].sessionId !== e.sessionId) {
            const tx = i === 0 ? padL + 2 : (x(i - 1) + x(i)) / 2;
            p.push(`<line x1="${tx}" x2="${tx}" y1="${padT - 8}" y2="${height - padB}" stroke="#cfe2ff" stroke-dasharray="3 3"/>`);
            p.push(`<text x="${tx + 4}" y="${padT - 10}" font-size="11" font-weight="bold" fill="#084298">${t(e.session)}</text>`);
        }
        p.push(`<text x="${x(i)}" y="${height - padB + 15}" text-anchor="middle" font-size="11" fill="#52514e">${t(shortExamName(e))}</text>`);
    });
    const cls = examsOldestFirst.map((e, i) => ({e, i})).filter(({e}) => e.classMean !== null);
    if (cls.length > 1) {
        p.push(`<polyline points="${cls.map(({e, i}) => `${x(i)},${y(e.classMean!)}`).join(' ')}" fill="none" stroke="#6c757d" stroke-width="2" stroke-dasharray="5 3"/>`);
    }
    cls.forEach(({e, i}) => p.push(`<circle cx="${x(i)}" cy="${y(e.classMean!)}" r="3" fill="#6c757d"/>`));
    examsOldestFirst.forEach((e, i) => {
        if (i > 0) p.push(`<line x1="${x(i - 1)}" y1="${y(examsOldestFirst[i - 1].mean)}" x2="${x(i)}" y2="${y(e.mean)}" stroke="${colour(e)}" stroke-width="2"/>`);
    });
    examsOldestFirst.forEach((e, i) => {
        p.push(`<circle cx="${x(i)}" cy="${y(e.mean)}" r="4" fill="${colour(e)}" stroke="#fff" stroke-width="1.5"/>`);
        p.push(`<text x="${x(i)}" y="${y(e.mean) - 8}" text-anchor="middle" font-size="10.5" font-weight="bold" fill="${colour(e)}">${e.mean}</text>`);
        if (e.meanGrade) p.push(`<text x="${x(i)}" y="${y(e.mean) + 15}" text-anchor="middle" font-size="9.5" font-weight="bold" fill="#6c757d">${t(e.meanGrade)}</text>`);
    });
    p.push('</svg>');
    return p.join('');
}

/** One learner's progress report for a year: heading, chart, and each exam against the class. */
export function progressReportPage(opts: {school?: string; name: string; className: string; yearName: string; exams: ProfileExam[]}): string {
    const exams = chronological(opts.exams);
    const rows = exams.map((e) => {
        const diff = e.classMean === null ? null : Math.round((e.mean - e.classMean) * 10) / 10;
        return `<tr><td>${escapeHtml(e.session)}</td><td>${escapeHtml(e.name)}</td><td class="n">${e.mean}%</td>
            <td>${escapeHtml(e.meanGrade || '')}</td><td class="n">${e.classMean === null ? '-' : e.classMean + '%'}</td>
            <td class="n ${diff === null ? '' : diff >= 0 ? 'up' : 'down'}">${diff === null ? '-' : (diff > 0 ? '+' : '') + diff}</td></tr>`;
    }).join('');
    return `<div class="progress-page">
        ${opts.school ? `<div class="school">${escapeHtml(opts.school)}</div>` : ''}
        <h1>${escapeHtml(opts.name)} - progress through ${escapeHtml(opts.yearName)}</h1>
        <div class="sub">${escapeHtml(opts.className)}. Mean mark in each exam against the class mean (green: at or above, red: below).</div>
        ${exams.length > 1 ? progressChartSvg(exams) : ''}
        <table><thead><tr><th>Term</th><th>Exam</th><th>Mean</th><th>Grade</th><th>Class mean</th><th>Against the class</th></tr></thead>
        <tbody>${rows || '<tr><td colspan="6">No exam results for this year.</td></tr>'}</tbody></table>
    </div>`;
}
