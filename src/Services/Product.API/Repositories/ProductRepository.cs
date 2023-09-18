using Contracts.Common.Interfaces;
using Infrastructure.Common;
using Infrastructure.Common.Repositories;
using Microsoft.EntityFrameworkCore;
using Product.API.Entities;
using Product.API.Persistence;
using Product.API.Repositories.Interfaces;

namespace Product.API.Repositories;

public class ProductRepository : RepositoryBase<CatalogProduct, long, ProductContext>, IProductRepository
{
    public ProductRepository(ProductContext dbContext, IUnitOfWork<ProductContext> unitOfWork) : base(dbContext,
        unitOfWork)
    {
    }

    public async Task<IEnumerable<CatalogProduct>> GetProducts() => await FindAll().ToListAsync();

    public Task<CatalogProduct> GetProduct(long id) => GetByIdAsync(id);

    public Task<CatalogProduct> GetProductByNo(string productNo) =>
        FindByCondition(o => o.No.Equals(productNo)).SingleOrDefaultAsync();

    public Task CreateProduct(CatalogProduct product) => CreateAsync(product);

    public Task UpdateProduct(CatalogProduct product) => UpdateAsync(product);

    public async Task DeleteProduct(long id)
    {
        var product = await GetProduct(id);
        if (product != null) await DeleteAsync(product);
    }
}