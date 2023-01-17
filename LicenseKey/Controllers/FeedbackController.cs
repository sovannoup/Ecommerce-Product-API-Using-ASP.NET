using LicenseKey.Controllers.Request;
using LicenseKey.Models;
using LicenseKey.Services.FeedbackService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LicenseKey.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IFeedbackService _feedbackService;
        public FeedbackController(IConfiguration configuration, IFeedbackService feedbackService)
        {
            _configuration = configuration;
            _feedbackService = feedbackService;
        }

        [HttpGet]
        public List<Feedback> GetFeedback()
        {
            return _feedbackService.GetFeedback();
        }

        [HttpPost]
        public string UploadFeedback(UploadFeedbackRequest request)
        {
            return _feedbackService.UploadFeedback(request);
        }

        [HttpPut("id")]
        public string UpdateFeedback(UploadFeedbackRequest request, int id)
        {
            return _feedbackService.UpdateFeedback(request, id);
        }

        [HttpDelete("id")]
        public string DeleteFeedback(int id)
        {
            return _feedbackService.DeleteFeedback(id);
        }
    }
}
