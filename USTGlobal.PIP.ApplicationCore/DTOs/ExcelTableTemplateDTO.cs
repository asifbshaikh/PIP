using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class ExcelTableTemplateDTO
    {
        public List<WorkSheets> WorkSheets { get; set; }
    }
    public class WorkSheets
    {
        public string SheetName { get; set; }
        public List<TableList> TableList { get; set; }
    }
    public class TableList
    {
        public string TableName { get; set; }

        public List<Columns> Columns { get; set; }

    }
    public class Columns
    {
        public string ColumnName { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }
    }
}
