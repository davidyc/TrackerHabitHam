using Microsoft.EntityFrameworkCore;
using TrackerHabiHamApi.Data;
using TrackerHabiHamApi.Models.Dto;

namespace TrackerHabiHamApi.Services
{
    public class WeightService : IWeightService
    {
        private readonly ApplicationDbContext _context;
        private readonly IGoogleSheetsService _googleSheetsService;

        public WeightService(ApplicationDbContext context, IGoogleSheetsService googleSheetsService)
        {
            _context = context;
            _googleSheetsService = googleSheetsService;
        }

        public async Task<IEnumerable<MounthWeight>> GetFromPeriod(DateOnly? start, DateOnly? end)
        {
            if (!start.HasValue)
            {
                start = DateOnly.FromDateTime(new DateTime(DateTime.UtcNow.Year, 1, 1));
            }

            var now = DateOnly.FromDateTime(DateTime.UtcNow.Date);

            if (!end.HasValue || end.Value > start.Value || end.Value > now)
            {
                end = now;
            }

            return await _context.MounthWeights
                   .Where(w => w.Date >= start.Value && w.Date <= end.Value)
                   .OrderBy(w => w.Date)
                   .ToListAsync();
        }

        public async Task<MounthWeight?> UpdateWeightAsync(DateOnly date, string weight)
        {
            MounthWeight? weightRecord;

            try
            {
                weightRecord = await _context.MounthWeights.FirstOrDefaultAsync(w => w.Date == date);

                if (weightRecord != null)
                {
                    weightRecord.Weight = weight;

                    await _context.SaveChangesAsync();
                }
                else
                {
                    weightRecord = new MounthWeight { Date = date, Weight = weight }; 
                    _context.MounthWeights.Add(weightRecord);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                weightRecord = new MounthWeight { Date = date, Weight = weight };
            }

            _googleSheetsService.WriteNumberToTodayRow(weight);
            return weightRecord;
        }

        public async Task<int> UpsertManyAsync(IEnumerable<MounthWeight> items)
        {
            var itemList = items
                .Select(i => new MounthWeight { Date = i.Date, Weight = i.Weight })
                .ToList();

            if (itemList.Count == 0)
            {
                return 0;
            }

            var dates = itemList.Select(i => i.Date).ToList();
            var existing = await _context.MounthWeights
                .Where(w => dates.Contains(w.Date))
                .ToListAsync();

            var existingByDate = existing.ToDictionary(w => w.Date, w => w);

            var inserts = new List<MounthWeight>();
            var updates = new List<MounthWeight>();

            foreach (var item in itemList)
            {
                if (!existingByDate.TryGetValue(item.Date, out var found))
                {
                    inserts.Add(item);
                }
                else if (!string.Equals(found.Weight, item.Weight, StringComparison.OrdinalIgnoreCase))
                {
                    found.Weight = item.Weight;
                    updates.Add(found);
                }
            }

            if (inserts.Count > 0)
            {
                await _context.MounthWeights.AddRangeAsync(inserts);
            }
                                   
            var affected = inserts.Count + updates.Count;
            if (affected > 0)
            {
                await _context.SaveChangesAsync();
            }

            return affected;
        }

    }
}


