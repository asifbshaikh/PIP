using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class DirectExpenseParentDTO : DirectExpenseDTO
    {
        public List<DirectExpensePeriodDTO> DirectExpensePeriodDTO { get; set; }
    }
}
