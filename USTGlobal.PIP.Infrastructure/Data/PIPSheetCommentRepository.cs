using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.Infrastructure.Data
{
    public class PIPSheetCommentRepository : IPIPSheetCommentRepository
    {
        private readonly PipContext pipContext;

        public PIPSheetCommentRepository(PipContext context)
        {
            this.pipContext = context;
        }

        public async Task<List<PIPSheetCommentDTO>> GetPIPSheetComments(int pipSheetId)
        {
            var comments = await
                (from pipsheet in pipContext.PipSheetComments
                 join user in pipContext.User on pipsheet.UserId equals user.UserId
                 where pipsheet.PIPSheetId == pipSheetId
                 select new PIPSheetCommentDTO
                 {
                     PIPSheetCommentId = pipsheet.PIPSheetCommentId,
                     PIPSheetId = pipsheet.PIPSheetId,
                     comment = pipsheet.comment,
                     CommentTimeStamp = pipsheet.CommentTimeStamp,
                     IsDeleted = pipsheet.IsDeleted,
                     UserId = pipsheet.UserId,
                     UserName = user.FirstName + ' ' + user.LastName
                 }).ToListAsync();

            return comments;

        }


        public async Task<int> SavePIPSheetComment(PIPSheetCommentDTO pipsheetComment, string userName)
        {
            int result = 0;
            try
            {
                SqlParameter[] inputParams = new SqlParameter[7];

                inputParams[0] = new SqlParameter("@Result", "Add");
                inputParams[1] = new SqlParameter("@PIPSheetCommentId", pipsheetComment.PIPSheetCommentId);

                inputParams[2] = new SqlParameter("@OutputResult", System.Data.SqlDbType.Int);
                inputParams[2].Direction = System.Data.ParameterDirection.Output;

                inputParams[3] = new SqlParameter("@UserName", userName);
                inputParams[4] = new SqlParameter("@PIPSheetId", pipsheetComment.PIPSheetId);
                inputParams[5] = new SqlParameter("@comment", pipsheetComment.comment);
                inputParams[6] = new SqlParameter("@UserId", pipsheetComment.UserId);

                await pipContext.Database.ExecuteSqlCommandAsync(" exec dbo.sp_PIPSheetComments @Result, @PIPSheetCommentId," +
                      " @OutputResult OUTPUT,@UserName, @PIPSheetId, @comment, @UserId", inputParams);

                await pipContext.SaveChangesAsync();

                result = Convert.ToInt32(inputParams[2].Value);
            }
            catch (Exception)
            {}
            return result;
        }

        public async Task<bool> DeletePIPSheetComment(int PIPSheetCommentId, string userName)
        {
            int result;
            try
            {
                SqlParameter[] inputParams = new SqlParameter[4];

                inputParams[0] = new SqlParameter("@Result", "Delete");
                inputParams[1] = new SqlParameter("@PIPSheetCommentId", PIPSheetCommentId);

                inputParams[2] = new SqlParameter("@OutputResult", System.Data.SqlDbType.Int);
                inputParams[2].Direction = System.Data.ParameterDirection.Output;

                inputParams[3] = new SqlParameter("@UserName", userName);


                await pipContext.Database.ExecuteSqlCommandAsync(" exec dbo.sp_PIPSheetComments @Result, @PIPSheetCommentId, @OutputResult OUTPUT, @UserName", inputParams);
                await pipContext.SaveChangesAsync();
                result = Convert.ToInt32(inputParams[2].Value);
            }
            catch (Exception)
            {
                throw;
            }
            return result == 1 ? true : false;
        }

    }
}
