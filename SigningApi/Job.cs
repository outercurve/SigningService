using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Outercurve.SigningApi
{
    public class Job
    {
        public Action PrefixedAction { get; set; }
        public Action Action { get; set; }
        public Action FailedAction { get; set; }
        public Action PostFixedAction { get; set; }
    }
}