using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class PIPSheetCommentService : IPIPSheetCommentService
    {
        private readonly IPIPSheetCommentRepository pipsheetCommentRepository;
        public PIPSheetCommentService(IPIPSheetCommentRepository pipsheetCommentRepository)
        {
            this.pipsheetCommentRepository = pipsheetCommentRepository;
        }
        public async Task<bool> DeletePIPSheetComment(int PIPSheetCommentId, string userName)
        {
            return await pipsheetCommentRepository.DeletePIPSheetComment(PIPSheetCommentId, userName);
        }

        public async Task<List<PIPSheetCommentDTO>> GetPIPSheetComments(int pipSheetId)
        {
            return await pipsheetCommentRepository.GetPIPSheetComments(pipSheetId);
        }

        public async Task<int> SavePIPSheetComment(PIPSheetCommentDTO comment, string userName)
        {
            return await pipsheetCommentRepository.SavePIPSheetComment(comment, userName);
        }
    }
}
