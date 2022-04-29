import {
  Directive,
  ElementRef,
  HostListener,
  Input
} from '@angular/core';

import {
  Constants
} from '../infrastructure/index';

import {
  ValidationService, LoggerService,
} from '@core';

@Directive({
  selector: '[pipRestrictInput]',
})
export class RestrictInputDirective {

  _regx: RegExp;
  _backSpaceKey = 8;
  _tabKey = 9;
  _deleteKey = 16;
  _leftArrowKey = 37;
  _rightArrowKey = 39;

  @Input('pipRestrictInput') regExType: string;
  constructor(private el: ElementRef
    , private validationService: ValidationService,
    private logger: LoggerService
  ) {
    this.logger.info('RestrictInput : constructor ');
  }

  @HostListener('keypress', ['$event']) keypress(event: any) {
    this.setRegExp();
    const charCode = (event.which) ? event.which : event.keyCode;

    // allow alter key, tab, backspace, delete, left arrow & right arrow keys
    if (event.altKey === true || charCode === this._tabKey || charCode === this._backSpaceKey || charCode === this._deleteKey) {
      return true;
    } // removed charCode === this._leftArrowKey || charCode === this._rightArrowKey as %(charCode 37) sign was allowed
    // detect ctrl allow it to proceed further to catch paste event (metaKey is for Mac)
    if (event.ctrlKey || event.metaKey) {
      return;
    }

    if (charCode === 45 && this.regExType === 'negativeDecimalPrecisionTwo') {
      const inputValueNegative = String.fromCharCode(charCode) + this.el.nativeElement.value;
      return this.validateInput(inputValueNegative, this._regx);
    }
    const inputValue = this.el.nativeElement.value + String.fromCharCode(charCode);
    return this.validateInput(inputValue, this._regx);
  }

  @HostListener('paste', ['$event']) paste(event: any) {
    this.setRegExp();
    const clipboardData = event.clipboardData;
    const inputValue = clipboardData.getData('text/plain');
    const isValidInput = this.validateInput(inputValue, this._regx);
    return isValidInput;
  }

  setRegExp = () => {
    // Change error messages as per specific RegEx type if needed (it would apply to all fields)
    switch (this.regExType) {
      case 'numeric':
        this._regx = Constants.regExType.numeric;
        break;
      case 'alphanumeric':
        this._regx = Constants.regExType.alphanumeric;
        break;
      case 'alphanumWithSpecial1':
        this._regx = Constants.regExType.alphanumWithSpecial1;
        break;
      case 'decimalPrecisionFour':
        this._regx = Constants.regExType.decimalPrecisionFour;
        break;
      case 'decimalPrecisionTwo':
        this._regx = Constants.regExType.decimalPrecisionTwo;
        break;
      case 'negativedecimalPrecisionFour':
        this._regx = Constants.regExType.negativedecimalPrecisionFour;
        break;
      case 'negativeDecimalPrecisionTwo':
        this._regx = Constants.regExType.negativeDecimalPrecisionTwo;
        break;
      case 'notAllowSpaceInBeginning':
        this._regx = Constants.regExType.notAllowSpaceInBeginning;
        break;
      case 'alphanumericSpaceNotAllowedInBeginning':
        this._regx = Constants.regExType.alphanumericSpaceNotAllowedInBeginning;
        break;
      case 'decimalPrecisionTwoWithNaturalNumbers':
        this._regx = Constants.regExType.decimalPrecisionTwoWithNaturalNumbers;
        break;
      default:
        this._regx = Constants.regExType.alphanumWithSpecial1;
    }
  }

  validateInput = (inputValue: string, regx: RegExp): boolean => {
    return this.validationService.regExpValidator(regx.source, inputValue);
  }
}
