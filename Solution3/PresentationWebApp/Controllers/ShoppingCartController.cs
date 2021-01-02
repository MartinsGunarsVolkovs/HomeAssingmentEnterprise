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
        
        public ShoppingCartController(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }
        public IActionResult Index()
        {
            var shoppingCartItems = SessionHelper.GetObjectFromJson<List<Guid>>(HttpContext.Session, "shoppingCart");

            var list = _shoppingCartService.GetShoppingCartProducts(shoppingCartItems);
            return View(list);
        }
        public IActionResult AddToCart(Guid id, int? page)
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
            List<Guid> listOfShoppingCartItems = SessionHelper.GetObjectFromJson<List<Guid>>(HttpContext.Session, "shoppingCart");
                listOfShoppingCartItems.Add(id);
                SessionHelper.SetObjectAsJson(HttpContext.Session, "shoppingCart", listOfShoppingCartItems);

            return RedirectToAction("Index");

        }

    }
}
