using System.ComponentModel.DataAnnotations;

namespace AspNetCoreCaching.DTOs;

public record ItemDto(
    [Required] string Name, 
    [Required] string Description);