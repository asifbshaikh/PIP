import { Pipe, PipeTransform } from '@angular/core';
import * as moment from 'moment';

@Pipe({
    name: 'negativeValue'
})

export class NegativeValuePipe implements PipeTransform {
    transform(value: string, format: string = ''): string {
        if (!value || value === '') {
            return '';
        }
        let stringMarginValue = '', originalMarginValue = '';
        stringMarginValue = value.toString();
        originalMarginValue = value.toString();
        if (stringMarginValue.indexOf('-') > -1) {
            originalMarginValue = stringMarginValue.replace('-', '');
            originalMarginValue = '(' + originalMarginValue.concat(')');
        }
        return originalMarginValue;
    }
}
