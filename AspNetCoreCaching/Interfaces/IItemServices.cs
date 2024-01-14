using AspNetCoreCaching.DTOs;
using AspNetCoreCaching.Models;

namespace AspNetCoreCaching.Interfaces;

public interface IItemServices
{
    Task<List<Item>> GetAsync();
    Task<Item?> GetByIdAsync(Guid id);
    Task<bool> CreateAsync(Item item);
    Task<bool> UpdateAsync(Item item);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> Save();
}