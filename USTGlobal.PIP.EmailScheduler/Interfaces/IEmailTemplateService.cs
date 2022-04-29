namespace USTGlobal.PIP.EmailScheduler.Interfaces
{
    public interface IEmailTemplateService
    {
        string RenderToStringAsync(string viewName, object model);
    }
}
