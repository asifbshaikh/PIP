using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface ISummaryRepository
    {
        Task<ProjectSummaryFinalDTO> GetProjectSummary(int pipSheetId);
        Task<GrossProfitFinalDTO> GetGrossProfit(int pipSheetId);
        Task<InvestmentViewResultSetDTO> GetInvestmentView(int pipSheetId);
        Task SaveInvestmentView(string userName, InvestmentViewDTO investmentViewDTO);
        Task<EffortSummaryDTO> GetEffortSummary(int pipSheetId);
        Task<BillingScheduleResultSetDTO> GetBillingScheduleDetail(int pipSheetId);
        Task<PLForecastDBResultDTO> GetPLForecastData(int pipSheetId);
        Task<SummaryYoyDbResultDTO> GetYearOverYearComparisonData(int pipSheetId);
        Task<KeyPerformanceIndicatorDTO> GetKeyPerformanceIndicatorsData(int pipSheetId);
        Task SavePLForecastData(string userName, int pipSheetId, IList<PLForecastSubDTO> plForecastSubDTO, IList<PLForecastPeriodDTO> plForecastPeriods);
        Task<LocationWiseCalculationMainDTO> GetLocationWiseDetail(int pipSheetId);
        Task<TotalDealFinancialsDBResultDTO> GetTotalDealFinancials(int pipSheetId);
    }
}
