using System.Collections.Generic;
using System.Web.Mvc;

namespace Bookland.Models
{
    public class SearchViewModel
    {
        public List<SearchResult> SearchResults { get; set; }

        public string SearchQuery { get; set; }

        public int CurrentPage { get; set; }

        public int TotalNumPages { get; set; }

        public int TotalNumResults { get; set; }

        public IEnumerable<SelectListItem> PagesSelectList { get; set; }
    }
}