using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Reviewer.Services;
using Reviewer.SharedModels;
using Newtonsoft.Json.Linq;

namespace Reviewer.Core
{
    public class MockAPIService : IAPIService
    {
        HttpClient webClient = new HttpClient();


        public async Task<List<Review>> GetReviewsForBusiness(string businessId)
        {
            var reviewJson = await webClient.GetStringAsync($"{APIKeys.WebAPIUrl}review/business/{businessId}");

            return JsonConvert.DeserializeObject<List<Review>>(reviewJson);
        }

        public async Task InsertReview(Review review, string token)
        {
            await Task.CompletedTask;
        }

        public async Task UpdateReview(Review review, string token)
        {
            await Task.CompletedTask;
        }

        public async Task<List<Review>> GetReviewsForAuthor(string authorId, string token)
        {
            return await Task.FromResult(new List<Review>());
        }

        public async Task<string> GetContainerWriteSasToken()
        {
            return await Task.FromResult("");
        }

        public async Task WritePhotoInfoToQueue(string reviewId, string photoUrl)
        {
            await Task.CompletedTask;
        }
    }
}
