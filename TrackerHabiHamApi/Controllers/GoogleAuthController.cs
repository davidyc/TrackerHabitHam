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
        public IActionResult CheckCredentials()
        {  
            var exists = _googleSheetsService.CredentialExists();
            return Ok($"File exist: {exists}");
        }

        [HttpGet("Period")]
        public IActionResult GetMounth(int year, int mounth)
        {
            var result = _googleSheetsService.GetMounth(year, mounth);
            return Ok(new {result});
        }
    }
}


