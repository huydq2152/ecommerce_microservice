using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Contracts.Common.Interfaces;
using IdentityServer4.AccessTokenValidation;
using Infrastructure.Identity.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Product.API.Entities;
using Product.API.Persistence;
using Product.API.Repositories.Interfaces;
using Shared.Common.Constants;
using Shared.DTOs.Product;

namespace Product.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(IdentityServerAuthenticationDefaults.AuthenticationScheme)]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public ProductsController(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    #region CRUD

    [HttpGet]
    [ClaimRequirement(FunctionCode.PRODUCT, CommandCode.VIEW)]
    public async Task<IActionResult> GetProducts()
    {
        var products = await _productRepository.GetProducts();
        var result = _mapper.Map<IEnumerable<ProductDto>>(products);
        return Ok(result);
    }

    [HttpGet("{id:long}")]
    [ClaimRequirement(FunctionCode.PRODUCT, CommandCode.VIEW)]
    public async Task<IActionResult> GetProduct([Required] long id)
    {
        var product = await _productRepository.GetProduct(id);
        if (product == null)
        {
            return NotFound();
        }

        var result = _mapper.Map<ProductDto>(product);
        return Ok(result);
    }

    [HttpPost]
    [ClaimRequirement(FunctionCode.PRODUCT, CommandCode.CREATE)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto productDto)
    {
        if (await IsExistedProductNo(productDto.No))
        {
            return BadRequest($"Product No: {productDto.No} is existed.");
        }
        
        var product = _mapper.Map<CatalogProduct>(productDto);
        await _productRepository.CreateProduct(product);
        await _productRepository.SaveChangeAsync();
        var result = _mapper.Map<ProductDto>(product);
        return Ok(result);
    }

    private async Task<bool> IsExistedProductNo([Required] string productNo)
    {
        var product = await _productRepository.GetProductByNo(productNo);
        if (product != null) return true;
        return false;
    }

    [HttpPut("{id:long}")]
    [ClaimRequirement(FunctionCode.PRODUCT, CommandCode.UPDATE)]
    public async Task<IActionResult> UpdateProduct([Required] long id, [FromBody] UpdateProductDto productDto)
    {
        var product = await _productRepository.GetProduct(id);
        if (product == null) return NotFound();

        _mapper.Map(productDto, product);
        await _productRepository.UpdateProduct(product);
        await _productRepository.SaveChangeAsync();
        var result = _mapper.Map<ProductDto>(product);
        return Ok(result);
    }

    [HttpDelete("{id:long}")]
    [ClaimRequirement(FunctionCode.PRODUCT, CommandCode.DELETE)]
    public async Task<IActionResult> DeleteProduct([Required] long id)
    {
        var product = await _productRepository.GetProduct(id);
        if (product == null) return NotFound();
        await _productRepository.DeleteProduct(id);
        await _productRepository.SaveChangeAsync();
        return NoContent();
    }

    #endregion

    #region Additional Resources

    [HttpGet("get-product-by-no/{productNo}")]
    [ClaimRequirement(FunctionCode.PRODUCT, CommandCode.VIEW)]
    public async Task<IActionResult> GetProductByNo([Required] string productNo)
    {
        var product = await _productRepository.GetProductByNo(productNo);
        if (product == null) return NotFound();
        var result = _mapper.Map<ProductDto>(product);
        return Ok(result);
    }

    #endregion
}