using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reviewer.Functions.Models
{
    public class VideoQueueMessage
    {
        [JsonProperty("reviewId")]
        public string ReviewId { get; set; }

        [JsonProperty("hlsUri")]
        public string HLSUri { get; set; }

        [JsonProperty("smoothStreamingUri")]
        public string SmoothStreamingUri { get; set; }

        [JsonProperty("mpegDashUri")]
        public string MpegDashUri { get; set; }

        [JsonProperty("thumbnailUri")]
        public string ThumbnailUri { get; set; }
    }
}
