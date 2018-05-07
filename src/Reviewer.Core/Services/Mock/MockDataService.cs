using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Reviewer.Services;
using Reviewer.SharedModels;

namespace Reviewer.Core
{
    public class MockDataService : IDataService
    {
        public MockDataService()
        {
        }

        public async Task<List<Business>> GetBusinesses()
        {
            var business1Id = Guid.NewGuid().ToString();
            var business2Id = Guid.NewGuid().ToString();

            var review1 = GetReview(business1Id);
            var review2 = GetReview(business2Id);

            var business1 = new Business
            {
                Address = new Address
                {
                    City = "Madison",
                    Id = Guid.NewGuid().ToString(),
                    Line1 = "123 Main St",
                    State = "WI",
                    Zip = "53706"
                },
                Id = business1Id,
                Name = "Fancy Restaurant",
                Phone = "123-123-1234",
                RecentReviews = new List<Review> { review1 }
            };

            var business2 = new Business
            {
                Address = new Address
                {
                    City = "Seattle",
                    Id = Guid.NewGuid().ToString(),
                    Line1 = "702 Pike St",
                    State = "WA",
                    Zip = "99806"
                },
                Id = business2Id,
                Name = "Ok Place",
                Phone = "888-238-1413",
                RecentReviews = new List<Review> { review2 }
            };

            return await Task.FromResult(new List<Business> { business1, business2 });
        }

        Review GetReview(string businessId)
        {
            var review = new Review
            {
                Author = "Matt",
                BusinessId = businessId,
                Date = DateTime.Now,
                Id = Guid.NewGuid().ToString(),
                Photos = new List<string>(),
                Rating = 5,
                ReviewText = "Great!"
            };

            return review;
        }
    }
}
