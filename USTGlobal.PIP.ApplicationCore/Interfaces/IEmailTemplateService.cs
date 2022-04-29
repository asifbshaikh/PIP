using System.Threading.Tasks;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IEmailTemplateService
    {
        Task<string> RenderToStringAsync(string viewName, object model);
    }
}
