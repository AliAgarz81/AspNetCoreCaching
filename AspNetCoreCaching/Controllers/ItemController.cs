using AspNetCoreCaching.DTOs;
using AspNetCoreCaching.Interfaces;
using AspNetCoreCaching.Models;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreCaching.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemController : ControllerBase
{
    private readonly IItemServices _itemServices;
    private readonly ICacheService _cacheService;

    public ItemController(IItemServices itemServices, ICacheService cacheService)
    {
        _itemServices = itemServices;
        _cacheService = cacheService;
    }

    [HttpGet]
    public async Task<IActionResult> GetItems(CancellationToken cancellationToken = default)
    {
        List<Item>? cachedItems = await _cacheService.GetAsync<List<Item>>("items", cancellationToken);
        if (cachedItems is not null)
        {
            return Ok(cachedItems);
        }
        var items = await _itemServices.GetAsync();
        await _cacheService.SetAsync("items", items, cancellationToken);
        return Ok(items);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetItem(Guid id)
    {
        var item = await _itemServices.GetByIdAsync(id);
        if (item is null)
        {
            return NotFound("Item not found");
        }
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> CreateItem(ItemDto itemDto, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        Item item = new Item()
        {
            Name = itemDto.Name,
            Description = itemDto.Description
        };
        var createItem = await _itemServices.CreateAsync(item);
        if (!createItem)
        {
            ModelState.AddModelError("", "Something went wrong while savin");
            return StatusCode(500, ModelState);
        }

        await _cacheService.RemoveAsync("items", cancellationToken);
        return Ok();
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateItem(Guid id,[FromBody] ItemDto itemDto, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var item = await _itemServices.GetByIdAsync(id);
        if (item is null)
        {
            return NotFound("Item not found");
        }
        item.Name = itemDto.Name;
        item.Description = itemDto.Description;
        var updateItem = await _itemServices.UpdateAsync(item);
        if (!updateItem)
        {
            ModelState.AddModelError("", "Something went wrong while savin");
            return StatusCode(500, ModelState);
        }
        await _cacheService.RemoveAsync("items", cancellationToken);
        return Ok();
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteItem(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _itemServices.GetByIdAsync(id);
        if (item is null)
        {
            return NotFound("Item not found");
        }
        var deleteItem = await _itemServices.DeleteAsync(id);
        if (!deleteItem)
        {
            ModelState.AddModelError("", "Something went wrong while savin");
            return StatusCode(500, ModelState);
        }
        await _cacheService.RemoveAsync("items", cancellationToken);
        return Ok();
    }
}