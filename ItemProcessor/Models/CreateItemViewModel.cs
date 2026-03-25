using System.ComponentModel.DataAnnotations;

namespace ItemProcessor.Models;

public class CreateItemViewModel
{
    [Required]
    [StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Range(0.01, 1000000)]
    public decimal Weight { get; set; }

    public int? ParentItemId { get; set; }
}
