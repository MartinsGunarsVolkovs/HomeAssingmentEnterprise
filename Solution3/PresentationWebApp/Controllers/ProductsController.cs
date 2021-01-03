using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PresentationWebApp.Helpers;
using ShoppingCart.Application.Interfaces;
using ShoppingCart.Application.ViewModels;

namespace PresentationWebApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductsService _productsService;
        private readonly ICategoriesService _categoriesService;
        private IWebHostEnvironment _env;
        public ProductsController(IProductsService productsService, ICategoriesService categoriesService,
             IWebHostEnvironment env, IShoppingCartService shoppingCartService)
        { 
            _productsService = productsService;
            _categoriesService = categoriesService;
            _env = env;
        }
        
        public IActionResult Index(int? page)
        {
            SetViewBag(page);
            var list = _productsService.GetProducts();
            return View(list);
        }

        public IActionResult Search(string keyword, int?page)
        {
            SetViewBag(page);
            //Saves the last search results in a cookie so this method could be used again without submitting the form again
            if (keyword==null)
            {
                keyword = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "lastSearchedItem");
            }
            else
            {
                SessionHelper.SetObjectAsJson(HttpContext.Session, "lastSearchedItem", keyword);
            }

            

            var list = _productsService.GetProducts(keyword).ToList();

            SessionHelper.SetObjectAsJson(HttpContext.Session, "lastSearchedType", "keyword");

            return View("Index", list);
        }
        public IActionResult SearchByCategory(int category, int?page)
        {
            //Saves the last search results in a cookie so this method could be used again without submitting the form again
            if (category == 0)
            {
                category = SessionHelper.GetObjectFromJson<int>(HttpContext.Session, "lastSearchedCategory");
            }
            else 
            {
                SessionHelper.SetObjectAsJson(HttpContext.Session, "lastSearchedCategory", category);
            }

            var list = _productsService.GetProductsByCategory(category).ToList();

            SetViewBag(page);

            //Stores the type of the last search for the AddCart function in the ShoppingCartController to redirect with a correct list
            SessionHelper.SetObjectAsJson(HttpContext.Session, "lastSearchedType", "category");

            return View("Index", list);
        }

        public IActionResult Details(Guid id)
        {
            var p = _productsService.GetProduct(id);
            return View( p);
        }

        //the engine will load a page with empty fields
        [HttpGet]
        [Authorize (Roles ="Admin")] //is going to be accessed only by authenticated users
        public IActionResult Create()
        {
            //fetch a list of categories
            var listOfCategeories = _categoriesService.GetCategories();

            //we pass the categories to the page
            ViewBag.Categories = listOfCategeories;

            return View();
        }

        //here details input by the user will be received
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Create(ProductViewModel data, IFormFile f)
        {
            try
            {
                if(f !=  null)
                {
                    if(f.Length > 0)
                    {
                        
                            //C:\Users\Ryan\source\repos\SWD62BEP\SWD62BEP\Solution3\PresentationWebApp\wwwroot
                            string newFilename = Guid.NewGuid() + System.IO.Path.GetExtension(f.FileName);
                            string newFilenameWithAbsolutePath = _env.WebRootPath + @"\Images\" + newFilename;

                            using (var stream = System.IO.File.Create(newFilenameWithAbsolutePath))
                            {
                                f.CopyTo(stream);
                            }

                            data.ImageUrl = @"\Images\" + newFilename;

                        _productsService.AddProduct(data);
                        TempData["feedback"] = "Product was added successfully";
                    }
                }
                else if (data.ImageUrl != null)
                {
                    _productsService.AddProduct(data);
                    TempData["feedback"] = "Product was added successfully";
                }
                else
                {
                    TempData["warning"] = "Product was not added, please add an image!";
                }

                

                
            }
            catch (Exception)
            {
                //log error
                TempData["warning"] = "Product was not added!";
            }

           var listOfCategeories = _categoriesService.GetCategories();
           ViewBag.Categories = listOfCategeories;
            return View(data);
        
        } //fiddler, burp, zap, postman

        [Authorize(Roles = "Admin")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                _productsService.DeleteProduct(id);
                TempData["feedback"] = "Product was deleted";
            }
            catch (Exception)
            {
                //log your error 

                TempData["warning"] = "Product was not deleted"; //Change from ViewData to TempData
            }

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Disable(Guid id,int?page)
        {
            try
            {
                _productsService.DisableProduct(id);
                TempData["feedback"] = "Product was disabled";
            }
            catch (Exception)
            {
                TempData["warning"] = "Product was not disabled";
                throw;
            }

            //Returns to the last searched view
            string lastSearchType = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "lastSearchedType");
            if (lastSearchType == "category")
            {
                return RedirectToAction("SearchByCategory", new { page = page });
            }
            else if (lastSearchType == "keyword")
            {
                return RedirectToAction("Search", new { page = page });
            }
            else
            {
                return RedirectToAction("Index", new { page = page });
            }
        }
        public void SetViewBag(int? page)
        {
            ViewBag.Categories = _categoriesService.GetCategories();
            if (page == null)
            {
                ViewBag.Page = 1;
            }
            else
            {
                ViewBag.Page = page;
            }
        }




    }
}
