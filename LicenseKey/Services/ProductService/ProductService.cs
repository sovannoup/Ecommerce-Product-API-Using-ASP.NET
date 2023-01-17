using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using LicenseKey.Controllers.Request.ProductRequest;
using LicenseKey.Helpers;
using LicenseKey.Models;
using LicenseKey.Repository;
using Microsoft.Extensions.Options;

namespace LicenseKey.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly Cloudinary _cloudinaryDotNet;

        public ProductService(ApplicationDbContext applicationDbContext, IConfiguration configuration, IWebHostEnvironment webHostEnvironment, IOptions<CloudinarySettings> config)
        {
            _applicationDbContext = applicationDbContext;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;

            var acc = new Account
            (
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinaryDotNet = new Cloudinary(acc);
        }

        public string DeleteProduct(int id)
        {
            if(id == 0)
            {
                throw new Exception("ID is not found");
            }
            Product? cur = _applicationDbContext.Product.FirstOrDefault(x => x.Id == id);
            if(cur == null)
            {
                throw new Exception("Product Not Found");
            }
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

        public List<Product> GetAllProduct()
        {
            List<Product> result = _applicationDbContext.Product.ToList();
            return result;
        }

        public async Task<Product> UpdateProduct(UploadProductRequest request)
        {
            if(request == null )
            {
                throw new ArgumentNullException(nameof(request));
            }
            Product Product = _applicationDbContext.Product.FirstOrDefault(x => x.Name == request.Name);
            if(Product == null)
            {
                throw new Exception("Product not found");
            }
            Product.Name= request.Name;
            Product.Total= request.Total;
            Product.LicenseKeyTo = request.LicenseKeyTo;
            if(request?.LogoUrl?.Length > 0)
            {
                var deleteParam = new DeletionParams(Product.ImagePublicIP);
                var deleted = _cloudinaryDotNet.Destroy(deleteParam);
                if(deleted.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception("Image url not found");
                }
                ImageUploadResult result = await UploadImageToCloud(request.LogoUrl);
                Product.LogoUrl = result.Url.ToString();
                Product.ImagePublicIP = result.PublicId.ToString();
            }
            _applicationDbContext.Update(Product);
            _applicationDbContext.SaveChanges();
            return Product;
        }

        public async Task<string> UploadProduct(UploadProductRequest request)
        {
            Product cur = _applicationDbContext.Product.FirstOrDefault(x => x.Name == request.Name);
            if(cur != null)
            {
                throw new Exception("Product exist!");
            }
            if (request?.LogoUrl?.Length > 0)
            {
                ImageUploadResult result = await UploadImageToCloud(request.LogoUrl);

                Product Product = new
                     (
                         request.Name,
                         result.Url.ToString(),
                         result.PublicId.ToString(),
                         request.Total,
                         request.LicenseKeyTo
                     );
                _applicationDbContext.Add(Product);
                _applicationDbContext.SaveChanges();
            }
            return "Success";




            //Upload to local project
            /* string uniqueFileName = null;
             string uploads = Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads");
             if(request?.LogoUrl?.Length > 0)

                 uniqueFileName = Guid.NewGuid().ToString() + "_" + request.LogoUrl.FileName;
             {
                 string filePath = Path.Combine(uploads, uniqueFileName);


                 using (Stream fileStream = new FileStream(filePath, FileMode.Create))
                 {
                     await request.LogoUrl.CopyToAsync(fileStream);
                 }
             }
             return uniqueFileName;*/
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
