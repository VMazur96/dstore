using Drajbot.Api.Data;
using Drajbot.Api.DTOs.Orders;
using Drajbot.Api.Interfaces;
using Drajbot.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Drajbot.Api.Services.Orders
{
    public class OrderService(ApplicationDbContext context) : IOrderService
    {
        public async Task<string> CreateOrderAsync(int userId, OrderCreateDto request)
        {
            decimal total = 0;
            foreach (var item in request.Items)
            {
                total += item.Price * item.Quantity;
            }

            var order = new Order
            {
                UserId = userId,
                GameUsername = request.GameUsername,
                TotalPrice = total,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            context.Orders.Add(order);
            await context.SaveChangesAsync();

            var orderItems = request.Items.Select(i => new OrderItem
            {
                OrderId = order.Id,
                ProductId = i.ProductId,
                ItemName = i.ItemName,
                Price = i.Price,
                Quantity = i.Quantity
            });

            context.OrderItems.AddRange(orderItems);
            await context.SaveChangesAsync();

            return $"Porudžbina #{order.Id} je kreirana. Nalog '{request.GameUsername}' je zabeležen.";
        }

        // Korisnik povlači samo svoju istoriju kupovina
        public async Task<IEnumerable<OrderResponseDto>> GetUserOrdersAsync(int userId)
        {
            return await context.Orders
                .Include(o => o.OrderItems)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderResponseDto
                {
                    Id = o.Id,
                    TotalPrice = o.TotalPrice,
                    GameUsername = o.GameUsername ?? "",
                    Status = o.Status,
                    CreatedAt = o.CreatedAt,
                    Items = o.OrderItems.Select(i => new OrderItemResponseDto
                    {
                        Id = i.Id,
                        ProductId = i.ProductId,
                        ItemName = i.ItemName,
                        Price = i.Price,
                        Quantity = i.Quantity
                    }).ToList()
                })
                .ToListAsync();
        }

        // Admin (Ti) povlači sve porudžbine sa sajta da vidiš kome šta isporučuješ
        public async Task<IEnumerable<OrderResponseDto>> GetAllOrdersAsync()
        {
            return await context.Orders
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderResponseDto
                {
                    Id = o.Id,
                    TotalPrice = o.TotalPrice,
                    GameUsername = o.GameUsername ?? "",
                    Status = o.Status,
                    CreatedAt = o.CreatedAt,
                    Items = o.OrderItems.Select(i => new OrderItemResponseDto
                    {
                        Id = i.Id,
                        ProductId = i.ProductId,
                        ItemName = i.ItemName,
                        Price = i.Price,
                        Quantity = i.Quantity
                    }).ToList()
                })
                .ToListAsync();
        }

        // Menjanje statusa sa ugrađenom zaštitom
        public async Task<string> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            // 1. Definišemo koji su statusi jedino dozvoljeni na tvom sajtu
            var dozvoljeniStatusi = new[] { "Pending", "Completed", "Refunded", "Odbijeno" };

            if (!dozvoljeniStatusi.Contains(newStatus))
                return $"Greška: Nevalidan status. Dozvoljeni su samo: {string.Join(", ", dozvoljeniStatusi)}";

            // 2. Tražimo porudžbinu
            var order = await context.Orders.FindAsync(orderId);
            if (order == null) return "Greška: Porudžbina ne postoji u sistemu.";

            // 3. Menjamo status
            order.Status = newStatus;
            await context.SaveChangesAsync();
            return $"Status porudžbine #{orderId} uspešno promenjen u '{newStatus}'.";
        }
    }
}