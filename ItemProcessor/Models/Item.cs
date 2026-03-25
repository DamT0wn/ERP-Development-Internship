using System.ComponentModel.DataAnnotations;

namespace ItemProcessor.Models;

public class Item
{
    public int Id { get; set; }

    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Range(0.01, 1000000)]
    public decimal Weight { get; set; }

    public bool IsProcessed { get; set; }

    public int? ParentItemId { get; set; }

    public DateTime? ProcessedAt { get; set; }

    public Item? ParentItem { get; set; }

    public List<Item> ChildItems { get; set; } = new();
}
