using Microsoft.AspNetCore.Mvc;
using TrackerHabiHamApi.Models.Dto;
using TrackerHabiHamApi.Services;

namespace TrackerHabiHamApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeightController : ControllerBase
    {
        private readonly IWeightService _weightService;

        public WeightController(IWeightService weightService)
        {
            _weightService = weightService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MounthWeight>>> GetFromPeriod(DateTime? start, DateTime? end)
        {
            var weights = await _weightService.GetFromPeriod(start, end);
            return Ok(weights);
        }

        [HttpPut]
        public async Task<ActionResult<MounthWeight>> UpdateWeight([FromBody] MounthWeight weight)
        {
            try
            {
                if(weight == null)
                    return BadRequest("Weight data is required.");

                if (weight.Date == default)
                    weight.Date = DateTime.UtcNow;

                var result = await _weightService.UpdateWeightAsync(weight.Date, weight.Weight);

                if (result == null)
                    return NotFound();
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error updating weight: {ex.Message}");
            }
        }
    }
}


