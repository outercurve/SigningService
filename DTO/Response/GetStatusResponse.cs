using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Outercurve.DTO.Response
{
    public class GetStatusResponse : BaseResponse
    {
        public StatusCode Status { get; set; }
    }

    public enum StatusCode
    {
        WaitingToRun,
        Running,
        Failed,
        Done
    }
}
