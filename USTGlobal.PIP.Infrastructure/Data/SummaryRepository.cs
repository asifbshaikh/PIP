using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Helpers;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Infrastructure.Data
{
    public class SummaryRepository : ISummaryRepository
    {
        private readonly PipContext pipContext;
        public SummaryRepository(PipContext context)
        {
            this.pipContext = context;
        }

        public async Task<ProjectSummaryFinalDTO> GetProjectSummary(int pipSheetId)
        {
            ProjectSummaryFinalDTO projectSummaryFinalDTO = new ProjectSummaryFinalDTO();
            await pipContext.LoadStoredProc("dbo.sp_GetProjectSummary")
          .WithSqlParam("@PIPSheetId", pipSheetId)
          .ExecuteStoredProcAsync((projectSummaryResultSet) =>
          {
              projectSummaryFinalDTO.ProjectSummaryDTO = projectSummaryResultSet.ReadToList<ProjectSummaryDTO>().FirstOrDefault();
              projectSummaryResultSet.NextResult();
              projectSummaryFinalDTO.CurrencyConversion = projectSummaryResultSet.ReadToList<CurrencyConversionDTO>().SingleOrDefault();

          });
            return projectSummaryFinalDTO;
        }

        public async Task<GrossProfitFinalDTO> GetGrossProfit(int pipSheetId)
        {
            GrossProfitFinalDTO grossProfitFinalDTO = new GrossProfitFinalDTO();
            await pipContext.LoadStoredProc("dbo.sp_GetGrossProfit")
            .WithSqlParam("@PIPSheetId", pipSheetId)
            .ExecuteStoredProcAsync((grossProfitResultSet) =>
            {
                grossProfitFinalDTO.GrossProfitDTO = grossProfitResultSet.ReadToList<GrossProfitDTO>().SingleOrDefault();
                grossProfitResultSet.NextResult();
                grossProfitFinalDTO.CurrencyConversion = grossProfitResultSet.ReadToList<CurrencyConversionDTO>().SingleOrDefault();

            });
            return grossProfitFinalDTO;
        }

        public async Task<InvestmentViewResultSetDTO> GetInvestmentView(int pipSheetId)
        {
            InvestmentViewResultSetDTO investmentViewResultSetDTO = new InvestmentViewResultSetDTO();
            await pipContext.LoadStoredProc("dbo.sp_GetInvestmentView")
          .WithSqlParam("@PIPSheetId", pipSheetId)
          .ExecuteStoredProcAsync((investmentViewResultSet) =>
          {
              investmentViewResultSetDTO.investmentView = investmentViewResultSet.ReadToList<InvestmentViewDTO>().FirstOrDefault();
              investmentViewResultSet.NextResult();

              investmentViewResultSetDTO.corporateTarget = investmentViewResultSet.ReadToList<CorporateTargetDTO>().ToList();
              investmentViewResultSet.NextResult();

              investmentViewResultSetDTO.CurrencyConversion = investmentViewResultSet.ReadToList<CurrencyConversionDTO>().SingleOrDefault();
          });
            return investmentViewResultSetDTO;
        }

        public async Task SaveInvestmentView(string userName, InvestmentViewDTO investmentViewDTO)
        {
            await pipContext.Database.ExecuteSqlCommandAsync(" exec dbo.sp_SaveInvestmentView {0}, {1}, {2}",
                userName,
                investmentViewDTO.PipSheetId,
                investmentViewDTO.CorporateTarget);

            await pipContext.SaveChangesAsync();
        }

        public async Task<EffortSummaryDTO> GetEffortSummary(int pipSheetId)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;
            EffortSummaryDBResultDTO effortSummaryDBDTO = new EffortSummaryDBResultDTO();
            EffortSummaryDTO effortSummaryDTO = new EffortSummaryDTO();

            await pipContext.LoadStoredProc("dbo.sp_GetEffortSummary")
                  .WithSqlParam("@PipSheetId", pipSheetId)
                  .ExecuteStoredProcAsync((result) =>
                  {
                      effortSummaryDBDTO = result.ReadToList<EffortSummaryDBResultDTO>().FirstOrDefault();
                      result.NextResult();
                  });
            if (effortSummaryDBDTO.StartDate == new DateTime() || effortSummaryDBDTO.EndDate == new DateTime())
            {
                effortSummaryDTO.StartDate = null;
                effortSummaryDTO.EndDate = null;
                effortSummaryDTO.Weeks = 0;
                effortSummaryDTO.Months = 0;
                effortSummaryDTO.TotalStaffHours = 0;
                effortSummaryDTO.FTEAvgPerMonth = 0;
            }
            else
            {
                effortSummaryDTO.StartDate = effortSummaryDBDTO.StartDate.ToString("MM-dd-yyyy");
                effortSummaryDTO.EndDate = effortSummaryDBDTO.EndDate.ToString("MM-dd-yyyy");
                effortSummaryDTO.Weeks = (Math.Floor((decimal)(effortSummaryDBDTO.EndDate.Subtract(effortSummaryDBDTO.StartDate).Days + 1) / 7) + ((decimal)(effortSummaryDBDTO.EndDate.Subtract(effortSummaryDBDTO.StartDate).Days + 1) % 7) / 10);
                effortSummaryDTO.Months = DateHelper.GetMonthsBetween(effortSummaryDBDTO.StartDate, effortSummaryDBDTO.EndDate);
                effortSummaryDTO.FTEAvgPerMonth = effortSummaryDBDTO.FTEAvgPerMonth;
                effortSummaryDTO.TotalStaffHours = effortSummaryDBDTO.TotalStaffHours;
            }

            return effortSummaryDTO;
        }

        public async Task<BillingScheduleResultSetDTO> GetBillingScheduleDetail(int pipSheetId)
        {
            BillingScheduleResultSetDTO billingScheduleDetailDTO = new BillingScheduleResultSetDTO();
            await pipContext.LoadStoredProc("dbo.sp_GetBillingScheduleDetail")
          .WithSqlParam("@PIPSheetId", pipSheetId)
          .ExecuteStoredProcAsync((billingScheduleDetailResultSet) =>
          {
              billingScheduleDetailDTO.billingScheduleCalculatedValueDTO = billingScheduleDetailResultSet.ReadToList<BillingScheduleCalculatedValueDTO>().FirstOrDefault();
              billingScheduleDetailResultSet.NextResult();

              billingScheduleDetailDTO.billingScheduleProjectResourceDTO = billingScheduleDetailResultSet.ReadToList<BillingScheduleProjectResourceDTO>().ToList();
              billingScheduleDetailResultSet.NextResult();

              billingScheduleDetailDTO.clientPriceDTO = billingScheduleDetailResultSet.ReadToList<ClientPriceDTO>().ToList();
              billingScheduleDetailResultSet.NextResult();

              billingScheduleDetailDTO.clientPricePeriodDTO = billingScheduleDetailResultSet.ReadToList<ClientPricePeriodDTO>().ToList();
              billingScheduleDetailResultSet.NextResult();

              billingScheduleDetailDTO.projectPeriodDTO = billingScheduleDetailResultSet.ReadToList<ProjectPeriodDTO>().ToList();
          });
            return billingScheduleDetailDTO;
        }

        public async Task<PLForecastDBResultDTO> GetPLForecastData(int pipSheetId)
        {
            PLForecastDBResultDTO pLForecastDBResultDTO = new PLForecastDBResultDTO();
            await pipContext.LoadStoredProc("dbo.sp_GetPLForecastData")
                  .WithSqlParam("@PipSheetId", pipSheetId)
                  .ExecuteStoredProcAsync((result) =>
                  {
                      pLForecastDBResultDTO.CalculatedValueDTO = result.ReadToList<CalculatedValueDTO>().FirstOrDefault();
                      result.NextResult();
                      pLForecastDBResultDTO.ProjectPeriodTotalDTO = result.ReadToList<ProjectPeriodTotalDTO>().ToList();
                      result.NextResult();
                      pLForecastDBResultDTO.FixBidCalcDTO = result.ReadToList<FixBidCalcDTO>().FirstOrDefault();
                      result.NextResult();
                      pLForecastDBResultDTO.FixBidCalcPeriodDTO = result.ReadToList<FixBidCalcPeriodDTO>().ToList();
                      result.NextResult();
                      pLForecastDBResultDTO.RiskManagementDTO = result.ReadToList<RiskManagementDTO>().FirstOrDefault();
                      result.NextResult();
                      pLForecastDBResultDTO.RiskManagementPeriodDTO = result.ReadToList<RiskManagementPeriodDetailDTO>().ToList();
                      result.NextResult();
                      pLForecastDBResultDTO.ProjectPeriodDTO = result.ReadToList<ProjectPeriodDTO>().ToList();
                      result.NextResult();
                      pLForecastDBResultDTO.CapitalCharge = result.ReadToValue<decimal>();
                  });
            return pLForecastDBResultDTO;
        }

        public async Task<SummaryYoyDbResultDTO> GetYearOverYearComparisonData(int pipSheetId)
        {
            SummaryYoyDbResultDTO summaryYoyDbResultDTO = new SummaryYoyDbResultDTO();
            await pipContext.LoadStoredProc("dbo.sp_GetYearOverYearComparison")
                  .WithSqlParam("@PipSheetId", pipSheetId)
                  .ExecuteStoredProcAsync((result) =>
                  {
                      summaryYoyDbResultDTO.CalculatedValueDTO = result.ReadToList<CalculatedValueDTO>().FirstOrDefault();
                      result.NextResult();
                      summaryYoyDbResultDTO.ProjectYearTotalDTO = result.ReadToList<ProjectYearTotalDTO>().ToList();
                      result.NextResult();
                      summaryYoyDbResultDTO.RiskManagementDTO = result.ReadToList<RiskManagementDTO>().FirstOrDefault();
                      result.NextResult();
                      summaryYoyDbResultDTO.CapitalCharge = result.ReadToValue<decimal>();
                  });

            return summaryYoyDbResultDTO;
        }

        public async Task SavePLForecastData(string userName, int pipSheetId, IList<PLForecastSubDTO> plForecastSubDTO, IList<PLForecastPeriodDTO> plForecastPeriods)
        {
            await pipContext.Database.ExecuteSqlCommandAsync(" exec dbo.sp_SavePLForecastData {0}, {1}, {2}, {3}",
            new SqlParameter("@InputPLForecast", SqlDbType.Structured)
            {
                Value = IListToDataTableHelper.ToDataTables(plForecastSubDTO),
                TypeName = "dbo.PLForecast"
            },
            new SqlParameter("@InputPLForecastPeriod", SqlDbType.Structured)
            {
                Value = IListToDataTableHelper.ToDataTables(plForecastPeriods),
                TypeName = "dbo.PLForecastPeriod"
            },
            pipSheetId,
            userName);

            await pipContext.SaveChangesAsync();
        }

        public async Task<KeyPerformanceIndicatorDTO> GetKeyPerformanceIndicatorsData(int pipSheetId)
        {
            KeyPerformanceIndicatorDTO keyPerformanceIndicatorDTO = new KeyPerformanceIndicatorDTO();         

            await pipContext.LoadStoredProc("dbo.sp_GetKeyPerformanceIndicators")
                    .WithSqlParam("@PIPSheetId", pipSheetId)
                    .ExecuteStoredProcAsync((keyPerformanceIndicatorsResultSet) =>
                    {
                        keyPerformanceIndicatorDTO = keyPerformanceIndicatorsResultSet.ReadToList<KeyPerformanceIndicatorDTO>().FirstOrDefault();
                        keyPerformanceIndicatorsResultSet.NextResult();

                        keyPerformanceIndicatorDTO.ServiceLineRevenueList = keyPerformanceIndicatorsResultSet.ReadToList<ServiceLineRevenueDTO>().ToList();
                        keyPerformanceIndicatorsResultSet.NextResult();

                        keyPerformanceIndicatorDTO.ServiceLineEbitdaPercentList = keyPerformanceIndicatorsResultSet.ReadToList<ServiceLineEbitdaDTO>().ToList();
                        keyPerformanceIndicatorsResultSet.NextResult();

                    });

            return keyPerformanceIndicatorDTO; 
        }
        
        public async Task<LocationWiseCalculationMainDTO> GetLocationWiseDetail(int pipSheetId)
        {
            LocationWiseCalculationMainDTO locationWiseCalculationMainDTO = new LocationWiseCalculationMainDTO();

            await pipContext.LoadStoredProc("dbo.sp_GetLocationWiseDetails")
                  .WithSqlParam("@PipSheetId", pipSheetId)
                  .ExecuteStoredProcAsync((resultSet) =>
                  {
                      locationWiseCalculationMainDTO.LocationWiseDetailDTO = resultSet.ReadToList<LocationWiseDetailDTO>().ToList();
                      resultSet.NextResult();

                      locationWiseCalculationMainDTO.projectSummaryLocationDTO = resultSet.ReadToList<ProjectSummaryLocationDTO>().ToList();
                  });
            return locationWiseCalculationMainDTO;
        }

        public async Task<TotalDealFinancialsDBResultDTO> GetTotalDealFinancials(int pipSheetId)
        {
            TotalDealFinancialsDBResultDTO totalDealFinancials = new TotalDealFinancialsDBResultDTO();

            await pipContext.LoadStoredProc("dbo.sp_GetTotalDealFinancials")
                  .WithSqlParam("@PipSheetId", pipSheetId)
                  .ExecuteStoredProcAsync((resultSet) =>
                  {
                      totalDealFinancials.CalculatedValue = resultSet.ReadToList<CalculatedValueDTO>().FirstOrDefault();
                      resultSet.NextResult();
                      totalDealFinancials.DealFinancialsYearTotalDTO = resultSet.ReadToList<DealFinancialsYearTotalsDTO>().ToList();
                      resultSet.NextResult();
                      totalDealFinancials.BeatTaxYearDTO = resultSet.ReadToList<BeatTaxImpactDTO>().ToList();
                      resultSet.NextResult();
                      totalDealFinancials.TotalBeatTaxImpactPercent = (resultSet.ReadToValue<decimal>() ?? 0);
                      resultSet.NextResult();
                      totalDealFinancials.LocalToUSDCurrencyFactor = (resultSet.ReadToValue<decimal>() ?? 0);
                  });
            return totalDealFinancials;
        }
    }
}
