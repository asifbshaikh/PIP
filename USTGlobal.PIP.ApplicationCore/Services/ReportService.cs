using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Helpers;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository reportRepository;
        private readonly IEmailNotificationService emailNotificationService;

        public ReportService(IReportRepository reportRepository, IEmailNotificationService emailNotificationService)
        {
            this.reportRepository = reportRepository;
            this.emailNotificationService = emailNotificationService;
        }

        public async Task<List<AccountBasedProjectDTO>> GetAccountBasedProjects(IList<AccountDTO> accountsList)
        {
            return await this.reportRepository.GetAccountBasedProjects(accountsList);
        }

        public async Task<List<ReportKPIDTO>> GetCustomReportKPIList()
        {
            return await this.reportRepository.GetCustomReportKPIList();
        }

        public async Task<bool> GenerateProjectReport(ReportRequestDTO report, string userName, string reportsFolderPath)
        {
            switch (report.ReportType)
            {
                case 1:
                    return await GenerateCustomReport(report, userName, reportsFolderPath);
                case 2:
                    return await GenerateProjectSummaryViewReport(report, userName, reportsFolderPath);
                case 3:
                    return await GenerateProjectDetailedLevelReport(report, userName, reportsFolderPath);
                case 4:
                    return await GenerateProjectResourceLevelReport(report, userName, reportsFolderPath);
                default:
                    return false;
            }
        }

        private async Task<bool> GenerateProjectResourceLevelReport(ReportRequestDTO report, string userName, string reportsFolderPath)
        {
            ReportDBResultDTO reportDBResult = await this.reportRepository.GetProjectResourceLevelReport(report.SelectedProjects[0].ProjectId, report.IsUSDCurrency, userName);
            IList<ReportDTO> finalReport = await GetProjectResourceLevelObject(reportDBResult);
            await CreateReportExcel(finalReport, reportDBResult.ReportPeriods, reportDBResult.ReportEmail, report, ReportType.ProjectResourceLevel, reportsFolderPath);
            return false;
        }

        private async Task<bool> GenerateProjectSummaryViewReport(ReportRequestDTO report, string userName, string reportsFolderPath)
        {
            ReportDBResultDTO reportDBResult = await this.reportRepository.GetSummaryReportViewReport(report.StartDate, report.EndDate, report.SelectedProjects, report.IsUSDCurrency, userName);
            if (reportDBResult.PLForecastPeriods.Count > 0)
            {
                IList<ReportDTO> finalReport = await CreateProjectSummarViewObject(reportDBResult);
                await CreateReportExcel(finalReport, reportDBResult.ReportPeriods, reportDBResult.ReportEmail, report, ReportType.ProjectSummaryView, reportsFolderPath);
                return false;
            }
            else
            {
                return true;        // Throw Zero Record validation to user
            }
        }

        private async Task<bool> GenerateProjectDetailedLevelReport(ReportRequestDTO report, string userName, string reportsFolderPath)
        {
            ReportDBResultDTO reportDBResult = await this.reportRepository.GetProjectDetailedLevelReport(report.StartDate, report.EndDate, report.SelectedProjects, report.IsUSDCurrency, userName);
            if (reportDBResult.PLForecastPeriods.Count > 0)
            {
                IList<ReportDTO> finalReport = await CreateProjectDetailedLevelObject(reportDBResult);
                await CreateReportExcel(finalReport, reportDBResult.ReportPeriods, reportDBResult.ReportEmail, report, ReportType.ProjectDetailedLevel, reportsFolderPath);
                return false;
            }
            else
            {
                return true;        // Throw Zero Record validation to user
            }
        }

        private async Task<bool> GenerateCustomReport(ReportRequestDTO report, string userName, string reportsFolderPath)
        {
            ReportDBResultDTO reportDBResult = await this.reportRepository.GetCustomReportData(report.StartDate, report.EndDate, report.SelectedProjects, report.IsUSDCurrency, userName, report.SelectedKPIs);
            if (reportDBResult.ReportResourcePeriodDetails.Count > 0)
            {
                IList<ReportDTO> finalReport = await GetCustomReportDataObject(reportDBResult);
                await CreateReportExcel(finalReport, reportDBResult.ReportPeriods, reportDBResult.ReportEmail, report, ReportType.CustomReport, reportsFolderPath);
                return false;
            }
            else
            {
                return true;        // Throw Zero Record validation to user
            }
        }

        public async Task<IList<ReportDTO>> GetCustomReportDataObject(ReportDBResultDTO reportDBResult)
        {
            // Write logic to create Custom Report object
            IList<ReportDTO> finalReport = new List<ReportDTO>();
            for (int i = 0; i < reportDBResult.ReportFirstHalf.Count; i++)
            {
                IList<ReportResourceDTO> reportResource = (from r in reportDBResult.ReportResources
                                                           where r.PipSheetId == reportDBResult.ReportFirstHalf[i].PipSheetId
                                                           select r).ToList();

                for (int l = 0; l < reportDBResult.ReportKPIs.Count; l++)
                {
                    for (int j = 0; j < reportResource.Count; j++)
                    {
                        decimal reportPeriodsForAvgRates = 0;
                        ReportDTO report = new ReportDTO();
                        report = PopulateReportFirstHalfFields(report, reportDBResult.ReportFirstHalf[i]);
                        report.Billablity = reportResource[j].Billablity;
                        report.PhaseMilestone = reportResource[j].PhaseMilestone;
                        report.Location = reportResource[j].Location;
                        report.Role = reportResource[j].Role;
                        report.KPI = reportDBResult.ReportKPIs[l].KPIName;
                        report.KPIId = reportDBResult.ReportKPIs[l].KPIId;
                        report.Band = reportResource[j].Band;
                        report.RoleGroup = reportResource[j].RoleGroup;

                        int projectResourceId = reportResource[j].ProjectResourceId;
                        decimal reportTotal = 0;
                        report.PeriodFields = new List<PeriodReportDTO>();
                        // Logic to write 19 KPIs for this PipSheetId
                        for (int k = 0; k < reportDBResult.ReportPeriods.Count; k++)
                        {
                            // Logic to write Periods
                            PeriodReportDTO period = new PeriodReportDTO();
                            period.BillingPeriodId = reportDBResult.ReportPeriods[k].BillingPeriodId;
                            period.Amount = (from p in reportDBResult.ReportResourcePeriodDetails
                                             where p.ProjectResourceId == projectResourceId
                                             && p.BillingPeriodId == reportDBResult.ReportPeriods[k].BillingPeriodId
                                             select GetPeriodReportValueForCustomReportKPIs(reportDBResult.ReportKPIs[l], p)).FirstOrDefault();
                            reportTotal += period.Amount;
                            report.PeriodFields.Add(period);

                            if (reportDBResult.ReportKPIs[l].KPIId == 2 || reportDBResult.ReportKPIs[l].KPIId == 4)
                            {
                                if (period.Amount > 0)
                                {
                                    reportPeriodsForAvgRates = reportPeriodsForAvgRates + 1;
                                }
                            }
                        }
                        if (reportDBResult.ReportKPIs[l].KPIId == 2 || reportDBResult.ReportKPIs[l].KPIId == 4)
                        {
                            if (reportTotal == 0)
                            {
                                // When KPI = 2: Bill Rate or KPI = 2: Cost Rate, show Average values in report and period totals
                                report.ReportTotal = 0;
                                report.ProjectTotal = 0;
                            }
                            else
                            {
                                // When KPI = 2: Bill Rate or KPI = 2: Cost Rate, show Average values in report and period totals
                                report.ReportTotal = reportTotal / (reportPeriodsForAvgRates == 0 ? reportResource[j].PeriodsCount : reportPeriodsForAvgRates);
                                report.ProjectTotal = GetProjectTotalForCustomReportKPIs(reportDBResult.ReportKPIs[l], reportResource[j]) / reportResource[j].PeriodsCount;
                            }
                        }
                        else
                        {
                            report.ReportTotal = reportTotal;
                            report.ProjectTotal = GetProjectTotalForCustomReportKPIs(reportDBResult.ReportKPIs[l], reportResource[j]);
                        }

                        finalReport.Add(report);
                    }
                }
            }
            return finalReport;
        }

        private async Task<IList<ReportDTO>> GetProjectResourceLevelObject(ReportDBResultDTO reportDBResult)
        {
            int[] InvoicingDescriptionId = new int[] { 2, 4, 5 };   // 2 => Total Invoiced, 3 => Net Cash Flow, 4 => Cumulative Cash Flow
            IList<ReportDTO> finalReport = new List<ReportDTO>();
            bool addInvoicingReportKPI = false;
            for (int i = 0; i < reportDBResult.ReportFirstHalf.Count; i++)
            {
                IList<ReportResourceDTO> reportResource = reportDBResult.ReportResources;
                for (int l = 0; l < reportDBResult.ReportKPIs.Count; l++)
                {

                    // Section for the Information belonging to plforecast DTO : 
                    if (reportDBResult.ReportKPIs[l].PLForecastLabelId != null)
                    {
                        ReportDTO report = new ReportDTO();
                        report = PopulateReportFirstHalfFields(report, reportDBResult.ReportFirstHalf[i]);
                        report.KPI = reportDBResult.ReportKPIs[l].KPIName;
                        report.KPIId = reportDBResult.ReportKPIs[l].KPIId;
                        decimal reportTotal = 0;
                        int plForecastId = (from pl in reportDBResult.PLForecast
                                            where pl.PipSheetId == reportDBResult.ReportFirstHalf[i].PipSheetId
                                            && pl.PLForecastLabelId == reportDBResult.ReportKPIs[l].PLForecastLabelId
                                            select pl.PLForecastId).FirstOrDefault();

                        report.ReportTotal = report.ProjectTotal = (from pl in reportDBResult.PLForecast
                                                                    where pl.PLForecastId == plForecastId
                                                                    select pl.TotalAmount).FirstOrDefault();

                        report.PeriodFields = new List<PeriodReportDTO>();

                        for (int k = 0; k < reportDBResult.ReportPeriods.Count; k++)
                        {
                            // Logic to write Periods
                            PeriodReportDTO period = new PeriodReportDTO();
                            period.BillingPeriodId = reportDBResult.ReportPeriods[k].BillingPeriodId;
                            period.Amount = (from p in reportDBResult.PLForecastPeriods
                                             where p.PLForecastId == plForecastId
                                             && p.BillingPeriodId == reportDBResult.ReportPeriods[k].BillingPeriodId
                                             select p.Amount).FirstOrDefault();
                            reportTotal += period.Amount;
                            report.PeriodFields.Add(period);
                        }
                        finalReport.Add(report);

                        // adding invoicing schedule KPIs
                        if (l == (reportDBResult.ReportKPIs.Count - 1))
                        {
                            addInvoicingReportKPI = true;
                            goto InvoicingScheduleFields;
                        }
                    }
                    else  // Section for Resource level DTO
                    {
                        for (int j = 0; j < reportResource.Count; j++)
                        {
                            int reportKPIId = reportDBResult.ReportKPIs[l].KPIId;
                            // 10: Cumulative cash flow , 20: Net Cash Flow, 27: Total Invoiced, 19: Margin calculation
                            if ((reportKPIId == 10 || reportKPIId == 20 || reportKPIId == 27 || reportKPIId == 19))
                            {
                                break;
                            }

                            ReportDTO report = new ReportDTO();
                            report = PopulateReportFirstHalfFields(report, reportDBResult.ReportFirstHalf[i]);
                            report.Billablity = reportResource[j].Billablity;
                            report.PhaseMilestone = reportResource[j].PhaseMilestone;
                            report.Location = reportResource[j].Location;
                            report.Role = reportResource[j].Role;
                            report.KPI = reportDBResult.ReportKPIs[l].KPIName;
                            report.KPIId = reportDBResult.ReportKPIs[l].KPIId;
                            report.Band = reportResource[j].Band;
                            report.RoleGroup = reportResource[j].RoleGroup;

                            int projectResourceId = reportResource[j].ProjectResourceId;
                            decimal reportTotal = 0;
                            report.PeriodFields = new List<PeriodReportDTO>();
                            // Logic to write 19 KPIs for this PipSheetId
                            for (int k = 0; k < reportDBResult.ReportPeriods.Count; k++)
                            {
                                // Logic to write Periods
                                PeriodReportDTO period = new PeriodReportDTO();
                                period.BillingPeriodId = reportDBResult.ReportPeriods[k].BillingPeriodId;
                                period.Amount = (from p in reportDBResult.ReportResourcePeriodDetails
                                                 where p.ProjectResourceId == projectResourceId
                                                 && p.BillingPeriodId == reportDBResult.ReportPeriods[k].BillingPeriodId
                                                 select GetPeriodReportValueForCustomReportKPIs(reportDBResult.ReportKPIs[l], p)).FirstOrDefault();
                                reportTotal += period.Amount;
                                report.PeriodFields.Add(period);
                            }
                            if (reportDBResult.ReportKPIs[l].KPIId == 2 || reportDBResult.ReportKPIs[l].KPIId == 4)
                            {
                                if (reportTotal == 0)
                                {
                                    report.ProjectTotal = 0;
                                    report.ReportTotal = 0;
                                }
                                else
                                {
                                    // When KPI = 2: Bill Rate or KPI = 2: Cost Rate, show Average values in report and period totals
                                    report.ProjectTotal = GetProjectTotalForCustomReportKPIs(reportDBResult.ReportKPIs[l], reportResource[j]) / reportResource[j].PeriodsCount;
                                    report.ReportTotal = report.ProjectTotal;
                                }
                            }
                            else
                            {
                                report.ReportTotal = reportTotal;
                                report.ProjectTotal = GetProjectTotalForCustomReportKPIs(reportDBResult.ReportKPIs[l], reportResource[j]);
                            }
                            finalReport.Add(report);
                        }
                    }


                // Execution will come here after loading all the KPIs in the finalReport object
                InvoicingScheduleFields:
                    if (addInvoicingReportKPI)
                    {
                        for (int j = 0; j < InvoicingDescriptionId.Length; j++)
                        {
                            ReportDTO report = new ReportDTO();
                            int clientPriceId = (from ris in reportDBResult.ReportInvoicingSchedule
                                                 where ris.PipSheetId == reportDBResult.ReportFirstHalf[i].PipSheetId
                                                 && ris.DescriptionId == InvoicingDescriptionId[j]
                                                 select ris.ClientPriceId).FirstOrDefault();

                            report = PopulateReportFirstHalfFields(report, reportDBResult.ReportFirstHalf[i]);
                            report.KPI = PopulateInvoicingScheduleKPIName(reportDBResult.ReportInvoicingSchedule[j].DescriptionId ?? 0);
                            report.ReportTotal = report.ProjectTotal = (from pl in reportDBResult.ReportInvoicingSchedule
                                                                        where pl.ClientPriceId == clientPriceId
                                                                        select (pl.TotalPrice ?? 0)).FirstOrDefault();
                            report.KPIId = reportDBResult.ReportKPIs[l].KPIId;
                            decimal reportTotal = 0;
                            report.PeriodFields = new List<PeriodReportDTO>();
                            // Logic to write 19 KPIs for this PipSheetId
                            for (int k = 0; k < reportDBResult.ReportPeriods.Count; k++)
                            {
                                // Logic to write Periods
                                PeriodReportDTO period = new PeriodReportDTO();
                                period.BillingPeriodId = reportDBResult.ReportPeriods[k].BillingPeriodId;
                                period.Amount = (from risp in reportDBResult.ReportInvoicingSchedulePeriodDetails
                                                 where risp.ClientPriceId == clientPriceId
                                                 && risp.BillingPeriodId == reportDBResult.ReportPeriods[k].BillingPeriodId
                                                 select (risp.Price ?? 0)).FirstOrDefault();
                                reportTotal += period.Amount;
                                report.PeriodFields.Add(period);
                            }
                            // For cummulative cash flow report total should e 
                            if (j == 2)
                            {
                                report.ReportTotal = 0;
                                report.ProjectTotal = 0;
                            }
                            finalReport.Add(report);
                        }
                    }



                }
            }

            return finalReport;
        }


        public decimal GetProjectTotalForCustomReportKPIs(ReportKPIDTO reportKPI, ReportResourceDTO reportResource)
        {
            if (reportKPI.KPIId == 2)        // Bill Rate
            {
                return reportResource.BillRatePerResource;
            }
            if (reportKPI.KPIId == 6)        // Billed hours
            {
                return reportResource.TotalHoursPerResource;
            }
            if (reportKPI.KPIId == 9)        // Cost hours
            {
                return reportResource.CostHrsPerResource;
            }
            else if (reportKPI.KPIId == 3)   // Cost of Staffling
            {
                return reportResource.CappedCostPerResource;
            }
            else if (reportKPI.KPIId == 4)   // Cost Rate
            {
                return reportResource.CostRatePerResource;
            }
            else if (reportKPI.KPIId == 1)  // Labor Revenue
            {
                return reportResource.RevenuePerResource;
            }
            else
            {
                return reportResource.FTEPerResource;     // Role Level FTE
            }
        }
        public decimal GetPeriodReportValueForCustomReportKPIs(ReportKPIDTO reportKPI, ReportResourcePeriodDetailDTO reportResourcePeriod)
        {
            if (reportKPI.KPIId == 2)        // Bill Rate
            {
                return reportResourcePeriod.FTE == 0 ? 0 : reportResourcePeriod.BillRate;
            }
            if (reportKPI.KPIId == 6)        // Billed hours
            {
                return reportResourcePeriod.TotalHours;
            }
            if (reportKPI.KPIId == 9)        // Cost hours
            {
                return reportResourcePeriod.CostHours;
            }
            else if (reportKPI.KPIId == 3)   // Cost of Staffling
            {
                return reportResourcePeriod.CappedCost;
            }
            else if (reportKPI.KPIId == 4)   // Cost Rate
            {
                return reportResourcePeriod.FTE == 0 ? 0 : reportResourcePeriod.CostRate;
            }
            else if (reportKPI.KPIId == 1)  // Labor Revenue
            {
                return reportResourcePeriod.Revenue;
            }
            else
            {
                return reportResourcePeriod.FTE;     // Role Level FTE
            }
        }

        private async Task<IList<ReportDTO>> CreateProjectDetailedLevelObject(ReportDBResultDTO summaryViewReportDBResult)
        {
            // ReportFirstHalf indicates number of PipSheets to be shown in the report
            // Report KPIs indicates number of KPIs to be shown for each report
            // ReportPeriods indicates periods to be shown for each KPI for each PipSheet

            int noOfRowsPerPipSheet = summaryViewReportDBResult.ReportKPIs.Count + 3;     // 3 => No. of Invoicing Schedule rows; Total 22 rows per Pip Sheet
            int[] InvoicingDescriptionId = new int[] { 2, 4, 5 };   // 2 => Total Invoiced, 3 => Net Cash Flow, 4 => Cumulative Cash Flow
            bool addInvoicingReportKPI = false;


            IList<ReportDTO> finalReport = new List<ReportDTO>();

            for (int i = 0; i < summaryViewReportDBResult.ReportFirstHalf.Count; i++)
            {
                for (int j = 0; j < summaryViewReportDBResult.ReportKPIs.Count; j++)
                {
                    // Populate first 22 PLForecast Summary related fields
                    int reportKPIId = summaryViewReportDBResult.ReportKPIs[j].KPIId;
                    // 10: Cumulative cash flow , 20: Net Cash Flow, 27: Total Invoiced, 19: Margin calculation
                    if ((reportKPIId == 10 || reportKPIId == 20 || reportKPIId == 27 || reportKPIId == 19))
                    {
                        continue;
                    }
                    ReportDTO report = new ReportDTO();
                    int plForecastId = (from pl in summaryViewReportDBResult.PLForecast
                                        where pl.PipSheetId == summaryViewReportDBResult.ReportFirstHalf[i].PipSheetId
                                        && pl.PLForecastLabelId == summaryViewReportDBResult.ReportKPIs[j].PLForecastLabelId
                                        select pl.PLForecastId).FirstOrDefault();
                    report = PopulateReportFirstHalfFields(report, summaryViewReportDBResult.ReportFirstHalf[i]);

                    report.KPI = summaryViewReportDBResult.ReportKPIs[j].KPIName;
                    report.KPIId = summaryViewReportDBResult.ReportKPIs[j].KPIId;
                    report.ProjectTotal = (from pl in summaryViewReportDBResult.PLForecast
                                           where pl.PLForecastId == plForecastId
                                           select pl.TotalAmount).FirstOrDefault();
                    decimal reportTotal = 0;
                    report.PeriodFields = new List<PeriodReportDTO>();
                    // Logic to write 22 KPIs for this PipSheetId
                    for (int k = 0; k < summaryViewReportDBResult.ReportPeriods.Count; k++)
                    {
                        // Logic to write Periods
                        PeriodReportDTO period = new PeriodReportDTO();
                        period.BillingPeriodId = summaryViewReportDBResult.ReportPeriods[k].BillingPeriodId;
                        period.Amount = (from p in summaryViewReportDBResult.PLForecastPeriods
                                         where p.PLForecastId == plForecastId
                                         && p.BillingPeriodId == summaryViewReportDBResult.ReportPeriods[k].BillingPeriodId
                                         select p.Amount).FirstOrDefault();
                        reportTotal += period.Amount;
                        report.PeriodFields.Add(period);
                    }
                    report.ReportTotal = reportTotal;
                    finalReport.Add(report);

                    if (j == (summaryViewReportDBResult.ReportKPIs.Count - 1))
                    {
                        addInvoicingReportKPI = true;
                        goto InvoicingScheduleFields;
                    }
                }
            // Execution will come here after loading all the KPIs in the finalReport object
            InvoicingScheduleFields:
                if (addInvoicingReportKPI)
                {
                    for (int j = 0; j < InvoicingDescriptionId.Length; j++)
                    {
                        ReportDTO report = new ReportDTO();
                        int clientPriceId = (from ris in summaryViewReportDBResult.ReportInvoicingSchedule
                                             where ris.PipSheetId == summaryViewReportDBResult.ReportFirstHalf[i].PipSheetId
                                             && ris.DescriptionId == InvoicingDescriptionId[j]
                                             select ris.ClientPriceId).FirstOrDefault();

                        report = PopulateReportFirstHalfFields(report, summaryViewReportDBResult.ReportFirstHalf[i]);
                        report.KPI = PopulateInvoicingScheduleKPIName(summaryViewReportDBResult.ReportInvoicingSchedule[j].DescriptionId ?? 0);
                        report.ProjectTotal = (from pl in summaryViewReportDBResult.ReportInvoicingSchedule
                                               where pl.ClientPriceId == clientPriceId
                                               select (pl.TotalPrice ?? 0)).FirstOrDefault();
                        decimal reportTotal = 0;
                        report.PeriodFields = new List<PeriodReportDTO>();
                        // Logic to write 19 KPIs for this PipSheetId
                        for (int k = 0; k < summaryViewReportDBResult.ReportPeriods.Count; k++)
                        {
                            // Logic to write Periods
                            PeriodReportDTO period = new PeriodReportDTO();
                            period.BillingPeriodId = summaryViewReportDBResult.ReportPeriods[k].BillingPeriodId;
                            period.Amount = (from risp in summaryViewReportDBResult.ReportInvoicingSchedulePeriodDetails
                                             where risp.ClientPriceId == clientPriceId
                                             && risp.BillingPeriodId == summaryViewReportDBResult.ReportPeriods[k].BillingPeriodId
                                             select (risp.Price ?? 0)).FirstOrDefault();
                            reportTotal += period.Amount;
                            report.PeriodFields.Add(period);
                        }
                        if (report.KPI == Constants.ReportCumulativeCashFlowKPI)
                        {
                            report.ReportTotal = 0;
                            report.ProjectTotal = 0;
                        }
                        else
                        {
                            report.ReportTotal = reportTotal;
                        }

                        finalReport.Add(report);
                    }
                }
            }
            return finalReport;
        }

        private ReportDTO PopulateReportFirstHalfFields(ReportDTO report, ReportFirstHalfDTO reportFirstHalf)
        {
            report.SFProjectId = reportFirstHalf.SFProjectId;
            report.AccountName = reportFirstHalf.AccountName;
            report.ProjectName = reportFirstHalf.ProjectName;
            report.ProjectStatus = reportFirstHalf.ProjectStatus;
            report.DeliveryOwner = reportFirstHalf.DeliveryOwner;
            report.Portfolio = reportFirstHalf.Portfolio;
            report.ServiceLine = reportFirstHalf.ServiceLine;
            report.DeliveryType = reportFirstHalf.DeliveryType;
            report.BillingType = reportFirstHalf.BillingType;
            report.Currency = reportFirstHalf.Currency;
            report.StartDate = reportFirstHalf.StartDate;
            report.EndDate = reportFirstHalf.EndDate;

            return report;
        }

        private string PopulateInvoicingScheduleKPIName(int descriptionId)
        {
            if (descriptionId == 2)
            {
                return Constants.ReportTotalInvoicedKPI;
            }
            else if (descriptionId == 4)
            {
                return Constants.ReportNetCashFlowKPI;
            }
            else
            {
                return Constants.ReportCumulativeCashFlowKPI;
            }
        }

        private async Task<IList<ReportDTO>> CreateProjectSummarViewObject(ReportDBResultDTO summaryViewReportDBResult)
        {
            // ReportFirstHalf indicates number of PipSheets to be shown in the report
            // Report KPIs indicates number of KPIs to be shown for each report
            // ReportPeriods indicates periods to be shown for each KPI for each PipSheet

            IList<ReportDTO> finalReport = new List<ReportDTO>();

            for (int i = 0; i < summaryViewReportDBResult.ReportFirstHalf.Count; i++)
            {
                for (int j = 0; j < summaryViewReportDBResult.ReportKPIs.Count; j++)
                {
                    ReportDTO report = new ReportDTO();
                    int plForecastId = (from pl in summaryViewReportDBResult.PLForecast
                                        where pl.PipSheetId == summaryViewReportDBResult.ReportFirstHalf[i].PipSheetId
                                        && pl.PLForecastLabelId == summaryViewReportDBResult.ReportKPIs[j].PLForecastLabelId
                                        select pl.PLForecastId).FirstOrDefault();
                    report = PopulateReportFirstHalfFields(report, summaryViewReportDBResult.ReportFirstHalf[i]);
                    report.KPI = summaryViewReportDBResult.ReportKPIs[j].KPIName;
                    report.KPIId = summaryViewReportDBResult.ReportKPIs[j].KPIId;
                    report.ProjectTotal = (from pl in summaryViewReportDBResult.PLForecast
                                           where pl.PLForecastId == plForecastId
                                           select pl.TotalAmount).FirstOrDefault();
                    decimal reportTotal = 0;
                    report.PeriodFields = new List<PeriodReportDTO>();
                    // Logic to write 4 KPIs for this PIpSheetId
                    for (int k = 0; k < summaryViewReportDBResult.ReportPeriods.Count; k++)
                    {
                        // Logic to write Periods
                        PeriodReportDTO period = new PeriodReportDTO();
                        period.BillingPeriodId = summaryViewReportDBResult.ReportPeriods[k].BillingPeriodId;
                        period.Amount = (from p in summaryViewReportDBResult.PLForecastPeriods
                                         where p.PLForecastId == plForecastId
                                         && p.BillingPeriodId == summaryViewReportDBResult.ReportPeriods[k].BillingPeriodId
                                         select p.Amount).FirstOrDefault();
                        reportTotal += period.Amount;
                        report.PeriodFields.Add(period);
                    }
                    report.ReportTotal = reportTotal;
                    finalReport.Add(report);
                }
            }
            return finalReport;
        }

        public async Task CreateReportExcel(IList<ReportDTO> finalReport, IList<ReportPeriodDTO> reportPeriods, ReportEmail reportEmail, ReportRequestDTO report, ReportType reportType, string reportsFolderPath)
        {
            int reportDataStartRow = 0;
            string fileName = GenerateReportFileName(reportType, report.IsUSDCurrency);

            //FileInfo file = new FileInfo(Path.Combine(rootFolder, fileName));

            using (FileStream s = File.Create(Path.Combine(reportsFolderPath, fileName)))
            {
                using (ExcelPackage package = new ExcelPackage(s))
                {
                    // Deleting the Report worksheet if present earlier
                    if (package.Workbook.Worksheets.Where(x => x.Name == Constants.ReportWorksheetName).Any())
                    {
                        package.Workbook.Worksheets.Delete("Report");
                    }

                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(Constants.ReportWorksheetName);
                    int totalRows = finalReport.Count();

                    // Populate the Report Request Details
                    worksheet.Cells[1, 1].Value = "Report Details";
                    worksheet.Cells[1, 1].Style.Font.Bold = true;
                    worksheet.Cells[1, 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, 1].Style.Fill.BackgroundColor.SetColor(Color.Orange);

                    worksheet.Cells[3, 1].Value = "Report Name";
                    worksheet.Cells[4, 1].Value = "Accounts Selected";
                    worksheet.Cells[5, 1].Value = "Projects Selected";
                    worksheet.Cells[6, 1].Value = "Report Currency";
                    worksheet.Cells[3, 1].Style.Font.Bold = true;
                    worksheet.Cells[4, 1].Style.Font.Bold = true;
                    worksheet.Cells[5, 1].Style.Font.Bold = true;
                    worksheet.Cells[6, 1].Style.Font.Bold = true;

                    worksheet.Cells[3, 2].Value = GetReportName(reportType);
                    worksheet.Cells[4, 2].Value = ShowCommaSeparatedAccounts(report.SelectedAccounts);
                    worksheet.Cells[5, 2].Value = ShowCommaSeparatedProjectIds(report.SelectedProjects);
                    worksheet.Cells[6, 2].Value = report.IsUSDCurrency ? Constants.USDCurrency : Constants.PipCurrency;
                    worksheet.Cells[3, 2].Style.WrapText = true;
                    worksheet.Cells[4, 2].Style.WrapText = true;
                    worksheet.Cells[5, 2].Style.WrapText = true;
                    worksheet.Cells[6, 2].Style.WrapText = true;

                    if (reportType != ReportType.ProjectResourceLevel)
                    {
                        if (reportType == ReportType.CustomReport)
                        {
                            worksheet.Cells[7, 1].Value = "KPIs Selected";
                            worksheet.Cells[7, 1].Style.Font.Bold = true;
                            worksheet.Cells[7, 2].Value = ShowCommaSeparatedKPIs(report.SelectedKPIs);
                            worksheet.Cells[7, 2].Style.WrapText = true;

                            worksheet.Cells[8, 1].Value = "Start Date";
                            worksheet.Cells[9, 1].Value = "End Date";
                            worksheet.Cells[10, 1].Value = "Requested On";
                            worksheet.Cells[8, 1].Style.Font.Bold = true;
                            worksheet.Cells[9, 1].Style.Font.Bold = true;
                            worksheet.Cells[10, 1].Style.Font.Bold = true;
                            worksheet.Cells[8, 2].Value = report.StartDate.ToString("MM/yyyy");
                            worksheet.Cells[9, 2].Value = report.EndDate.ToString("MM/yyyy");
                            worksheet.Cells[10, 2].Value = DateTime.Now.ToString("MM-dd-yyyy HH-mm-ss");
                            worksheet.Cells[8, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[9, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[10, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            reportDataStartRow = 12;
                        }
                        else
                        {
                            worksheet.Cells[7, 1].Value = "Start Date";
                            worksheet.Cells[8, 1].Value = "End Date";
                            worksheet.Cells[9, 1].Value = "Requested On";
                            worksheet.Cells[7, 1].Style.Font.Bold = true;
                            worksheet.Cells[8, 1].Style.Font.Bold = true;
                            worksheet.Cells[9, 1].Style.Font.Bold = true;
                            worksheet.Cells[7, 2].Value = report.StartDate.ToString("MM/yyyy");
                            worksheet.Cells[8, 2].Value = report.EndDate.ToString("MM/yyyy");
                            worksheet.Cells[9, 2].Value = DateTime.Now.ToString("MM-dd-yyyy HH-mm-ss");
                            worksheet.Cells[7, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[8, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            worksheet.Cells[9, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            reportDataStartRow = 11;
                        }

                    }
                    else
                    {
                        worksheet.Cells[7, 1].Value = "Requested On";
                        worksheet.Cells[7, 1].Style.Font.Bold = true;
                        worksheet.Cells[7, 2].Value = DateTime.Now.ToString("MM/dd/yyyy HH-mm-ss");
                        worksheet.Cells[7, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        reportDataStartRow = 9;
                    }

                    // Set Headers
                    worksheet.Cells[reportDataStartRow, 1].Value = "Project Id";
                    worksheet.Cells[reportDataStartRow, 2].Value = "Account Name";
                    worksheet.Cells[reportDataStartRow, 3].Value = "Project Name";
                    worksheet.Cells[reportDataStartRow, 4].Value = "PIP Status";
                    worksheet.Cells[reportDataStartRow, 5].Value = "Delivery Owner";
                    worksheet.Cells[reportDataStartRow, 6].Value = "Service Portfolio Group";
                    worksheet.Cells[reportDataStartRow, 7].Value = "Service Line";
                    worksheet.Cells[reportDataStartRow, 8].Value = "Delivery Type";
                    worksheet.Cells[reportDataStartRow, 9].Value = "Billing Type";
                    worksheet.Cells[reportDataStartRow, 10].Value = "PIP Currency";
                    worksheet.Cells[reportDataStartRow, 11].Value = "Start Date";
                    worksheet.Cells[reportDataStartRow, 12].Value = "End Date";
                    worksheet.Cells[reportDataStartRow, 13].Value = "KPIs";
                    worksheet.Cells[reportDataStartRow, 14].Value = "Project Total";
                    worksheet.Cells[reportDataStartRow, 15].Value = "Report Total";

                    // Logic to populate period headers
                    if (reportType == ReportType.CustomReport || reportType == ReportType.ProjectResourceLevel)
                    {
                        worksheet.Cells[reportDataStartRow, 16].Value = "Role";
                        worksheet.Cells[reportDataStartRow, 17].Value = "Role Group";
                        worksheet.Cells[reportDataStartRow, 18].Value = "Grade";
                        worksheet.Cells[reportDataStartRow, 19].Value = "Location";
                        worksheet.Cells[reportDataStartRow, 20].Value = "Phase/ Milestone";
                        worksheet.Cells[reportDataStartRow, 21].Value = "Billablity";

                        int k = 0;
                        for (int x = 22; x < reportPeriods.Count + 1 + 21; x++)
                        {
                            worksheet.Cells[reportDataStartRow, x].Value = reportPeriods[k].Period;
                            k++;
                        }
                    }
                    else
                    {
                        int k = 0;
                        for (int x = 16; x < reportPeriods.Count + 1 + 15; x++)
                        {
                            worksheet.Cells[reportDataStartRow, x].Value = reportPeriods[k].Period;
                            k++;
                        }
                    }

                    // Populate values from the second row
                    int i = 0;
                    for (int row = reportDataStartRow + 1; row <= totalRows + reportDataStartRow; row++)
                    {
                        worksheet.Cells[row, 1].Value = finalReport[i].SFProjectId;
                        worksheet.Cells[row, 2].Value = finalReport[i].AccountName;
                        worksheet.Cells[row, 3].Value = finalReport[i].ProjectName;
                        worksheet.Cells[row, 4].Value = finalReport[i].ProjectStatus;
                        worksheet.Cells[row, 5].Value = finalReport[i].DeliveryOwner;
                        worksheet.Cells[row, 6].Value = finalReport[i].Portfolio;
                        worksheet.Cells[row, 7].Value = finalReport[i].ServiceLine;
                        worksheet.Cells[row, 8].Value = finalReport[i].DeliveryType;
                        worksheet.Cells[row, 9].Value = finalReport[i].BillingType;
                        worksheet.Cells[row, 10].Value = finalReport[i].Currency;
                        worksheet.Cells[row, 11].Value = Convert.ToDateTime(finalReport[i].StartDate);
                        worksheet.Cells[row, 12].Value = Convert.ToDateTime(finalReport[i].EndDate);
                        worksheet.Cells[row, 13].Value = finalReport[i].KPI;
                        if (finalReport[i].KPIId == 16 || finalReport[i].KPIId == 23)   // Gross Margin % or Project Ebitda %
                        {
                            worksheet.Cells[row, 14].Value = Math.Round(finalReport[i].ProjectTotal, 2) + "%";
                            worksheet.Cells[row, 15].Value = Math.Round(finalReport[i].ReportTotal, 2) + "%";
                            worksheet.Cells[row, 14].Style.Numberformat.Format = "##0.00%";
                            worksheet.Cells[row, 15].Style.Numberformat.Format = "##0.00%";
                            worksheet.Cells[row, 14].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            worksheet.Cells[row, 15].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        }
                        else
                        {
                            worksheet.Cells[row, 14].Value = Math.Round(finalReport[i].ProjectTotal, 2);
                            worksheet.Cells[row, 15].Value = Math.Round(finalReport[i].ReportTotal, 2);
                        }

                        // Logic to populate period values
                        if (reportType == ReportType.CustomReport || reportType == ReportType.ProjectResourceLevel)
                        {
                            worksheet.Cells[row, 16].Value = finalReport[i].Role;
                            worksheet.Cells[row, 17].Value = finalReport[i].RoleGroup;
                            worksheet.Cells[row, 18].Value = finalReport[i].Band;
                            worksheet.Cells[row, 19].Value = finalReport[i].Location;
                            worksheet.Cells[row, 20].Value = finalReport[i].PhaseMilestone;
                            worksheet.Cells[row, 21].Value = finalReport[i].Billablity;

                            int k = 0;
                            for (int x = 22; x < reportPeriods.Count + 1 + 21; x++)     // 21 - number of columns already occupied in row
                            {
                                if (finalReport[i].KPIId == 16 || finalReport[i].KPIId == 23)   // Gross Margin % or Project Ebitda %
                                {
                                    worksheet.Cells[row, x].Value = Math.Round(finalReport[i].PeriodFields[k].Amount, 2) + "%";
                                    worksheet.Cells[row, x].Style.Numberformat.Format = "##0.00%";
                                    worksheet.Cells[row, x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                }
                                else
                                {
                                    worksheet.Cells[row, x].Value = Math.Round(finalReport[i].PeriodFields[k].Amount, 2);
                                }
                                k++;
                            }
                            i++;
                        }
                        else
                        {
                            int k = 0;
                            for (int x = 16; x < reportPeriods.Count + 1 + 15; x++)     // 15 - number of columns already occupied in row
                            {
                                if (finalReport[i].KPIId == 16 || finalReport[i].KPIId == 23)   // Gross Margin % or Project Ebitda %
                                {
                                    worksheet.Cells[row, x].Value = Math.Round(finalReport[i].PeriodFields[k].Amount, 2) + "%";
                                    worksheet.Cells[row, x].Style.Numberformat.Format = "##0.00%";
                                    worksheet.Cells[row, x].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                }
                                else
                                {
                                    worksheet.Cells[row, x].Value = Math.Round(finalReport[i].PeriodFields[k].Amount, 2);
                                }
                                k++;
                            }
                            i++;
                        }
                    }
                    worksheet = ApplyExcelFormattingOnFirstRow(worksheet, reportDataStartRow);
                    package.Save();
                }
                s.Close();
            }

            // Mail Sending Logic
            EmailDTO emailDTO = await PopulateEmailDetails(reportEmail, report, fileName, Path.Combine(reportsFolderPath, fileName), reportType);
            await emailNotificationService.SendEmail(emailDTO);
        }

        public ExcelWorksheet ApplyExcelFormattingOnFirstRow(ExcelWorksheet worksheet, int reportDataStartRow)
        {
            // Formatting of the first row
            int totalCols = worksheet.Dimension.End.Column;
            int totalRows = worksheet.Dimension.End.Row;
            worksheet.View.FreezePanes(1, 3);
            var headerCells = worksheet.Cells[reportDataStartRow, 1, reportDataStartRow, totalCols];
            var headerFont = headerCells.Style.Font;
            headerFont.Bold = true;
            var headerFill = headerCells.Style.Fill;
            headerFill.PatternType = ExcelFillStyle.Solid;
            headerFill.BackgroundColor.SetColor(Color.Orange);
            worksheet.Cells[reportDataStartRow, 1, totalRows, totalCols].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[reportDataStartRow, 1, totalRows, totalCols].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[reportDataStartRow, 1, totalRows, totalCols].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[reportDataStartRow, 1, totalRows, totalCols].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            worksheet.Cells[reportDataStartRow, 11, totalRows, 12].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;     // Formating for Start Date and End Date columns
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
            worksheet.Cells[reportDataStartRow, 14, totalRows, totalCols].Style.Numberformat.Format = "###,###,###,###,##0.00";

            return worksheet;
        }

        public async Task<EmailDTO> PopulateEmailDetails(ReportEmail reportEmail, ReportRequestDTO report, string fileName, string filePath, ReportType reportType)
        {
            EmailDTO emailDTO = new EmailDTO();
            emailDTO.TemplateData = new PlaceHolderDTO();
            emailDTO.To = reportEmail.Email;
            emailDTO.ReportFileName = fileName;
            emailDTO = await emailNotificationService.PostComposeEmail(emailDTO, null, OperationType.Report);
            emailDTO.TemplateData.OperationName = Constants.ReportOperation;
            emailDTO.TemplateData.TemplateName = Constants.ReportTemplateName;

            // Fields to be populated in View
            if (reportType == ReportType.ProjectResourceLevel)
            {
                emailDTO.IsResourceLevelReport = true;
            }
            else
            {
                emailDTO.IsResourceLevelReport = false;
                emailDTO.ReportStartDate = report.StartDate;
                emailDTO.ReportEndDate = report.EndDate;
            }
            emailDTO.ReportName = GetReportName(reportType);
            emailDTO.ReportCurrency = report.IsUSDCurrency ? Constants.USDCurrency : Constants.PipCurrency;
            emailDTO.TemplateData.ReceiverFirstName = reportEmail.FirstName;
            emailDTO.TemplateData.ReceiverLastName = reportEmail.LastName;
            emailDTO.SelectedProjects = ShowCommaSeparatedProjectIds(report.SelectedProjects);
            emailDTO.SelectedAccounts = ShowCommaSeparatedAccounts(report.SelectedAccounts);
            emailDTO.ReportFilePath = filePath;

            if (reportType == ReportType.CustomReport)
            {
                emailDTO.SelectedKPIs = ShowCommaSeparatedKPIs(report.SelectedKPIs);
            }
            else
            {
                emailDTO.SelectedKPIs = null;
            }

            return emailDTO;
        }


        private string GenerateReportFileName(ReportType type, bool isUSDCurrency)
        {
            string reportName;
            switch (type)
            {
                case ReportType.ProjectSummaryView:
                    reportName = @"Key KPIs Report - Project Level - " + (isUSDCurrency ? "USD" : "Pip Currency") + " - " + DateTime.Now.ToString("MM-dd-yyyy HH-mm-ss") + ".xlsx";
                    break;
                case ReportType.ProjectDetailedLevel:
                    reportName = @"All KPIs Report - Project Level - " + (isUSDCurrency ? "USD" : "Pip Currency") + " - " + DateTime.Now.ToString("MM-dd-yyyy HH-mm-ss") + ".xlsx";
                    break;
                case ReportType.ProjectResourceLevel:
                    reportName = @"All KPIs Report - Role & Project Level - " + (isUSDCurrency ? "USD" : "Pip Currency") + " - " + DateTime.Now.ToString("MM-dd-yyyy HH-mm-ss") + ".xlsx";
                    break;
                case ReportType.CustomReport:
                    reportName = @"Key KPIs Report - Role Level - " + (isUSDCurrency ? "USD" : "Pip Currency") + " - " + DateTime.Now.ToString("MM-dd-yyyy HH-mm-ss") + ".xlsx";
                    break;
                default:
                    reportName = null;
                    break;
            }

            return reportName;
        }

        private string ShowCommaSeparatedProjectIds(IList<AccountBasedProjectDTO> projectsList)
        {
            string commaSeparatedProjectIds = null;
            if (projectsList.Count > 5)
            {
                commaSeparatedProjectIds = "Multiple Projects";
            }
            else
            {
                foreach (var project in projectsList)
                {
                    commaSeparatedProjectIds += project.SFProjectId + ", ";
                }
                commaSeparatedProjectIds = commaSeparatedProjectIds.Remove(commaSeparatedProjectIds.Length - 2, 2);      // Removal of last comma and space
            }
            return commaSeparatedProjectIds;
        }

        private string ShowCommaSeparatedKPIs(IList<ReportKPIDTO> reportKPIList)
        {
            string commaSeparatedKPIs = null;
            foreach (var kpi in reportKPIList)
            {
                commaSeparatedKPIs += kpi.KPIName + ", ";
            }
            commaSeparatedKPIs = commaSeparatedKPIs.Remove(commaSeparatedKPIs.Length - 2, 2);      // Removal of last comma and space
            return commaSeparatedKPIs;
        }

        private string ShowCommaSeparatedAccounts(IList<AccountDTO> accountsList)
        {
            string commaSeparatedAccounts = null;
            if (accountsList.Count > 5)
            {
                commaSeparatedAccounts = "Multiple Accounts";
            }
            else
            {
                foreach (var account in accountsList)
                {
                    commaSeparatedAccounts += account.AccountName + ", ";
                }
                commaSeparatedAccounts = commaSeparatedAccounts.Remove(commaSeparatedAccounts.Length - 2, 2);      // Removal of last comma and space
            }
            return commaSeparatedAccounts;
        }

        private string GetReportName(ReportType reportType)
        {
            string reportName = null;
            switch (reportType)
            {
                case ReportType.ProjectSummaryView:
                    reportName = Constants.ProjectSummaryViewReport;
                    break;
                case ReportType.ProjectDetailedLevel:
                    reportName = Constants.ProjectDetailedLevelReport;
                    break;
                case ReportType.ProjectResourceLevel:
                    reportName = Constants.ProjectResourceLevelReport;
                    break;
                case ReportType.CustomReport:
                    reportName = Constants.CustomReport;
                    break;
                default:
                    reportName = null;
                    break;
            }
            return reportName;
        }

        public async Task<List<AccountId>> GetAuthorizedAccounts(string userName)
        {
            return await this.reportRepository.GetAuthorizedAccounts(userName);
        }
    }
}
