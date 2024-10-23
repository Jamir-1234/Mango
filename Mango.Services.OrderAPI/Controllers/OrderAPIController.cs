using AutoMapper;
using Mango.Services.OrderAPI.Data;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.Models.Dto;
using Mango.Services.OrderAPI.Services.Iservice;
using Mango.Services.OrderAPI.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace Mango.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderAPIController : ControllerBase
    {
        protected ResponseDto _response;
        private IMapper _mapper;
        private readonly AppDbContext _db;
        private readonly IProductService _productService;
        public OrderAPIController(IMapper mapper,AppDbContext db,IProductService productService)
        {
           this._response =new ResponseDto();
            _mapper = mapper;
            _db = db;
            _productService = productService;
            
        }
        //[Authorize]
        [HttpPost("CreateOrder")]
        public async Task<ResponseDto> CreateOrder([FromBody] CartDto cartDto)
        {
            try {
                OrderHeaderDto orderHeaderDto =_mapper.Map<OrderHeaderDto>(cartDto.CartHeader);
                orderHeaderDto.OrderTime=DateTime.Now;
                orderHeaderDto.Status=SD.Status_Pending;
                orderHeaderDto.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDto>>(cartDto.CartDetails);

                OrderHeader orderCreated = _db.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDto)).Entity;
                await _db.SaveChangesAsync();

                orderHeaderDto.OrderHeaderId=orderCreated.OrderHeaderId;
                _response.Result= orderHeaderDto;
            }
            catch (Exception ex) 
            {
                _response.IsSucess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
        //[Authorize]
        [HttpPost("CreateStripeSession")]
        public async Task<ResponseDto> CreateStripeSession([FromBody] StripeRequestDto stripeRequestDto)
        {
            try
            {
               // StripeConfiguration.ApiKey = "sk_test_51Mw60JApPi2utTvquswhI64DpVvFhmayoFddTGzeT8uHEFE9iSL47FusP1PvolKDuZ7Nwlseqi6MlqRfJfchntFB005arYwrGvI";
                var options = new SessionCreateOptions
                {
                    //PaymentMethodTypes = new List<string> { "card" }, // Specify payment methods
                    Mode = "payment", // Use "payment" for one-time charges or "subscription" for recurring payments
                    SuccessUrl =stripeRequestDto.ApprovedUrl,
                    CancelUrl=stripeRequestDto.CancelUrl,
                    LineItems = new List<SessionLineItemOptions>()

                };
                var DiscountObj = new List<SessionDiscountOptions>()
                {
                    new SessionDiscountOptions
                    {
                        Coupon=stripeRequestDto.OrderHeader.CouponCode
                    }
                 };
                foreach (var item in stripeRequestDto.OrderHeader.OrderDetails)
                {
                    var SessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),//$20.99=>2099
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(SessionLineItem);
                }
                if (stripeRequestDto.OrderHeader.Discount > 0)
                {
                    options.Discounts = DiscountObj;
                }
                var service = new SessionService();
                Session session = service.Create(options);
                stripeRequestDto.StripeSessionUrl = session.Url;
                OrderHeader orderHeader = _db.OrderHeaders.First(x => x.OrderHeaderId == stripeRequestDto.OrderHeader.OrderHeaderId);
                orderHeader.StripeSessionId=session.Id;
                _db.SaveChanges();
                _response.Result = stripeRequestDto;
               // service.Create(options);
            }catch (Exception ex) { 
                _response.Message= ex.Message;
                _response.IsSucess = false;
            }
            return _response;
        }



        [HttpPost("ValidateStripeSession")]
        public async Task<ResponseDto> ValidateStripeSession([FromBody] int oderheaderid)
        {
            try
            {
                var orderHeader = _db.OrderHeaders.First(x => x.OrderHeaderId == oderheaderid);

                var service = new SessionService();
                Session session = service.Get(orderHeader.StripeSessionId);
                var payemtIntentService=new PaymentIntentService();
                PaymentIntent paymentIntent = payemtIntentService.Get(session.PaymentIntentId);

                if (paymentIntent.Status == "succeeded")
                {
                    //tehn payemnt was succesful
                    orderHeader.PayementIntentId=paymentIntent.Id;
                    orderHeader.Status = SD.Status_Approved;
                    _db.SaveChanges();
                    _response.Result=_mapper.Map<OrderHeader>(orderHeader);
                }
               
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message;
                _response.IsSucess = false;
            }
            return _response;
        }
    }
}
