using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Mango.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IProductService _productService;
        public HomeController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task <IActionResult> Index()
        {
            List<ProductDto?> list = new();
            ResponseDto? response = await _productService.GetAllProductAsync();
            if (response != null && response.IsSucess)
            {
                list = JsonConvert.DeserializeObject<List<ProductDto>>(Convert.ToString(response.Result));
                return View(list);
            }
            else
            {
                TempData["error"] = "User is Unauthorized";              //response?.Message;
            }
            return View(list);
            
        }
        [Authorize]
        public async Task<IActionResult> ProductDetails(int ProductId)
        {
            ProductDto? model = new();
            ResponseDto? response = await _productService.GetProductByIdAsync(ProductId);
            if (response != null && response.IsSucess)
            {
                model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
            }
            else
            {
                TempData["error"] = "User is Unauthorized";              //response?.Message;
            }
            return View(model);

        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
