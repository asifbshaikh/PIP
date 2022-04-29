using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.Interfaces;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.WorkflowServices
{
    public class OtherPriceAdjustmentWorkflowService : IOtherPriceAdjustmentWorkflowService
    {
        private readonly IOtherPriceAdjustmentService otherPriceAdjustmentService;
        private readonly IFixBidAndMarginService fixbidAndMarginService;
        private readonly IClientPriceService clientPriceService;
        private readonly ICapitalChargeService capitalChargeService;
        private readonly ISharedRepository sharedRepository;

        public OtherPriceAdjustmentWorkflowService(IOtherPriceAdjustmentService otherPriceAdjustmentService,
             IFixBidAndMarginService fixbidAndMarginService, IClientPriceService clientPriceService, ICapitalChargeService capitalChargeService,
             ISharedRepository sharedRepository)
        {
            this.otherPriceAdjustmentService = otherPriceAdjustmentService;
            this.fixbidAndMarginService = fixbidAndMarginService;
            this.clientPriceService = clientPriceService;
            this.capitalChargeService = capitalChargeService;
            this.sharedRepository = sharedRepository;
        }

        public async Task ProcessOtherPriceAdjustmentSaving(string userName, OtherPriceAdjustmentMainDTO otherPriceAdjustmentMainDTO)
        {
            int pipSheetId = otherPriceAdjustmentMainDTO.OtherPriceAdjustmentParent[0].PipSheetId;
            await this.otherPriceAdjustmentService.SaveOtherPriceAdjustmentData(userName, otherPriceAdjustmentMainDTO);

            // Bring PipSheetSaveStatus DTO here
            PipSheetSaveStatusDTO pipSheetSaveStatus = await sharedRepository.GetPipSheetSaveStatus(pipSheetId);


            // Fixed Bid and Margin
            if (pipSheetSaveStatus.FixBidAndMargin == true)
            {
                await this.fixbidAndMarginService.CalculateAndSaveFixBidData(pipSheetId, userName);
            }

            // Other Price Adjustment
            if (pipSheetSaveStatus.OtherPriceAdjustment == true)
            {
                OtherPriceAdjustmentMainDTO priceAdjustment = await this.otherPriceAdjustmentService.GetOtherPriceAdjustment(pipSheetId);
                await this.otherPriceAdjustmentService.SaveOtherPriceAdjustmentData(userName, priceAdjustment);
            }

            // Client Price 
            if (pipSheetSaveStatus.ClientPrice == true)
            {
                ClientPriceMainDTO clientPriceMainDTO = await this.clientPriceService.CalculateTotalClientPrice(pipSheetId, userName);
                await this.clientPriceService.SaveClientPriceData(clientPriceMainDTO, userName);
            }

            // Capital Charge 
            if (pipSheetSaveStatus.CapitalCharge == true)
            {
                CapitalChargeResultSetDTO capitalChargeResultSetDTO = await this.capitalChargeService.CalculateCapitalCharges(pipSheetId);
                await this.capitalChargeService.SaveCapitalCharge(userName, capitalChargeResultSetDTO);
            }

            // Client Price 
            if (pipSheetSaveStatus.ClientPrice == true)
            {
                ClientPriceMainDTO clientPriceMainDTO = await this.clientPriceService.CalculateTotalClientPrice(pipSheetId, userName);
                await this.clientPriceService.SaveClientPriceData(clientPriceMainDTO, userName);
            }
        }

        public async Task<OtherPriceAdjustmentMainDTO> GetOtherPriceAdjustment(int pipSheetId, string userName)
        {
            await this.fixbidAndMarginService.CalculateAndSaveFixBidData(pipSheetId, userName);
            return await this.otherPriceAdjustmentService.GetOtherPriceAdjustment(pipSheetId);
        }
    }
}
