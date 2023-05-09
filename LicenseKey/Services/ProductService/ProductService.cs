using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using LicenseKey.Controllers.Request.ProductRequest;
using LicenseKey.Helpers;
using LicenseKey.Helpers.Dto;
using LicenseKey.Models;
using LicenseKey.Repository;
using Microsoft.Extensions.Options;

namespace LicenseKey.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly Cloudinary _cloudinaryDotNet;

        public ProductService(ApplicationDbContext applicationDbContext, IConfiguration configuration, IMapper mapper , IOptions<CloudinarySettings> config)
        {
            _applicationDbContext = applicationDbContext;
            _configuration = configuration;
            _mapper = mapper;

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

        public List<Product> GetAllProduct(int page, string category)
        {
            List<Product> result = _applicationDbContext.Product.Where(x => x.Category == category).Skip((page - 1) * 10).Take(10).ToList();
            return result;
        }

        public Product GetProductById(int id)
        {
            Product? product = _applicationDbContext.Product.FirstOrDefault(x => x.Id.Equals(id)) ?? throw new Exception("Product Not Found");
            return product;
        }

        public async Task<Product> UpdateProduct(UploadProductRequest request)
        {
            if(request == null )
            {
                throw new ArgumentNullException(nameof(request));
            }
            Product? Product = _applicationDbContext.Product.FirstOrDefault(x => x.Title == request.Name) ?? throw new Exception("Product not found");
            if (request?.LogoUrl?.Length > 0)
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

        public async Task<string> UploadProduct(ProductDto request)
        {
            Product? isProduct = _applicationDbContext.Product.FirstOrDefault(x => x.Title == request.Title);
            if(isProduct != null)
            {
                throw new Exception("Product exist!");
            }
            if (request?.LogoUrl?.Length > 0)
            {
                ImageUploadResult result = await UploadImageToCloud(request.LogoUrl);
                Product product = _mapper.Map<Product>(request);

                product.LogoUrl = result.Url.ToString();
                product.ImagePublicIP = result.PublicId.ToString();

                _applicationDbContext.Add(product);
                _applicationDbContext.SaveChanges();
            }
            else { throw new AppException("No Image Found"); }
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
