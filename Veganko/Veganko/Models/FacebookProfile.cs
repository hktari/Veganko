using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Veganko.Models
{
    public class FacebookPictureData
    {
        public string Url { get; set; }
    }
    public class FacebookPicture
    {
        public FacebookPictureData Data { get; set; }
    }
    public class FacebookDetails
    {
        [JsonProperty("first_name")]
        public string FirstName { get; set; }
        [JsonProperty("last_name")]
        public string LastName { get; set; }
        public FacebookPicture Picture { get; set; }
    }
}
