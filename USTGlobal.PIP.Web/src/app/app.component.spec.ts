import { AppComponent } from './app.component';
import { LoggerService } from '@core';


describe('AppComponent', () => {

  let comp: AppComponent;

  const logger = {
    info : (x) => {}
  };

  const translate = {
    addLangs : (x) => {},
    setDefaultLang : (x) => {},
    getBrowserLang : () => '',
    use : () => ''
  };

  const authService = {
    isLoggedIn : () => true
  };

  beforeEach(() => {
    spyOn(translate, 'use');
    spyOn(logger, 'info');
    comp = new AppComponent(logger as any, translate as any, authService as any, {} as any);
  });

  it('should setup translation', () => {
    expect(translate.use).toHaveBeenCalled();
  });

  it('should print that i am inside app component', () => {
    expect(logger.info).toHaveBeenCalledTimes( 4);
  });

  it('should set is logged in set to true', () => {
    expect(authService.isLoggedIn).toBeTruthy();
  });

});
