using System.Collections;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;

namespace USTGlobal.PIP.ApplicationCore.Interfaces
{
    public interface IUploadExcelService
    {
        string ReadExcel(byte[] file, ExcelTableTemplateDTO excelSetting);
        Task<IList> ReadExcelToList(byte[] file);
    }
}
