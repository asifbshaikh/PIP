using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.Interfaces;
using USTGlobal.PIP.ApplicationCore.DTOs;


namespace USTGlobal.PIP.ApplicationCore.WorkflowServices
{
    public class VacationAbsenceWorkflowService : IVacationAbsenceWorkflowService
    {
        private readonly IVacationAbsencesService vacationAbsencesService;
        private readonly IFixBidAndMarginService fixbidAndMarginService;
        private readonly IClientPriceService clientPriceService;
        private readonly ICapitalChargeService capitalChargeService;
        private readonly ISharedRepository sharedRepository;
        private readonly IOtherPriceAdjustmentService otherPriceAdjustmentService;

        public VacationAbsenceWorkflowService(ISharedRepository sharedRepository, ICapitalChargeService capitalChargeService,
            IFixBidAndMarginService fixbidAndMarginService, IClientPriceService clientPriceService, IVacationAbsencesService vacationAbsencesService,
            IOtherPriceAdjustmentService otherPriceAdjustmentService)
        {
            this.vacationAbsencesService = vacationAbsencesService;
            this.fixbidAndMarginService = fixbidAndMarginService;
            this.clientPriceService = clientPriceService;
            this.capitalChargeService = capitalChargeService;
            this.sharedRepository = sharedRepository;
            this.otherPriceAdjustmentService = otherPriceAdjustmentService;
        }

        public async Task ProcessVacationAbsencesSaving(string userName, VacationAbsencesParentDTO vacationAbsencesParentDTO)
        {
            int pipSheetId = vacationAbsencesParentDTO.PIPSheetId;
            await this.vacationAbsencesService.SaveVacationAbsencesData(userName, vacationAbsencesParentDTO);

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
    }
}
