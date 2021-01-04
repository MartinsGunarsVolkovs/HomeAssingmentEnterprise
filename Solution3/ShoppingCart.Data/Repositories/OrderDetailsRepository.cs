using ShoppingCart.Data.Context;
using ShoppingCart.Domain.Interfaces;
using ShoppingCart.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShoppingCart.Data.Repositories
{
    public class OrderDetailsRepository : IOrderDetailsRepository
    {
        ShoppingCartDbContext _context;
        public OrderDetailsRepository(ShoppingCartDbContext context)
        {
            _context = context;
        }
        public int AddOrderDetails(OrderDetails od)
        {
            var product = _context.Products.SingleOrDefault(x => x.Id == od.ProductFK);
            _context.OrderDetails.Add(od);
            product.Stock -= od.Quantity;
            _context.SaveChanges();
            return od.Id;
        }

        public void AddOrderDetailsList(List<OrderDetails> od)
        {
            foreach(var item in od)
            {
                AddOrderDetails(item);
            }   
        }
    }
}
