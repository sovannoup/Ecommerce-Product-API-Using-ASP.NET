using LicenseKey.Controllers.Request;
using LicenseKey.Models;
using LicenseKey.Repository;

namespace LicenseKey.Services.FeedbackService
{
    public class FeedbackService : IFeedbackService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public FeedbackService(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public string DeleteFeedback(int id)
        {
            Feedback? feedback = _applicationDbContext.Feedback.FirstOrDefault(x => x.Id == id);
            if (feedback == null)
            {
                throw new Exception("Feedback not found");
            }
            _applicationDbContext.Feedback.Remove(feedback);
            _applicationDbContext.SaveChanges();
            return "Success";
        }

        public List<Feedback> GetFeedback()
        {
            List<Feedback> feedbacks = _applicationDbContext.Feedback.ToList();
            return feedbacks;
        }

        public string UpdateFeedback(UploadFeedbackRequest req, int id)
        {
            Feedback? feedback = _applicationDbContext.Feedback.FirstOrDefault(x =>x.Id == id);
            if(feedback == null)
            {
                throw new Exception("Feedback not found");
            }
            feedback.Message = req.Message;
            _applicationDbContext.Feedback.Update(feedback);
            _applicationDbContext.SaveChanges();
            return "Success";
        }

        public string UploadFeedback(UploadFeedbackRequest req)
        {
            Feedback? feedback = _applicationDbContext.Feedback.FirstOrDefault(x => x.Message == req.Message);
            if (feedback != null)
            {
                throw new Exception("Feedback already sent");
            }
            _applicationDbContext.Add(feedback);
            _applicationDbContext.SaveChanges();
            return "Success";
        }
    }
}
