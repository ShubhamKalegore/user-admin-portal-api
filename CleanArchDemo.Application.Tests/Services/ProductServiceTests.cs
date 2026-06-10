using AutoMapper;
using CleanArchDemo.Application.DTOs;
using CleanArchDemo.Application.Interfaces;
using CleanArchDemo.Application.Mappings;
using CleanArchDemo.Application.Services;
using CleanArchDemo.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace CleanArchDemo.Application.Tests.Services;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _repositoryMock = new();
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile<MappingProfile>())
            .CreateMapper();

        _service = new ProductService(
            _repositoryMock.Object,
            Mock.Of<ILogger<ProductService>>(),
            mapper);
    }

    [Fact]
    public async Task GetAllProductsAsync_ShouldReturnProducts()
    {
        var products = new List<Product>
        {
            new() { Id = 1, Name = "Laptop", Price = 55000 },
            new() { Id = 2, Name = "Mouse", Price = 800 }
        };

        _repositoryMock
            .Setup(repository => repository.GetAllAsync())
            .ReturnsAsync(products);

        var result = await _service.GetAllProductsAsync();

        Assert.Equal(2, result.Count);
        Assert.Equal("Laptop", result[0].Name);
    }

    [Fact]
    public async Task GetAllProductsDtoAsync_ShouldReturnMappedProductDtos()
    {
        var products = new List<Product>
        {
            new() { Id = 1, Name = "Keyboard", Price = 1500 }
        };

        _repositoryMock
            .Setup(repository => repository.GetAllAsync())
            .ReturnsAsync(products);

        var result = await _service.GetAllProductsDtoAsync();

        Assert.Single(result);
        Assert.IsType<ProductDto>(result[0]);
        Assert.Equal("Keyboard", result[0].Name);
        Assert.True(result[0].IsDto);
    }

    [Fact]
    public async Task GetProductByIdAsync_ShouldReturnProduct_WhenProductExists()
    {
        var product = new Product { Id = 10, Name = "Monitor", Price = 12000 };

        _repositoryMock
            .Setup(repository => repository.GetByIdAsync(10))
            .ReturnsAsync(product);

        var result = await _service.GetProductByIdAsync(10);

        Assert.NotNull(result);
        Assert.Equal(10, result.Id);
        Assert.Equal("Monitor", result.Name);
    }

    [Fact]
    public async Task AddProductAsync_ShouldMapDtoAndCallRepository()
    {
        var dto = new CreateProductDto
        {
            Name = "Desk",
            Price = 7000
        };

        await _service.AddProductAsync(dto);

        _repositoryMock.Verify(
            repository => repository.AddAsync(It.Is<Product>(product =>
                product.Name == "Desk" &&
                product.Price == 7000)),
            Times.Once);
    }
}
