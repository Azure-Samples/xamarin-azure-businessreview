using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reviewer.Functions
{
    public class Address
    {        
        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("line1")]
        public string Line1 { get; set; }
        
        [JsonProperty("city")]
        public string City { get; set; }
        
        [JsonProperty("state")]
        public string State { get; set; }
        
        [JsonProperty("zip")]
        public string Zip { get; set; }
    }
}
