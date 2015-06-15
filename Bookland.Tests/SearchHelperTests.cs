using Bookland.Helpers;
using Bookland.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bookland.Tests
{
    [TestClass]
    public class SearchHelperTests
    {
        private SearchHelpers searchHelperTests = new SearchHelpers();

        private Category computingCategory = new Category { CategoryName = "Books", CategoryDescription = "Books about computers.", CategoryLevel = 3 };
        private Category electronicCategory = new Category { CategoryName = "Electronic", CategoryDescription = "Electronic music CDs.", CategoryLevel = 3 };
        private Category medicalScienceCategory = new Category { CategoryName = "Medical Science", CategoryDescription = "Medical science books.", CategoryLevel = 3 };
        private List<Product> testProducts = new List<Product>();

        public SearchHelperTests()
        {
            testProducts.Add(new Product { ProductID = 1, Name = "Pro ASP.NET MVC 4", Description = "AUTHOR(S): Freeman, Adam", Category = computingCategory, Price = 25.99M, Year = 2012, DateAdded = DateTime.Now });
            testProducts.Add(new Product { ProductID = 2, Name = "Pro C# and the .NET 4.5 Framework", Description = "AUTHOR(S): Troelsen, Andrew", Category = computingCategory, Price = 24.95M, Year = 2012, DateAdded = DateTime.Now });
            testProducts.Add(new Product { ProductID = 3, Name = "Beginning jQuery", Description = "AUTHOR(S): Franklin, Jack", Category = computingCategory, Price = 24.95M, Year = 2013, DateAdded = DateTime.Now });
            testProducts.Add(new Product { ProductID = 4, Name = "Burial - Untrue", Description = "ARTIST(S): Burial; Burial's 2007 sophomore album", Category = electronicCategory, Price = 7.99M, Year = 2007, DateAdded = DateTime.Now });
            testProducts.Add(new Product { ProductID = 5, Name = "Burial - Burial", Description = "ARTIST(S): Burial; Burial's 2006 debut album", Category = electronicCategory, Price = 7.99M, Year = 2006, DateAdded = DateTime.Now });
            testProducts.Add(new Product { ProductID = 6, Name = "X-Ray Technology", Description = "A comprehensive look into the history and wonders of x-ray tech.", Category = medicalScienceCategory, Price = 15.99M, Year = 2012, DateAdded = DateTime.Now });
        }

        [TestMethod]
        public void Search_Fail_EmptyString()
        {
            // ARRANGE
            string query = string.Empty;

            // ACT
            var result = searchHelperTests.Search(testProducts, query);

            // ASSERT
            Assert.IsTrue(result == null || result.Count() <= 0);
        }

        [TestMethod]
        public void Search_Fail_NoMatch()
        {
            // ARRANGE
            string query = "Aphex Twin";

            // ACT
            var result = searchHelperTests.Search(testProducts, query);

            // ASSERT
            Assert.IsTrue(result == null || result.Count() <= 0);
        }

        #region Product name tests

        [TestMethod]
        public void Search_Success_ProductName_MatchExact()
        {
            // ARRANGE
            string query = "Beginning jQuery";

            // ACT
            var result = searchHelperTests.Search(testProducts, query).ToArray<SearchResult>();

            // ASSERT
            Assert.IsTrue(result != null && result.Length > 0);
            Assert.IsTrue(result[0].Product.ProductID == 3);
            Assert.IsTrue(result[0].SimiliarityWeight == 6);
        }

        [TestMethod]
        public void Search_Success_ProductName_MatchCaseInsensitive()
        {
            // ARRANGE
            string query = "beginning Jquery";

            // ACT
            var result = searchHelperTests.Search(testProducts, query).ToArray<SearchResult>();

            // ASSERT
            Assert.IsTrue(result != null && result.Length > 0);
            Assert.IsTrue(result[0].Product.ProductID == 3);
            Assert.IsTrue(result[0].SimiliarityWeight == 6);
        }

        [TestMethod]
        public void Search_Success_ProductName_MatchOneTerm()
        {
            // ARRANGE
            string query = "mvc";

            // ACT
            var result = searchHelperTests.Search(testProducts, query).ToArray<SearchResult>();

            // ASSERT
            Assert.IsTrue(result != null && result.Length > 0);
            Assert.IsTrue(result[0].Product.ProductID == 1);
            Assert.IsTrue(result[0].SimiliarityWeight == 3);
        }

        [TestMethod]
        public void Search_Success_ProductName_MatchMultipleTerms()
        {
            // ARRANGE
            string query = "pro mvc 4";

            // ACT
            var result = searchHelperTests.Search(testProducts, query).ToArray<SearchResult>();

            // ASSERT
            Assert.IsTrue(result != null && result.Length > 0);
            Assert.IsTrue(result[0].Product.ProductID == 1);
            Assert.IsTrue(result[0].SimiliarityWeight == 9);
        }

        [TestMethod]
        public void Search_Success_ProductName_DeepSearch_MatchPartial()
        {
            // ARRANGE
            // Test match with "jQuery"
            string query = "query";

            // ACT 
            var result = searchHelperTests.Search(testProducts, query, true).ToArray<SearchResult>();

            // ASSERT
            Assert.IsTrue(result != null && result.Length > 0);
            Assert.IsTrue(result[0].Product.ProductID == 3);
            Assert.IsTrue(result[0].SimiliarityWeight == 1);
        }

        [TestMethod]
        public void Search_Fail_ProductName_DeepSearchDisabled_MatchPartialIfEnabled()
        {
            // ARRANGE
            string query = "query";

            // ACT 
            var result = searchHelperTests.Search(testProducts, query, false).ToArray<SearchResult>();

            // ASSERT
            Assert.IsTrue(result == null || result.Length <= 0);
        }

        #endregion

        #region Product description tests

        [TestMethod]
        public void Search_Success_ProductDescription_MatchExact()
        {
            // ARRANGE
            string query = "AUTHOR(S): Franklin, Jack";

            // ACT
            var result = searchHelperTests.Search(testProducts, query).ToArray<SearchResult>();

            // ASSERT
            Assert.IsTrue(result != null && result.Length > 0);
            Assert.IsTrue(result[0].Product.ProductID == 3);
            Assert.IsTrue(result[0].SimiliarityWeight == 4);
        }

        [TestMethod]
        public void Search_Success_ProductDescription_MatchCaseInsensitive()
        {
            // ARRANGE
            string query = "freeMan";

            // ACT
            var result = searchHelperTests.Search(testProducts, query).ToArray<SearchResult>();

            // ASSERT
            Assert.IsTrue(result != null && result.Length > 0);
            Assert.IsTrue(result[0].Product.ProductID == 1);
            Assert.IsTrue(result[0].SimiliarityWeight == 2);
        }

        [TestMethod]
        public void Search_Success_ProductDescription_MatchMultipleTerms()
        {
            // ARRANGE
            string query = "Andrew Troelsen";

            // ACT
            var result = searchHelperTests.Search(testProducts, query).ToArray<SearchResult>();

            // ASSERT
            Assert.IsTrue(result != null && result.Length > 0);
            Assert.IsTrue(result[0].Product.ProductID == 2);
            Assert.IsTrue(result[0].SimiliarityWeight == 4);
        }

        [TestMethod]
        public void Search_Success_ProductDescription_DeepSearch_MatchPartial()
        {
            // ARRANGE
            string query = "debu";

            // ACT
            var result = searchHelperTests.Search(testProducts, query, true).ToArray<SearchResult>();

            // ASSERT
            Assert.IsTrue(result != null && result.Length > 0);
            Assert.IsTrue(result[0].Product.ProductID == 5);
            Assert.IsTrue(result[0].SimiliarityWeight == 1);
        }

        [TestMethod]
        public void Search_Fail_ProductDescription_DeepSearchDisabled_MatchPartialIfEnabled()
        {
            // ARRANGE
            string query = "debu";

            // ACT
            var result = searchHelperTests.Search(testProducts, query, false).ToArray<SearchResult>();

            // ASSERT
            Assert.IsTrue(result == null || result.Length <= 0);
        }

        #endregion

        #region Multiple matches tests

        [TestMethod]
        public void Search_Success_MultipleMatches()
        {
            // ARRANGE
            string query = ".NET";

            // ACT
            var result = searchHelperTests.Search(testProducts, query).ToArray<SearchResult>();

            // ASSERT
            Assert.IsTrue(result != null && result.Length > 0);
            Assert.IsTrue(result.Any(s => s.Product.ProductID == 1));
            Assert.IsTrue(result.Any(s => s.Product.ProductID == 2));
        }

        [TestMethod]
        public void Search_Success_MultipleMatches_DescendingOrder()
        {
            // ARRANGE
            string query = "ASP.NET";

            // ACT
            var result = searchHelperTests.Search(testProducts, query).ToArray<SearchResult>();

            // ASSERT
            Assert.IsTrue(result != null && result.Length > 0);
            Assert.IsTrue(result[0].Product.ProductID == 1);
            Assert.IsTrue(result[0].SimiliarityWeight == 6);    // "ASP" and "NET" term matches
            Assert.IsTrue(result[1].Product.ProductID == 2);
            Assert.IsTrue(result[1].SimiliarityWeight == 3);    // "NET" term matches
        }

        #endregion

        #region Arbitrary term-related tests

        // Arbitrary terms tests
        [TestMethod]
        public void Search_Success_ArbitraryTermIgnored()
        {
            // ARRANGE
            string query = "the";

            // ACT
            var result = searchHelperTests.Search(testProducts, query).ToArray<SearchResult>();

            // ASSERT
            Assert.IsTrue(result == null || result.Length == 0);
        }

        [TestMethod]
        public void Search_Success_ArbitrarySymbolIgnored()
        {
            // ARRANGE
            string query = "- ";

            // ACT
            var result = searchHelperTests.Search(testProducts, query).ToArray<SearchResult>();

            // ASSERT
            Assert.IsTrue(result == null || result.Length == 0);
        }

        [TestMethod]
        public void Search_Success_HyphenatedTermsNotSeparatedOrArbitrary()
        {
            // ARRANGE
            string query = "x-ray";

            // ACT
            var result = searchHelperTests.Search(testProducts, query).ToArray<SearchResult>();

            // ASSERT
            Assert.IsTrue(result != null && result.Length > 0);
            Assert.IsTrue(result[0].Product.ProductID == 6);
            Assert.IsTrue(result[0].SimiliarityWeight == 5);
        }

        #endregion
    }
}
