import { BreadCrumb } from '@/core/models/bread-crumb';
import { Component, HostBinding, Renderer2 } from '@angular/core';

@Component({
  selector: 'app-academics',
  templateUrl: './academics.component.html',
  styleUrl: './academics.component.scss'
})
export class AcademicsComponent {
  dashboardTitle = 'SchoolSoftWeb Academics';
  breadcrumbs: BreadCrumb[] = [
      {link: ['/'], title: 'Home'},
      {link: ['/'], title: 'School'}
  ];

  @HostBinding('class') class = 'wrapper';
  public sidebarMenuOpened = true;

  constructor(private renderer: Renderer2) {}
  ngOnInit(): void {
      this.renderer.removeClass(
          document.querySelector('app-root'),
          'login-page'
      );
      this.renderer.removeClass(
          document.querySelector('app-root'),
          'register-page'
      );
  }

  toggleMenuSidebar() {
      // The page's class is the truth: the menu also closes on its own on small screens.
        if (!document.querySelector('app-root')!.classList.contains('sidebar-collapse')) {
          this.renderer.removeClass(
              document.querySelector('app-root'),
              'sidebar-open'
          );
          this.renderer.addClass(
              document.querySelector('app-root'),
              'sidebar-collapse'
          );
          this.sidebarMenuOpened = false;
      } else {
          this.renderer.removeClass(
              document.querySelector('app-root'),
              'sidebar-collapse'
          );
          this.renderer.addClass(
              document.querySelector('app-root'),
              'sidebar-open'
          );
          this.sidebarMenuOpened = true;
      }
  }
}
