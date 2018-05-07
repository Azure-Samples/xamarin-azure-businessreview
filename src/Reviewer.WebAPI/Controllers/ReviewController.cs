using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reviewer.Services;
using Reviewer.SharedModels;

namespace Reviewer.WebAPI.Controllers
{
    [Produces("application/json")]
    public class ReviewController : Controller
    {
        static CosmosDataService cosmosService = new CosmosDataService();

        bool ScopeValid(string theScope)
        {
            var scopes = HttpContext.User.FindFirst("http://schemas.microsoft.com/identity/claims/scope")?.Value;
            return scopes != null && scopes.Split(' ').Any(s => s.Equals(theScope));
        }

        [HttpGet("/Review/Business/{id}")]
        public async Task<IEnumerable<Review>> ReviewsForBusiness(string id)
        {
            return await cosmosService.GetReviewsForBusiness(id);
        }

        [HttpGet("/Review/Author/{id}"), Authorize]
        public async Task<IActionResult> ReviewsForAuthor(string id)
        {
            if (ScopeValid(Startup.AdminScope))
            {
                var results = await cosmosService.GetReviewsByAuthor(id);

                return Ok(results);
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost("/Review"), Authorize]
        public async Task<IActionResult> InsertReview([FromBody]Review review)
        {
            if (ScopeValid(Startup.AdminScope))
            {
                await cosmosService.InsertReview(review);

                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }

        [HttpPut("/Review"), Authorize]
        public async Task<IActionResult> UpdateReview([FromBody]Review review)
        {
            if (ScopeValid(Startup.AdminScope))
            {
                await cosmosService.UpdateReview(review);
                return Ok();
            }
            else
            {
                return Unauthorized();
            }
        }
    }

}