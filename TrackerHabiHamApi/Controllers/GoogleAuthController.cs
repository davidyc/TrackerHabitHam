using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using TrackerHabiHamApi.Services;

namespace TrackerHabiHamApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GoogleAuthController : ControllerBase
    {
        private readonly ILogger<GoogleAuthController> _logger;
        private readonly IGoogleSheetsService _googleSheetsService;
        public GoogleAuthController(ILogger<GoogleAuthController> logger, IGoogleSheetsService googleSheetsService)
        {
            _logger = logger;              
            _googleSheetsService = googleSheetsService;
        }

        [HttpGet("check-credentials")]
        public IActionResult CheckCredentials([FromQuery] string path = "just-turbine-406810-ec3a22d281b9.json")
        {
            var fullPath = Path.Combine(AppContext.BaseDirectory, path);
            var exists = System.IO.File.Exists(fullPath); 

            return Ok(new { file = fullPath, exists });
        }

        [HttpGet("Period")]
        public IActionResult GetMounth(int year, int mounth)
        {
            var result = _googleSheetsService.GetMounth(year, mounth);
            return Ok(new {result});
        }
    }
}


