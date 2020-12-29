using AutoMapper;
using AutoMapper.QueryableExtensions;
using ShoppingCart.Application.Interfaces;
using ShoppingCart.Application.ViewModels;
using ShoppingCart.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShoppingCart.Application.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        
        private readonly IProductsRepository _productsRepository;
        private IMapper _mapper;

        public ShoppingCartService( IProductsRepository productsrepository, IMapper mapper)
        {
            _productsRepository = productsrepository;
            _mapper = mapper;
            
        }

        public IQueryable<ProductViewModel> GetShoppingCartProducts(List<Guid> shoppingCartItems)
        {
            if (shoppingCartItems == null)
            {
                return null;
            }
            else
            {
                List<ProductViewModel> temp = new List<ProductViewModel>();


                foreach(Guid id in shoppingCartItems)
                {
                    var item = _productsRepository.GetProduct(id);
                    temp.Add(_mapper.Map<ProductViewModel>(item));
                }

                IQueryable<ProductViewModel> ShoppingCartItems = temp.AsQueryable();
                return ShoppingCartItems;

            }
        }
    }
}
