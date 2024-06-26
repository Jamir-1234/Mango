﻿using AutoMapper;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.ProductAPI.Controllers
{
    [Route("api/product")]
    [ApiController]
   // [Authorize]
    public class ProductAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ResponseDto _response;
        private  IMapper _mapper;
        public ProductAPIController(AppDbContext db,IMapper mapper)
        {
                _db = db;
            _mapper = mapper;
            _response=new ResponseDto();
               
        }
        [HttpGet]
      
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Product> obj = _db.Products.ToList();
                _response.Result = _mapper.Map<IEnumerable<ProductDto>>(obj);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                _response.IsSucess = false;
                _response.Message = ex.Message;
            }
            return _response;

        }
        [HttpGet]
        [Route("{ProductId:int}")]
        public ResponseDto Get(int ProductId) 
        {
            try
            {
                 Product obj = _db.Products.FirstOrDefault(p => p.ProductId ==ProductId);
                _response.Result = _mapper.Map<ProductDto>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSucess = false;
                _response.Message = ex.Message;
            }
            return _response;

        }
        [HttpPost]
       [Authorize(Roles = "ADMIN")]
        public ResponseDto Post([FromBody] ProductDto  productDto)

        {
            try
            {
                Product obj = _mapper.Map<Product>(productDto);
                _db.Products.Add(obj);
                _db.SaveChanges();
                _response.Result = _mapper.Map<ProductDto>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSucess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Put([FromBody] ProductDto productDto)

        {
            try
            {
                Product obj = _mapper.Map<Product>(productDto);
                _db.Products.Update(obj);
                _db.SaveChanges();
                _response.Result = _mapper.Map<ProductDto>(obj);
            }
            catch (Exception ex)
            {
                _response.IsSucess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles ="ADMIN")]
        public ResponseDto Delete(int id)
        {
            try
            {
                Product obj = _db.Products.FirstOrDefault(p => p.ProductId==id);
                _db.Products.Remove(obj);
                _db.SaveChanges();

            }
            catch (Exception ex) 
            {
                _response.IsSucess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}
