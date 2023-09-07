using Contracts.Common.Interfaces;
using Product.API.Entities;
using Product.API.Persistence;

namespace Product.API.Repositories.Interfaces;

public interface IProductRepository : IRepositoryBaseAsync<CatalogProduct, long, ProductContext>
{
    Task<IEnumerable<CatalogProduct>> GetProducts();

    Task<CatalogProduct> GetProduct(long id);

    Task<CatalogProduct> GetProductByNo(string productNo);

    Task CreateProduct(CatalogProduct product);

    Task UpdateProduct(CatalogProduct product);

    Task DeleteProduct(long id);
}