namespace LicenseKey.Controllers.Response
{
    public class UserInfoResponse
    {
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? PhotoUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsVerified { get; set; }

        public UserInfoResponse(string? email, string? name, string? photoUrl, DateTime createdAt, bool isVerified)
        {
            Email = email;
            Name = name;
            PhotoUrl = photoUrl;
            CreatedAt = createdAt;
            IsVerified = isVerified;
        }
    }
}
