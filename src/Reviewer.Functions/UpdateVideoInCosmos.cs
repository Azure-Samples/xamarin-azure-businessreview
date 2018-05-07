using System;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Reviewer.Functions.Models;
using System.Linq;

namespace Reviewer.Functions
{
    public static class UpdateVideoInCosmos
    {
        [FunctionName("UpdateVideoInCosmos")]
        public static void Run(
            [QueueTrigger("review-videos", Connection = "AzureWebJobsStorage")]VideoQueueMessage myQueueItem,
            [DocumentDB("BuildReviewer", "Reviews", Id = "{reviewId}", ConnectionStringSetting = "Reviews_Cosmos")]Review review,
            [DocumentDB("BuildReviewer", "Reviews", Id = "id", ConnectionStringSetting = "Reviews_Cosmos")]out dynamic outputReview,
            TraceWriter log)
        {
            log.Info($"Processing with ReviewID: {myQueueItem.ReviewId}");

            if (review.Videos == null)
                review.Videos = new List<Video>();

            var video = new Video
            {
                HLSUrl = myQueueItem.HLSUri,
                MPEGDashUrl = myQueueItem.MpegDashUri,
                SmoothStreamUrl = myQueueItem.SmoothStreamingUri,
                ThumbnailUrl = myQueueItem.ThumbnailUri
            };

            review.Videos.Insert(0, video);

            outputReview = review;
        }
    }
}
