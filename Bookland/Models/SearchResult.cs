namespace Bookland.Models
{
    public class SearchResult
    {
        public SearchResult(Product product)
        {
            Product = product;
            SimiliarityWeight = 0;
        }

        public Product Product { get; set; }

        public int SimiliarityWeight { get; set; }
    }
}