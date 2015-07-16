namespace Bookland.Models
{
    public class SearchResult
    {
        public SearchResult(Product product)
        {
            Product = product;
            SimiliarityWeight = 0;
            TermMatchRatio = 0.0M;
        }

        public Product Product { get; set; }

        public int SimiliarityWeight { get; set; }

        public decimal TermMatchRatio { get; set; }
    }
}