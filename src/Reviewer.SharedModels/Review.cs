using System;
using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
namespace Reviewer.SharedModels
{
    public class Review : ObservableObject
    {
        public Review()
        {
        }

        string id;
        [JsonProperty("id")]
        public string Id { get => id; set => SetProperty(ref id, value); }

        string businessId;
        [JsonProperty("businessId")]
        public string BusinessId { get => businessId; set => SetProperty(ref businessId, value); }

        string businessName;
        [JsonProperty("businessName")]
        public string BusinessName { get => businessName; set => SetProperty(ref businessName, value); }

        string author;
        [JsonProperty("author")]
        public string Author { get => author; set => SetProperty(ref author, value); }

        string authorId;
        [JsonProperty("authorId")]
        public string AuthorId { get => authorId; set => SetProperty(ref authorId, value); }

        string reviewText;
        [JsonProperty("reviewText")]
        public string ReviewText { get => reviewText; set => SetProperty(ref reviewText, value); }

        DateTime date = DateTime.Now;
        [JsonProperty("date")]
        public DateTime Date { get => date; set => SetProperty(ref date, value); }

        List<string> photos;
        [JsonProperty("photos")]
        public List<string> Photos { get => photos; set => SetProperty(ref photos, value); }

        List<Video> videos;
        public List<Video> Videos { get => videos; set => SetProperty(ref videos, value); }

        int rating = 1;
        [JsonProperty("rating")]
        public int Rating { get => rating; set => SetProperty(ref rating, value); }
    }
}

