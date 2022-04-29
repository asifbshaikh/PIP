import { Component, OnInit, ViewChild, ElementRef } from '@angular/core';
import { LoggerService, UtilityService, AuthService } from '@core';
import { SharedDataService } from '@global';
import { LogoutConfirmationComponent } from './logout/logout-confirmation.component';
import { environment } from '@env';
import { MenuItem } from 'primeng/api';

@Component({
  moduleId: module.id,
  selector: 'app-header',
  templateUrl: 'header.component.html'
})
export class HeaderComponent implements OnInit {
  @ViewChild('logout')
  logoutComp: LogoutConfirmationComponent;

  @ViewChild('userNavigationMenu') userNavigationMenu: ElementRef;

  userName: string;
  items: MenuItem[];

  constructor(
    private logger: LoggerService,
    private sharedDataService: SharedDataService,
    private utilityService: UtilityService,
    private authService: AuthService
  ) {
    this.logger.info('HeaderComponent : constructor ');
    this.userName = this.sharedDataService.sharedData.userRoleAccountDTO.firstName;
  }

  ngOnInit() {
    this.logger.info('HeaderComponent : ngOnInit');
    this.items = [
      { label: 'Profile', icon: 'pi pi-fw pi-user' },
      { label: 'Setting', icon: 'pi pi-fw pi-cog' },
      { label: 'Help', icon: 'pi pi-fw pi-question-circle' },
      { label: 'Logout', icon: 'pi pi-fw pi-power-off', command: this.showLogoutConfirmation() }
    ];
  }

  showLogoutConfirmation() {
    return () => {
      this.logger.info('HeaderComponent : showLogoutConfirmation ');
      this.logoutComp.showConfirmation();
    };
  }

  onLogoutConfirmation(eventData: boolean) {
    this.logger.info('HeaderComponent : onLogoutConfirmation ');
    this.authService.logOut();
  }

  openInNewWindow(url: string) {
    this.logger.info('HeaderComponent : openInNewWindow ');
    this.utilityService.openInNewWindow(url);
  }

  toggleUserMenu(closeMenu?: boolean) {
    const classOpen = 'open';

    if (!this.userNavigationMenu.nativeElement.classList.contains(classOpen)
      && (closeMenu === undefined || closeMenu === false)) {
      this.userNavigationMenu.nativeElement.classList.add(classOpen);
    } else {
      this.userNavigationMenu.nativeElement.classList.remove(classOpen);
    }
  }

}
