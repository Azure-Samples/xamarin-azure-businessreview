using System;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
namespace Reviewer.SharedModels
{
    public class Address : ObservableObject
    {
        string id;
        [JsonProperty("id")]
        public string Id { get => id; set => SetProperty(ref id, value); }

        string line1;
        [JsonProperty("line1")]
        public string Line1 { get => line1; set => SetProperty(ref line1, value); }

        string city;
        [JsonProperty("city")]
        public string City { get => city; set => SetProperty(ref city, value); }

        string state;
        [JsonProperty("state")]
        public string State { get => state; set => SetProperty(ref state, value); }

        string zip;
        [JsonProperty("zip")]
        public string Zip { get => zip; set => SetProperty(ref zip, value); }
    }
}
