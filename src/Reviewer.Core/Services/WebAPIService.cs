using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Reviewer.SharedModels;
using System.Net.Http;
using Reviewer.Services;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Headers;

namespace Reviewer.Core
{
    public class WebAPIService : IAPIService
    {
        HttpClient webClient = new HttpClient();

        public async Task<List<Review>> GetReviewsForAuthor(string authorId, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{APIKeys.WebAPIUrl}review/author/{authorId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await webClient.SendAsync(request);

            var reviewJson = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<Review>>(reviewJson);
        }

        public async Task<List<Review>> GetReviewsForBusiness(string businessId)
        {
            var reviewJson = await webClient.GetStringAsync($"{APIKeys.WebAPIUrl}review/business/{businessId}");

            return JsonConvert.DeserializeObject<List<Review>>(reviewJson);
        }

        public async Task InsertReview(Review review, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{APIKeys.WebAPIUrl}Review");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            request.Content = new StringContent(JsonConvert.SerializeObject(review), Encoding.UTF8, "application/json");

            await webClient.SendAsync(request);
        }

        public async Task UpdateReview(Review review, string token)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, $"{APIKeys.WebAPIUrl}Review");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            request.Content = new StringContent(JsonConvert.SerializeObject(review), Encoding.UTF8, "application/json");

            await webClient.SendAsync(request);
        }

        public async Task<string> GetContainerWriteSasToken()
        {
            var spr = new StoragePermissionRequest { Permission = "Write" };

            var request = new HttpRequestMessage(HttpMethod.Post, APIKeys.SASRetrievalUrl);

            var bearerToken = await GetAccessBearerToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            var content = new StringContent(JsonConvert.SerializeObject(spr), Encoding.UTF8, "application/json");

            request.Content = content;

            var response = await webClient.SendAsync(request);

            return await response.Content.ReadAsStringAsync();
        }

        public async Task WritePhotoInfoToQueue(string reviewId, string photoUrl)
        {
            var queueInfo = new { reviewId, photoUrl };

            var request = new HttpRequestMessage(HttpMethod.Post, APIKeys.WriteToQueueUrl);

            var bearerToken = await GetAccessBearerToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            var content = new StringContent(JsonConvert.SerializeObject(queueInfo), Encoding.UTF8, "application/json");

            request.Content = content;

            await webClient.SendAsync(request);
        }

        async Task<string> GetAccessBearerToken()
        {
            var identityService = Xamarin.Forms.DependencyService.Get<IIdentityService>(Xamarin.Forms.DependencyFetchTarget.GlobalInstance);

            var authResult = await identityService.GetCachedSignInToken();
            var bearerToken = authResult.AccessToken;

            return bearerToken;
        }
    }
}
