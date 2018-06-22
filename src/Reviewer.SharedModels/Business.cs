using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Reviewer.SharedModels
{
    public class Business : ObservableObject
    {
        public Business()
        {
            Address = new Address { Id = Guid.NewGuid().ToString() };
        }

        string id;
        [JsonProperty("id")]
        public string Id { get => id; set => SetProperty(ref id, value); }

        string name;
        [JsonProperty("name")]
        public string Name { get => name; set => SetProperty(ref name, value); }

        Address address;
        [JsonProperty("address")]
        public Address Address { get => address; set => SetProperty(ref address, value); }

        string phone;
        [JsonProperty("phone")]
        public string Phone { get => phone; set => SetProperty(ref phone, value); }

        List<Review> recentReviews;
        [JsonProperty("recentReviews")]
        public List<Review> RecentReviews { get => recentReviews; set => SetProperty(ref recentReviews, value); }

        [JsonIgnore()]
        public string FirstInitial { get => Name?.Substring(0, 1); }
    }
}
