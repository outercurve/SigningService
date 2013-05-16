using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace Outercurve.Cmdlets
{
    public static class CmdletExtensions
    {
        public static void WriteErrorsAndThrowOnLast(this Cmdlet cmdlet, IEnumerable<ErrorRecord> errors)
        {
            var errorRecords = errors as ErrorRecord[] ?? errors.ToArray();
            var allButLast1 = errorRecords.Reverse().Skip(1).Reverse();
            cmdlet.WriteErrors(allButLast1);
            cmdlet.ThrowTerminatingError(errorRecords.Last());
        }

        public static void WriteErrors(this Cmdlet cmdlet, IEnumerable<ErrorRecord> errors)
        {
            foreach (var errorRecord in errors)
            {
                cmdlet.WriteError(errorRecord);
            }
        }
    }
}
