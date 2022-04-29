import { Injectable } from '@angular/core';
import { Observable } from 'rxjs/Observable';
import { LoggerService } from '../services/logger.service';
import { AbstractControl } from '@angular/forms';

@Injectable()
export class ValidationService {
  constructor(
    private logger: LoggerService
  ) {
    this.logger.info('ValidationService : constructor ');
  }

  public isNumberKey(event: any): boolean {
    this.logger.info('ValidationService : isNumberKey ');
    const charCode = (event.which) ? event.which : event.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57) && !event.ctrlKey) {
      return false;
    }

    return true;
  }

  public isEnterKey(event: any): boolean {
    this.logger.info('ValidationService : isEnterKey ');
    return ((event.keyCode || event.which) === 13);
  }

  public regExpValidator = (regExPattern: string, valueToValidate: string, flags?: string): boolean => {
    this.logger.info('ValidationService: regExpValidator');
    const regEx: RegExp = new RegExp(regExPattern, flags || '');
    return regEx.test(valueToValidate); // test and set the validity after update.
  }

  public validateDeselectedDropdown(control: AbstractControl): { [key: string]: any } | null {
    const valid = (control.value != null) ? (control.value.id === -1 ? false : true) : true;
    return valid
      ? null
      : { invalidNumber: { valid: false, value: control.value } };
  }

  public noWhitespaceValidator(control: AbstractControl) {
    const isWhitespace = (control.value || '').trim().length === 0;
    const isValid = !isWhitespace;
    return isValid ? null : { 'whitespace': true };
  }
}
