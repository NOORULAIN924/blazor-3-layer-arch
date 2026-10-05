using ProductManagementApp.DataAccessLayer.Interfaces;
using ProductManagementApp.Models;
using System.Text.Json;
using System.Xml.Linq;

namespace ProductManagementApp.DataAccessLayer.Repositories
{
    public class ProductRepositoryJson : IProductRepository
    {
        private readonly string filePath;

        public ProductRepositoryJson()
        {
            filePath = Path.Combine(
                AppContext.BaseDirectory,
                "Data",
                "products.json"
            );
        }

        private async Task<List<ProductModel>> ReadProductsAsync()
        {
            if (!File.Exists(filePath))
            {
                return new List<ProductModel>();
            }

            string json = await File.ReadAllTextAsync(filePath);

            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<ProductModel>();
            }

            try
            {
                return JsonSerializer.Deserialize<List<ProductModel>>(json)
                       ?? new List<ProductModel>();
            }
            catch (JsonException)
            {
                return new List<ProductModel>();
            }
        }

        private async Task WriteProductsAsync(List<ProductModel> products)
        {
            string json = JsonSerializer.Serialize(
                products,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                }
            );

            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task<List<ProductModel>> GetAllAsync()
        {
            return await ReadProductsAsync();
        }

        public async Task<ProductModel?> GetByIdAsync(Guid id)
        {
            List<ProductModel> products = await ReadProductsAsync();

            return products.FirstOrDefault(p => p.Id == id);
        }

        public async Task AddAsync(ProductModel product)
        {
            List<ProductModel> products = await ReadProductsAsync();

            products.Add(product);

            await WriteProductsAsync(products);
        }

        public async Task UpdateAsync(ProductModel product)
        {
            List<ProductModel> products = await ReadProductsAsync();

            ProductModel? existingProduct =
                products.FirstOrDefault(p => p.Id == product.Id);

            if (existingProduct == null)
            {
                return;
            }

            existingProduct.Name = product.Name;
            existingProduct.Price = product.Price;
            existingProduct.Category = product.Category;
            existingProduct.Description = product.Description;
            existingProduct.IsActive = product.IsActive;

            await WriteProductsAsync(products);
        }

        public async Task DeleteAsync(Guid id)
        {
            List<ProductModel> products = await ReadProductsAsync();

            ProductModel? product =
                products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return;
            }

            products.Remove(product);

            await WriteProductsAsync(products);
        }

        public async Task<List<ProductModel>> SearchAsync(string keyword)
        {
            List<ProductModel> products = await ReadProductsAsync();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                return products;
            }

            keyword = keyword.ToLower();

            return products
                .Where(p =>
                    p.Name.ToLower().Contains(keyword) ||
                    p.Category.ToLower().Contains(keyword) ||
                    p.Description.ToLower().Contains(keyword))
                .ToList();
        }
    }
}