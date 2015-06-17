using Bookland.DAL.Abstract;
using Bookland.Helpers.Abstract;
using Bookland.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Bookland.Controllers
{
    public class SearchController : Controller
    {
        IProductRepository productRepo;
        ISearchHelpers searchHelpers;

        public SearchController(IProductRepository productRepo, ISearchHelpers searchHelpers)
        {
            this.productRepo = productRepo;
            this.searchHelpers = searchHelpers;
        }

        public ActionResult Index(string searchQuery)
        {
            if (string.IsNullOrEmpty(searchQuery))
                return HttpNotFound("Search term(s) not specified.");

            IEnumerable<SearchResult> searchResults = searchHelpers.Search(productRepo.GetProducts(p => p.ProductID), searchQuery);

            return View(new SearchViewModel { SearchResults = searchResults, SearchQuery = searchQuery });
        }
    }
}
