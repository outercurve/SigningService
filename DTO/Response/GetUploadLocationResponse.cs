namespace Outercurve.DTO.Response
{
    public class GetUploadLocationResponse : BaseResponse
    {
        public string Location { get; set; }
        public string Name { get; set; }
        public string Sas { get; set; }
        public string Account { get; set; }
    }
}
