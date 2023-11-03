using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Store.Data;

namespace Store.Areas.Admin.Pages.Orders;

public class IndexModel : PageModel {
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context) {
        _context = context;
    }

    public IList<OrderIndex> Orders { get; set; } = default!;

    public class OrderIndex {
        [DisplayName("آی‌دی")] public int Id { get; set; }
        [DisplayName("تاریخ و زمان")] public DateTime DateTime { get; set; } = DateTime.UtcNow;
        [DisplayName("وضعیت")] public int Status { get; set; }
        [DisplayName("سفارش‌دهنده")] public string UserName { get; set; }
    }

    public async Task OnGetAsync() {
        Orders = await _context.Orders
                     .AsNoTracking()
                     .OrderByDescending(o => o.Id)
                     .Include(o => o.User)
                     .Select(order => new OrderIndex {
                         Id       = order.Id,
                         Status   = order.Status,
                         DateTime = order.DateTime,
                         UserName = order.User.UserName!
                     }).ToListAsync();
    }
}