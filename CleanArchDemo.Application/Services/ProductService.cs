using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CleanArchDemo.Application.DTOs;
using CleanArchDemo.Application.Interfaces;
using CleanArchDemo.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace CleanArchDemo.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly ILogger<ProductService> _logger;
    private readonly IMapper _mapper;


    public ProductService(IProductRepository repository, ILogger<ProductService> logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<List<ProductDto>> GetAllProductsDtoAsync()
    {
        _logger.LogInformation("Fetching and mapping to DTOs...");
        var products = await _repository.GetAllAsync();
        return _mapper.Map<List<ProductDto>>(products);
    }

    public async Task AddProductAsync(CreateProductDto dto)
    {
        var product = _mapper.Map<Product>(dto);
        await _repository.AddAsync(product);
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task AddProductAsync(Product product)
    {
        await _repository.AddAsync(product);
    }
}