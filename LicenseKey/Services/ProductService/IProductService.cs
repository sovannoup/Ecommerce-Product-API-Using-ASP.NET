using LicenseKey.Controllers.Request.ProductRequest;
using LicenseKey.Models;

namespace LicenseKey.Services.ProductService
{
    public interface IProductService
    {
        public List<Product> GetAllProduct();
        public Task<string> UploadProduct(UploadProductRequest request);
        public Task<Product> UpdateProduct(UploadProductRequest request);
        public string DeleteProduct(int id);
    }
}
