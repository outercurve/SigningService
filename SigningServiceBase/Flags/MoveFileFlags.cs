using System;

namespace SigningServiceBase.Flags
{
    [Flags]
    public enum MoveFileFlags
    {
        MOVEFILE_REPLACE_EXISTING = 1,
        MOVEFILE_COPY_ALLOWED = 2,
        MOVEFILE_DELAY_UNTIL_REBOOT = 4,
        MOVEFILE_WRITE_THROUGH = 8
    }
}