using Mango.Services.OrderAPI.Models.Dto;

namespace Mango.Services.OrderAPI.Services.Iservice
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}
