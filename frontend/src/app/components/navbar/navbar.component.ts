import { Component, OnInit, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SIDEBAR_ITEMS, SidebarMenuItem } from '../sidebar/sidebar.component';
import { Location } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { AuthService } from '../../core/auth/auth.service';
import { MatDividerModule } from '@angular/material/divider';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatMenuModule, MatDividerModule, RouterLink],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
  private listTitles: SidebarMenuItem[] = [];
  location: Location;
  mobile_menu_visible = 0;
  private toggleButton?: HTMLElement;
  private sidebarVisible = false;

  constructor(
    location: Location,
    private element: ElementRef<HTMLElement>,
    private router: Router,
    private auth: AuthService
  ) {
    this.location = location;
  }

  private getBody(): HTMLElement | null {
    return document.body;
  }

  ngOnInit(): void {
    this.listTitles = SIDEBAR_ITEMS;
    const navbar = this.element.nativeElement as HTMLElement;
    this.toggleButton = navbar.getElementsByClassName('navbar-toggler')[0] as HTMLElement | undefined;
    this.router.events.subscribe(() => {
      this.sidebarClose();
      const existingLayer = document.getElementsByClassName('close-layer')[0] as HTMLElement | undefined;
      if (existingLayer) {
        existingLayer.remove();
        this.mobile_menu_visible = 0;
      }
    });
  }

  sidebarOpen(): void {
    const toggleButton = this.toggleButton;
    const body = this.getBody();
    if (!toggleButton || !body) {
      return;
    }
    setTimeout(() => {
      toggleButton.classList.add('toggled');
    }, 500);

    body.classList.add('nav-open');
    this.sidebarVisible = true;
  }

  sidebarClose(): void {
    const body = this.getBody();
    const toggleButton = this.toggleButton;
    if (!toggleButton || !body) {
      return;
    }
    toggleButton.classList.remove('toggled');
    this.sidebarVisible = false;
    body.classList.remove('nav-open');
  }

  sidebarToggle(): void {
    const toggleButton = this.toggleButton;
    const body = this.getBody();
    if (!toggleButton || !body) {
      return;
    }
    const existingLayer = document.getElementsByClassName('close-layer')[0] as HTMLElement | undefined;

    if (this.sidebarVisible === false) {
      this.sidebarOpen();
    } else {
      this.sidebarClose();
    }

    if (this.mobile_menu_visible === 1) {
        body.classList.remove('nav-open');
        if (existingLayer) {
          existingLayer.remove();
        }
      setTimeout(() => {
        toggleButton.classList.remove('toggled');
      }, 400);

      this.mobile_menu_visible = 0;
    } else {
      setTimeout(() => {
        toggleButton.classList.add('toggled');
      }, 430);

      const closeLayer = document.createElement('div');
      closeLayer.setAttribute('class', 'close-layer');

      const mainPanel = document.getElementsByClassName('main-panel')[0] as HTMLElement | undefined;
      if (mainPanel) {
        mainPanel.appendChild(closeLayer);
      } else if (body.classList.contains('off-canvas-sidebar')) {
        const wrapperFullPage = document.getElementsByClassName('wrapper-full-page')[0] as HTMLElement | undefined;
        if (wrapperFullPage) {
          wrapperFullPage.appendChild(closeLayer);
        }
      }

      setTimeout(() => {
        closeLayer.classList.add('visible');
      }, 100);

      closeLayer.onclick = () => {
        const bodyElement = this.getBody();
        if (!bodyElement) {
          return;
        }
        bodyElement.classList.remove('nav-open');
        this.mobile_menu_visible = 0;
        closeLayer.classList.remove('visible');
        setTimeout(() => {
          closeLayer.remove();
          toggleButton.classList.remove('toggled');
        }, 400);
      };

      body.classList.add('nav-open');
      this.mobile_menu_visible = 1;
    }
  }

  getTitle(): string {
    let titlee = this.location.prepareExternalUrl(this.location.path());
    if (titlee.charAt(0) === '#') {
      titlee = titlee.slice(1);
    }

    for (const item of this.listTitles) {
      if (item.path === titlee) {
        return item.title;
      }
    }
    return 'Dashboard';
  }

  logout(): void {
    this.auth.logout().subscribe({
      next: () => this.afterLogout(),
      error: () => this.afterLogout()
    });
  }

  private afterLogout(): void {
    localStorage.removeItem('accessToken');
    localStorage.removeItem('refreshToken');
    this.router.navigateByUrl('/login');
  }
}
