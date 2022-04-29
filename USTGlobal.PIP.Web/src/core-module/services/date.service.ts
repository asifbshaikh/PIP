
import { Injectable } from '@angular/core';
import { LoggerService } from './logger.service';
import { SelectItem } from 'primeng/api';

@Injectable()
export class DateService {

  constructor(
    private logger: LoggerService,
  ) {
    this.logger.info('CookieService : constructor ');

  }

  // Get month names
  getMonthName(monthIndex) {
    const monthName = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct',
      'Nov', 'Dec'][monthIndex];
    return monthName;
  }

  // Check if leap year
  isLeapYear(year) {
    return ((year % 4 === 0 && year % 100 !== 0) || year % 400 === 0);
  }

  // Get days in month
  getDaysInMonth(month, year) {
    const totalDaysInaMonth = [31, (this.isLeapYear(year) ? 29 : 28), 31, 30, 31, 30, 31, 31, 30, 31, 30, 31][month];
    return totalDaysInaMonth;
  }

  // Get number of working days
  getNumberOfWorkingDays(startDateOriginal, endDateOriginal): number {
    const startDate = new Date(startDateOriginal);
    const endDate = new Date(endDateOriginal);

    const millisecondsPerDay = 86400 * 1000; // Day in milliseconds
    startDate.setHours(0, 0, 0, 1);  // Start just after midnight
    endDate.setHours(23, 59, 59, 999);  // End just before midnight
    const diff = endDate.getTime() - startDate.getTime();  // Milliseconds between datetime objects
    let days = Math.ceil(diff / millisecondsPerDay);

    const weeks = Math.floor(days / 7);
    days = days - (weeks * 2);

    // Handle special cases
    const startDay = startDate.getDay();
    const endDay = endDate.getDay();

    // Remove weekend not previously removed.
    if (startDay - endDay > 1) {
      days = days - 2;
    }

    // Remove start day if span starts on Sunday but ends before Saturday
    if (startDay === 0 && endDay !== 6) {
      days = days - 1;
    }

    // Remove end day if span ends on Saturday but starts after Sunday
    if (endDay === 6 && startDay !== 0) {
      days = days - 1;
    }
    return days;
  }

  // Get total days between dates
  getTotalDaysBetweenDates(startDateOriginal, endDateOriginal): number {
    const startDate = new Date(startDateOriginal);
    const endDate = new Date(endDateOriginal);
    const millisecondsPerDay = 86400 * 1000; // Day in milliseconds
    startDate.setHours(0, 0, 0, 1);  // Start just after midnight
    endDate.setHours(23, 59, 59, 999);  // End just before midnight
    const diff = endDate.getTime() - startDate.getTime();  // Milliseconds between datetime objects
    const days = Math.ceil(diff / millisecondsPerDay);
    return days;
  }

  // Get total days in months
  getTotalDaysInMonths(startDate, endDate): number {

    const newStartDate: Date = startDate;
    const newEndDate: Date = endDate;
    const startMonth = newStartDate.getMonth();
    const endMonth = newEndDate.getMonth();
    const startYear = newStartDate.getFullYear();
    const endYear = newEndDate.getFullYear();
    let sumOfDaysInMonths = 0;
    let totalDaysInaMonth = 0;
    let comparerDate: Date = new Date(startYear, startMonth, 1);

    for (let i = startMonth, j = startYear; comparerDate <= endDate;) {
      if (j <= endYear) {
        totalDaysInaMonth = this.getDaysInMonth(i, j);
        sumOfDaysInMonths = sumOfDaysInMonths + totalDaysInaMonth;
        i++;
        if (i > 11) {
          i = 0;
          j++;
          comparerDate = new Date(j, i, 1);
        }
        else {
          comparerDate = new Date(j, i, 1);
        }
      }
    }
    return sumOfDaysInMonths;
  }

  getDifferenceInMonths(date1: Date, date2: Date): number {
    let noOfMonths = 0;
    noOfMonths = ((date2.getMonth() - date1.getMonth()) + (12 * (date2.getFullYear() - date1.getFullYear())) + 1);
    return noOfMonths;
  }
}
