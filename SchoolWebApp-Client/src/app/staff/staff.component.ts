import { BreadCrumb } from '@/core/models/bread-crumb';
import { Component, HostBinding, OnInit, Renderer2 } from '@angular/core';

@Component({
  selector: 'app-staff',
  templateUrl: './staff.component.html',
  styleUrl: './staff.component.scss'
})
export class StaffComponent implements OnInit{
  dashboardTitle = 'SchoolSoftWeb Staff';
  breadcrumbs: BreadCrumb[] = [
      {link: ['/'], title: 'Home'},
      {link: ['/'], title: 'Staff'}
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
      if (this.sidebarMenuOpened) {
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
