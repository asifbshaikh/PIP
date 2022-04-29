using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class ReimbursementAndSalesWorkflowService : IReimbursementAndSalesWorkflowService
    {
        private readonly IReimbursementAndSalesService reimbursementAndSalesService;
        private readonly IFixBidAndMarginService fixbidAndMarginService;
        private readonly IClientPriceService clientPriceService;
        private readonly ICapitalChargeService capitalChargeService;
        private readonly ISharedRepository sharedRepository;

        public ReimbursementAndSalesWorkflowService(IReimbursementAndSalesService reimbursementAndSalesService, ISharedRepository sharedRepository,
             IFixBidAndMarginService fixbidAndMarginService, IClientPriceService clientPriceService, ICapitalChargeService capitalChargeService)
        {
            this.reimbursementAndSalesService = reimbursementAndSalesService;
            this.fixbidAndMarginService = fixbidAndMarginService;
            this.clientPriceService = clientPriceService;
            this.capitalChargeService = capitalChargeService;
            this.sharedRepository = sharedRepository;
        }

        public async Task ProcessReimbursementAndSalesSaving(string userName, ReimbursementAndSalesDTO reimbursementAndSalesDTO)
        {
            int pipSheetId = reimbursementAndSalesDTO.Reimbursements[0].PipSheetId;
            await this.reimbursementAndSalesService.SaveReimbursementAndSalesDetails(userName, reimbursementAndSalesDTO);

            // Bring PipSheetSaveStatus DTO here
            PipSheetSaveStatusDTO pipSheetSaveStatus = await sharedRepository.GetPipSheetSaveStatus(pipSheetId);

            // Fixed Bid and Margin
            if (pipSheetSaveStatus.FixBidAndMargin == true)
            {
                await this.fixbidAndMarginService.CalculateAndSaveFixBidData(pipSheetId, userName);
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
    }
}
