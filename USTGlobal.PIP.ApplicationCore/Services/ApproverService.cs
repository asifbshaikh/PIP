using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class ApproverService : IApproverService
    {
        private readonly IApproverRepository approverRepository;

        public ApproverService(IApproverRepository approverRepository)
        {
            this.approverRepository = approverRepository;
        }

        public async Task<List<ApproverDTO>> GetApproversData(string userName)
        {
            List < ApproverDTO > approversList = await this.approverRepository.GetApproversData(userName);
            foreach (ApproverDTO approver in approversList)
            {
                approver.ResendOnString = approver.ResendOn.ToString("MM-dd hh:mm");
                approver.ApprovedOnString = approver.ApprovedOn.ToString("MM-dd hh:mm");
            }
            return approversList;
        }
    }
}
