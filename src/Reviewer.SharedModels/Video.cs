using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Reviewer.SharedModels
{
    public class Video : ObservableObject
    {
        string hlsUrl;
        [JsonProperty("hlsUrl")]
        public string HLSUrl { get => hlsUrl; set => SetProperty(ref hlsUrl, value); }

        string smoothStreamUrl;
        [JsonProperty("smoothStreamUrl")]
        public string SmoothStreamUrl { get => smoothStreamUrl; set => SetProperty(ref smoothStreamUrl, value); }

        string mpegDashUrl;
        [JsonProperty("mpegDashUrl")]
        public string MPEGDashUrl { get => mpegDashUrl; set => SetProperty(ref mpegDashUrl, value); }

        string thumbnailUrl;
        [JsonProperty("thumbnailUrl")]
        public string ThumbnailUrl { get => thumbnailUrl; set => SetProperty(ref thumbnailUrl, value); }
    }
}
