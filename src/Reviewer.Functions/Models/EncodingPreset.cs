namespace Reviewer.Functions.Models
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class EncodingPreset
    {
        [JsonProperty("Version")]
        public double Version { get; set; }

        [JsonProperty("Codecs")]
        public List<Codec> Codecs { get; set; }

        [JsonProperty("Outputs")]
        public List<Output> Outputs { get; set; }
    }

    public partial class Codec
    {
        [JsonProperty("KeyFrameInterval", NullValueHandling = NullValueHandling.Ignore)]
        public string KeyFrameInterval { get; set; }

        [JsonProperty("SceneChangeDetection", NullValueHandling = NullValueHandling.Ignore)]
        public string SceneChangeDetection { get; set; }

        [JsonProperty("H264Layers", NullValueHandling = NullValueHandling.Ignore)]
        public List<H264Layer> H264Layers { get; set; }

        [JsonProperty("Type")]
        public string Type { get; set; }

        [JsonProperty("JpgLayers", NullValueHandling = NullValueHandling.Ignore)]
        public List<Layer> JpgLayers { get; set; }

        [JsonProperty("Start", NullValueHandling = NullValueHandling.Ignore)]
        public string Start { get; set; }

        [JsonProperty("PngLayers", NullValueHandling = NullValueHandling.Ignore)]
        public List<Layer> PngLayers { get; set; }

        [JsonProperty("Step", NullValueHandling = NullValueHandling.Ignore)]
        public string Step { get; set; }

        [JsonProperty("Range", NullValueHandling = NullValueHandling.Ignore)]
        public string Range { get; set; }

        [JsonProperty("BmpLayers", NullValueHandling = NullValueHandling.Ignore)]
        public List<Layer> BmpLayers { get; set; }

        [JsonProperty("Channels", NullValueHandling = NullValueHandling.Ignore)]
        public long? Channels { get; set; }

        [JsonProperty("SamplingRate", NullValueHandling = NullValueHandling.Ignore)]
        public long? SamplingRate { get; set; }

        [JsonProperty("Bitrate", NullValueHandling = NullValueHandling.Ignore)]
        public long? Bitrate { get; set; }
    }

    public partial class Layer
    {
        [JsonProperty("Type")]
        public string Type { get; set; }

        [JsonProperty("Width")]
        public long Width { get; set; }

        [JsonProperty("Height")]
        public long Height { get; set; }

        [JsonProperty("Quality", NullValueHandling = NullValueHandling.Ignore)]
        public long? Quality { get; set; }
    }

    public partial class H264Layer
    {
        [JsonProperty("Profile")]
        public string Profile { get; set; }

        [JsonProperty("Level")]
        public string Level { get; set; }

        [JsonProperty("Bitrate")]
        public long Bitrate { get; set; }

        [JsonProperty("MaxBitrate")]
        public long MaxBitrate { get; set; }

        [JsonProperty("BufferWindow")]
        public string BufferWindow { get; set; }

        [JsonProperty("Width")]
        public long Width { get; set; }

        [JsonProperty("Height")]
        public long Height { get; set; }

        [JsonProperty("ReferenceFrames")]
        public long ReferenceFrames { get; set; }

        [JsonProperty("EntropyMode")]
        public string EntropyMode { get; set; }

        [JsonProperty("AdaptiveBFrame")]
        public bool AdaptiveBFrame { get; set; }

        [JsonProperty("Type")]
        public string Type { get; set; }

        [JsonProperty("FrameRate")]
        public string FrameRate { get; set; }
    }

    public partial class Output
    {
        [JsonProperty("FileName")]
        public string FileName { get; set; }

        [JsonProperty("Format")]
        public Format Format { get; set; }
    }

    public partial class Format
    {
        [JsonProperty("Type")]
        public string Type { get; set; }
    }

    public partial class EncodingPreset
    {
        public static EncodingPreset FromJson(string json) => JsonConvert.DeserializeObject<EncodingPreset>(json, Reviewer.Functions.Models.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this EncodingPreset self) => JsonConvert.SerializeObject(self, Reviewer.Functions.Models.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    public static class VideoEncodingPresetGenerator
    {
        public static EncodingPreset Thumbnail()
        {
            var h264Codec = new Codec
            {
                Type = "H264Video",
                KeyFrameInterval = "00:00:02",
                SceneChangeDetection = "true",
                H264Layers = new List<H264Layer>
                {
                   new H264Layer
                   {
                       Profile="Auto",
                       Level="auto",
                       Bitrate=4500,
                       MaxBitrate=4500,
                       BufferWindow="00:00:05",
                       Width=1280,
                       Height=720,
                       ReferenceFrames=3,
                       EntropyMode="Cabac",
                       AdaptiveBFrame=true,
                       Type="H264Layer",
                       FrameRate="0/1"
                   }
                }
            };
            
            var pngImage = new Codec
            {
                Type = "PngImage",
                //Start = "00:00:01",
                //Step = "00:00:10",
                //Range = "00:00:58",
                Start = "{Best}",
                PngLayers = new List<Layer>
                {
                   new Layer
                   {
                       Type = "PngLayer",
                       Width = 640,
                       Height = 360
                   }
                }
            };

            var audioCodec = new Codec
            {
                Type = "AACAudio",
                SamplingRate = 48000,
                Bitrate = 128,
                Channels = 2
            };

            var preset = new EncodingPreset
            {
                Version = 1.0,
                Codecs = new List<Codec>
                {
                    h264Codec,
                    pngImage,
                    audioCodec
                },
                Outputs = new List<Output>
                {
                    new Output { FileName = "{Basename}_{Index}{Extension}", Format = new Format { Type="PngFormat" } },
                    new Output { FileName = "{Basename}_{Width}x{Height}_{VideoBitrate}.mp4", Format = new Format { Type = "MP4Format" } }
                }
            };

            return preset;
        }

    }
}

