// Lightweight, dependency-free CSV export used by the security log/audit
// pages (and reusable anywhere else). Produces a UTF-8 CSV that opens cleanly
// in Excel - a leading BOM keeps Excel from mangling non-ASCII characters, and
// every field is quoted/escaped per RFC 4180 so commas, quotes and newlines in
// messages or stack traces don't break the columns.

export interface CsvColumn<T> {
    header: string;
    // Value for this column given a row. Return anything; it's stringified.
    value: (row: T) => unknown;
}

// Quote a single cell: wrap in double quotes and double any embedded quotes.
// null/undefined become empty; everything else is coerced to string first.
const escapeCell = (raw: unknown): string => {
    let s = raw == null ? '' : String(raw);
    return `"${s.replace(/"/g, '""')}"`;
};

// Build the CSV text (including header row) from columns + rows.
export const toCsv = <T>(columns: CsvColumn<T>[], rows: T[]): string => {
    let header = columns.map((c) => escapeCell(c.header)).join(',');
    let body = rows.map((row) =>
        columns.map((c) => escapeCell(c.value(row))).join(',')
    );
    // CRLF line endings are the safest for Excel/Windows consumers.
    return [header, ...body].join('\r\n');
};

// Trigger a browser download of the given rows as `<fileName>.csv`.
export const downloadCsv = <T>(
    fileName: string,
    columns: CsvColumn<T>[],
    rows: T[]
): void => {
    // ﻿ = UTF-8 BOM so Excel detects the encoding.
    let blob = new Blob(['﻿' + toCsv(columns, rows)], {
        type: 'text/csv;charset=utf-8;'
    });
    let url = URL.createObjectURL(blob);
    let a = document.createElement('a');
    a.href = url;
    a.download = fileName.toLowerCase().endsWith('.csv')
        ? fileName
        : `${fileName}.csv`;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    // Release the object URL on the next tick so the click has fired.
    setTimeout(() => URL.revokeObjectURL(url), 0);
};

// A yyyyMMdd-HHmmss stamp for unique, sortable export filenames.
export const exportStamp = (d: Date = new Date()): string => {
    let p = (n: number) => String(n).padStart(2, '0');
    return (
        `${d.getFullYear()}${p(d.getMonth() + 1)}${p(d.getDate())}` +
        `-${p(d.getHours())}${p(d.getMinutes())}${p(d.getSeconds())}`
    );
};
