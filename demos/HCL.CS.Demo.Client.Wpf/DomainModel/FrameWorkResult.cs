using System;
using System.Collections.Generic;
using System.Text;

namespace HCL.CS.DemoClientWpfApp.DomainModel
{
    public sealed class FrameworkResult
    {
        public ResultStatus Status { get; set; } = ResultStatus.Failed;

        public IEnumerable<FrameworkError> Errors { get; set; }
    }

    public class FrameworkError
    {
        public string Code { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}


