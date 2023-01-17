using LicenseKey.Controllers.Request.ProductRequest;
using LicenseKey.Models;
using LicenseKey.Services.ProductService;
using Microsoft.AspNetCore.Mvc;

namespace LicenseKey.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _ProductService;
        private readonly IConfiguration _configuration;

        public ProductController(IProductService ProductService, IConfiguration configuration)
        {
            _ProductService = ProductService;
            _configuration = configuration;
        }

        [HttpGet]
        public List<Product> GetAllProduct()
        {
            return _ProductService.GetAllProduct();
        }

        [HttpPost]
        public Task<string> UploadProduct([FromForm] UploadProductRequest request)
        {
            return _ProductService.UploadProduct(request);
        }

        [HttpPut]
        public Task<Product> UpdateProduct([FromForm] UploadProductRequest request)
        {
            return _ProductService.UpdateProduct(request);
        }

        [HttpDelete("id")]
        public string DeleteProduct(int id)
        {
            return _ProductService.DeleteProduct(id);
        }
    }
}
