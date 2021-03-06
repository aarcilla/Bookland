﻿using Bookland.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bookland.Helpers
{
    public class SearchHelpers : Abstract.ISearchHelpers
    {
        private static readonly char[] Separators = { ' ', ',', '.', ';', ':', '#', '+', '/', '(', ')', '[', ']', '{', '}', '|' };

        public IEnumerable<SearchResult> Search(IEnumerable<Product> products, string searchQuery, decimal? termMatchRatioMinimum = null, bool deepSearchAllowed = false)
        {
            var searchTerms = new HashSet<string>(searchQuery.Split(Separators), StringComparer.OrdinalIgnoreCase);
            RemoveArbitraryTerms(searchTerms);

            var searchResults = new Dictionary<string, SearchResult>();

            Action<Dictionary<string, SearchResult>, Product, int, decimal> increaseProductWeight = 
                (results, product, weight, matchRatio) => {
                    SearchResult result = results.ContainsKey(product.Name)
                        ? results[product.Name] : new SearchResult(product);

                    result.SimiliarityWeight += weight;
                    result.TermMatchRatio = matchRatio;
                    results[product.Name] = result;
                };

            foreach (Product product in products)
            {
                HashSet<string> alreadyMatchedSearchTerms;

                int basicSearchWeight = BasicProductSearchResultWeight(searchTerms, product, out alreadyMatchedSearchTerms);
                if (basicSearchWeight > 0)
                    increaseProductWeight(searchResults, product, basicSearchWeight, 
                                            (decimal)alreadyMatchedSearchTerms.Count/(decimal)searchTerms.Count);

                // If search on a term-to-term basis wasn't fulfilling enough, do a deeper search 
                // (i.e. search across whole product name and description without regard to spaces)
                if (deepSearchAllowed)
                {
                    foreach (string searchTerm in searchTerms)
                    {
                        // Disregard search terms already found in basic product search (as they would come up as duplicates)
                        if (!alreadyMatchedSearchTerms.Contains(searchTerm))
                        {
                            alreadyMatchedSearchTerms.Add(searchTerm);
                            int deepSearchWeight = DeepProductSearchResultWeight(searchTerm, product);
                            if (deepSearchWeight > 0)
                                increaseProductWeight(searchResults, product, deepSearchWeight, 
                                                        (decimal)alreadyMatchedSearchTerms.Count / (decimal)searchTerms.Count);
                        }
                    }
                }
            }

            // Term match ratio minimum threshold only applies if there are 3 or more search terms;
            // otherwise all terms should match
            if (searchTerms.Count == 2)
                termMatchRatioMinimum = 1.0M;

            if (termMatchRatioMinimum.HasValue)
            {
                var resultsBelowTermMatchRatioMinimum = new List<string>();
                foreach (var result in searchResults)
                {
                    if (result.Value.TermMatchRatio < termMatchRatioMinimum.Value)
                        resultsBelowTermMatchRatioMinimum.Add(result.Key);
                }

                foreach (string failedResultKey in resultsBelowTermMatchRatioMinimum)
                    searchResults.Remove(failedResultKey);
            }

            return searchResults.Values.OrderByDescending(sr => sr.TermMatchRatio)
                                        .ThenByDescending(sr => sr.SimiliarityWeight)
                                        .AsEnumerable<SearchResult>();
        }

        private int BasicProductSearchResultWeight(HashSet<string> searchTerms, Product product, out HashSet<string> alreadyMatchedSearchTerms)
        {
            alreadyMatchedSearchTerms = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            int currentSearchWeight = 0;

            // If there is an exact match between the search term and one of the product name's terms (regardless of letter case),
            // give it the most weight
            foreach (string nameTerm in product.Name.Split(Separators))
            {
                if (searchTerms.Contains(nameTerm))
                {
                    currentSearchWeight += 3;
                    alreadyMatchedSearchTerms.Add(nameTerm);
                }
            }

            foreach (string descriptionTerm in product.Description.Split(Separators))
            {
                if (searchTerms.Contains(descriptionTerm))
                {
                    currentSearchWeight += 2;
                    alreadyMatchedSearchTerms.Add(descriptionTerm);
                }
            }

            string yearString = product.Year.ToString();
            if (searchTerms.Contains(yearString))
            {
                currentSearchWeight += 1;
                alreadyMatchedSearchTerms.Add(yearString);
            }

            return currentSearchWeight;
        }
        
        private int DeepProductSearchResultWeight(string searchTerm, Product product)
        {
            int currentSearchWeight = 0;

            // Consider any matches with terms less than 3 characters not valid for a decent separation-independent match
            if (searchTerm.Length < 3)
                return 0;

            // i.e. if there are any (case insensitive) matches at all, regardless of term separation, within the product name
            if (product.Name.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                currentSearchWeight++;
            
            if (product.Description.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                currentSearchWeight++;

            return currentSearchWeight;
        }

        private void RemoveArbitraryTerms(HashSet<string> searchTerms)
        {
            // Utilise hash set to hold arbitrary terms for O(1)-time lookup
            HashSet<string> arbitraryTerms = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
                "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", 
                "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
                "the", "of", "to", 
                "in", "on", "and", "at", "it", "is",
                "author", "artist",
                "", "&", "*", "^", "?", "!", ",", ".", @"/", @"\", ":", ";", "\"", "'", "|",
                "<", ">", "{", "}", "[", "]", "(", ")", "-", "_", "=", "+", "$", "~", "`"
            };

            searchTerms.RemoveWhere(t => arbitraryTerms.Contains(t));
        }
    }
}