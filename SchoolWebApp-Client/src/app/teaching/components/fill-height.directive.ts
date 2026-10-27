import {Directive, ElementRef, HostListener, NgZone, OnDestroy, OnInit} from '@angular/core';

/**
 * Sizes a scrolling grid to reach the bottom of the window, so the grid scrolls
 * (keeping its header row in view) and the page itself does not also need to.
 * Recomputed when the window or the page's layout changes size.
 */
@Directive({selector: '[appFillHeight]'})
export class FillHeightDirective implements OnInit, OnDestroy {
    // Room left under the grid for the page footer and margins.
    private static readonly BOTTOM_GAP = 48;
    private static readonly MIN_HEIGHT = 250;
    private observer?: ResizeObserver;
    private frame = 0;

    constructor(private el: ElementRef<HTMLElement>, private zone: NgZone) {}

    ngOnInit(): void {
        // Layout changes above the grid (filters wrapping, alerts) move its top.
        this.zone.runOutsideAngular(() => {
            this.observer = new ResizeObserver(() => this.schedule());
            this.observer.observe(document.body);
        });
        this.schedule();
    }

    ngOnDestroy(): void {
        this.observer?.disconnect();
        cancelAnimationFrame(this.frame);
    }

    @HostListener('window:resize')
    schedule() {
        cancelAnimationFrame(this.frame);
        this.frame = requestAnimationFrame(() => this.fit());
    }

    private fit() {
        let el = this.el.nativeElement;
        let top = el.getBoundingClientRect().top + window.scrollY;
        let height = Math.max(FillHeightDirective.MIN_HEIGHT, window.innerHeight - top - FillHeightDirective.BOTTOM_GAP);
        let value = `${Math.floor(height)}px`;
        if (el.style.maxHeight !== value) el.style.maxHeight = value;
    }
}
