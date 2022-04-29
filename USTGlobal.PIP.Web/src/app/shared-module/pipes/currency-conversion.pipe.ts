import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'currencyConversion'
})
export class CurrencyConversionPipe implements PipeTransform {

  result: number;

  transform(value: number, usdToLocal: number): any {
    this.result = value * usdToLocal;
    return this.result;
  }
}
