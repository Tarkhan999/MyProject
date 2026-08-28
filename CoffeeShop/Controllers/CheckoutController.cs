using System.Security.Claims;
using System.Text.Json;
using FiorelloBackendPractice.Data;
using FiorelloBackendPractice.Models;
using FiorelloBackendPractice.ViewModels.Order;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;

namespace FiorelloBackendPractice.Controllers;

[Authorize]
public class CheckoutController : Controller
{
    private readonly AppDbContext _context;

    public CheckoutController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Pay(OrderCreateVM request)
    {
        // 1. Form Doğrulama Kontrolü
        if (!ModelState.IsValid)
        {
            var errors = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            ModelState.AddModelError("", $"Form hatası: {errors}");
            return View("Index", request);
        }

        // 2. Cookie Varlığı Kontrolü
        var basketCookie = Request.Cookies["basket"];
        if (string.IsNullOrEmpty(basketCookie))
        {
            ModelState.AddModelError("", "HATA: 'basket' adında bir Cookie bulunamadı! Sepetiniz boş veya çerez ismi farklı.");
            return View("Index", request);
        }

        // 3. JSON Okuma Kontrolü
        List<BasketCookieVM>? basketCookieItems = null;
        try
        {
            var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            basketCookieItems = JsonSerializer.Deserialize<List<BasketCookieVM>>(basketCookie, jsonOptions);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"HATA: Cookie JSON formatı okunamadı! Gelen Veri: {basketCookie} - Hata: {ex.Message}");
            return View("Index", request);
        }

        if (basketCookieItems == null || !basketCookieItems.Any())
        {
            ModelState.AddModelError("", "HATA: Cookie içeriği boş nesne döküldü.");
            return View("Index", request);
        }

        
        List<int> productIds = basketCookieItems.Select(b => b.GetRealId()).Where(id => id > 0).ToList();
        var products = await _context.Products.Where(p => productIds.Contains(p.Id)).ToListAsync();

        if (!products.Any())
        {
            ModelState.AddModelError("", $"HATA: Cookie içindeki ID'ler ({string.Join(",", productIds)}) ile veritabanındaki ürünler eşleşmedi.");
            return View("Index", request);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var order = new Order
        {
            AppUserId = userId!,
            Address = request.Address,
            City = request.City,
            Status = "Pending",
            OrderItems = new List<OrderItem>()
        };

        decimal totalPrice = 0;
        var stripeLineItems = new List<SessionLineItemOptions>();

        string debugPrices = "";

        foreach (var item in basketCookieItems)
        {
            int targetId = item.GetRealId();
            int itemCount = item.GetRealCount();
            var product = products.FirstOrDefault(p => p.Id == targetId);

            if (product != null)
            {
                decimal productPrice = Convert.ToDecimal(product.Price);

                if (productPrice <= 0)
                {
                    debugPrices += $"[{product.Name} fiyatı: {productPrice}] ";
                    continue;
                }

                decimal itemTotal = productPrice * itemCount;
                totalPrice += itemTotal;

                order.OrderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    Count = itemCount,
                    Price = productPrice
                });

                stripeLineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(productPrice * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = product.Name,
                        },
                    },
                    Quantity = itemCount,
                });
            }
        }

        if (!stripeLineItems.Any())
        {
            ModelState.AddModelError("", $"HATA: Stripe için geçerli ürün bulunamadı. Veritabanındaki Fiyat Durumu: {debugPrices}");
            return View("Index", request);
        }

        if (!stripeLineItems.Any())
        {
            ModelState.AddModelError("", "HATA: Stripe için geçerli fiyat/adet içeren ürün kalmadı.");
            return View("Index", request);
        }

        order.TotalPrice = totalPrice;

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // 5. Stripe Oturum Oluşturma
        try
        {
            var domain = $"{Request.Scheme}://{Request.Host}";
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = stripeLineItems,
                Mode = "payment",
                SuccessUrl = $"{domain}/Checkout/Success?orderId={order.Id}",
                CancelUrl = $"{domain}/Basket/Index",
            };

            var service = new SessionService();
            Session session = await service.CreateAsync(options);

            return Redirect(session.Url);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", $"Stripe Bağlantı Hatası: {ex.Message}");
            return View("Index", request);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Success(int orderId)
    {
        var order = await _context.Orders.FindAsync(orderId);
        if (order != null)
        {
            order.Status = "Completed";
            await _context.SaveChangesAsync();
            Response.Cookies.Delete("basket");
        }

        return View();
    }
}

public class BasketCookieVM
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Count { get; set; }
    public int Quantity { get; set; } 
    public int Qty { get; set; }

    public int GetRealId()
    {
        return Id != 0 ? Id : ProductId;
    }


    public int GetRealCount()
    {
        if (Count > 0) return Count;
        if (Quantity > 0) return Quantity;
        if (Qty > 0) return Qty;
        return 1; 
    }
}
