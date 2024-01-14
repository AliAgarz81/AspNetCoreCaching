using AspNetCoreCaching.Data;
using AspNetCoreCaching.DTOs;
using AspNetCoreCaching.Interfaces;
using AspNetCoreCaching.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreCaching.Services;

public class ItemServices : IItemServices
{
    private readonly DataContext _context;

    public ItemServices(DataContext context)
    {
        _context = context;
    }
    public async Task<List<Item>> GetAsync()
    {
        var items = await _context.Items.ToListAsync();
        return items;
    }

    public async Task<Item?> GetByIdAsync(Guid id)
    {
        var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == id);
        return item;
    }

    public async Task<bool> CreateAsync(Item item)
    {
        await _context.Items.AddAsync(item);
        return await Save();
    }

    public async Task<bool> UpdateAsync(Item item)
    {
         _context.Update(item);
         return await Save();
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == id);
        _context.Remove(item);
        return await Save();
        
    }

    public async Task<bool> Save()
    {
        var saved = await _context.SaveChangesAsync();
        return saved > 0;
    }
}