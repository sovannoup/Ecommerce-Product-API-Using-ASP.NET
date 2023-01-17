using LicenseKey.Controllers.Request;
using LicenseKey.Models;

namespace LicenseKey.Services.FeedbackService
{
    public interface IFeedbackService
    {
        public string UploadFeedback(UploadFeedbackRequest req);
        public string UpdateFeedback(UploadFeedbackRequest req, int id);
        public string DeleteFeedback(int id);
        public List<Feedback> GetFeedback();
    }
}
