using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloPolly.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Polly : ControllerBase
    {
        private readonly IService _service;

        public Polly(IService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _service.Get();
            return Ok(result);
        }


    }
}
