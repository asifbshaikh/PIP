using System;
using System.Collections.Generic;
using System.Linq;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Helpers
{
    public static class DateHelper
    {
        public static int GetNumberOfWorkingDays(DateTime startDate, DateTime endDate)
        {
            int workingDays = Convert.ToInt32((1 + ((endDate - startDate).TotalDays * 5 - (startDate.DayOfWeek - endDate.DayOfWeek) * 2) / 7));

            if (endDate.DayOfWeek == DayOfWeek.Saturday) workingDays--;
            if (startDate.DayOfWeek == DayOfWeek.Sunday) workingDays--;

            return workingDays;
        }

        public static int GetHolidaysCount(DateTime startDate, DateTime endDate, int locationId, IList<HolidayDTO> resourceHolidayDTO)
        {
            int count = 0;
            List<HolidayDTO> holidayDTO = new List<HolidayDTO>();
            holidayDTO = resourceHolidayDTO.Where(holiday => holiday.LocationId == locationId).ToList();

            holidayDTO.ForEach(holiday =>
            {
                if (holiday.Date >= startDate && holiday.Date <= endDate && holiday.Date.DayOfWeek != DayOfWeek.Sunday
                && holiday.Date.DayOfWeek != DayOfWeek.Saturday)
                {
                    // 0 - Sunday, 6 - Saturday
                    count++;
                }
            });
            return count;
        }

        public static int GetMonthsBetween(DateTime startDate, DateTime endDate)
        {
            return (endDate.Month + endDate.Year * 12) - (startDate.Month + startDate.Year * 12) + 1;
        }

        public static DateTime GetFirstDayOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static DateTime GetLastDayOfMonth(DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);
        }

        public static int getDaysInMonth(int month, int year)
        {

            int totalDaysInaMonth = System.DateTime.DaysInMonth(year, month);
            return totalDaysInaMonth;
        }

        public static int GetTotalDaysBetweenDates(DateTime startDateOriginal, DateTime endDateOriginal)
        {
            DateTime startDate = new DateTime(startDateOriginal.Year, startDateOriginal.Month, startDateOriginal.Day);
            DateTime endDate = new DateTime(endDateOriginal.Year, endDateOriginal.Month, endDateOriginal.Day);
            int millisecondsPerDay = 86400 * 1000; // Day in milliseconds
            TimeSpan startTimeSpan = new TimeSpan(0, 0, 0, 0, 1); // startDate.setHours(0, 0, 0, 1);  // Start just after midnight
            startDate = startDate + startTimeSpan;

            TimeSpan endTimeSpan = new TimeSpan(0, 23, 59, 59, 999);//endDate.setHours(23, 59, 59, 999);  // End just before midnight
            endDate = endDate + endTimeSpan;
            //  int diff = endDate.getTime() - startDate.getTime();  // Milliseconds between datetime objects
            double diff = endDate.Subtract(startDate).TotalMilliseconds;

            double days = Math.Ceiling(diff / millisecondsPerDay);
            return Convert.ToInt32(days);
        }

        public static int GetTotalDaysInMonths(DateTime startDate, DateTime endDate)
        {
            DateTime newStartDate = startDate;
            DateTime newEndDate = endDate;
            int startMonth = newStartDate.Month;
            int endMonth = newEndDate.Month;
            int startYear = newStartDate.Year;
            int endYear = newEndDate.Year;
            int sumOfDaysInMonths = 0, totalDaysInaMonth = 0;
            DateTime comparerDate = new DateTime(startYear, startMonth, 1);

            for (int i = startMonth, j = startYear; comparerDate <= endDate;)
            {
                if (j <= endYear)
                {
                    totalDaysInaMonth = DateHelper.getDaysInMonth(i, j);
                    sumOfDaysInMonths += totalDaysInaMonth;
                    i++;
                    if (i > 12)
                    {
                        i = 1;
                        j++;
                        comparerDate = new DateTime(j, i, 1);
                    }
                    else
                    {
                        comparerDate = new DateTime(j, i, 1);
                    }
                }
            }
            return sumOfDaysInMonths;
        }
    }
}
