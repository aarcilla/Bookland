using Bookland.Models;
using System.Collections.Generic;

namespace Bookland.Helpers.Abstract
{
    public interface ISearchHelpers
    {
        IEnumerable<SearchResult> Search(IEnumerable<Product> products, string searchQuery, bool deepSearchAllowed = false);
    }
}
