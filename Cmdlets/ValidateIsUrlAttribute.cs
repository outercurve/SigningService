using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using ClrPlus.Core.Extensions;

namespace Outercurve.Cmdlets
{
    public class ValidateIsUrlAttribute : ValidateArgumentsAttribute
    {
        protected override void Validate(object arguments, EngineIntrinsics engineIntrinsics)
        {
             if (!Uri.IsWellFormedUriString(arguments.ToString(), UriKind.Absolute))
             {
                 throw new ValidationMetadataException("{0} is not a valid uri.".format(arguments.ToString()));
             }
        }
    }
}
