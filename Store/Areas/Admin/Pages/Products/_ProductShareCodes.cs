using Microsoft.EntityFrameworkCore;
using Store.Data;

namespace Store.Areas.Admin.Pages.Products;

public static class ProductShareCodes {
    public static async Task<bool> AllEntitiesExistAsync(this ApplicationDbContext context, List<int> entityIds) {
        // Use LINQ to query the database and check if all IDs exist
        var matchingCount = await context.Products
                                .Where(e => entityIds.Contains(e.Id))
                                .CountAsync();

        return matchingCount == entityIds.Count;
    }
}