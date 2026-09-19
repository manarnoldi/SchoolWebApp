// Styles for the printed payroll reports (Muster Roll, Statutory Returns, Bank
// Schedule). Each prints from its own frame (see printInFrame), so only these
// rules apply.
//
// The Muster Roll is A4 landscape: sixteen columns of figures do not fit across
// a portrait page at a readable size. Statutory Returns and the Bank Schedule
// have few columns and print A4 portrait.
//
// Column headings repeat at the top of every sheet. Totals are body rows, not a
// <tfoot>, because browsers repeat a <tfoot> at the foot of every printed page -
// which would show the grand totals under each page as though they were that
// page's totals.
export function payrollReportPrintCss(orientation: 'portrait' | 'landscape'): string {
    return `
@page { size: A4 ${orientation}; margin: ${orientation === 'landscape' ? '10mm' : '15mm'}; }

html, body { margin: 0; padding: 0; background: #fff; }
/* A hairline of room at the sides: a collapsed table draws half of its outer
   border outside its box, which the page edge would otherwise clip. */
body { padding: 0 1px; }

body {
    color: #000;
    font-family: Arial, Helvetica, sans-serif;
    font-size: ${orientation === 'landscape' ? '8pt' : '10pt'};
    line-height: 1.3;
}

/* Header */
.rp-header { text-align: center; margin-bottom: ${orientation === 'landscape' ? '3mm' : '6mm'}; }
.rp-logo { max-height: 14mm; max-width: 30mm; display: block; margin: 0 auto 2px; }
.rp-school { font-size: 11pt; font-weight: bold; text-transform: uppercase; }
.rp-pin { font-weight: bold; }
.rp-title { font-size: 10pt; font-weight: bold; margin-top: 2px; text-decoration: underline; }
.rp-meta { margin-top: 2px; color: #333; }

/* Table */
table.rp { width: 100%; border-collapse: collapse; }
table.rp thead { display: table-header-group; }
table.rp th, table.rp td { border: 1px solid #555; padding: ${orientation === 'landscape' ? '1px 4px' : '4px 6px'}; }
table.rp th { background: #e9e9e9; font-weight: bold; text-align: left; white-space: nowrap; }
table.rp th.num, table.rp td.num { text-align: right; white-space: nowrap; font-variant-numeric: tabular-nums; }
table.rp td.name { white-space: nowrap; }
table.rp tr { break-inside: avoid; page-break-inside: avoid; }
table.rp td.strong { font-weight: bold; }
/* A subtotal inside the table, e.g. the NSSF remittance */
table.rp tr.subtotal td { font-weight: bold; background: #f3f3f3; }
table.rp tr.totals td { font-weight: bold; border-top: 2px solid #000; background: #e9e9e9; }

/* Summary figures under a table */
.rp-summary {
    display: flex;
    border: 1px solid #555;
    margin-top: 6mm;
    break-inside: avoid;
    page-break-inside: avoid;
}
.rp-summary > div { flex: 1; text-align: center; padding: 3mm 2mm; border-left: 1px solid #555; }
.rp-summary > div:first-child { border-left: none; }
.rp-summary .k { color: #333; font-size: 0.9em; }
.rp-summary .v { font-weight: bold; font-size: 1.1em; margin-top: 1mm; }

/* Keep the header shading and totals bands when printed */
* { -webkit-print-color-adjust: exact; print-color-adjust: exact; }

/* Sign-off */
.rp-signoff {
    display: flex;
    justify-content: space-between;
    gap: 10mm;
    margin-top: ${orientation === 'landscape' ? '6mm' : '15mm'};
    break-inside: avoid;
    page-break-inside: avoid;
}
.rp-signoff > div { flex: 1; }
.rp-signoff .role { font-weight: bold; margin-bottom: 5mm; }
.rp-signoff .line { border-bottom: 1px solid #000; margin-bottom: 1mm; }
.rp-signoff .label { color: #333; margin-bottom: 5mm; }
.rp-signoff .label:last-child { margin-bottom: 0; }
`;
}
