using Newtonsoft.Json.Linq;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using USTGlobal.PIP.ApplicationCore.DTOs;
using USTGlobal.PIP.ApplicationCore.Interfaces;

namespace USTGlobal.PIP.ApplicationCore.Services
{
    public class UploadExcelService : IUploadExcelService
    {
        public string ReadExcel(byte[] file, ExcelTableTemplateDTO excelSetting)
        {
            JObject excelObj = new JObject();
            if (file.Length > 0)
            {
                using (var stream = new MemoryStream(file))
                {

                    using (var package = new ExcelPackage(stream))
                    {
                        var excelTemplate = excelSetting;

                        if (excelTemplate.WorkSheets != null)
                        {
                            foreach (var workSheet in excelTemplate.WorkSheets)
                            {
                                var workSheetName = workSheet.SheetName;
                                ExcelWorksheet worksheet = package.Workbook.Worksheets[workSheetName];
                                JObject workSheetObj = new JObject();

                                if (worksheet != null)
                                {
                                    foreach (var table in workSheet.TableList)
                                    {
                                        try
                                        {
                                            var tableTableName = table.TableName;
                                            int rowCounter = 0;
                                            bool isRowDataPresent = true;
                                            JArray tableObj = new JArray();

                                            while (isRowDataPresent)
                                            {
                                                JObject rowObj = new JObject();

                                                foreach (var item in table.Columns)
                                                {
                                                    var columnName = item.ColumnName;
                                                    var cellValue = worksheet.Cells[item.Row + rowCounter, item.Column]?.Value;
                                                    if (cellValue != null)
                                                    {
                                                        rowObj.Add(new JProperty(item.ColumnName, cellValue.ToString()));
                                                    }
                                                    else
                                                    {
                                                        isRowDataPresent = false;
                                                        break;
                                                    }
                                                }
                                                if (rowObj.Children().Count() > 0)
                                                {
                                                    tableObj.Add(new JObject(rowObj));
                                                }
                                                rowCounter++;
                                            }
                                            workSheetObj.Add(new JProperty(tableTableName, tableObj));
                                        }
                                        catch (Exception)
                                        {
                                            // log the exception with logging mechanism in place                                                  
                                        }
                                    }
                                    excelObj.Add(new JProperty(workSheetName, workSheetObj));
                                }
                                else
                                {
                                    excelObj.Add(new JProperty("Error", "Worksheet " + workSheetName + "can not be found"));
                                }
                            }
                            return excelObj.ToString();
                        }
                        else
                        {
                            excelObj.Add(new JProperty("Error", "Can not read configuration settings"));
                            return excelObj.ToString();
                        }
                    }
                }
            }
            else
            {
                excelObj.Add(new JProperty("Error", "Please upload the file correctly"));
                return excelObj.ToString();
            }
        }

        public async Task<IList> ReadExcelToList(byte[] file)
        {
            IList<UserListDTO> userListDTO = new List<UserListDTO>();
            if (file.Length > 0)
            {
                using (var stream = new MemoryStream(file))
                {

                    using (var package = new ExcelPackage(stream))
                    {
                        ExcelWorksheet workSheet = package.Workbook.Worksheets["UserList"];
                        int totalRows = workSheet.Dimension.Rows;

                        for (int i = 2; i <= totalRows; i++)
                        {
                            for (int j = 1; j <= 4; j++)
                            {
                                if (!(workSheet.Cells[i, j].Count() == 0 && workSheet.Cells[i, j + 1].Count() == 0 && workSheet.Cells[i, j + 2].Count() == 0 && workSheet.Cells[i, j + 3].Count() == 0))
                                {
                                    userListDTO.Add(new UserListDTO
                                    {
                                        FirstName = workSheet.Cells[i, j].Value?.ToString(),
                                        LastName = workSheet.Cells[i, j + 1].Value?.ToString(),
                                        Email = workSheet.Cells[i, j + 2].Value?.ToString(),
                                        UID = workSheet.Cells[i, j + 3].Value?.ToString()
                                    });
                                }
                                break;
                            }
                        }
                    }
                }
            }
            return (IList)userListDTO;
        }
    }
}
