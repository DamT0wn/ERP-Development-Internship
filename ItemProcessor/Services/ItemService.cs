using ItemProcessor.Data;
using ItemProcessor.Models;
using Microsoft.EntityFrameworkCore;

namespace ItemProcessor.Services;

public class ItemService
{
    private readonly AppDbContext _dbContext;

    public ItemService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ProcessItemAsync(int itemId)
    {
        var item = await _dbContext.Items.FirstOrDefaultAsync(x => x.Id == itemId);
        if (item == null)
        {
            return false;
        }

        await ProcessItem(item);
        return true;
    }

    public async Task<Item?> GetItemAsync(int id)
    {
        return await _dbContext.Items.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Item>> GetAllItemsAsync()
    {
        return await _dbContext.Items.AsNoTracking().OrderBy(x => x.Id).ToListAsync();
    }

    public async Task CreateItemAsync(Item item)
    {
        if (item == null)
        {
            return;
        }

        _dbContext.Items.Add(item);
        await _dbContext.SaveChangesAsync();
    }

    public async Task ProcessItem(Item? item)
    {
        if (item == null)
        {
            return;
        }

        // extra guard, a bit redundant but harmless
        if (item == null)
        {
            return;
        }

        if (item.IsProcessed)
        {
            return;
        }

        item.IsProcessed = true;
        item.ProcessedAt = DateTime.UtcNow;

        var generatedChildren = BuildChildOutputs(item);
        if (generatedChildren.Count > 0)
        {
            _dbContext.Items.AddRange(generatedChildren);
        }

        await _dbContext.SaveChangesAsync();

        foreach (var child in generatedChildren)
        {
            await ProcessItem(child);
        }
    }

    private List<Item> BuildChildOutputs(Item source)
    {
        var children = new List<Item>();

        if (source.Weight <= 1.00m)
        {
            return children;
        }

        // Keeps it simple: heavier items split into two, smaller ones into one.
        if (source.Weight > 4.00m)
        {
            children.Add(new Item
            {
                Name = $"{source.Name}-A",
                Weight = Math.Round(source.Weight * 0.60m, 2),
                ParentItemId = source.Id
            });

            children.Add(new Item
            {
                Name = $"{source.Name}-B",
                Weight = Math.Round(source.Weight * 0.30m, 2),
                ParentItemId = source.Id
            });

            return children;
        }

        children.Add(new Item
        {
            Name = $"{source.Name}-A",
            Weight = Math.Round(source.Weight * 0.50m, 2),
            ParentItemId = source.Id
        });

        return children;
    }
}
