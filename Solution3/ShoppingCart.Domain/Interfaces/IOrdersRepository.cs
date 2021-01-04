using ShoppingCart.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShoppingCart.Domain.Interfaces
{
    public interface IOrdersRepository
    {
        Guid AddOrder(Order o);
        bool RemoveOrder(Order o);
    }
}
