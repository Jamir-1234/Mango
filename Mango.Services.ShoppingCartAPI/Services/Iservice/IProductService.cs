using Mango.Services.ProductAPI.Models;

namespace Mango.Services.ShoppingCartAPI.Services.Iservice
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}
