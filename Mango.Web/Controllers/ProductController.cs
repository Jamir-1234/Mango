using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService)
        {
                _productService = productService;
        }
        [HttpGet]
        public async Task<IActionResult> ProductIndex()
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
        public async Task<IActionResult> ProductCreate()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ProductCreate(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _productService.CreateProductAsync(model);
                if(response!=null&&response.IsSucess)
                 {
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> ProductEdit(int productId)
        {
            ResponseDto? response=await _productService.GetProductByIdAsync(productId);
            if (response != null && response.IsSucess)
            {
                ProductDto? productDto=JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(productDto);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();

        }
        [HttpPost]
        public async Task<IActionResult> ProductEdit(ProductDto productDto)
        {
            ResponseDto? response = await _productService.UpdateProductBYIdAsync(productDto);
            if (response != null && response.IsSucess)
            {
                ProductDto?  model= JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return  View(productDto);

        }
        [HttpGet]
        public async Task<IActionResult> ProductDelete(int productId)
        {
            ResponseDto? response=await _productService.GetProductByIdAsync(productId);
            if (response != null && response.IsSucess)
            {
               ProductDto? productDto=JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return View(productDto);
                }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
            
        }
        [HttpPost]
        public async Task<IActionResult> ProductDelete(ProductDto productDto)
        {
            ResponseDto? response = await _productService.DeleteProductAsync(productDto.ProductId);
            if (response != null && response.IsSucess)
            {
                ProductDto model = JsonConvert.DeserializeObject<ProductDto>(Convert.ToString(response.Result));
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return View(productDto);
        }
        //public IActionResult Index()
        //    return View();
        //}
    }
}
