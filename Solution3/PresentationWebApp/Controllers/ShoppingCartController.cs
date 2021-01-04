using Microsoft.AspNetCore.Mvc;
using PresentationWebApp.Helpers;
using ShoppingCart.Application.Interfaces;
using ShoppingCart.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PresentationWebApp.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IProductsService _productsService;
        
        public ShoppingCartController(IShoppingCartService shoppingCartService, IProductsService productsService)
        {
            _shoppingCartService = shoppingCartService;
            _productsService = productsService;
        }
        public IActionResult Index()
        {
            var shoppingCartItems = SessionHelper.GetObjectFromJson<List<Guid>>(HttpContext.Session, "shoppingCart");

            var list = _shoppingCartService.GetShoppingCartProducts(shoppingCartItems);
            return View(list);
        }
        public void AddToCookie(Guid id)
        {
            var itemToAdd = _productsService.GetProduct(id);
            if (itemToAdd.Disable == false)
            {
                if (SessionHelper.GetObjectFromJson<List<Guid>>(HttpContext.Session, "shoppingCart") == null)
                {
                    //If cookie is empty then it adds initial values to shoppingCart cookie
                    List<Guid> listOfShoppingCartItems = new List<Guid>();
                    listOfShoppingCartItems.Add(id);
                    SessionHelper.SetObjectAsJson(HttpContext.Session, "shoppingCart", listOfShoppingCartItems);
                }
                else
                {
                    //If there are Guids stored on the cookie, then it adds them on the cookie
                    List<Guid> listOfShoppingCartItems = SessionHelper.GetObjectFromJson<List<Guid>>(HttpContext.Session, "shoppingCart");
                    listOfShoppingCartItems.Add(id);
                    SessionHelper.SetObjectAsJson(HttpContext.Session, "shoppingCart", listOfShoppingCartItems);

                }
                TempData["feedback"] = "Product was added to the shopping cart";
            }
            else
            {
                TempData["warning"] = "Cant add disabled products";
            }
        }
        public IActionResult AddToCartDetails(Guid id)
        {
            AddToCookie(id);
            return RedirectToAction("Details","Products", new {id=id });
        }
        public IActionResult AddToCart(Guid id, int? page)
        {
            AddToCookie(id);

            //For correct redirection to products catalouge

            string lastSearchType = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "lastSearchedType");
            
            if (lastSearchType == "category")
            {
                return RedirectToAction("SearchByCategory", "Products",new {page=page});
            }
            else if(lastSearchType == "keyword")
            {
                return RedirectToAction("Search", "Products", new { page = page });
            }
            else
            {
                return RedirectToAction("Index", "Products", new { page = page });
            }
            
        }
        public IActionResult RemoveFromCart(Guid id)
        {
            List<Guid> listOfShoppingCartItems = SessionHelper.GetObjectFromJson<List<Guid>>(HttpContext.Session, "shoppingCart");
            listOfShoppingCartItems.RemoveAll(x=>x==id);
            SessionHelper.SetObjectAsJson(HttpContext.Session, "shoppingCart", listOfShoppingCartItems);

            return RedirectToAction("Index");
        }
        public IActionResult decrementCount(Guid id)
        {
            List<Guid> listOfShoppingCartItems = SessionHelper.GetObjectFromJson<List<Guid>>(HttpContext.Session, "shoppingCart");
            if (listOfShoppingCartItems.Count(x => x == id) > 1)
            {
                listOfShoppingCartItems.Remove(id);
                SessionHelper.SetObjectAsJson(HttpContext.Session, "shoppingCart", listOfShoppingCartItems);
            }

            return RedirectToAction("Index");

        }
        public IActionResult incrementCount(Guid id)
        {
            var stockOfItem = _productsService.GetStock(id);

            List<Guid> listOfShoppingCartItems = SessionHelper.GetObjectFromJson<List<Guid>>(HttpContext.Session, "shoppingCart");
            //Checks the stock and does not allow to add more if there isn't more in stock
            if (listOfShoppingCartItems.Count(x => x == id) < stockOfItem)
            {
                listOfShoppingCartItems.Add(id);
                SessionHelper.SetObjectAsJson(HttpContext.Session, "shoppingCart", listOfShoppingCartItems);
            }
            else
            {
                TempData["feedback"] = "Cant add more!";
            }
                

            return RedirectToAction("Index");

        }

    }
}
