using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reviewer.Functions.Models
{
    public class Video
    {
        [JsonProperty("hlsUrl")]
        public string HLSUrl { get; set; }

        [JsonProperty("smoothStreamUrl")]
        public string SmoothStreamUrl { get; set; }

        [JsonProperty("mpegDashUrl")]
        public string MPEGDashUrl { get; set; }

        [JsonProperty("thumbnailUrl")]
        public string ThumbnailUrl { get; set; }
    }
}
