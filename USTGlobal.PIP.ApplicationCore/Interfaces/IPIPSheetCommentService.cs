using System.Collections.Generic;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IPIPSheetCommentService
    {
        Task<List<PIPSheetCommentDTO>> GetPIPSheetComments(int pipSheetId);
        Task<int> SavePIPSheetComment(PIPSheetCommentDTO comment, string userName);
        Task<bool> DeletePIPSheetComment(int PIPSheetCommentId, string userName);
    }
}
