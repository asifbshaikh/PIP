using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.WorkflowServices
{
    public class ClientPriceWorkflowService : IClientPriceWorkflowService
    {
        private readonly ICapitalChargeService capitalChargeService;
        private readonly ISharedRepository sharedRepository;
        private readonly IClientPriceService clientPriceService;

        public ClientPriceWorkflowService(ICapitalChargeService capitalChargeService, ISharedRepository sharedRepository, IClientPriceService clientPriceService)
        {
            this.clientPriceService = clientPriceService;
            this.capitalChargeService = capitalChargeService;
            this.sharedRepository = sharedRepository;
        }

        public async Task ProcessClientPriceSaving(ClientPriceMainDTO clientPriceMainDTO, string userName)
        {
            int pipSheetId = clientPriceMainDTO.ClientPriceDTO[0].PipSheetId;
            await this.clientPriceService.SaveClientPriceData(clientPriceMainDTO, userName);

            // Bring PipSheetSaveStatus DTO here
            PipSheetSaveStatusDTO pipSheetSaveStatus = await sharedRepository.GetPipSheetSaveStatus(pipSheetId);

            // Capital Charge 
            if (pipSheetSaveStatus.CapitalCharge == true)
            {
                CapitalChargeResultSetDTO capitalChargeResultSetDTO = await this.capitalChargeService.CalculateCapitalCharges(pipSheetId);
                await this.capitalChargeService.SaveCapitalCharge(userName, capitalChargeResultSetDTO);
            }

            // Client Price 
            if (pipSheetSaveStatus.ClientPrice == true)
            {
                clientPriceMainDTO = await clientPriceService.CalculateTotalClientPrice(pipSheetId, userName);
                await clientPriceService.SaveClientPriceData(clientPriceMainDTO, userName);
            }
        }
    }
}
