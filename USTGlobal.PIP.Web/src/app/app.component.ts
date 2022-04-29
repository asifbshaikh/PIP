import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { LoggerService, AuthService, UtilityService } from '@core';
import { TranslateService } from '@ngx-translate/core';

import {
  ConfigurationSettings, Constants
} from '@shared';

import { environment } from '@env';
import { TooltipConfig } from 'ngx-bootstrap/tooltip';
import { Idle, DEFAULT_INTERRUPTSOURCES } from '@ng-idle/core';
import { Keepalive } from '@ng-idle/keepalive';
export function getAlertConfig(): TooltipConfig {
  return Object.assign(new TooltipConfig(), {
    placement: 'right',
    container: 'body'
  });
}

@Component({
  moduleId: module.id,
  selector: 'pip-app',
  templateUrl: 'app.component.html',
  providers: [{ provide: TooltipConfig, useFactory: getAlertConfig }],
  encapsulation: ViewEncapsulation.None
})

export class AppComponent implements OnInit {

  public collapsed = false;
  isUserLoggedIn = false;
  loginBackgroundImageUrl = '/assets/Images/login.png';

  idleState = 'Not started.';
  timedOut = false;
  lastPing?: Date = null;
  display = false;
  expanded = false;

  constructor(
    private logger: LoggerService,
    private translate: TranslateService,
    private authService: AuthService,
    private utilityService: UtilityService,
    private idle: Idle,
    private keepalive: Keepalive
  ) {
    this.logger.info('AppComponent : constructor ');

    // Start Session Handling Logic
    this.handleSessionTimeout(this.idle, this.keepalive);
    // End Session Handling Logic

    this.logger.info('"AppComponent : constructor => language configured');
    this.translate.addLangs(ConfigurationSettings.supportedBrowserLanguages);
    this.translate.setDefaultLang(ConfigurationSettings.fallbackBrowserLanguage);

    const browserLang = this.translate.getBrowserLang();

    this.logger.info('AppComponent : constructor => Current browserLang Is :' + browserLang);

    const languageConfiguredForApplication = browserLang.match(
      ConfigurationSettings.supportedBrowserLanguages.join('|'))
      ? browserLang : ConfigurationSettings.fallbackBrowserLanguage;

      this.translate.use(languageConfiguredForApplication);

    this.logger.info('AppComponent : constructor => Application language is set to :' + languageConfiguredForApplication);
  }

  getBackgroundImageUrl() {
    return `url(${this.loginBackgroundImageUrl})`;
  }

  ngOnInit() {
    this.isUserLoggedIn = this.authService.isUserLoggedIn();

    this.logger.info('AppComponent : ngOnInit() ');

    // set function to check if need need to refresh token
    if (this.isUserLoggedIn) { // if user logged in
      setInterval(() => {
        this.authService.refreshToken(); }, 60000); // 60000 ms = 1 minute
    }
  }

  handleSessionTimeout(idle: Idle, keepalive: Keepalive) {
    // sets an idle timeout of 30 minutes
    idle.setIdle(environment.session.idleTimeout);
    // sets a timeout period of 60 seconds. after 60 seconds of inactivity, the user will be considered timed out.
    idle.setTimeout(environment.session.timeout);
    // sets the default interrupts, in this case, things like clicks, scrolls, touches to the document
    idle.setInterrupts(DEFAULT_INTERRUPTSOURCES);

    idle.onTimeout.subscribe(() => {
      this.display = false;
      this.idleState = 'Session expired!';
      this.timedOut = true;
      this.logout();
    });

    idle.onIdleStart.subscribe(() => {
        this.idleState = 'You\'ve gone idle!';
        this.display = true;
    });

    idle.onTimeoutWarning.subscribe((countdown) => {
      this.idleState = 'Your session will expire in ' + countdown + ' seconds!';
    });

    // sets the ping interval to 15 seconds
    keepalive.interval(environment.session.keepaliveInterval);

    keepalive.onPing.subscribe(() => this.lastPing = new Date());

    if (this.authService.isUserLoggedIn()) {
      idle.watch();
      this.timedOut = false;
    } else {
      idle.stop();
    }

  }

  reset() {
    this.idle.watch();
    this.timedOut = false;
  }

  stay() {
    this.display = false;
    this.reset();
  }

  logout() {
    this.display = false;
    this.authService.logOut();
    // todo - check if below line is required or not
    // this.utilityService.redirectToURL(environment.appUrl);
  }

  isCollapsed(ev) {
    this.expanded = !ev;
  }
}

