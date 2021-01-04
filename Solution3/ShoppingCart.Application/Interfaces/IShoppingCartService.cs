using ShoppingCart.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShoppingCart.Application.Interfaces
{
    public interface IShoppingCartService
    {
        IQueryable<ProductViewModel> GetShoppingCartProducts(List<Guid> shoppingCartItems);
        void AddOrder(List<Guid> shoppingCartItems, string email);

    }
}
