using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace Reviewer.Functions
{
    [StorageAccount("AzureWebJobsStorage")]
    public static class WritePhotoInfoQueue
    {
        [FunctionName("WritePhotoInfoQueue")]
        [return: Queue("review-photos")]
        public static QueueMessage Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")]QueueMessage req,
            [DocumentDB("BuildReviewer", "Reviews", Id = "{reviewId}", ConnectionStringSetting = "Reviews_Cosmos")]Review review,
            TraceWriter log)
        {
            log.Info($"Review ID: {req.reviewId}, Photo Url: {req.photoUrl}");

            if (!Thread.CurrentPrincipal.Identity.IsAuthenticated)
                throw new System.Exception("Not authenticated");

            var claimsPrincipal = (ClaimsPrincipal)Thread.CurrentPrincipal;

            var objectClaimTypeName = @"http://schemas.microsoft.com/identity/claims/objectidentifier";

            var objectClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == objectClaimTypeName);

            var userId = objectClaim?.Value;

            if (review.AuthorId != userId)
                throw new System.Exception("Unauthorized");

            return req;
        }


    }


}
