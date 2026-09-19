// Styles for printed payslips. Payslips print from a hidden frame holding only
// the slips and this stylesheet, so none of the app's own print rules - the 25mm
// report margins, AdminLTE's 992px minimum width, page padding, pop-up
// notifications - can move, shrink or cover them.
//
// Layout: A4 with 8mm margins leaves 194mm across; two 85mm slips and a 4mm gap
// take 174mm of it, so slips print in pairs, top-left, ready to be cut. A pair
// that will not fit below the one above moves to the next sheet whole.
//
// Each slip reads top to bottom - Earnings, how PAYE was worked out, Deductions,
// Net pay - so an employee can follow every figure back to where it came from.
// Amounts taken off are shown in brackets. Type follows the school's existing
// payslip: plain Arial, regular labels, bold underlined block headings and bold
// totals, in blue ink. Change #0000cd to #000 for black.
export const PAYSLIP_PRINT_CSS = `
@page { size: A4 portrait; margin: 8mm; }

html, body { margin: 0; padding: 0; background: #fff; }

.payslip-pair {
    display: flex;
    align-items: flex-start;
    gap: 4mm;
    margin-bottom: 4mm;
    break-inside: avoid;
    page-break-inside: avoid;
}

.payslip {
    width: 85mm;
    flex: 0 0 85mm;
    border: 1px solid #0000cd;
    border-radius: 6px;
    color: #0000cd;
    font-family: Arial, Helvetica, sans-serif;
    font-size: 9pt;
    font-weight: normal;
    line-height: 1.45;
    box-sizing: border-box;
    background: #fff;
    overflow: hidden;
    break-inside: avoid;
    page-break-inside: avoid;
}

/* Header */
.payslip .payslip-header { text-align: center; padding: 5px 8px; border-bottom: 1px solid #0000cd; }
.payslip .payslip-logo { max-height: 16mm; max-width: 30mm; display: block; margin: 0 auto 4px; }
.payslip .school-name { font-weight: bold; text-transform: uppercase; }
.payslip .school-pin { font-weight: bold; }
.payslip .payslip-title { font-weight: bold; }

/* Employee: two columns of label/value pairs so six identifiers take three lines */
.payslip .payslip-employee { padding: 4px 8px; border-bottom: 1px solid #0000cd; }
.payslip .emp-name { font-weight: bold; text-transform: uppercase; margin-bottom: 1px; }
.payslip .emp-grid { display: grid; grid-template-columns: 1fr 1fr; column-gap: 10px; row-gap: 0; }
.payslip .emp-grid > div { display: flex; gap: 4px; }
.payslip .emp-grid .k { min-width: 15mm; }
.payslip .emp-grid .k::after { content: ':'; }
.payslip .emp-grid .v { overflow: hidden; text-overflow: ellipsis; white-space: nowrap; }

/* Blocks */
.payslip .payslip-block { padding: 4px 8px 5px; border-bottom: 1px solid #0000cd; }
.payslip .payslip-block:last-child { border-bottom: none; }
.payslip .block-title { font-weight: bold; text-decoration: underline; margin-bottom: 2px; }

.payslip .row-line { display: flex; justify-content: space-between; gap: 8px; }
.payslip .row-line .amt { text-align: right; white-space: nowrap; font-variant-numeric: tabular-nums; }
/* A line taken off the running figure above it */
.payslip .row-line.less { padding-left: 8px; }
/* A running figure: ruled above so the subtraction reads clearly */
.payslip .row-line.subtotal { font-weight: bold; border-top: 1px solid #8c8ce6; margin-top: 1px; padding-top: 1px; }
/* The answer for the block */
.payslip .row-line.total { font-weight: bold; border-top: 1px solid #8c8ce6; margin-top: 3px; padding-top: 2px; }
/* What the employee actually receives - double-ruled */
.payslip .row-line.net-pay {
    font-weight: bold;
    border-top: 1px solid #0000cd;
    border-bottom: 3px double #0000cd;
    margin-top: 3px;
    padding: 2px 0;
}

.payslip .note { font-style: normal; }
`;
