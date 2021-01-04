using AutoMapper;
using AutoMapper.QueryableExtensions;
using ShoppingCart.Application.Interfaces;
using ShoppingCart.Application.ViewModels;
using ShoppingCart.Domain.Interfaces;
using ShoppingCart.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShoppingCart.Application.Services
{
    public class ShoppingCartService : IShoppingCartService
    {
        
        private readonly IProductsRepository _productsRepository;
        private readonly IOrdersRepository _ordersRepository;
        private readonly IOrderDetailsRepository _orderDetailsRepository;
        private IMapper _mapper;

        public ShoppingCartService( IProductsRepository productsrepository, IMapper mapper,IOrdersRepository ordersRepository, IOrderDetailsRepository orderDetailsRepository)
        {
            _productsRepository = productsrepository;
            _mapper = mapper;
            _ordersRepository = ordersRepository;
            _orderDetailsRepository = orderDetailsRepository;
            
        }
        public bool CheckStock(List<OrderDetails> listToAdd)
        {
            
            foreach(var detail in listToAdd)
            {
                if (_productsRepository.GetStock(detail.ProductFK) < detail.Quantity)
                {
                    return false;
                }
            }
            return true;
        }
        public bool AddOrder(List<Guid> shoppingCartItems,string email)
        {
            //makes a new Order
            Order order = new Order();
            order.UserEmail = email;
            order.DatePlaced = DateTime.Now;

           Guid orderId= _ordersRepository.AddOrder(order);

            //Makes a list with unique Guids from shoppinCartItems list
            List<Guid> filteredShoppingCartItems = new List<Guid>();
            foreach (var item in shoppingCartItems)
            {
                if (!filteredShoppingCartItems.Any(x => x == item))
                {
                    filteredShoppingCartItems.Add(item);
                }
            }
            List<OrderDetails> listOfOrderDetails = new List<OrderDetails>();
            //Fills the OrderDetailsList with OrderDetails
            foreach(var item in filteredShoppingCartItems)
            {
                int quantity = shoppingCartItems.Count(x => x == item);
                Guid ProductFk = item;
                Guid OrderFk = orderId;
                double price = _productsRepository.GetProduct(item).Price*quantity;

                OrderDetails orderDetails = new OrderDetails();
                orderDetails.Quantity = quantity;
                orderDetails.ProductFK = ProductFk;
                orderDetails.OrderFK = OrderFk;
                orderDetails.Price = price;

                listOfOrderDetails.Add(orderDetails);
               
            }
            //Checks for stock before adding OrderDetails
            if (CheckStock(listOfOrderDetails))
            {
                _orderDetailsRepository.AddOrderDetailsList(listOfOrderDetails);
                return true;
                
            }
            else
            {
                //Removes the previously created Order that doesnt have OrderDetails, because of the failed stock check
                _ordersRepository.RemoveOrder(order);
                return false;
            }
            

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
