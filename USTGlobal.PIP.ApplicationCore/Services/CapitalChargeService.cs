using System;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class CapitalChargeService : ICapitalChargeService
    {
        private readonly ICapitalChargeRepository capitalChargeRepository;
        private readonly ISharedRepository sharedRepository;

        public CapitalChargeService(ICapitalChargeRepository capitalChargeRepository, ISharedRepository sharedRepository)
        {
            this.capitalChargeRepository = capitalChargeRepository;
            this.sharedRepository = sharedRepository;
        }

        public async Task<CapitalChargeResultSetDTO> GetCapitalCharge(int pipSheetId)
        {
            return await this.capitalChargeRepository.GetCapitalCharge(pipSheetId);
        }

        public async Task<CapitalChargeResultSetDTO> CalculateCapitalCharges(int pipSheetId)
        {
            CapitalChargeResultSetDTO capitalChargeResultSetDTO = await this.GetCapitalCharge(pipSheetId);
            if (capitalChargeResultSetDTO.capitalChargeDTO != null)
            {

                decimal capitalChargeDailyRate = Convert.ToDecimal(capitalChargeResultSetDTO.capitalChargeDTO.CapitalChargeDailyRate);
                decimal totalPaymentLag = 0, netEstimatedRevenue = 0, periodCapitalCharge = 0, totalCapitalCharge = 0;

                capitalChargeResultSetDTO.capitalChargeDTO.PipSheetId = pipSheetId;

                if (capitalChargeResultSetDTO.projectPeriodTotalDTO.Count > 0)
                {
                    if (capitalChargeResultSetDTO.capitalChargeDTO.PaymentLag > 30)
                    {
                        totalPaymentLag = (decimal)((capitalChargeResultSetDTO.capitalChargeDTO.PaymentLag - 30) * (capitalChargeDailyRate / 100));
                    }
                    else
                        totalPaymentLag = 0;

                    capitalChargeResultSetDTO.projectPeriodTotalDTO.ForEach(period =>
                    {
                        netEstimatedRevenue = period.ClientPrice - period.FeesAtRisk;
                        if (capitalChargeResultSetDTO.capitalChargeDTO.IsTargetMarginPrice)
                        {
                            periodCapitalCharge = totalPaymentLag * period.ClientPrice;
                        }
                        else
                        {
                            periodCapitalCharge = totalPaymentLag * netEstimatedRevenue;
                        }

                        period.CapitalCharge = periodCapitalCharge;
                        period.NetEstimatedRevenue = netEstimatedRevenue;
                        totalCapitalCharge += periodCapitalCharge;
                    });
                }
                else
                {
                    capitalChargeResultSetDTO.projectPeriodTotalDTO.ForEach(period =>
                    {
                        period.CapitalCharge = 0;
                        period.NetEstimatedRevenue = 0;
                    });
                }
                capitalChargeResultSetDTO.capitalChargeDTO.CapitalCharge = totalCapitalCharge;
            }
            return capitalChargeResultSetDTO;
        }

        public async Task SaveCapitalCharge(string userName, CapitalChargeResultSetDTO capitalCharge)
        {
            decimal? totalProjectCost = capitalCharge.capitalChargeDTO.TotalCostBeforeCap + capitalCharge.capitalChargeDTO.CapitalCharge;
            await this.capitalChargeRepository.SaveCapitalCharge(userName, capitalCharge, totalProjectCost);
        }
    }
}
