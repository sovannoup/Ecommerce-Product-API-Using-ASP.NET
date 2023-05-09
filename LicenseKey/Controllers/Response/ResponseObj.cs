namespace LicenseKey.Controllers.Response
{
    public class ResponseObj
    {
        public int Status { get; set; } = 200;
        public string? Message { get; set; }
        public Dictionary<string, Object>? Result { get; set; } = new Dictionary<string, Object>();
        public DateTime ResponseDate = DateTime.Now;
    }
}
