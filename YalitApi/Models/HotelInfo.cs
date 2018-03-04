using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YalitApi.Models
{
    public class HotelInfo : Resource
    {
        public string Title { get; set; }
        public string Tagline { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public Address Location { get; set; }
    }
}
