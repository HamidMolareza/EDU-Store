using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Store.Data;
using Store.Models;

namespace Store.Areas.Admin.Pages.Orders;

public class DetailsModel : PageModel {
    private readonly ApplicationDbContext _context;

    public DetailsModel(ApplicationDbContext context) {
        _context = context;
    }

    public OrderDetail Order { get; set; } = default!;

    public class OrderDetail {
        [DisplayName("آی‌دی سفارش")] public int Id { get; set; }
        [DisplayName("تاریخ و زمان")] public DateTime DateTime { get; set; }
        public List<OrderedProduct> Products { get; set; } = new();
        [DisplayName("وضعیت")] public int Status { get; set; } = OrderStatus.Processing;
        [DisplayName("سفارش‌دهنده")] public string UserName { get; set; }
    }

    public class OrderedProduct {
        [DisplayName("نام محصول")] public string Name { get; set; } = default!;

        [DisplayName("قیمت (تومان)")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal Price { get; set; }

        [DisplayName("تعداد سفارش")] public int StockQuantity { get; set; }
        public string Image { get; set; } = default!;
    }

    public async Task<IActionResult> OnGetAsync(int? id) {
        var order = await LoadDataAsync(id);
        if (order is null) return NotFound();
        Order = ConvertToViewModel(order);
        return Page();
    }

    private static OrderDetail ConvertToViewModel(Order order) => new() {
        Id       = order.Id,
        DateTime = order.DateTime,
        UserName = order.User.UserName!,
        Products = order.Products.Select(p => new OrderedProduct {
            Name          = p.Name,
            Price         = p.Price,
            Image         = p.Image,
            StockQuantity = p.StockQuantity
        }).ToList(),
        Status = order.Status
    };

    private async Task<Order?> LoadDataAsync(int? id) {
        if (id == null) return null;

        var order = await _context.Orders
                        .Include(o => o.Products)
                        .Include(o => o.User)
                        .FirstOrDefaultAsync(m => m.Id == id);
        return order;
    }

    //Toggle Status
    public async Task<IActionResult> OnPostAsync(int? id) {
        if (id == null) return NotFound();

        var order = await LoadDataAsync(id);
        if (order is null) return NotFound();

        order.Status = order.Status == OrderStatus.Processing
                           ? OrderStatus.Delivered
                           : OrderStatus.Processing;
        await _context.SaveChangesAsync();

        Order = ConvertToViewModel(order);
        return Page();
    }
}