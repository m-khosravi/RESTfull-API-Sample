using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YalitApi.Models;

namespace YalitApi.Controllers
{
    [Route("/[controller]")]
    public class InfoController : Controller
    {
        private readonly HotelInfo _hotelInfo;
        public InfoController(HotelInfo hotelInfo)
        {
            _hotelInfo = hotelInfo;
        }

        [HttpGet(Name =nameof(GetInfo))]
        public IActionResult GetInfo()
        {
            _hotelInfo.Href = Url.Link(nameof(GetInfo), null);
            return Ok(_hotelInfo);
        }
    }
}
