using Microsoft.AspNetCore.Mvc;
using TrackerHabiHamApi.Services;

namespace TrackerHabiHamApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkoutController : ControllerBase
    {
        private readonly IWorkoutService _workoutService;

        public WorkoutController(IWorkoutService workoutService)
        {
            _workoutService = workoutService;
        }

        [HttpGet("check")]
        public async Task<ActionResult> CheckConnectionAsync(CancellationToken ct = default)
        {
            try
            {
                var isConnected = await _workoutService.CheckConnectionAsync(ct);

                if (isConnected)
                {
                    return Ok(new { connected = true, message = "PostgreSQL connection is healthy." });
                }
                else
                {
                    return StatusCode(503, new { connected = false, message = "PostgreSQL connection failed." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { connected = false, message = $"An error occurred while checking connection: {ex.Message}" });
            }
        }
    }
}
