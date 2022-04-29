using System;
using System.Collections.Generic;
using System.Text;

namespace USTGlobal.PIP.ApplicationCore.DTOs
{
    public class PipOverrideDTO
    {
        public string OverrideItem { get; set; }
        public string OverrideValue { get; set; }
        public string DefaultValue { get; set; }
        public bool IsPositive { get; set; }
        public int TabIndex { get; set; }
        public int StepIndex { get; set; }
    }
}
