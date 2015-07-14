using Bookland.Constants;
using Bookland.DAL.Abstract;
using Bookland.Helpers.Abstract;
using Bookland.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Bookland.Controllers
{
    public class SearchController : Controller
    {
        private IProductRepository productRepo;
        private ISearchHelpers searchHelpers;

        private const int ItemsPerPage = 5;

        public SearchController(IProductRepository productRepo, ISearchHelpers searchHelpers)
        {
            this.productRepo = productRepo;
            this.searchHelpers = searchHelpers;
        }

        public ActionResult Index(string searchQuery, int page = 1, bool includeDiscontinued = false)
        {
            if (string.IsNullOrEmpty(searchQuery))
            {
                TempData["message"] = "No search term(s) specified.";
                return RedirectToAction("Index", "Home");
            }
            
            IEnumerable<Product> searchableProducts = productRepo.GetProducts<object>(where: p => p.ProductStatus.ProductStatusAvailable
                                                            || (includeDiscontinued && p.ProductStatus.ProductStatusName.Equals(ProductStatusOptions.Discontinued)));

            IEnumerable<SearchResult> searchResults = searchHelpers.Search(searchableProducts, searchQuery);

            int searchResultsCount = searchResults.Count();
            int totalNumPages = (int)Math.Ceiling((decimal)searchResultsCount / (decimal)ItemsPerPage);

            // Set up items for page selection drop down list
            SelectListItem[] pages = new SelectListItem[totalNumPages];
            for (int i = 1; i <= totalNumPages; i++)
            {
                string pageString = i.ToString();
                pages[i-1] = new SelectListItem
                {
                    Selected = i == page,
                    Text = pageString,
                    Value = pageString
                };
            }

            return View(new SearchViewModel
            {
                SearchResults = searchResults.Skip(ItemsPerPage * (page - 1)).Take(ItemsPerPage).ToList<SearchResult>(),
                SearchQuery = searchQuery,
                CurrentPage = page,
                TotalNumPages = totalNumPages,
                TotalNumResults = searchResultsCount,
                PagesSelectList = pages
            });
        }
    }
}
