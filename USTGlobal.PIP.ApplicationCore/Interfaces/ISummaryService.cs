using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface ISummaryService
    {
        Task<List<ProjectSummaryMainDTO>> GetProjectSummary(int pipSheetId);
        Task<List<GrossProfitMainDTO>> GetGrossProfit(int pipSheetId);
        Task<List<InvestmentViewResultSetDTO>> GetInvestmentView(int pipSheetId);
        Task SaveInvestmentView(string userName, InvestmentViewDTO investmentViewDTO);
        Task<EffortSummaryDTO> GetEffortSummary(int pipSheetId);
        Task<BillingScheduleDetailDTO> GetBillingScheduleDetail(int pipSheetId);
        Task<PLForecastParentDTO> GetPLForecastData(int pipSheetId);
        Task<List<SummaryYoyDTO>> GetYearOverYearComparisonData(int pipSheetId);
        Task SavePLForecastData(string userName, IList<PLForecastDTO> plForecastDTO, int pipSheetId);
        Task<KeyPerformanceIndicatorDTO> GetKeyPerformanceIndicatorsData(int pipSheetId);
        Task<LocationWiseSummaryDTO> GetLocationWiseDetails(int pipSheetId);
        Task<List<TotalDealFinancialsDTO>> GetTotalDealFinancials(int pipSheetId);
    }
}
