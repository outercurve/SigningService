using System.Collections.Generic;

namespace Outercurve.DTO.Response
{
    public class BaseResponse
    {
        public List<string> Warnings { get; set; }

        public List<string> Errors { get; set; }
    }
}