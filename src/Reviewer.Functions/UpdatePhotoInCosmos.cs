using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace Reviewer.Functions
{
    public static class UpdatePhotoInCosmos
    {
        [FunctionName("UpdatePhotoInCosmos")]
        public static void Run(
            [QueueTrigger("review-photos", Connection = "AzureWebJobsStorage")]QueueMessage myQueueItem,
            [DocumentDB("BuildReviewer", "Reviews", Id = "{reviewId}", ConnectionStringSetting = "Reviews_Cosmos")]Review review,
            [DocumentDB("BuildReviewer", "Reviews", Id = "id", ConnectionStringSetting = "Reviews_Cosmos")]out dynamic outputReview,
            TraceWriter log)
        {
            log.Info($"C# Queue trigger function processed: {myQueueItem}");

            if (review.Photos == null)
                review.Photos = new List<string>();

            review.Photos.Add(myQueueItem.photoUrl);

            outputReview = review;
        }
    }
}
