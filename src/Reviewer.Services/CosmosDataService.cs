using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Reviewer.SharedModels;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using System.Linq.Expressions;
using Microsoft.Azure.Documents.Linq;
using System.Linq;

namespace Reviewer.Services
{
    public class CosmosDataService : IDataService
    {
        string databaseName = "BuildReviewer";
        string businessCollectionName = "Businesses";
        string reviewCollectionName = "Reviews";

        DocumentClient client;

        public void Initialize()
        {
            if (client != null)
                return;

            client = new DocumentClient(new Uri(APIKeys.CosmosEndpointUrl), APIKeys.CosmosAuthKey);
        }

        public async Task<List<Business>> GetBusinesses()
        {
            try
            {
                Initialize();

                var businesses = new List<Business>();

                var businessQuery = client.CreateDocumentQuery<Business>(
                    UriFactory.CreateDocumentCollectionUri(databaseName, businessCollectionName),
                    new FeedOptions { MaxItemCount = -1 }).AsDocumentQuery();

                while (businessQuery.HasMoreResults)
                {
                    var queryResults = await businessQuery.ExecuteNextAsync<Business>();

                    businesses.AddRange(queryResults);
                }

                return businesses;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"*** ERROR: {ex.Message}");
            }

            return new List<Business>();
        }

        public async Task<List<Review>> GetReviewsForBusiness(string businessId)
        {
            Initialize();

            var reviews = new List<Review>();

            var reviewQuery = client.CreateDocumentQuery<Review>(
                UriFactory.CreateDocumentCollectionUri(databaseName, reviewCollectionName),
                new FeedOptions { MaxItemCount = -1 })
                                    .Where(r => r.BusinessId == businessId)
                                    .AsDocumentQuery();

            while (reviewQuery.HasMoreResults)
            {
                var queryResults = await reviewQuery.ExecuteNextAsync<Review>();

                reviews.AddRange(queryResults);
            }

            return reviews;
        }

        public async Task<List<Review>> GetReviewsByAuthor(string authorId)
        {
            Initialize();

            var reviews = new List<Review>();

            var reviewQuery = client.CreateDocumentQuery<Review>(
                UriFactory.CreateDocumentCollectionUri(databaseName, reviewCollectionName),
                new FeedOptions { MaxItemCount = -1 })
                                    .Where(r => r.AuthorId == authorId)
                                    .AsDocumentQuery();

            while (reviewQuery.HasMoreResults)
            {
                var queryResults = await reviewQuery.ExecuteNextAsync<Review>();

                reviews.AddRange(queryResults);
            }

            return reviews;
        }

        public async Task InsertReview(Review review)
        {
            Initialize();

            await client.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(databaseName, reviewCollectionName),
                review);
        }

        public async Task UpdateReview(Review review)
        {
            Initialize();

            var reviewUri = UriFactory.CreateDocumentUri(databaseName, reviewCollectionName, review.Id);

            var existingDoc = (await client.ReadDocumentAsync<Review>(reviewUri)).Document;

            // Since the update of the review from the client won't have any new videos in it, need to preserve
            // any videos that have been added via the AMS functionality
            review.Videos = existingDoc.Videos;

            await client.ReplaceDocumentAsync(reviewUri, review);
        }

        public async Task InsertBusiness(Business business)
        {
            Initialize();

            await client.CreateDocumentAsync(
                UriFactory.CreateDocumentCollectionUri(databaseName, businessCollectionName),
                business);
        }

        public async Task UpdateBusiness(Business business)
        {
            Initialize();

            var businessUri = UriFactory.CreateDocumentUri(databaseName, businessCollectionName, business.Id);
            var existingBusiness = (await client.ReadDocumentAsync<Business>(businessUri)).Document;

            await client.ReplaceDocumentAsync(businessUri, business);
        }
    }
}
