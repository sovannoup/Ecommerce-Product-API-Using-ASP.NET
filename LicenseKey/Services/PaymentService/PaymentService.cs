using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using LicenseKey.Models;
using AutoMapper;
using LicenseKey.Repository;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Org.BouncyCastle.Asn1.Ocsp;
using LicenseKey.Helpers.Dto;
using static Org.BouncyCastle.Math.EC.ECCurve;
using LicenseKey.Helpers;
using Microsoft.Extensions.Options;

namespace LicenseKey.Services.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly Cloudinary _cloudinaryDotNet;

        public PaymentService(ApplicationDbContext applicationDbContext, IMapper mapper, IConfiguration configuration, IOptions<CloudinarySettings> config)
        {
            _applicationDbContext = applicationDbContext;
            _mapper = mapper;
            _configuration = configuration;

            var acc = new Account
            (
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinaryDotNet = new Cloudinary(acc);
        }

        public string DeletePayment(PaymentDto payment)
        {
            throw new NotImplementedException();
        }

        public List<Payment> GetPayments()
        {
            List<Payment> result = _applicationDbContext.Payment.ToList();
            return result;
        }

        public async Task<string> UpdatePayment(PaymentDto payment)
        {
            try
            {
                if (payment == null)
                {
                    throw new ArgumentNullException(nameof(payment));
                }
                Payment? Payment = _applicationDbContext.Payment.FirstOrDefault(x => x.Id == payment.Id) ?? throw new Exception("Payment not found");
                if (payment?.PhotoUrl?.Length > 0)
                {
                    var deleteParam = new DeletionParams(Payment.ImagePublicIP);
                    var deleted = _cloudinaryDotNet.Destroy(deleteParam);
                    if (deleted.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        throw new Exception("Image url not found");
                    }
                    ImageUploadResult result = await UploadImageToCloud(payment.PhotoUrl);
                    Payment.PhotoUrl = result.Url.ToString();
                    Payment.ImagePublicIP = result.PublicId.ToString();
                }
                _applicationDbContext.Update(Payment);
                _applicationDbContext.SaveChanges();
            }
            catch (Exception)
            {
                throw new Exception("Error");
            }
            return "Success";
        }

        public async Task<string> UploadPaymentMethod(PaymentDto payment)
        {
            Payment? isPayment = _applicationDbContext.Payment.FirstOrDefault(x => x.Title == payment.Title);
            if (isPayment != null)
            {
                throw new Exception("Payment exist!");
            }

            Payment payObj = _mapper.Map<Payment>(payment);

            if (payment?.PhotoUrl?.Length > 0)
            {
                ImageUploadResult result = await UploadImageToCloud(payment.PhotoUrl);

                payObj.PhotoUrl = result.Url.ToString();
                payObj.ImagePublicIP = result.PublicId.ToString();
            }

            _applicationDbContext.Add(payObj);
            _applicationDbContext.SaveChanges();

            return "Success";
        }

        public string DeletePayment(int id)
        {
            Payment? cur = _applicationDbContext.Payment.FirstOrDefault(x => x.Id == id) ?? throw new Exception("Payment Not Found");
            var deleteParam = new DeletionParams(cur.ImagePublicIP);
            var deleted = _cloudinaryDotNet.Destroy(deleteParam);
            if (deleted.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception("Image url not found");
            }
            _applicationDbContext.Remove(cur);
            _applicationDbContext.SaveChanges();
            return "success";
        }
        public async Task<ImageUploadResult> UploadImageToCloud(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
                };
                uploadResult = await _cloudinaryDotNet.UploadAsync(uploadParams);
            }
            Console.WriteLine(uploadResult.PublicId.ToString());
            return uploadResult;
        }
    }
}
