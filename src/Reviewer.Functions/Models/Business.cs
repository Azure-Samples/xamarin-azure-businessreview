using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reviewer.Functions
{
    public class Business
    {

        [JsonProperty("id")]
        public string Id { get; set; }


        [JsonProperty("name")]
        public string Name { get; set; }


        [JsonProperty("address")]
        public Address Address { get; set; }


        [JsonProperty("phone")]
        public string Phone { get; set; }

        
        [JsonProperty("recentReviews")]
        public List<Review> RecentReviews { get; set; }


    }
}
