using System.Collections.Generic;

namespace Bookland.Models
{
    public class SearchViewModel
    {
        public IEnumerable<SearchResult> SearchResults { get; set; }

        public string SearchQuery { get; set; }

    }
}