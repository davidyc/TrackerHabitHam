using Microsoft.AspNetCore.Mvc;
using TrackerHabiHamApi.Services;

namespace TrackerHabiHamApi.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class SyncController : ControllerBase
	{
		private readonly ILogger<SyncController> _logger;
		private readonly ISyncService _syncService;

		public SyncController(ILogger<SyncController> logger, ISyncService syncService)
		{
			_logger = logger;
			_syncService = syncService;
		}

		[HttpGet]
		public async Task<ActionResult<int>> SyncByYear([FromQuery] int year)
		{
			if (year < 1900 || year > DateTime.UtcNow.Year + 1)
			{
				return BadRequest("Invalid year.");
			}

			_logger.LogInformation("Sync requested for year {Year}", year);
			var affected = await _syncService.SyncByYearAsync(year);
			return Ok(new { affected = $"Was synced {affected}"} );
		}
	}
}


