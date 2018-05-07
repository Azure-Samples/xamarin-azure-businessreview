using System;
using System.Collections.Generic;
using Reviewer.SharedModels;
using System.Threading.Tasks;
namespace Reviewer.Core
{
    public interface IAPIService
    {
        Task<List<Review>> GetReviewsForBusiness(string businessId);

        Task InsertReview(Review review, string token);

        Task UpdateReview(Review review, string token);

        Task<List<Review>> GetReviewsForAuthor(string authorId, string token);

        Task<string> GetContainerWriteSasToken();

        Task WritePhotoInfoToQueue(string reviewId, string photoUrl);
    }
}
