using LicenseKey.Controllers.Request.ProductRequest;
using LicenseKey.Helpers.Dto;
using LicenseKey.Models;

namespace LicenseKey.Services.ProductService
{
    public interface IProductService
    {
        public List<Product> GetAllProduct(int page, string category);
        public Product GetProductById(int id);
        public Task<string> UploadProduct(ProductDto request);
        public Task<Product> UpdateProduct(UploadProductRequest request);
        public string DeleteProduct(int id);
    }
}
