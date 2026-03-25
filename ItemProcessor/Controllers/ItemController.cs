using ItemProcessor.Models;
using ItemProcessor.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ItemProcessor.Controllers;

public class ItemController : Controller
{
    private readonly ItemService _itemService;

    public ItemController(ItemService itemService)
    {
        _itemService = itemService;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _itemService.GetAllItemsAsync();
        var rows = BuildHierarchyRows(items);
        return View(rows);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await LoadParentOptions();
        return View(new CreateItemViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateItemViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await LoadParentOptions();
            TempData["ErrorMessage"] = "Please fix the validation errors.";
            return View(model);
        }

        var item = new Item
        {
            Name = model.Name.Trim(),
            Weight = model.Weight,
            ParentItemId = model.ParentItemId
        };

        var items = await _itemService.GetAllItemsAsync();
        var parentExists = !item.ParentItemId.HasValue || items.Any(x => x.Id == item.ParentItemId.Value);
        if (!parentExists)
        {
            ModelState.AddModelError(nameof(model.ParentItemId), "Selected parent item was not found.");
            await LoadParentOptions();
            TempData["ErrorMessage"] = "Could not create item.";
            return View(model);
        }

        await _itemService.CreateItemAsync(item);

        TempData["SuccessMessage"] = "Item created.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Process(int id)
    {
        if (id <= 0)
        {
            TempData["ErrorMessage"] = "Invalid item id.";
            return RedirectToAction(nameof(Index));
        }

        var ok = await _itemService.ProcessItemAsync(id);
        if (!ok)
        {
            TempData["ErrorMessage"] = "Item not found.";
            return RedirectToAction(nameof(Index));
        }

        TempData["SuccessMessage"] = "Item processed.";
        return RedirectToAction(nameof(Detail), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Detail(int id)
    {
        if (id <= 0)
        {
            return NotFound();
        }

        var item = await _itemService.GetItemAsync(id);
        if (item == null)
        {
            TempData["ErrorMessage"] = "Item not found.";
            return RedirectToAction(nameof(Index));
        }

        var allItems = await _itemService.GetAllItemsAsync();
        var descendants = BuildDescendantRows(id, allItems);

        var vm = new ItemDetailViewModel
        {
            Item = item,
            Descendants = descendants
        };

        return View(vm);
    }

    private List<ItemTreeRow> BuildHierarchyRows(List<Item> items)
    {
        var rows = new List<ItemTreeRow>();

        void AddChildren(int? parentId, int level)
        {
            var children = items
                .Where(x => x.ParentItemId == parentId)
                .OrderBy(x => x.Id)
                .ToList();

            foreach (var child in children)
            {
                rows.Add(new ItemTreeRow
                {
                    Item = child,
                    Level = level
                });

                AddChildren(child.Id, level + 1);
            }
        }

        AddChildren(null, 0);
        return rows;
    }

    private List<ItemTreeRow> BuildDescendantRows(int rootItemId, List<Item> allItems)
    {
        var rows = new List<ItemTreeRow>();

        void AddChildren(int parentId, int level)
        {
            var children = allItems
                .Where(x => x.ParentItemId == parentId)
                .OrderBy(x => x.Id)
                .ToList();

            foreach (var child in children)
            {
                rows.Add(new ItemTreeRow
                {
                    Item = child,
                    Level = level
                });

                AddChildren(child.Id, level + 1);
            }
        }

        AddChildren(rootItemId, 1);
        return rows;
    }

    private async Task LoadParentOptions()
    {
        var items = await _itemService.GetAllItemsAsync();
        ViewBag.ParentItems = new SelectList(items, nameof(Item.Id), nameof(Item.Name));
    }
}
