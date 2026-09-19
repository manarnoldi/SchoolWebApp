// Prints a piece of HTML on its own, from a hidden frame holding nothing but
// that HTML and the stylesheet given for it.
//
// Printing the app page itself lets the app's print rules interfere - the 25mm
// report margins, AdminLTE's 992px minimum print width (which shrinks the page
// to fit), page padding, even pop-up notifications printing on top. A document
// printed from its own frame gets none of that, so its @page size, margins and
// layout come out exactly as its stylesheet says.

let currentFrame: HTMLIFrameElement | null = null;

export function removePrintFrame(): void {
    currentFrame?.remove();
    currentFrame = null;
}

export function escapeHtml(text: string): string {
    return (text || '').replace(/[&<>"]/g, (c) =>
        ({'&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;'} as any)[c]);
}

/**
 * @param title Document title - shown by the browser if headers are printed,
 *              and used as the file name when saving as PDF.
 * @param css   The complete stylesheet, including its @page rule.
 * @param body  The HTML to print.
 * @param orientation The orientation the stylesheet's @page asks for. The frame
 *              is sized to that sheet: a frame wider than the page (landscape
 *              width for a portrait report) had the browser scale the layout
 *              to fit, which clipped the right-hand borders.
 */
export function printInFrame(title: string, css: string, body: string,
                             orientation: 'portrait' | 'landscape' = 'portrait'): void {
    removePrintFrame();
    let frame = document.createElement('iframe');
    // Off screen but laid out at the page width - a frame with no size or
    // display:none can print blank in some browsers.
    let [width, height] = orientation === 'landscape' ? ['297mm', '210mm'] : ['210mm', '297mm'];
    frame.setAttribute('aria-hidden', 'true');
    frame.style.cssText = `position:fixed; left:-10000px; top:0; width:${width}; height:${height}; border:0;`;
    document.body.appendChild(frame);
    currentFrame = frame;

    let doc = frame.contentDocument!;
    doc.open();
    doc.write(`<!DOCTYPE html><html><head><meta charset="utf-8">
        <title>${escapeHtml(title)}</title>
        <style>${css}</style></head><body>${body}</body></html>`);
    doc.close();

    // Wait for images (the school logo) so nothing prints without them.
    let images = Array.from(doc.images);
    Promise.all(images.map((img) => img.complete
        ? Promise.resolve()
        : new Promise<void>((done) => { img.onload = img.onerror = () => done(); })))
        .then(() => {
            let win = frame.contentWindow!;
            // Tidy up once the dialog closes; the next print also clears it.
            win.addEventListener('afterprint', () => { if (currentFrame === frame) removePrintFrame(); });
            win.focus();
            win.print();
        });
}
