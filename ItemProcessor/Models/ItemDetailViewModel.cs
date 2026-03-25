namespace ItemProcessor.Models;

public class ItemDetailViewModel
{
    public Item Item { get; set; } = new();

    public List<ItemTreeRow> Descendants { get; set; } = new();
}
