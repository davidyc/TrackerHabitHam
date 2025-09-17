using TrackerHabiHamApi.Models.Dto;

namespace TrackerHabiHamApi.Services
{
	public class SyncService : ISyncService
	{
		private readonly IGoogleSheetsService _googleSheetsService;
		private readonly IWeightService _weightService;
		private readonly ILogger<SyncService> _logger;

		public SyncService(IGoogleSheetsService googleSheetsService, IWeightService weightService, ILogger<SyncService> logger)
		{
			_googleSheetsService = googleSheetsService;
			_weightService = weightService;
			_logger = logger;
		}

		public async Task<int> SyncByYearAsync(int year)
		{
			var allGoogle = GetYearFromGoogle(year).ToList();
            var allDb = (await GetYearFromDB(year)).ToList();

            var dbByDate = allDb.ToDictionary(x => x.Date, x => x);
            var notSynced = new List<MounthWeight>(allGoogle.Count);

            foreach (var g in allGoogle)
            {
                if (!dbByDate.TryGetValue(g.Date, out var dbItem))
                {
                    notSynced.Add(g);
                    continue;
                }

                if (!string.Equals(dbItem.Weight, g.Weight, System.StringComparison.OrdinalIgnoreCase))
                {
                    notSynced.Add(g);
                }
            }

            var affected = await _weightService.UpsertManyAsync(notSynced);
            return affected;
		}

        private IEnumerable<MounthWeight> GetYearFromGoogle(int year)
		{
            var all = new List<MounthWeight>(366);
            for (int month = 1; month <= 12; month++)
            {
                try
                {
                    var items = _googleSheetsService.GetMounth(year, month);
                    all.AddRange(items);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error syncing month {Month} of {Year}", month, year);
                }
            }

            var today = DateOnly.FromDateTime(DateTime.Today);
            all.RemoveAll(w => w.Date > today);
			return all;
        }

        private async Task<IEnumerable<MounthWeight>> GetYearFromDB(int year)
		{
            var today = DateOnly.FromDateTime(DateTime.Today);
            var allDb = new List<MounthWeight>(366);
            var start = DateOnly.FromDateTime(new DateTime(year, 1, 1));
            var end = DateOnly.FromDateTime(new DateTime(year, 12, 31));

            try
            {                                 
                var dbItemsTask = await _weightService.GetFromPeriod(start, end);
                var dbItems = dbItemsTask
                    .Where(w => w.Date <= today && w.Date.Year == year)
                    .ToList();

                var byDate = allDb.ToDictionary(x => x.Date, x => x);
                foreach (var dbItem in dbItems)
                {
                    if (byDate.TryGetValue(dbItem.Date, out var existing))
                    {
                        existing.Weight = dbItem.Weight;
                    }
                    else
                    {
                        allDb.Add(new MounthWeight { Date = dbItem.Date, Weight = dbItem.Weight });
                    }
                }

                allDb = allDb
                    .Where(x => x.Date <= today && x.Date.Year == year)
                    .OrderBy(x => x.Date)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error merging DB data for {Year}", year);
            }

            return allDb.ToList();

        }

	}
}


